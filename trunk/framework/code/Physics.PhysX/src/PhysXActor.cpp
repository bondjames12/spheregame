// PhysXActor.cpp
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

#define NOMINMAX
#include <windows.h>
#include <stdio.h>

// 
#pragma unmanaged
#include "NxPhysics.h"
#include "NxCooking.h"
#include "NxStream.h"
#pragma managed

using namespace System;
using namespace System::Collections::Generic;
using namespace QuickStart;
using namespace Microsoft::Xna::Framework;

#include "CommonDefs.h"
#include "PhysXScene.h"
#include "PhysXActor.h"
#include "MemoryBuffer.h"

// Force native code generation for these helper functions
#pragma unmanaged

// Cooks a PhysX TriangleMesh from raw vertex data
NxTriangleMeshShapeDesc CookTriangleMesh(int numVerts, int numTris, NxVec3* verts, NxU32* faces, NxScene* physXScene)
{
	NxTriangleMeshDesc hfDesc;
	hfDesc.numVertices				= numVerts;
	hfDesc.numTriangles				= numTris;
	hfDesc.pointStrideBytes			= sizeof(NxVec3);
	hfDesc.triangleStrideBytes		= 3*sizeof(NxU32);
	hfDesc.points					= verts;
	hfDesc.triangles				= faces;							
	hfDesc.flags					= 0;

	hfDesc.heightFieldVerticalAxis		= NX_Y;
	hfDesc.heightFieldVerticalExtent	= -1000;

	NxTriangleMeshShapeDesc hfShapeDesc;

	// Initialize NxCooking library
	// TODO: Need to determine a better place for this one-time call.
	NxInitCooking();

	MemoryWriteBuffer buf;

	// Create the triangle mesh using NxCooking library
	bool status = NxCookTriangleMesh(hfDesc, buf);
	hfShapeDesc.meshData = physXScene->getPhysicsSDK().createTriangleMesh(MemoryReadBuffer(buf.data));

	return hfShapeDesc;
}

#define HEIGHT_FIELD_SCALE	32000.0f

// Creates a PhysX HeightField from raw height data
NxHeightFieldShapeDesc CreateHeightField(int numRows, int numColumns, float* data, float sizeX, float sizeZ, NxScene* scene)
{
	NxHeightFieldDesc desc;
	desc.nbRows						= numRows;
	desc.nbColumns					= numColumns;
	desc.verticalExtent				= -1000;
	desc.convexEdgeThreshold		= 0;
	desc.samples					= new NxU32[numRows * numColumns];
	desc.sampleStride				= sizeof(NxU32);

	NxHeightFieldSample* sample = (NxHeightFieldSample*)desc.samples;
	float maxHeight = -1e30f;

	// Determine max height
	for(int i = 0; i < numRows; ++i)
	{
		for(int j = 0; j < numColumns; ++j)
		{
			if(maxHeight < data[i * numColumns + j])
				maxHeight = data[i * numColumns + j];
		}
	}

	// Create height field samples
	// We need to scale the height values since PhysX wants integer values.
	for(int i = 0; i < numRows; ++i)
	{
		for(int j = 0; j < numColumns; ++j)
		{
			sample->height = (NxI16)(data[i * numColumns + j] / maxHeight * HEIGHT_FIELD_SCALE);
			sample->materialIndex0 = 1;
			sample->materialIndex1 = 1;
			sample->tessFlag = 0;

			sample++;
		}
	}

	// Create the height field
	NxHeightField* heightField = scene->getPhysicsSDK().createHeightField(desc);

	// Clean up our buffer
	delete desc.samples;

	NxHeightFieldShapeDesc heightFieldShapeDesc;
	heightFieldShapeDesc.heightField			= heightField;
	heightFieldShapeDesc.shapeFlags				= NX_SF_FEATURE_INDICES;
	heightFieldShapeDesc.heightScale			= maxHeight / HEIGHT_FIELD_SCALE;
	heightFieldShapeDesc.rowScale				= sizeX / NxReal(numRows - 1);
	heightFieldShapeDesc.columnScale			= sizeZ / NxReal(numColumns-1);
	heightFieldShapeDesc.meshFlags				= NX_MESH_SMOOTH_SPHERE_COLLISIONS;
	heightFieldShapeDesc.materialIndexHighBits	= 0;
	heightFieldShapeDesc.holeMaterial			= 2;

	return heightFieldShapeDesc;
}

#pragma managed

BEGIN_QS_NAMESPACE  // QuickStart::Physics::PhysX

// Default constructor
PhysXActor::PhysXActor(NxScene* scene, ActorDesc^ desc)
{
	physXScene = scene;

	NxActorDesc actorDesc;

	// Only assign a body if the actor is not static
	if(desc->Dynamic)
	{
		NxBodyDesc bodyDesc;
		actorDesc.body = &bodyDesc;
	}

	// Initialize density, position, and orientation
	actorDesc.density = desc->Density;
	actorDesc.globalPose.t = VECTOR3_TO_NXVEC3(desc->Position);
	
	Matrix orientation = desc->Orientation;

	NxVec3 row0 = NxVec3(orientation.M11, orientation.M12, orientation.M13);
	NxVec3 row1 = NxVec3(orientation.M21, orientation.M22, orientation.M23);
	NxVec3 row2 = NxVec3(orientation.M31, orientation.M32, orientation.M33);

	actorDesc.globalPose.M = NxMat33(row0, row1, row2);

	// Make sure we have at least one shape
	if(desc->Shapes->Count == 0)
	{
		throw gcnew Exception(L"Cannot create physics actor without at least one shape.");
	}

	// Process each shape
	for(int i = 0; i < desc->Shapes->Count; ++i)
	{
		ShapeDesc^ shapeDesc = desc->Shapes[i];

		if(shapeDesc->GetType() == BoxShapeDesc::typeid)  // Box primitive
		{
			BoxShapeDesc^ boxShape = (BoxShapeDesc^)shapeDesc;

			// Create PhysX box shape
			NxBoxShapeDesc boxDesc;
			boxDesc.dimensions.set(VECTOR3_TO_NXVEC3(boxShape->Extents));
			actorDesc.shapes.pushBack(&boxDesc);
		}
		else if(shapeDesc->GetType() == SphereShapeDesc::typeid)  // Sphere primitive
		{
			SphereShapeDesc^ sphereShape = (SphereShapeDesc^)shapeDesc;

			// Create PhysX sphere shape
			NxSphereShapeDesc sphereDesc;
			sphereDesc.radius = sphereShape->Radius;
			actorDesc.shapes.pushBack(&sphereDesc);
		}
		else if(shapeDesc->GetType() == HeightFieldShapeDesc::typeid)  // HeightField primitive
		{
			HeightFieldShapeDesc^ heightFieldShape = (HeightFieldShapeDesc^)shapeDesc;

			// Copy our height data to a local buffer
			float* data = new float[heightFieldShape->HeightField->GetLength(0) * heightFieldShape->HeightField->GetLength(1)];

			for(int i = 0; i < heightFieldShape->HeightField->GetLength(0); ++i)
			{
				for(int j = 0; j < heightFieldShape->HeightField->GetLength(1); ++j)
				{
					data[i * heightFieldShape->HeightField->GetLength(1) + j] = heightFieldShape->HeightField[i,j];
				}
			}

			// Create the height field shape
			NxHeightFieldShapeDesc shapeDesc = CreateHeightField(heightFieldShape->HeightField->GetLength(0), heightFieldShape->HeightField->GetLength(1), data, heightFieldShape->SizeX, heightFieldShape->SizeZ, physXScene);

			// Clean-up
			delete data;

			actorDesc.shapes.pushBack(&shapeDesc);
		}
		else if(shapeDesc->GetType() == TriangleMeshShapeDesc::typeid)  // TriMesh primitive (EXPERIMENTAL!)
		{
			TriangleMeshShapeDesc^ triMeshShape = (TriangleMeshShapeDesc^)shapeDesc;

			// Copy raw data to local buffers
			NxVec3* hfVerts = new NxVec3[triMeshShape->Vertices->Length];
			NxU32* hfFaces = new NxU32[triMeshShape->Indices->Length];

			for(int j = 0; j < triMeshShape->Vertices->Length; ++j)
			{
				hfVerts[j] = VECTOR3_TO_NXVEC3(triMeshShape->Vertices[j]);
			}

			for(int j = 0; j < triMeshShape->Indices->Length; ++j)
			{
				hfFaces[j] = triMeshShape->Indices[j];
			}


			// Create the tri-mesh shape
			NxTriangleMeshShapeDesc hfShapeDesc = CookTriangleMesh(triMeshShape->Vertices->Length, triMeshShape->Indices->Length / 3, hfVerts, hfFaces, physXScene);

			// Clean-up
			delete hfVerts;
			delete hfFaces;

			actorDesc.shapes.pushBack(&hfShapeDesc);
		}
	}

	// Store the shape list
	shapes = desc->Shapes;

	density = desc->Density;

	// Create the actor
	physXActor = physXScene->createActor(actorDesc);

	if(physXActor == NULL)
	{
		throw gcnew Exception(L"Failed to create PhysX actor.");
	}
}

// Default destructor
PhysXActor::~PhysXActor()
{
	if(physXActor != NULL)
	{
		physXScene->releaseActor(*physXActor);
	}
}

END_QS_NAMESPACE

