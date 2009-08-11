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
        //control events
        public delegate void SelectedComponentChange(object sender, XComponent selectedObj);
        public event SelectedComponentChange OnSelectedComponentChange;

        //Timer redraw;
        public XMain X;
        public string ContentRootDir="";

        XEditorGameTime time;

        public XFreeLookCamera camera;
        public XKeyboard keyboard;
        public XMouse mouse;

        public Manipulator mManipulator;
        private XPickBuffer mPickBuffer;
        private WinFormsInput mInput;

        public Manipulator Manipulator
        {
            get { return mManipulator; }
        }


        public RenderControl()
        {
            
        }

        public void Init()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            time = new XEditorGameTime();
            X = new XMain(this.GraphicsDevice,this.Services, ContentRootDir, camera);
            //setup some basic usefull settings
            X.Gravity = new Vector3(0, -40, 0);
            X.UpdatePhysics = false;
            X.FrameRate.DisplayFrameRate = true;
            X.Console.AutoDraw = false;
            X.Debug.StartPosition.Y = 200;

            camera = new XFreeLookCamera(ref X, 0.1f, 1000f);
            //set the initial position of the camera such that we are looking at the center of the game world (0,0,0)
            camera.Position = new Vector3(30f, 15f, 40f);
            //so we look at center add in a rotation (trial and error)
            camera.Rotate(new Vector3(MathHelper.ToRadians(-10f), MathHelper.ToRadians(40f), 0));

            X.LoadContent();

            X.Physics.EnableFreezing = true;

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
            keyboard = new XKeyboard(ref X);
            mouse = new XMouse(ref X);
            mouse.Reset = false;

            mInput = new WinFormsInput(this);

            mManipulator = new Manipulator(this.graphicsDeviceService, camera, mInput);
            mManipulator.EnabledModes = TransformationMode.TranslationAxis;

            mPickBuffer = new XPickBuffer(this.graphicsDeviceService);

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (mManipulator.Focused)
                return;

            if ((GraphicsDevice == null) || GraphicsDevice.IsDisposed)
                return;

            Viewport vp = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = mInput.Viewport;

            mPickBuffer.PushMatrix(MatrixMode.View, camera.ViewMatrix);
            mPickBuffer.PushMatrix(MatrixMode.Projection, camera.ProjectionMatrix);

            //draw each Component into pick buffer
            foreach (XComponent comp in X.Components)
                comp.DrawPick(mPickBuffer, camera);

            mPickBuffer.PopMatrix(MatrixMode.View);
            mPickBuffer.PopMatrix(MatrixMode.Projection);

            mPickBuffer.Render();
            
            uint pick_id = mPickBuffer.Pick(e.X, e.Y);
            //find the xcomponent we selected
            XComponent component = X.Components.Find(delegate(XComponent obj) { return obj.ComponentID == pick_id; });

            if (component != null && (component is XITransform))
            {//we selected a valid component we should update our windows UI
                mManipulator.Transform = (XITransform) component;
                //fire change event
                if (OnSelectedComponentChange != null)
                {
                    OnSelectedComponentChange(this, component);
                }

            }


            this.GraphicsDevice.Viewport = vp;
        }

        public bool hasFocus = false;
        public bool dragdroprelease = false;
        public bool manipulators = true;

        protected override void Draw()
        {
            this.GraphicsDevice.Clear(Color.Red);
            GameTime gameTime = new GameTime(time.TotalGameTime.Elapsed, time.ElapsedGameTime.Elapsed, time.TotalGameTime.Elapsed, time.ElapsedGameTime.Elapsed, false);
            time.ElapsedGameTime.Reset();
            time.ElapsedGameTime.Start();

            

            if (hasFocus && keyboard != null && mouse != null)
            {
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                    camera.Translate(Vector3.Backward * 400);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    camera.Translate(Vector3.Forward * 400);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    camera.Translate(Vector3.Left * 400);
                if (keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    camera.Translate(Vector3.Right * 400);

                if (mouse.ButtonPressed(XMouse.Buttons.Left))
                {
                    mouse.InitialPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    mouse.CurrentPosition = mouse.InitialPosition;
                    mouse.Delta = Vector2.Zero;
                    mouse.Reset = true;
                    manipulators = false;
                    //Cursor.Hide();
                }
                else if (mouse.ButtonReleased(XMouse.Buttons.Left))
                {
                    mouse.Reset = false;
                    manipulators = true;

                    if (!dragdroprelease)
                        Cursor.Show();
                    else
                        dragdroprelease = false;

                    ((EditorForm)Tag).RefreshPropertiesTab(); 
                }

                if (mouse.ButtonDown(XMouse.Buttons.Left))
                    camera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));

                if (mouse.ButtonReleased(XMouse.Buttons.Right))
                    ((EditorForm)Tag).RefreshPropertiesTab();

                
            }

            X.AdvancePhysics(ref gameTime);
            X.UpdateComponents(ref gameTime);

            if(manipulators) mManipulator.Update();

            X.Renderer.Draw(ref gameTime,ref camera.Base);
            mManipulator.Draw();

            float fps = (float)1000.0f / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            this.Parent.Parent.Parent.Text = "Elapsed:" + gameTime.ElapsedGameTime.ToString() + " Total:" + gameTime.TotalGameTime.ToString() + " FPS:" + fps.ToString();

            
        }

        void redraw_Tick(object sender, EventArgs e)
        {
            Invalidate();
            //Draw();
        }
    }
}
