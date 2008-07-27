﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XDebug : XComponent, XDrawable
    {
        public int RowHeight = 17;
        List<string> toWrite = new List<string>();
        List<string> toWriteStatic = new List<string>();

        public Vector2 StartPosition = new Vector2(5, 3);

        public XDebug(XMain X)
            : base(X)
        {
            AutoDraw = false;
        }

        public void Write(string Text, bool Static)
        {
            if (Static)
                toWriteStatic.Add(Text);
            else
                toWrite.Add(Text);
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            int offset = (int)StartPosition.Y;
            foreach (string Text in toWrite)
            {
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.White);
                offset += RowHeight;
            }
            toWrite.Clear();
            foreach (string Text in toWriteStatic)
            {
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.White);
                offset += RowHeight;
            }
        }
    }
}
