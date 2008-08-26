using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XTextureSequence
    {
        public Texture2D[] Frames;

        public XTextureSequence(Texture2D[] frames)
        {
            this.Frames = frames;
        }
    }
}
