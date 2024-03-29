using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace KiloWatt.Pipeline
{
  [ContentProcessor(DisplayName = "Bounding Calculator")]
  public class BoundingCalculator : ModelProcessor
  {
    public BoundingCalculator()
    {
      base.ColorKeyEnabled = false;
      base.SwapWindingOrder = true;
    }

    public override ModelContent Process(NodeContent input, ContentProcessorContext context)
    {
 	    ModelContent ret = base.Process(input, context);
      CalculateBounding(input, ret);
      return ret;
    }

    public override bool SwapWindingOrder
    {
      get
      {
        return true;
      }
      set
      {
        base.SwapWindingOrder = true;
      }
    }

    public override bool ColorKeyEnabled
    {
      get {
        return false;
      }
      set {
        base.ColorKeyEnabled = false;
      }
    }

    /// <summary>
    /// Calculate a bounding box for the model.
    /// </summary>
    /// <param name="model">The model to calculate AABBs for</param>
    public static void CalculateBounding(NodeContent input, ModelContent model)
    {
      BoundingBox box = new BoundingBox();
      BoundingSphere sphere = new BoundingSphere();
      CalculateBounding(input, ref box, ref sphere);
      if (model.Tag == null)
        model.Tag = new Dictionary<string, object>();
      (model.Tag as Dictionary<string, object>).Add("BoundingBox", box);
      (model.Tag as Dictionary<string, object>).Add("BoundingSphere", sphere);
    }
    
    public static void CalculateBounding(NodeContent node, ref BoundingBox box, ref BoundingSphere sphere)
    {
      if (node is MeshContent)
      {
        MeshContent mc = node as MeshContent;
        Vector3[] pts = new Vector3[mc.Positions.Count];
        Fill(pts, mc.Positions);
        Matrix mat = mc.AbsoluteTransform;
        Vector3.Transform(pts, ref mat, pts);
        BoundingBox b2 = BoundingBox.CreateFromPoints(pts);
        box = BoundingBox.CreateMerged(box, b2);
        sphere = BoundingSphere.CreateFromBoundingBox(box);
      }
      foreach (NodeContent cld in node.Children)
      {
        CalculateBounding(cld, ref box, ref sphere);
      }
    }

    public static void Fill(Vector3[] ary, IEnumerable<Vector3> en)
    {
      IEnumerator<Vector3> e = en.GetEnumerator();
      int ix = 0;
      while (e.MoveNext())
      {
        ary[ix++] = e.Current;
      }
      System.Diagnostics.Debug.Assert(ix == ary.Length);
    }
  }
}
