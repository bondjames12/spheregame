using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
}
