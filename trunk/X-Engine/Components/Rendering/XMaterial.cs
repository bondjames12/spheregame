using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XEngine;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XMaterial
    {
        Texture2D texture;
        Texture2D normalmap;
        public Vector4? diffuse;

        public bool EnableNormalMapping;
        public bool EnableTexureMapping;

        public CullMode VertexWinding;
        public bool AlphaBlendable;

        public float Specularity;

        public XMaterial(XMain X, string Texture, bool EnableTextureMapping, string NormalMap, bool EnableNormalMapping, Vector4? DiffuseColor, bool AlphaBlendable, float Specularity)
        {
            if (!string.IsNullOrEmpty(Texture))
                texture = X.Content.Load<Texture2D>(Texture);

            if (!string.IsNullOrEmpty(NormalMap))
                normalmap = X.Content.Load<Texture2D>(NormalMap);

            if (DiffuseColor != null)
                diffuse = DiffuseColor;
            else
                diffuse = new Vector4(1f, 1f, 1f, 1);

            VertexWinding = CullMode.CullCounterClockwiseFace;
            this.AlphaBlendable = AlphaBlendable;

            this.EnableTexureMapping = EnableTextureMapping;
            this.EnableNormalMapping = EnableNormalMapping;

            this.Specularity = Specularity;
        }

        public XMaterial(XMain X, Texture2D Texture, bool EnableTextureMapping, Texture2D NormalMap, bool EnableNormalMapping, Vector4? DiffuseColor, bool AlphaBlendable, float Specularity)
        {
            if (Texture != null)
                texture = Texture;

            if (NormalMap != null)
                normalmap = NormalMap;

            if (DiffuseColor != null)
                diffuse = DiffuseColor;
            else
                diffuse = new Vector4(1f, 1f, 1f, 1);

            VertexWinding = CullMode.CullCounterClockwiseFace;
            this.AlphaBlendable = AlphaBlendable;

            this.EnableTexureMapping = EnableTextureMapping;
            this.EnableNormalMapping = EnableNormalMapping;

            this.Specularity = Specularity;
        }

        public void SetupEffect(Effect effect)
        {
                effect.Parameters["Texture"].SetValue(texture);
           
                effect.Parameters["TexturingEnabled"].SetValue(EnableTexureMapping);
            
                effect.Parameters["NormalMap"].SetValue(normalmap);
            
                effect.Parameters["NormalMapEnabled"].SetValue(EnableNormalMapping);
            
           effect.Parameters["Diffuse"].SetValue((Vector4)diffuse);
        }
    }
}
