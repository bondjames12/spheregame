using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace ContentModel
{
    public class ModelData
    {
        public List<object> VertexData;
        public SkinningData SkinningData;

        public ModelData(List<object> VertexData, SkinningData SkinningData)
        {
            this.VertexData = VertexData;
            this.SkinningData = SkinningData;
        }
    }

    [ContentTypeWriter]
    public class ModelDataWriter : ContentTypeWriter<ModelData>
    {
        protected override void Write(ContentWriter output, ModelData value)
        {
            output.WriteObject(value.VertexData);
            output.WriteObject(value.SkinningData);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelDataReader).AssemblyQualifiedName;
        }
    }

    public class ModelDataReader : ContentTypeReader<ModelData>
    {
        protected override ModelData Read(ContentReader input,
                                             ModelData existingInstance)
        {
            ModelData Data = new ModelData(input.ReadObject<List<object>>(), input.ReadObject<SkinningData>());

            return Data;
        }
    }
}
