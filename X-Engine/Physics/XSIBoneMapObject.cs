using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Math;

namespace XEngine
{
    class XSIBoneMapObject : XPhysicsObject
    {
        public XSIBoneMapObject(Vector3 position,  ModelBoneCollection bones)
        {
            //create a list to hold the collision primitives
            List<Primitive> prims = new List<Primitive>();

            //parse for collision bones
            foreach (ModelBone bone in bones)
            {
                string[] keypairs = bone.Name.ToLower().Split('_');
                //if checks for valid naming convention on collision bones
                if((keypairs.Length == 2) && ((keypairs[0] == "sphere") || (keypairs[0] == "box") || (keypairs[0] == "capsule") ))
                {
                    //determine object number
                    int objectnum;
                    if ((keypairs[1] != "") || (keypairs[1] != null))
                        objectnum = int.Parse(keypairs[1]);
                    else
                        objectnum = 0;

                    //decompose bone transforms to components
                    Vector3 pos;
                    Vector3 scale;
                    Quaternion qrot;
                    bone.Transform.Decompose(out scale, out qrot, out pos);
                    Matrix rot = Matrix.CreateFromQuaternion(qrot);
                    
                    //create  collision primitive objects and add to list
                    switch(keypairs[0])
                    {
                        case("sphere"):
                            JigLibX.Geometry.Sphere sph = new JigLibX.Geometry.Sphere(pos, scale.X);
                            prims.Add(sph);
                            break;
                        case ("box"):
                            Box box = new Box(pos, rot, scale);
                            prims.Add(box);
                            break;
                        case ("capsule"):
                            break;
                    }
                }
            }

            body = new Body();

            if (prims.Count > 0)
            {
                collision = new CollisionSkin(body);
                //Elasticity = e,StaticRoughness = sr,DynamicRoughness = dr;
                foreach (Primitive prim in prims)
                {
                    collision.AddPrimitive(prim, (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.8f, 0.8f, 0.7f));
                }
                body.CollisionSkin = this.collision;

            }
            
            
            Vector3 com = SetMass(1.0f);
            body.MoveTo(position, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
        }
    }
}
