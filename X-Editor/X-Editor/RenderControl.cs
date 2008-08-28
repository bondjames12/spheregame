using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using XEngine;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;

namespace X_Editor
{
    public class RenderControl : GraphicsDeviceControl
    {
        //Timer redraw;
        public XMain X;
        public string ContentRootDir="";

        XEditorGameTime time = new XEditorGameTime();

        public XFreeLookCamera camera;
        public XKeyboard keyboard;
        public XMouse mouse;

        public RenderControl()
        {
            
        }

        public void Init()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            X = new XMain(this.GraphicsDevice,this.Services, ContentRootDir);
            //setup some basic usefull settings
            X.Gravity = new Vector3(0, -40, 0);
            X.FrameRate.DisplayFrameRate = true;
            X.Console.AutoDraw = false;
            X.Debug.StartPosition.Y = 200;

            X.LoadContent();

            X.Physics.EnableFreezing = false;

            time.TotalGameTime.Reset();
            time.TotalGameTime.Start();
            time.ElapsedGameTime.Reset();
            time.ElapsedGameTime.Start();

            hasFocus = false;
            dragdroprelease = false;
            SetupBaseComponents();

            //Begin draw timer!
            //redraw = new Timer();
            //redraw.Interval = 1;
            //redraw.Start();

            //redraw.Tick += new EventHandler(redraw_Tick);
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        public void SetupBaseComponents()
        {
            camera = new XFreeLookCamera(ref X,0.1f,1000f);
            //set the initial position of the camera such that we are looking at the center of the game world (0,0,0)
            camera.Position = new Vector3(30f, 15f, 40f);
            //so we look at center add in a rotation (trial and error)
            camera.Rotate(new Vector3(MathHelper.ToRadians(-10f), MathHelper.ToRadians(40f), 0));

            keyboard = new XKeyboard(ref X);
            mouse = new XMouse(ref X);
            mouse.Reset = false;
        }

        public bool hasFocus = false;
        public bool dragdroprelease = false;

        protected override void Draw()
        {
            this.GraphicsDevice.Clear(Color.Red);
            GameTime gameTime = new GameTime(time.TotalGameTime.Elapsed, time.ElapsedGameTime.Elapsed, time.TotalGameTime.Elapsed, time.ElapsedGameTime.Elapsed, false);

            X.Update(gameTime);

            //foreach(XComponent component in X.Components)
              //  X.Debug.Write(component.ToString(), false);

            if (hasFocus && keyboard != null && mouse != null)
            {
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                    camera.Translate(Vector3.Backward * 40);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    camera.Translate(Vector3.Forward * 40);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    camera.Translate(Vector3.Left * 40);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    camera.Translate(Vector3.Right * 40);

                if (mouse.ButtonPressed(XMouse.Buttons.Left))
                {
                    mouse.InitialPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    mouse.CurrentPosition = mouse.InitialPosition;
                    mouse.Delta = Vector2.Zero;
                    mouse.Reset = true;
                    //Cursor.Hide();
                }
                else if (mouse.ButtonReleased(XMouse.Buttons.Left))
                {
                    mouse.Reset = false;

                    if (!dragdroprelease)
                        Cursor.Show();
                    else
                        dragdroprelease = false;
                }

                if (mouse.ButtonDown(XMouse.Buttons.Left))
                    camera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));
            }

            
            X.Renderer.Draw(ref gameTime,ref camera.Base);

            time.ElapsedGameTime.Reset();
            time.ElapsedGameTime.Start();
            
            
        }

        void redraw_Tick(object sender, EventArgs e)
        {
            Invalidate();
            //Draw();
        }
    }
}
