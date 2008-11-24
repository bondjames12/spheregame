using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace LevelProcessor
{
  /// <summary>
  /// This ModelProcessor subclass fixes some problems with DXT and transparency 
  /// usage in the original ModelProcessor. Specifically, if there is full 
  /// transparency, but with color, in the source texture, the stock ModelProcessor
  /// will generate DXT1 transparent data, which will be all black. This processor 
  /// just forces all textures to DXT5, which is generally better behavior.
  ///
  /// Additionally, this processor can apply new shaders to geometry based on whether 
  /// it is skinned (has blend weights) or not.
  /// </summary>
  [ContentProcessor(DisplayName = "Dxt5 Model Processor - KiloWatt")]
  public class Dxt5ModelProcessor : ModelProcessor
  {
    public Dxt5ModelProcessor()
    {
      //  default to ColorKeyEnabled off
      this.ColorKeyEnabled = false;
      foundSkinning_ = false;
    }

    /// <summary>
    /// If set, force all textures to be DXT5 format.
    /// </summary>
    [DefaultValue(true)]
    public bool ForceDXT5 { get { return forceDxt5_; } set { forceDxt5_ = value; } }
    bool forceDxt5_ = true;

    bool foundSkinning_;
    protected bool FoundSkinning { get { return foundSkinning_; } }

    /// <summary>
    /// If non-empty, set all materials for all non-skinned geometry to use the given shader.
    /// If the string starts with a "+," use the base name of the existing shader and add the 
    /// name of this shader.
    /// </summary>
    [DefaultValue("")]
    public string ForceShader { get { return forceShader_; } set { forceShader_ = value; } }
    string forceShader_ = "";

    /// <summary>
    /// If non-empty, set all materials for all skinned geometry to use the given shader.
    /// If the string starts with a "+," use the base name of the existing shader and add the 
    /// name of this shader.
    /// </summary>
    [DefaultValue("")]
    public string ForceSkinnedShader { get { return forceSkinnedShader_; } set { forceSkinnedShader_ = value; } }
    string forceSkinnedShader_ = "";

    public override ModelContent Process(NodeContent input, ContentProcessorContext context)
    {
      foundSkinning_ = false;
      if (!forceShader_.Equals("") || !forceSkinnedShader_.Equals(""))
        ReplaceShaders(input, context, input.Identity);
      return base.Process(input, context);
    }
    
    /// <summary>
    /// Recurse the entire node structure, looking for materials that may need its 
    /// shader replacing.
    /// </summary>
    /// <param name="input">The node hierarchy to replace shaders in.</param>
    /// <param name="context">To generate errors.</param>
    protected virtual void ReplaceShaders(NodeContent input, ContentProcessorContext context,
        ContentIdentity identity)
    {
      MeshContent mc = input as MeshContent;
      if (mc != null)
        ReplaceShaders(mc, context, identity);
      foreach (NodeContent child in input.Children)
        ReplaceShaders(child, context, identity);
    }
    
    /// <summary>
    /// Examine each geometry, deciding whether it should have its shader replaced, 
    /// and if so to replace with a skinned or non-skinned shader.
    /// </summary>
    /// <param name="mc">The mesh to perhaps replace shaders on.</param>
    /// <param name="context">For reporting errors.</param>
    protected virtual void ReplaceShaders(MeshContent mc, ContentProcessorContext context,
        ContentIdentity identity)
    {
      foreach (GeometryContent gc in mc.Geometry)
      {
        //  figure out whether the geometry is skinned or not
        if (gc.Vertices == null)
          continue;
        bool isSkinned = VerticesAreSkinned(gc.Vertices, context);
        if (isSkinned)
          foundSkinning_ = true;
        MaybeReplaceMaterial(gc, context, isSkinned ? forceSkinnedShader_ : forceShader_, identity);
      }
    }
    
    /// <summary>
    /// If the replacement shader string is not empty, replace the shader of the 
    /// given material with the given replacement shader. Copy all shader parameters across.
    /// </summary>
    /// <param name="gc">The geometry content to replace the shader in.</param>
    /// <param name="context">For generating errors.</param>
    /// <param name="shader">The shader to replace to (or empty string, to do nothing).</param>
    protected virtual void MaybeReplaceMaterial(GeometryContent gc, ContentProcessorContext context, 
        string shader, ContentIdentity identity)
    {
      if (shader.Equals(""))
        return;
      EffectMaterialContent emc = new EffectMaterialContent();
      string prevEffect = "";
      foreach (KeyValuePair<string, object> kvp in gc.Material.OpaqueData)
      {
#if DEBUG
        context.Logger.LogMessage("param {0}: value {1}", kvp.Key, kvp.Value);
#endif
        if (kvp.Key.Equals("Effect"))
        {
          prevEffect = (kvp.Value as ExternalReference<EffectContent>).Filename;
        }
        else if (kvp.Key.Equals("CompiledEffect"))
        {
          throw new System.ArgumentException("Dxt5ModelProcessor cannot do effect substitution of already compiled effects.");
        }
        else
        {
          emc.OpaqueData.Add(kvp.Key, kvp.Value);
        }
      }
      foreach (KeyValuePair<string, ExternalReference<TextureContent>> tr in gc.Material.Textures)
      {
        emc.Textures.Add(tr.Key, tr.Value);
      }
      string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(identity.SourceFilename), shader);
      if (shader[0] == '+')
      {
        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(identity.SourceFilename),
            System.IO.Path.GetFileNameWithoutExtension(prevEffect) + shader.Substring(1) + ".fx");
      }
      context.Logger.LogImportantMessage("{2}: Replacing shader {0} to {1} for {3}", prevEffect, path, identity.SourceFilename, gc.Name);
      emc.OpaqueData.Add("Effect", new ExternalReference<EffectContent>(path));
      gc.Material = emc;
    }

    /// <summary>
    /// Return true if the vertex content contains skinning blend indices.
    /// </summary>
    /// <param name="vc">The vertex content to check.</param>
    /// <param name="context">For reporting errors.</param>
    /// <returns>true if skinning should be applied to these vertices</returns>
    protected virtual bool VerticesAreSkinned(VertexContent vc, ContentProcessorContext context)
    {
      return vc.Channels.Contains(VertexChannelNames.Weights());
    }

    /// <summary>
    /// Given the provided material, convert it, possibly forcing texture format to DXT5.
    /// </summary>
    /// <param name="material">The material to convert.</param>
    /// <param name="context">To generate errors.</param>
    /// <returns>The converted material using the Dxt5MaterialProcessor.</returns>
    protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
    {
      OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();
      processorParameters["ColorKeyColor"] = this.ColorKeyColor;
      processorParameters["ColorKeyEnabled"] = this.ColorKeyEnabled;
      processorParameters["TextureFormat"] = this.TextureFormat;
      processorParameters["GenerateMipmaps"] = this.GenerateMipmaps;
      processorParameters["ResizeTexturesToPowerOfTwo"] = this.ResizeTexturesToPowerOfTwo;
      processorParameters["ForceDXT5"] = this.ForceDXT5;
      return context.Convert<MaterialContent, MaterialContent>(material, typeof(Dxt5MaterialProcessor).Name, processorParameters);
    }
  }

  [ContentProcessor(DisplayName = "Dxt5 Material Processor - KiloWatt"), DesignTimeVisible(false)]
  public class Dxt5MaterialProcessor : MaterialProcessor
  {
    public Dxt5MaterialProcessor()
    {
    }

    [DefaultValue(true)]
    public bool ForceDXT5 { get { return forceDxt5_; } set { forceDxt5_ = value; } }
    bool forceDxt5_ = true;

    protected override ExternalReference<TextureContent> BuildTexture(string textureName, ExternalReference<TextureContent> texture, ContentProcessorContext context)
    {
      OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();
      processorParameters.Add("ColorKeyColor", this.ColorKeyColor);
      processorParameters.Add("ColorKeyEnabled", this.ColorKeyEnabled);
      processorParameters.Add("TextureFormat", this.TextureFormat);
      processorParameters.Add("GenerateMipmaps", this.GenerateMipmaps);
      processorParameters.Add("ResizeToPowerOfTwo", this.ResizeTexturesToPowerOfTwo);
      processorParameters.Add("ForceDXT5", this.ForceDXT5);
      return context.BuildAsset<TextureContent, TextureContent>(texture, typeof(Dxt5TextureProcessor).Name, processorParameters, null, null);
    }
  }

  [ContentProcessor(DisplayName = "Dxt5 Texture Processor - KiloWatt")]
  public class Dxt5TextureProcessor : TextureProcessor
  {
    public Dxt5TextureProcessor()
    {
    }

    [DefaultValue(true)]
    public bool ForceDXT5 { get { return forceDxt5_; } set { forceDxt5_ = value; } }
    bool forceDxt5_ = true;

    public override TextureContent Process(TextureContent input, ContentProcessorContext context)
    {
      if (input == null)
        throw new ArgumentNullException("input");
      if (context == null)
        throw new ArgumentNullException("context");

      if (!this.ForceDXT5)
        return base.Process(input, context);

      TextureContent ret = null;
      TextureProcessorOutputFormat fmt = this.TextureFormat;
      this.TextureFormat = TextureProcessorOutputFormat.NoChange;
      try
      {
        ret = base.Process(input, context);
        Type originalType = ret.Faces[0][0].GetType();
        if (originalType != typeof(Dxt5BitmapContent))
          ret.ConvertBitmapType(typeof(Dxt5BitmapContent));
      }
      finally
      {
        this.TextureFormat = fmt;
      }
      return ret;
    }
  }
}
