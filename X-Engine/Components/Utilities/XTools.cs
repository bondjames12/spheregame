using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XTools
    {
        XMain X;

        private Random rand = new Random();

        public XTools(XMain X)
        {
            this.X = X;
        }

        public int GetRandomInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        public float GetRandomFloat(float min, float max)
        {
            return ((float)rand.NextDouble() * (max - min)) + min;
        }

        public Vector2 GetRandomVector2(Vector2 min, Vector2 max)
        {
            return new Vector2(GetRandomFloat(min.X, max.X), GetRandomFloat(min.Y, max.Y));
        }

        public Vector3 GetRandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(GetRandomFloat(min.X, max.X), GetRandomFloat(min.Y, max.Y), GetRandomFloat(min.Z, max.Z));
        }

        public Vector2 GetRandomPointOnCircle(float radius, float height)
        {
            Random random = new Random();

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector2(x * radius, y * radius + height);
        }

        public Vector3 ConvertVector4ToVector3(Vector4 vector4)
        {
            return new Vector3(vector4.X, vector4.Y, vector4.Z);
        }

        public Vector4 ConvertVector3ToVector4(Vector3 vector3)
        {
            return new Vector4(vector3, 1.0f);
        }

        public BoundingBox CreateBoundingBox(Model model)
        {
            Vector3 max = Vector3.Zero;
            Vector3 min = Vector3.Zero;

            foreach (ModelMesh mesh in model.Meshes)
            {
                int vertexCount = mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes;

                VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[vertexCount];
                mesh.VertexBuffer.GetData(verts);

                for (int i = 0; i < vertexCount; i++)
                {
                    if (min.X > verts[i].Position.X) min.X = verts[i].Position.X;
                    if (max.X < verts[i].Position.X) max.X = verts[i].Position.X;
                    if (min.Y > verts[i].Position.Y) min.Y = verts[i].Position.Y;
                    if (max.Y < verts[i].Position.Y) max.Y = verts[i].Position.Y;
                    if (min.Z > verts[i].Position.Z) min.Z = verts[i].Position.Z;
                    if (max.Z < verts[i].Position.Z) max.Z = verts[i].Position.Z;
                }
            }

            return new BoundingBox(min, max);
        }

        /// <param name="Input">Format: (X, Y, Z)</param>
        public Vector3 ParseVector3(string Input)
        {
            Input = Input.Replace("(", "");
            Input = Input.Replace(")", "");
            Input = Input.Replace(" ", "");

            string[] values = Input.Split(',');

            if (values.Length == 3)
                return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            else
                return Vector3.Zero;
        }

        /// <param name="Input">Format: {X:# Y:# Z:#}</param>
        public Vector3 ParseXMLVector3(string Input)
        {
            Input = Input.Replace("{", "");
            Input = Input.Replace("}", "");
            Input = Input.Replace(":", "");
            Input = Input.Replace("X", "");
            Input = Input.Replace("Y", "");
            Input = Input.Replace("Z", "");
            Input = Input.Replace(" ", ",");

            string[] values = Input.Split(',');

            if (values.Length == 3 || values.Length == 4)
                return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            else
                return Vector3.Zero;
        }

        public Vector2 ParseXMLVector2(string Input)
        {
            Input = Input.Replace("{", "");
            Input = Input.Replace("}", "");
            Input = Input.Replace(":", "");
            Input = Input.Replace("X", "");
            Input = Input.Replace("Y", "");
            Input = Input.Replace(" ", ",");

            string[] values = Input.Split(',');

            if (values.Length == 2)
                return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
            else
                return Vector2.Zero;
        }

        /// <param name="Input">Format: { {M11:# M12:# M13:# M14:#} {M21:# M22:# M23:# M24:#} {M31:# M32:# M33:# M34:#} {M41:# M42:# M43:# M44:#} }</param>
        public Matrix ParseMatrix(string Input)
        {
            Input = Input.Replace("{", "");
            Input = Input.Replace("}", "");
            //Input = Input.Replace(" ", "");

            string[] values = Input.Split(' ');

            Matrix res = new Matrix();

            foreach (string val in values)
            {
                string[] keypair = val.Split(':');
                if (keypair.Length == 2)
                {
                    switch (keypair[0])
                    {
                        case "M11":
                            res.M11 = float.Parse(keypair[1]);
                            break;
                        case "M12":
                            res.M12 = float.Parse(keypair[1]);
                            break;
                        case "M13":
                            res.M13 = float.Parse(keypair[1]);
                            break;
                        case "M14":
                            res.M14 = float.Parse(keypair[1]);
                            break;
                        case "M21":
                            res.M21 = float.Parse(keypair[1]);
                            break;
                        case "M22":
                            res.M22 = float.Parse(keypair[1]);
                            break;
                        case "M23":
                            res.M23 = float.Parse(keypair[1]);
                            break;
                        case "M24":
                            res.M24 = float.Parse(keypair[1]);
                            break;
                        case "M31":
                            res.M31 = float.Parse(keypair[1]);
                            break;
                        case "M32":
                            res.M32 = float.Parse(keypair[1]);
                            break;
                        case "M33":
                            res.M33 = float.Parse(keypair[1]);
                            break;
                        case "M34":
                            res.M34 = float.Parse(keypair[1]);
                            break;
                        case "M41":
                            res.M41 = float.Parse(keypair[1]);
                            break;
                        case "M42":
                            res.M42 = float.Parse(keypair[1]);
                            break;
                        case "M43":
                            res.M43 = float.Parse(keypair[1]);
                            break;
                        case "M44":
                            res.M44 = float.Parse(keypair[1]);
                            break;
                        default:
                            break;
                    }
                }
            }
            return res;
        }

        public Vector3 GetPositionOnHeightMap(Vector3 Position, HeightmapObject Actor, Vector3 Offset)
        {
            return new Vector3(Position.X, Actor.info.GetHeight(Position), Position.Z) + Offset;
        }

        public float Max(float one, float two)
        {
            if (one > two)
                return one;
            else
                return two;
        }

        public float Min(float one, float two)
        {
            if (one < two)
                return one;
            else
                return two;
        }

        public Rectangle GetTitleSafeArea(float Percentage)
        {
            return new Rectangle((int)(X.GraphicsDevice.Viewport.Width * (1f - Percentage)),
                (int)(X.GraphicsDevice.Viewport.Height * (1f - Percentage)),
                (int)(X.GraphicsDevice.Viewport.Width * Percentage),
                (int)(X.GraphicsDevice.Viewport.Height * Percentage));
        }

        public void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        public Vector3 UnprojectVector3(Vector3 Vector, XCamera Camera, Matrix World)
        {
            return X.GraphicsDevice.Viewport.Unproject(Vector, Camera.Projection, Camera.View, World);
        }
    }
}