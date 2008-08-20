using System;
using System.Collections.Generic;
using System.Text;
using XEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Sphere
{
    public class InputProcessor
    {
        XMain X;
        Game parent;
#if XBOX == FALSE
        XMouse mouse;
#endif
        XKeyboard keyboard;
        XGamePad gamepad;

        public InputProcessor(XMain X, Game parent)
        {
            this.X = X;
            this.parent = parent;
        }

        public void Load()
        {
#if XBOX == FALSE
            mouse = new XMouse(X);
            //mouse.Reset = false;
#endif
            keyboard = new XKeyboard(X);
            gamepad = new XGamePad(X, 1);
        }

        /// <summary>
        /// Processes user input updates
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (keyboard.KeyPressed(Keys.OemTilde))
                X.Console.Visible = !X.Console.Visible;

            //if console is showing capture all input!
            if (X.Console.Visible)
            {
                X.Console.AcceptInput(keyboard);
                //return;
            }

            // Allows the game to exit
            //if (gamepad.ButtonDown(Buttons.B))
            //    parent.Exit();

            if (keyboard.KeyPressed(Keys.Escape))
            {
                //toggle physics on/off
                X.UpdatePhysics = !X.UpdatePhysics;
                //toggle game menu on/off
                parent.menus.Toggle();
            }

            if (parent.menus.MenuOpen)
            {
                if (keyboard.KeyPressed(Keys.Up))
                    parent.menus.SelectPrevious();
                if (keyboard.KeyPressed(Keys.Down))
                    parent.menus.SelectNext();
                if (keyboard.KeyPressed(Keys.Enter))
                    switch (parent.menus.Current)
                    {
                        case 2: //exit button
                            parent.Exit();
                            break;
                        default:
                            break;
                    }
                return;
            }

//mouse code
#if XBOX == FALSE
            parent.freeCamera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));
            parent.driverCamera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));
#endif
            parent.freeCamera.Rotate(new Vector3((gamepad.Thumbstick(XGamePad.Thumbsticks.Right).Y * .5f) * (float)gameTime.ElapsedGameTime.TotalSeconds, (-gamepad.Thumbstick(XGamePad.Thumbsticks.Right).X * .5f) * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            
//Camera Movement with KB or Gamepad directional pad
            float speed = 40f;
            if(keyboard.KeyDown(Keys.LeftShift)) speed = 2f;
            if(keyboard.KeyDown(Keys.LeftControl)) speed = 200f;

            if (keyboard.KeyDown(Keys.W) || gamepad.ButtonDown(Buttons.DPadUp))
                parent.freeCamera.Translate(Vector3.Forward * speed);
            if (keyboard.KeyDown(Keys.S) || gamepad.ButtonDown(Buttons.DPadDown))
                parent.freeCamera.Translate(Vector3.Backward * speed);
            if (keyboard.KeyDown(Keys.A) || gamepad.ButtonDown(Buttons.DPadLeft))
                parent.freeCamera.Translate(Vector3.Left * speed);
            if (keyboard.KeyDown(Keys.D) || gamepad.ButtonDown(Buttons.DPadRight))
                parent.freeCamera.Translate(Vector3.Right * speed);

            //write camera corrds for debug
            X.Debug.Write("Free Camera: X" + parent.freeCamera.Position.X.ToString() + " Y:" + parent.freeCamera.Position.Y.ToString() + " Z:" + parent.freeCamera.Position.Z.ToString(), false);
            X.Debug.Write("Driver Camera: X" + parent.driverCamera.Position.X.ToString() + " Y:" + parent.driverCamera.Position.Y.ToString() + " Z:" + parent.driverCamera.Position.Z.ToString(), false);
            X.Debug.Write("Box Actors in List:" + parent.boxes.Count.ToString(), false);
            X.Debug.Write("Press F6 to switch Camera. Current: " + parent.currentCamera.Name, false);


            //drive car
            if (parent.Car != null)
            {
                if (keyboard.KeyDown(Keys.Left))
                    parent.Car.Steer(-1);
                if (keyboard.KeyDown(Keys.Right))
                    parent.Car.Steer(1);
                if (keyboard.KeyDown(Keys.Up))
                    parent.Car.Accelerate(2);
                if (keyboard.KeyDown(Keys.Down))
                    parent.Car.Accelerate(-3);
            }
#if !XBOX
            if (mouse.ButtonPressed(XMouse.Buttons.Left))
                parent.boxes.Add(new XActor(X,parent.model, parent.freeCamera.Position, Vector3.Zero, Vector3.Normalize(parent.freeCamera.Target - parent.freeCamera.Position) * 30, 10));
#else
            if (gamepad.ButtonPressed(Buttons.A))
                parent.boxes.Add(new XActor(X, new CapsuleObject(1,1,Matrix.Identity,parent.freeCamera.Position), parent.model, Vector3.One, Vector3.Zero, Vector3.Normalize(parent.freeCamera.Target - parent.freeCamera.Position) * 30, 10));
#endif

            if (keyboard.KeyPressed(Keys.F1))
                if (parent.fire == null)
                    parent.fire = new Fire(X);

            if (keyboard.KeyDown(Keys.F2))
                parent.sky.Theta -= .5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.KeyPressed(Keys.F3))
                if (parent.water == null)
                {
                    parent.water = new XWater(X, new Vector2(-128, -128), new Vector2(128, 128), 3f);
                    parent.water.Load(parent.Content);
                    parent.water.Update(ref gameTime);
                    parent.resources.AddComponent(parent.water);
                }

            if (keyboard.KeyPressed(Keys.E))
                if (parent.water != null)
                    parent.water.DoesReflect = !parent.water.DoesReflect;

            if (keyboard.KeyPressed(Keys.R))
                if (parent.water != null)
                    parent.water.DoesRefract = !parent.water.DoesRefract;

            if (parent.water != null)
            {
                //WindDirection
                if(keyboard.KeyDown(Keys.NumPad4))
                    parent.water.WindDirection -= .25f;
                if (keyboard.KeyDown(Keys.NumPad6))
                    parent.water.WindDirection += .25f;
                //WaveHeight
                if (keyboard.KeyDown(Keys.NumPad2))
                    parent.water.WaveHeight -= .01f;
                if (keyboard.KeyDown(Keys.NumPad8))
                    parent.water.WaveHeight += .01f;
                //Wave Length
                if (keyboard.KeyDown(Keys.NumPad7))
                    parent.water.WaveLenth -= .01f;
                if (keyboard.KeyDown(Keys.NumPad9))
                    parent.water.WaveLenth += .01f;
                //Wind Force
                if (keyboard.KeyDown(Keys.NumPad1))
                    parent.water.WindForce -= .05f;
                if (keyboard.KeyDown(Keys.NumPad3))
                    parent.water.WindForce += .05f;
            }

            if (keyboard.KeyPressed(Keys.F4))
                for (int x = 0; x < 10; x++)
                    for (int e = x; e < 10; e++)
                        parent.boxes.Add(new XActor(X, new CapsuleObject(1, 1, Matrix.Identity, new Vector3(20, x * 1.01f + 1, e - 0.5f * x)), parent.model, Vector3.One, Vector3.Zero, Vector3.Zero, 10));            
            
            if (keyboard.KeyPressed(Keys.F5))
            {
                List<XActor> chainBoxes = new List<XActor>();

                for (int i = 0; i < 25; i++)
                {
                    XActor actor = new XActor(X, new CapsuleObject(1, 1, Matrix.Identity, new Vector3(i + 10, 45 - i, 0)), parent.model, Vector3.One, Vector3.Zero, Vector3.Zero, 10);
                    if (i == 0) actor.Immovable = true;
                    chainBoxes.Add(actor);
                }

                for (int i = 1; i < 25; i++)
                {
                    XHingeJoint hinge = new XHingeJoint(X, chainBoxes[i - 1], chainBoxes[i], Vector3.Backward, new Vector3(0.5f, -0.5f, 0.0f), 1.0f, 90.0f, 90.0f, 0.2f, 0.2f);
                }
            }

            if (keyboard.KeyPressed(Keys.F6))
            {
                if (parent.currentCamera == parent.chase)
                {
                    parent.currentCamera = parent.freeCamera;
                    return;
                }

                if (parent.currentCamera == parent.freeCamera)
                {
                    parent.currentCamera = parent.driverCamera;
                    return;
                }

                if (parent.currentCamera == parent.driverCamera)
                {
                    parent.currentCamera = parent.chase;
                    return;
                }
            }
                

            if (keyboard.KeyPressed(Keys.F7))
            {
            }

            if (keyboard.KeyPressed(Keys.F8))
            {
                //if (parent.houseactor == null)
                //{
                parent.houseactor = new XAnimatedActor(X, new BoxObject(new Vector3(5,5,1),Matrix.Identity,new Vector3(2,2,2)), parent.housemodel, Vector3.One, Vector3.Zero, Vector3.Zero, .1f);
                parent.houseactor.Load(X.Content);
                parent.houseactor.Immovable = false;
                parent.boxes.Add(parent.houseactor);
                //}
            }

            if (keyboard.KeyPressed(Keys.F9))
            {
                foreach (XActor xa in parent.boxes)
                    xa.Disable();
                parent.boxes.Clear();
            }

            if(keyboard.KeyPressed(Keys.Add))
            {
                parent.houseactor.AnimationIndex++; 
            }

            if(keyboard.KeyPressed(Keys.F10))
            {
                XTextureGenerator texGen = new XTextureGenerator(X);
                texGen.GetMandelBrot(new Vector2(0.25f, 0),3);

                /*
                if (pad.Buttons.A == ButtonState.Pressed)
                zoom /= 1.05f;

                if (pad.Buttons.B == ButtonState.Pressed)
                    zoom *= 1.05f;

                float panSensitivity = 0.01f * (float)Math.Log(zoom + 1);

                pan += new Vector2(pad.ThumbSticks.Left.X, -pad.ThumbSticks.Left.Y) * panSensitivity;
                */
            }

            if(keyboard.KeyPressed(Keys.PageUp))
            {
                
            }

            if (keyboard.KeyPressed(Keys.PageDown))
            {
                
            }
            
            //Example Code
/*            if (!parent.menus.MenuOpen)
            {
                X.UpdatePhysics = true;

                if (parent.scene.Ball != null)
                {
                    parent.Camera.TargetPosition = parent.scene.Ball.Position;

                    Vector3 ballPos = new Vector3(parent.scene.Ball.Position.X, 0, parent.scene.Ball.Position.Z);
                    Vector3 camPos = new Vector3(parent.Camera.Position.X, 0, parent.Camera.Position.Z);
                    Vector3 dir = (ballPos - camPos);

#if XBOX == FALSE
                    parent.Camera.Rotate(mouse.Delta.X * .008f, mouse.Delta.Y * .008f);
                    parent.Camera.Zoom(-mouse.ScrollDelta * .015f);

                    if (!jumping && keyboard.KeyPressed(Keys.Space))
                    {
                        jumping = true;
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(new Vector3(0, 1200000, 0), Matrix.Invert(parent.scene.Ball.Orientation)));
                    }
                    else if (jumping)
                    {
                        if (parent.scene.Ball.Actor.Collisions.Count > 0)
                            jumping = false;
                    }

                    if (parent.scene.Ball.Actor.Collisions.Count > 0)
                    {
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.W) ? 1 : 0) * 2800, Matrix.Invert(parent.scene.Ball.Orientation)));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(-Vector3.Transform(dir * (keyboard.KeyDown(Keys.S) ? 1 : 0) * 4000, Matrix.Invert(parent.scene.Ball.Orientation)));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.A) ? 1 : 0) * 3300, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(-90)))));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.D) ? 1 : 0) * 3300, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(90)))));
                    }
                    else
                    {
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.W) ? 1 : 0) * 1000, Matrix.Invert(parent.scene.Ball.Orientation)));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(-Vector3.Transform(dir * (keyboard.KeyDown(Keys.S) ? 1 : 0) * 2000, Matrix.Invert(parent.scene.Ball.Orientation)));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.A) ? 1 : 0) * 1300, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(-90)))));
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * (keyboard.KeyDown(Keys.D) ? 1 : 0) * 1300, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(90)))));
                    }
#endif
                    parent.Camera.Rotate(-Gamepad.Thumbstick(XgamePad.Thumbsticks.Right).X * .04f, Gamepad.Thumbstick(XgamePad.Thumbsticks.Right).Y * .04f);
                    parent.Camera.Zoom((Gamepad.ButtonDown(Buttons.DPadDown) ? 0.15f : 0) + (Gamepad.ButtonDown(Buttons.DPadUp) ? -0.15f : 0));

                    if (!jumping && Gamepad.ButtonPressed(Buttons.A))
                    {
                        jumping = true;
                        parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(new Vector3(0, 1200000, 0), Matrix.Invert(parent.scene.Ball.Orientation)));
                    }
                    else if (jumping)
                    {
                        if (parent.scene.Ball.Actor.Collisions.Count > 0)
                            jumping = false;
                    }

                    parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * Gamepad.Thumbstick(XgamePad.Thumbsticks.Left).Y * 1000, Matrix.Invert(parent.scene.Ball.Orientation)));
                    parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(-Vector3.Transform(dir * -Gamepad.Thumbstick(XgamePad.Thumbsticks.Left).Y * 1000, Matrix.Invert(parent.scene.Ball.Orientation)));
                    parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(Vector3.Transform(dir * Gamepad.Thumbstick(XgamePad.Thumbsticks.Left).X * 1000, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(90)))));
                    parent.scene.Ball.Actor.PhysicsBody.PhysicsBody.ApplyBodyImpulse(-Vector3.Transform(dir * -Gamepad.Thumbstick(XgamePad.Thumbsticks.Left).X * 1000, Matrix.Invert(parent.scene.Ball.Orientation * Matrix.CreateRotationY(MathHelper.ToRadians(90)))));
                }
            }
            else
            {
                X.UpdatePhysics = false;

                if (keyboard.KeyPressed(Keys.Up))
                    parent.menus.SelectPrevious();
                else if (keyboard.KeyPressed(Keys.Down))
                    parent.menus.SelectNext();
            }

            if (keyboard.KeyPressed(Keys.Escape))
                parent.menus.Toggle();
 */
        }
    }
}
