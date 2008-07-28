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
            BasicMaterialContent basicMaterial = material as BasicMaterialContent;

            EffectMaterialContent effectMaterial = new EffectMaterialContent();

            // Store a reference to our skinned mesh effect.
            string effectPath = "../../X-Engine/Content/XEngine/Effects/Model.fx";

            effectMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

            // Copy texture settings from the input
            // BasicMaterialContent over to our new material.
            if (basicMaterial.Texture != null)
            {
                effectMaterial.Textures.Add("Texture", basicMaterial.Texture);
            }

            // Chain to the base ModelProcessor converter.
            return base.ConvertMaterial(effectMaterial, context);
        }

    }
}
