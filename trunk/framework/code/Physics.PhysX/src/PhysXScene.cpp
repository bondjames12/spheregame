// PhysXScene.cpp
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

// Every part of the PhysX SDK should be treated as native code.
#pragma unmanaged
#include "NxPhysics.h"
#pragma managed

using namespace System;
using namespace System::Collections::Generic;
using namespace QuickStart;

using namespace Microsoft::Xna::Framework;

#include "CommonDefs.h"
#include "PhysXScene.h"
#include "PhysXActor.h"

BEGIN_QS_NAMESPACE  // QuickStart::Physics::PhysX

// Default constructor
PhysXScene::PhysXScene(NxPhysicsSDK* sdk)
{
	physXSDK = sdk;

	// Create a software scene for now.  We can worry about hardware support later.
	NxSceneDesc desc;
	desc.simType = NX_SIMULATION_SW;
	
	// TODO: Setup threading based on number of processors on the system
	desc.flags |= NX_SF_ENABLE_MULTITHREAD;
    desc.internalThreadCount = 2;
	desc.backgroundThreadCount = 2;
	//desc.threadMask = 0x0000000F;

	// Create the scene
	physXScene = physXSDK->createScene(desc);
	if(physXScene == NULL)
	{
		throw gcnew Exception(L"Failed to create PhysX scene.  Please ensure that all scene parameters are valid.");
	}
	
	// Setup default material
	NxMaterial* defaultMaterial = physXScene->getMaterialFromIndex(0); 
    defaultMaterial->setRestitution(0.5f);
    defaultMaterial->setStaticFriction(0.75f);
    defaultMaterial->setDynamicFriction(0.75f);
}

// Default destructor
PhysXScene::~PhysXScene()
{
	if(physXScene != NULL)
	{
		physXSDK->releaseScene(*physXScene);
	}
}

// Creates a new actor.
IPhysicsActor^ PhysXScene::CreateActor(ActorDesc^ desc)
{
	return gcnew PhysXActor(physXScene, desc);
}

// Starts processing of a new physics frame.
void PhysXScene::BeginFrame(GameTime^ gameTime)
{
	float dt = (float)gameTime->ElapsedGameTime.TotalMilliseconds / 1000.0f;

	physXScene->simulate(dt);
	physXScene->flushStream();
}

// Blocks until processing of the current frame has completed.
void PhysXScene::EndFrame()
{
	while(!physXScene->fetchResults(NX_RIGID_BODY_FINISHED, false));
}

END_QS_NAMESPACE

