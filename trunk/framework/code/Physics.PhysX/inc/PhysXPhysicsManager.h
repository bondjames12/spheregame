// PhysXPhysicsManager.h
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

#if !defined(QUICKSTART_PHYSICS_PHYSX_PHYSXPHYSICSSYSTEM_INC)
#define QUICKSTART_PHYSICS_PHYSX_PHYSXPHYSICSSCENE_INC 1

BEGIN_QS_NAMESPACE  // QuickStart::Physics::PhysX

// Implementation of the QuickStart physics system using the PhysX engine.
public ref class PhysXPhysicsManager : public PhysicsManager
{
public:
	// Default constructor
	PhysXPhysicsManager(QSGame^ game);

	// Default destructor
	~PhysXPhysicsManager();

	// Creates a new physics scene.
	virtual IPhysicsScene^ CreateScene() override;

private:
	NxPhysicsSDK*	physXSDK;
};

END_QS_NAMESPACE


#endif
