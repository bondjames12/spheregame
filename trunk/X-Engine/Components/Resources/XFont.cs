using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XFont : XComponent
    {
        SpriteFont font;
        string filename;

        public XFont(XMain X, string Filename) : base(ref X)
        {
            filename = Filename;
        }

        public override void Load(ContentManager Content)
        {
            font = Content.Load<SpriteFont>(filename);
            base.Load(Content);
        }

        public void Draw(string Text, Vector2 Location, Color color)
        {
            X.spriteBatch.DrawString(font, Text, Location, color);
        }
    }
}
