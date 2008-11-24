using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XEngine;

namespace KiloWatt.Base.Animation
{
  //  Create a ModelDraw instance to make animating and rendering a model simpler.
  //  The ModelDraw represents a given instance of a given model -- you can use 
  //  multiple ModelDraw instances targeted at the same Model to draw multiple 
  //  instances in different positions.
  public class ModelDraw
  {
    public ModelDraw(ref XMain X, Model m, string name)
    {
      this.X = X;
      name_ = name;
      model_ = m;

      //  I'll need the world-space pose of each bone
      matrices_ = new Matrix[m.Bones.Count];

      //  inverse bind pose for skinning pose only
      object ibp;
      if (((Dictionary<string, object>)m.Tag).TryGetValue("InverseBindPose", out ibp))
      {
        inverseBindPose_ = ibp as SkinnedBone[];
        CalculateIndices();
      }

      //  information about bounds, in case the bind pose contains scaling
      object bi;
      if (((Dictionary<string, object>)m.Tag).TryGetValue("BoundsInfo", out bi))
        boundsInfo_ = bi as BoundsInfo;
      if (boundsInfo_ == null)
        boundsInfo_ = new BoundsInfo(1, 0);

      //  pick apart the model, so I know how to draw the different pieces
      List<Chunk> chl = new List<Chunk>();
      foreach (ModelMesh mm in m.Meshes)
      {
        foreach (ModelMeshPart mmp in mm.MeshParts)
        {
          //  chunk is used to draw an individual subset
          Chunk ch = new Chunk();

          //  set up all the well-known parameters through the EffectConfig helper.
          //ch.Fx = new EffectConfig(ref X, mmp.Effect);
            ch.SAS = new SASContainer();
            ch.Fx = mmp.Effect;

          //  if this effect is skinned, set up additional data
          if (mmp.Effect.Parameters["Pose"] != null)
          {
            //  If I haven't built the pose, then build it now
            if (pose_ == null)
            {
              if (inverseBindPose_ == null)
                throw new System.ArgumentNullException(String.Format(
                    "The model {0} should have an inverse bone transform because it has a pose, but it doesn't.",
                    name));
              //  Send bones as sets of three 4-vectors (column major) to the shader
              pose_ = new Vector4[inverseBindPose_.Length * 3];
              for (int i = 0; i != inverseBindPose_.Length; ++i)
              {
                //  start out with the identity pose (which is terrible)
                pose_[i * 3 + 0] = new Vector4(1, 0, 0, 0);
                pose_[i * 3 + 1] = new Vector4(0, 1, 0, 0);
                pose_[i * 3 + 2] = new Vector4(0, 0, 1, 0);
              }
            }
            ch.SAS.Pose = pose_;
          }

          ch.Mesh = mm;
          ch.Part = mmp;

          //  check out whether the technique contains transparency
          EffectAnnotation ea = mmp.Effect.CurrentTechnique.Annotations["transparent"];
          if (ea != null && ea.GetValueBoolean() == true)
            ch.Deferred = true;

          chl.Add(ch);
        }
      }
      //  use a native array instead of a List<> for permanent storage
      chunks_ = chl.ToArray();
      //  calculate bounds information (won't take animation into account)
      CalcBoundingSphere();
    }

    //  verify that the bone indices are what I expect them to be (debugging help)
    void CalculateIndices()
    {
#if DEBUG
      string findBone = "";
      try
      {
        for (int i = 0, n = inverseBindPose_.Length; i != n; ++i)
        {
          findBone = inverseBindPose_[i].Name;
          int index = model_.Bones[findBone].Index;
          if (index != inverseBindPose_[i].Index)
          {
            throw new System.ArgumentException(String.Format("Bone {0} was re-indexed during import! from {1} to {2}.",
                findBone, inverseBindPose_[i].Index, index));
          }
          for (int j = 0, m = model_.Bones.Count; j != m; ++j)
          {
            if (j != inverseBindPose_[i].Index && model_.Bones[j].Name == findBone)
            {
              throw new System.ArgumentException(String.Format("Duplicate bone name found: {0}",
                  inverseBindPose_[i].Name));
            }
          }
        }
      }
      catch (System.Exception x)
      {
        throw new System.ArgumentException(
            String.Format("The required bone '{0}' could not be found.", findBone),
            x);
      }
#endif
    }
    
    public override string ToString() { return String.Format("ModelDraw: Name = {0}", name_); }

    BoundsInfo boundsInfo_;
    string name_;
    //  the name is used for debugging purposes
    public string Name { get { return name_; } }
    Model model_;
    //  get the original Model instance back
    public Model Model { get { return model_; } }
    Matrix[] matrices_;
    SkinnedBone[] inverseBindPose_;
    Vector4[] pose_;
    BoundingSphere boundingSphere_;
    public BoundingSphere BoundingSphere { get { return boundingSphere_; } }
    private XMain X;
    private XCamera camera_;
    private Matrix world_;
    Chunk[] chunks_;

    //  information needed to draw each piece, even if it's deferred for transparency
    class Chunk
    {
      //public EffectConfig Fx;
        public Effect Fx;
        public SASContainer SAS;
      public ModelMesh Mesh;
      public ModelMeshPart Part;
      public bool Deferred;
    }

    //  calculate the bounding sphere
    public BoundingSphere CalcBoundingSphere()
    {
      Vector3 a = Vector3.Zero;
      Vector3 b = Vector3.Zero;
      Vector3 center = Vector3.Zero;
      float longest = 0;
      //  use the bind pose that comes in at the start
      model_.CopyAbsoluteBoneTransformsTo(matrices_);
      foreach (ModelMesh m1 in model_.Meshes)
      {
        Vector3 c1 = Vector3.Transform(m1.BoundingSphere.Center, matrices_[m1.ParentBone.Index]);
        foreach (ModelMesh m2 in model_.Meshes)
        {
          //  calculate the pose of each bone, and the geometry that goes to that bone
          Vector3 c2 = Vector3.Transform(m2.BoundingSphere.Center, matrices_[m2.ParentBone.Index]);
          float l = (c2 - c1).Length();
          float d = m1.BoundingSphere.Radius + m2.BoundingSphere.Radius + l;
          if (d > longest)
          {
            //  merge the two bounding spheres
            longest = d;
            Vector3 q = (l < 1e-6) ? Vector3.Zero : (c2 - c1) * (1.0f / l);
            center = (c1 + c2) * 0.5f + q * ((m2.BoundingSphere.Radius - m1.BoundingSphere.Radius) * 0.5f);
          }
        }
      }
      //  using the bounding box calculated, return a new bounding sphere
      boundingSphere_ = new BoundingSphere(center, longest * 0.5f * boundsInfo_.MaxScale + boundsInfo_.MaxOffset);
      return boundingSphere_;
    }

    //  keep a list of instances that need deferred drawing
    internal static List<ModelDraw> deferred_ = new List<ModelDraw>();
    //  use a comparer to Z sort far to near
    internal static Comparison<ModelDraw> compareDistance_ = new Comparison<ModelDraw>(CompareDistance);

    //  for proper Z sorting, compare far to near
    internal static int CompareDistance(ModelDraw a, ModelDraw b)
    {
      float fa = Vector3.Dot(a.world_.Translation, a.camera_.ViewProjection.Backward);
      float fb = Vector3.Dot(b.world_.Translation, b.camera_.ViewProjection.Backward);
      if (fa < fb) return -1;
      if (fa > fb) return 1;
      return 0;
    }

    //  actually draw the elements that have been marked deferred
    public static void DrawDeferred()
    {
      if (deferred_.Count > 0)
      {
        deferred_.Sort(compareDistance_);
        foreach (ModelDraw md in deferred_)
          md.Draw2();
        deferred_.Clear();
      }
    }

    //  Only call Draw() once per object instance per frame. Else 
    //  transparently sorted pieces won't draw correctly, as there is 
    //  only one set of state per ModelDraw. Use multiple ModelDraw
    //  instances for multiple object instances.
    //  Immediately draw the parts that do not require transparency.
    //  put parts that need transparency on a deferred list, to be 
    //  drawn later (z sorted) using DrawDeferred().
    //  The worldspace transform of the object should be in dd.world.
    public void Draw(ref XCamera Camera, Matrix World, IAnimationInstance instance)
    {
      //  if animating, then pose it
      if (instance != null)
      {
        Matrix temp;
        Keyframe[] kfs = instance.CurrentPose;
        unchecked
        {
          int i = 0, n = matrices_.Length;
          foreach (Keyframe kf in kfs)
          {
            if (i == n)
              break;
            if (kf != null)
            {
              kf.ToMatrix(out temp);
              //  set up the model in parent-relative pose
              model_.Bones[i].Transform = temp;
            }
            ++i;
          }
        }
      }
      //  get the object-relative matrices (object->world is separate)
      model_.CopyAbsoluteBoneTransformsTo(matrices_);
#if DEBUG
      //  draw the skeleton, but only in debug mode
      foreach (ModelBone mb in model_.Bones)
      {
        if (mb.Parent != null)
        {
            Matrix m = matrices_[mb.Index] * World;
          Vector3 c = m.Translation;
          X.DebugDrawer.DrawLine(
              c,
              (matrices_[mb.Parent.Index]*World).Translation,
              Color.White);
          X.DebugDrawer.DrawLine(
              c,
              c + m.Right * 0.5f,
              Color.Red);
          X.DebugDrawer.DrawLine(
              c,
              c + m.Up * 0.5f,
              Color.Green);
          X.DebugDrawer.DrawLine(
              c,
              c + m.Backward * 0.5f,
              Color.Blue);
        }
      }
#endif
      //  If I have a 3x4 matrix pose, then generate that for skinning
      if (pose_ != null)
        GeneratePose();
      //  chain to an internal helper
      Draw(ref Camera, World, false);
    }

    void GeneratePose()
    {
      unchecked
      {
        Matrix mat;
        for (int i = 0, n = inverseBindPose_.Length, j = 0; i != n; ++i)
        {
          //  todo: I really could hoist this to the animation keyframes themselves,
          //  and pre-transform the geometry by the bone matrices
          SkinnedBone sb = inverseBindPose_[i];
          Matrix.Multiply(ref sb.InverseBindTransform, ref matrices_[sb.Index], out mat);
          pose_[j++] = new Vector4(mat.M11, mat.M21, mat.M31, mat.M41);
          pose_[j++] = new Vector4(mat.M12, mat.M22, mat.M32, mat.M42);
          pose_[j++] = new Vector4(mat.M13, mat.M23, mat.M33, mat.M43);
        }
      }
    }

    //  callback for transparent drawing
    internal void Draw2()
    {
      Draw(ref camera_, world_, true);
    }

    //  Draw helper that actually issues geometry, or defers for later.
    internal void Draw(ref XCamera Camera, Matrix world, bool asDeferred)
    {
      bool added = false;
      //  each chunk is drawn separately, as it represents a different 
      //  shader set-up (world transform, texture, etc).
      foreach (Chunk ch in chunks_)
      {
        if (asDeferred)
        {
          //  If I'm called back to draw deferred pieces, don't draw if this 
          //  piece is not deferred.
          if (!ch.Deferred)
            continue;
        }
        else if (ch.Deferred)
        {
          //  if the chunk is deferred, and I'm not being drawn for deferred 
          //  pieces right now, then add me to the list of things to draw 
          //  later.
          if (!added)
          {
            added = true;
            //  lazy allocation of drawDetails_ copy, only if needed
            //if (Camera == null)
            //  deferDetails_ = new DrawDetails();
            //dd.CopyTo(deferDetails_);
            //  use the original world matrix, not the working temp in the dd
            //deferDetails_.world = world;
            camera_ = Camera;
            world_ = world;
            deferred_.Add(this);
          }
          continue;
        }
        DrawChunk(ref Camera, ch, ref world);
      }
    }

    //  do the device and effect magic to render a given chunk of geometry
    private void DrawChunk(ref XCamera Camera, Chunk ch, ref Matrix world)
    {
      //  configure the device and actually draw
      X.GraphicsDevice.Indices = ch.Mesh.IndexBuffer;
      X.GraphicsDevice.VertexDeclaration = ch.Part.VertexDeclaration;
      X.GraphicsDevice.Vertices[0].SetSource(ch.Mesh.VertexBuffer, ch.Part.StreamOffset, ch.Part.VertexStride);
      //  note: calculating the world matrix overrides the previous value, hence the use
      //  of the saved copy of the world transform
      Matrix.Multiply(ref matrices_[ch.Mesh.ParentBone.Index], ref world, out world_);
      
        //ch.Fx.Setup();
        ch.SAS.Projection = Camera.Projection;
        ch.SAS.World = world_;
        ch.SAS.View = Camera.View;

        ch.SAS.ComputeViewAndProjection();
        ch.SAS.ComputeModel();

      for (int k = 0; k < ch.Fx.Parameters.Count; k++)
          ch.SAS.SetEffectParameterValue(ch.Fx.Parameters[k]);

      ch.Fx.Begin(); //  be lazy and save state
      EffectTechnique et = ch.Fx.CurrentTechnique;
      //  most my effects are single-pass, but at least transparency is multi-pass
      for (int i = 0, n = et.Passes.Count; i != n; ++i)
      {
        EffectPass ep = et.Passes[i];
        ep.Begin();
        X.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, ch.Part.BaseVertex,
            0, ch.Part.NumVertices, ch.Part.StartIndex, ch.Part.PrimitiveCount);
        ep.End();
      }
      ch.Fx.End();
    }
  }
}
