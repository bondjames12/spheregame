// PhysXActor.h
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

#if !defined(QUICKSTART_PHYSICS_PHYSX_PHYSXACTOR_INC)
#define QUICKSTART_PHYSICS_PHYSX_PHYSXACTOR_INC 1

BEGIN_QS_NAMESPACE  // QuickStart::Physics::PhysX

// Implementation of the physics actor class for the PhysX physics simulator back-end.
public ref class PhysXActor : public IPhysicsActor
{
internal:
	// Constructs a new actor.
	PhysXActor(NxScene* scene, ActorDesc^ desc);

	// Default destructor.
	~PhysXActor();

public:
	// Sets the actor's density.
	property float Density
	{
		virtual float get()
		{
			return density;
		};
		virtual void set(float value)
		{
			physXActor->updateMassFromShapes(value, 0.0f);
			density = value;
		};
	};

	// Sets the actor's mass.
	property float Mass
	{
		virtual float get()
		{
			return physXActor->getMass();
		};
		virtual void set(float value)
		{
			physXActor->updateMassFromShapes(0.0f, value);
		};
	};

	// Retrieves the actor's position.
	property Vector3 Position
	{
		virtual Vector3 get()
		{
			return NXVEC3_TO_VECTOR3(physXActor->getGlobalPosition());
		};
		virtual void set(Vector3 vec)
		{
			physXActor->setCMassGlobalPosition(VECTOR3_TO_NXVEC3(vec));
		}
	};

	// Retrieves the actor's orientation as a transformation matrix.
	property Matrix Orientation
	{
		virtual Matrix get()
		{
			NxMat33 orient = physXActor->getGlobalOrientation();

			// A fast memcpy here would work wonders... oh well :(
			NxVec3 row0 = orient.getRow(0);
			NxVec3 row1 = orient.getRow(1);
			NxVec3 row2 = orient.getRow(2);

			Matrix ret = Matrix(row0.x, row0.y, row0.z, 0.0f, row1.x, row1.y, row1.z, 0.0f, row2.x, row2.y, row2.z, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
			return ret.Transpose(ret);
		};
		virtual void set(Matrix orient)
		{
			NxVec3 row0 = NxVec3(orient.M11, orient.M12, orient.M13);
			NxVec3 row1 = NxVec3(orient.M21, orient.M22, orient.M23);
			NxVec3 row2 = NxVec3(orient.M31, orient.M32, orient.M33);
			NxMat33 newOrient = NxMat33(row0, row1, row2);
			newOrient.setTransposed();

			physXActor->setCMassGlobalOrientation(newOrient);
		};
	};

	// Retrieves the list of shapes that make up the actor.
	property List<ShapeDesc^>^ Shapes
	{
		virtual List<ShapeDesc^>^ get()
		{
			return shapes;
		};
	};

protected:
	NxScene*			physXScene;
	NxActor*			physXActor;
	float density;

	List<ShapeDesc^>^	shapes;
};

END_QS_NAMESPACE

#endif
