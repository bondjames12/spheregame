// PhysXScene.h
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

#if !defined(QUICKSTART_PHYSICS_PHYSX_PHYSXSCENE_INC)
#define QUICKSTART_PHYSICS_PHYSX_PHYSXSCENE_INC 1

BEGIN_QS_NAMESPACE // QuickStart::Physics::PhysX

// Implementation of the QuickStart physics scene interface using the PhysX engine.
public ref class PhysXScene : public IPhysicsScene
{
internal:
	// Constructs a new scene.
	PhysXScene(NxPhysicsSDK* sdk);

	// Default destructor.
	~PhysXScene();

public:
	// Gets/sets the scene gravity vector.
	property Vector3 Gravity
	{
		virtual Vector3 get()
		{
			NxVec3 grav;
			physXScene->getGravity(grav);

			return NXVEC3_TO_VECTOR3(grav);
		};

		virtual void set(Vector3 value)
		{
			physXScene->setGravity(VECTOR3_TO_NXVEC3(value));
		};
	};

	// Creates a new physics actor from the given ActorDesc.
	virtual IPhysicsActor^ CreateActor(ActorDesc^ desc);

	// Begin processing a physics simulation frame.
	virtual void BeginFrame(GameTime^ gameTime);

	// Blocks until the current physics frame is complete.
	virtual void EndFrame();

private:
	NxPhysicsSDK*	physXSDK;
	NxScene*		physXScene;
};

END_QS_NAMESPACE

#endif
