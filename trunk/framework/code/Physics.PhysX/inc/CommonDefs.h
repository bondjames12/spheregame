// CommonDefs.h
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

#if !defined(QUICKSTART_PHYSICS_PHYSX_COMMONDEFS_INC)
#define QUICKSTART_PHYSICS_PHYSX_COMMONDEFS_INC 1

// Quick shortcut for namespace definitions
#define BEGIN_QS_NAMESPACE namespace QuickStart { \
						   namespace Physics { \
						   namespace PhysX {

#define END_QS_NAMESPACE } } };

// Conversion macros for XNA Vector3 and PhysX NxVec3
#define NXVEC3_TO_VECTOR3(vec) Vector3(vec.x, vec.y, vec.z)
#define VECTOR3_TO_NXVEC3(vec) NxVec3(vec.X, vec.Y, vec.Z)

#endif
