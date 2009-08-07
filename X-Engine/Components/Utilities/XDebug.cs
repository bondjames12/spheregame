using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XDebug : XComponent, XIDrawable
    {
        public int RowHeight = 17;
        List<string> toWrite = new List<string>();
        List<string> toWriteStatic = new List<string>();

        public Vector2 StartPosition = new Vector2(5, 3);

        public XDebug(XMain X)
            : base(ref X)
        {
            DrawOrder = 300;
            AutoDraw = true;
        }

        public void Write(string Text, bool Static)
        {
            if (Static)
                toWriteStatic.Add(Text);
            else
                toWrite.Add(Text);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            int offset = (int)StartPosition.Y;

            if (X.DebugMode)
            {
                //write debug text with the camera position passed into this draw method
                string Text = "Current Camera Name: " + Camera.Name;
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.Black);
                offset += RowHeight;
                Text = "Curent Camera Position X" + Camera.Position.X.ToString() + " Y:" + Camera.Position.Y.ToString() + " Z:" + Camera.Position.Z.ToString();
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.Black);
                offset += RowHeight;     
            }
            for(int i=0;i<toWrite.Count;i++)
            {
                string Text = toWrite[i];
            
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.Black);
                offset += RowHeight;
            }
            toWrite.Clear();

            for(int j=0;j<toWriteStatic.Count;j++)
            {
                string Text = toWriteStatic[j];
            
                X.SystemFont.Draw(Text, new Vector2(StartPosition.X, offset), Color.Black);
                offset += RowHeight;
            }
        }
    }
}
