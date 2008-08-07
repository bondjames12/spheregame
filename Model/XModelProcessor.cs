using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;

namespace ContentModel
{
    [ContentProcessor(DisplayName = "X-Engine Model Processor")]
    public class XEngineModelProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ModelData data = new ModelData(VertexHelper.FindVertices(input), null);

            ModelContent model = base.Process(input, context);
            model.Tag = data;

            return model;
        }

        protected override MaterialContent ConvertMaterial(MaterialContent material,
                                                ContentProcessorContext context)
        {
            //System.Diagnostics.Debugger.Launch();
            //the new effectmaterial we will be returning
            EffectMaterialContent effectMaterial;

            if (material is BasicMaterialContent)
            {
                
                BasicMaterialContent basicMaterial = material as BasicMaterialContent;

                effectMaterial = new EffectMaterialContent();
                // Store a reference to our skinned mesh effect.
                effectMaterial.Effect = new ExternalReference<EffectContent>("../../X-Engine/Content/XEngine/Effects/Model.fx");
                
                //copy over TextureReferenceDictionary
                //this won't work tex.name is allways null!
                //foreach(ExternalReference<TextureContent> tex in basicMaterial.Textures.Values)
                //{
                //    effectMaterial.Textures.Add(tex.Name, tex);
                //}

                // Copy texture settings from the input
                // BasicMaterialContent over to our new material.
                // Supposed to support multiple textures but I just take 1 (diffuse texture)
                if (basicMaterial.Texture != null)
                    effectMaterial.Textures.Add("Texture", basicMaterial.Texture);

                if (basicMaterial.Identity != null)
                    effectMaterial.Identity = basicMaterial.Identity;

                if (basicMaterial.Name != null)
                    effectMaterial.Name = basicMaterial.Name;

                //foreach(object obj in basicMaterial.OpaqueData)
                //        effectMaterial.OpaqueData.Add(obj.ToString

                // And you can also set any arbitrary effect parameters at this point
                //myMaterial.OpaqueData.Add("Shininess", basicMaterial.SpecularPower * 10);
                //myMaterial.OpaqueData.Add("BumpSize", 42);
            }
            else if (material is EffectMaterialContent)
            {
                effectMaterial =  (EffectMaterialContent)material;
                // todo: put something interesting here if you want
                // or don't bother if you don't want
                // whatever really...
            }
            else
                throw new Exception("huh? this is very odd");
            
            // Chain to the base ModelProcessor converter.
            return base.ConvertMaterial(effectMaterial, context);
        }

    }
}
