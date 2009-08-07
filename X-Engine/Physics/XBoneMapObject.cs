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
    class XBoneMapObject : XPhysicsObject
    {
        public XBoneMapObject(Vector3 position,  ref XModel model)
        {
            //create a list to hold the collision primitives
            List<Primitive> prims = new List<Primitive>();

            //parse for collision bones
            foreach (ModelBone bone in model.Model.Bones)
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
            collision = new CollisionSkin(body);

            if (prims.Count > 0)
            {
                foreach (Primitive prim in prims)
                {
                    //TODO: Add ability to specify physics material type in art editor somehow
                    collision.AddPrimitive(prim, (int)JigLibX.Collision.MaterialTable.MaterialID.NormalSmooth);
                }
            }
            else
            {//no collision prims detected from XSI so create a default one here using the mesh bounding spheres
                foreach(ModelMesh mesh in model.Model.Meshes)
                {
                    //collision.AddPrimitive(new JigLibX.Geometry.Sphere(mesh.BoundingSphere.Center, mesh.BoundingSphere.Radius), new MaterialProperties(0.8f, 0.8f, 0.7f));
                    collision.AddPrimitive(new JigLibX.Geometry.Box(position, Matrix.Identity,new Vector3(mesh.BoundingSphere.Radius / 2f)),(int) JigLibX.Collision.MaterialTable.MaterialID.NormalRough);
                }
            }

            body.CollisionSkin = this.collision;
            
            Vector3 com = SetMass(1.0f);
            body.MoveTo(position, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
        }
    }
}
