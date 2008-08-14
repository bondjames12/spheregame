using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Sphere
{
    public class MenuManager : XComponent, XUpdateable, XDrawable
    {
        Texture2D Background;
        Texture2D Button;
        Texture2D OffButton;
        Texture2D Black;
        SpriteFont Font;
        //Texture2D logo;

        public List<string> Buttons = new List<string>();

        public bool MenuOpen;

        public int Current = 1;

        float scrollamount = 0;
        bool updating = false;
        int dir = 0;

        public MenuManager(XMain X) : base(X)
        {
            DrawOrder = 500;
            
            Buttons.Add("Button 1");
            Buttons.Add("Button 2");
            Buttons.Add("Exit");
        }

        public override void Load(ContentManager Content)
        {
            XTextureGenerator gen = new XTextureGenerator(X);
            
            Background = gen.GetFlatColor(new Color(33, 57, 109));
            Button = gen.GetFlatColor(new Color(251, 149, 36));
            OffButton = gen.GetFlatColor(new Color(35, 43, 87));
            Black = gen.GetFlatColor(Color.Black);

            Font = Content.Load<SpriteFont>(@"Content\Textures\Fonts\Menu");
            //logo = Content.Load<Texture2D>(@"Content\Textures\xengine");
        }

        public override void Draw(ref GameTime gameTime, ref XCamera Camera)
        {
            if (!MenuOpen) return; //the menu is not open DON'T DRAW IT
            X.spriteBatch.Draw(Black, new Rectangle(X.GraphicsDevice.Viewport.Width - ((int)(X.GraphicsDevice.Viewport.Width * scrollamount)), 0, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 25));
            X.spriteBatch.Draw(Background, new Rectangle(100, X.GraphicsDevice.Viewport.Height - ((int)(X.GraphicsDevice.Viewport.Height * scrollamount)), 400, X.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 191));

            int num = 0;
            foreach (string str in Buttons)
            {
                if (Buttons.IndexOf(str) == Current)
                {
                    X.spriteBatch.Draw(Button, new Rectangle(110, X.GraphicsDevice.Viewport.Height - ((int)((X.GraphicsDevice.Viewport.Height-50) * scrollamount)) + num * 34, 380, 30), new Color(255, 255, 255, 153));
                    X.spriteBatch.DrawString(Font, str, new Vector2(115, X.GraphicsDevice.Viewport.Height - ((int)((X.GraphicsDevice.Viewport.Height - 55) * scrollamount)) + num * 34), Color.White);
                }
                else
                {
                    X.spriteBatch.Draw(OffButton, new Rectangle(110, X.GraphicsDevice.Viewport.Height - ((int)((X.GraphicsDevice.Viewport.Height - 50) * scrollamount)) + num * 34, 380, 30), new Color(255, 255, 255, 153));
                    X.spriteBatch.DrawString(Font, str, new Vector2(115, X.GraphicsDevice.Viewport.Height - ((int)((X.GraphicsDevice.Viewport.Height - 55) * scrollamount)) + num * 34), Color.LightGray);
                }

                num++;
            }

            //X.spriteBatch.Draw(logo, new Rectangle(X.GraphicsDevice.Viewport.Width - 230, X.GraphicsDevice.Viewport.Height - 176, 220, 166), new Color(255, 255, 255, 127));

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;
        }

        public void Open()
        {
            updating = true;
            dir = 1;
            scrollamount = 0;
            MenuOpen = true;
        }

        public void Close()
        {
            updating = true;
            dir = -1;
            scrollamount = 1;
            MenuOpen = false;
        }

        public override void Update(ref GameTime gameTime)
        {
            if (updating)
            {
                if (scrollamount >= 1 && dir == 1)
                {
                    updating = false;
                    dir = 0;
                }
                else if (scrollamount <= -1 && dir == -1)
                {
                    updating = false;
                    dir = 0;
                }
                else
                    scrollamount += (float)gameTime.ElapsedGameTime.TotalSeconds * 4 * dir;
            }
        }

        public void SelectNext()
        {
            Current++;
            if (Current == Buttons.Count)
                Current = 0;
        }

        public void SelectPrevious()
        {
            Current--;
            if (Current == -1)
                Current = Buttons.Count - 1;
        }

        public void Toggle()
        {
            if (MenuOpen)
                Close();
            else
                Open();
        }
    }
}
