using System;
using System.Collections.Generic;
using System.Text;
using XEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
#endif
            keyboard = new XKeyboard(X);
            gamepad = new XGamePad(X, 1);
        }

        public void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (gamepad.ButtonDown(Buttons.B))
                parent.Exit();
            if (keyboard.KeyPressed(Keys.Escape))
                parent.Exit();

//mouse code
#if XBOX == FALSE
            parent.camera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));
#endif
            parent.camera.Rotate(new Vector3(-gamepad.Thumbstick(XGamePad.Thumbsticks.Right).X * .04f, gamepad.Thumbstick(XGamePad.Thumbsticks.Right).Y * .04f,0));

//Camera Movement with KB or Gamepad directional pad
            if (keyboard.KeyDown(Keys.W) || gamepad.ButtonDown(Buttons.DPadUp))
                parent.camera.Translate(Vector3.Forward * 40f);
            if (keyboard.KeyDown(Keys.S) || gamepad.ButtonDown(Buttons.DPadDown))
                parent.camera.Translate(Vector3.Backward * 40);
            if (keyboard.KeyDown(Keys.A) || gamepad.ButtonDown(Buttons.DPadLeft))
                parent.camera.Translate(Vector3.Left * 40);
            if (keyboard.KeyDown(Keys.D) || gamepad.ButtonDown(Buttons.DPadRight))
                parent.camera.Translate(Vector3.Right * 40);

            if (parent.Car != null)
            {
                if (keyboard.KeyDown(Keys.Left))
                    parent.Car.Steer(-1);
                if (keyboard.KeyDown(Keys.Right))
                    parent.Car.Steer(1);
                if (keyboard.KeyDown(Keys.Up))
                    parent.Car.Accelerate(2);
                if (keyboard.KeyDown(Keys.Down))
                    parent.Car.Accelerate(-1);
            }
#if !XBOX
            if (mouse.ButtonPressed(XMouse.Buttons.Left))
                parent.boxes.Add(new XActor(X, new BoxObject(new Vector3(10), Matrix.Identity, parent.camera.Position), parent.model, new Vector3(10), new Vector3(0, 0, 0), Vector3.Normalize(parent.camera.Target - parent.camera.Position) * 30, 10));
#else
            if (pad.ButtonPressed(Buttons.A))
                parent.boxes.Add(new XActor(X, new BoxObject(new Vector3(10), Matrix.Identity, parent.camera.Position), model, new Vector3(10), new Vector3(0, 0, 0), Vector3.Normalize(parent.camera.Target - parent.camera.Position) * 30, 10));
#endif
            //need to set camera.position, rotation, size of box????
            //sky.Theta += mouse.ScrollDelta * .0004f;

            if (keyboard.KeyPressed(Keys.F1))
                if (parent.fire == null)
                    parent.fire = new Fire(X);

            if (keyboard.KeyDown(Keys.F2))
                parent.sky.Theta -= .5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.KeyPressed(Keys.F3))
                if (parent.water == null)
                {
                    parent.water = new XWater(X, new Vector2(-128, -128), new Vector2(128, 128), 6);
                    parent.water.Load(parent.Content);
                    parent.water.Update(gameTime);
                    parent.resources.AddComponent(parent.water);
                }

            if (keyboard.KeyPressed(Keys.F4))
                for (int x = 0; x < 10; x++)
                    for (int e = x; e < 10; e++)
                        parent.boxes.Add(new XActor(X, new BoxObject(new Vector3(10), Matrix.Identity, new Vector3(20, x * 1.01f + 1, e - 0.5f * x)), parent.model, new Vector3(10), new Vector3(0, 0, 0), Vector3.Zero, 10));
            /*
                        if (keyboard.KeyPressed(Keys.F5))
                        {
                            List<XActor> chainBoxes = new List<XActor>();

                            for (int i = 0; i < 25; i++)
                            {
                                XActor actor = new XActor(X, XActor.ActorType.Box, model, new Vector3(i + 10, 45 - i, 0), Matrix.Identity, new Vector3(10), Vector3.Zero, Vector3.One, Vector3.Zero, 1);
                                if (i == 0) actor.Immovable = true;
                                chainBoxes.Add(actor);
                            }

                            for (int i = 1; i < 25; i++)
                            {
                                XHingeJoint hinge = new XHingeJoint(X, chainBoxes[i - 1], chainBoxes[i], Vector3.Backward, new Vector3(0.5f, -0.5f, 0.0f), 1.0f, 90.0f, 90.0f, 0.2f, 0.2f);
                            }
                        }
            */
            if (keyboard.KeyPressed(Keys.F6))
                if (parent.Car == null)
                {
                    parent.Car = new XCar(X, parent.Chassis, parent.Wheel, true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f, 0.45f, 0.3f, 1, 520.0f, Math.Abs(X.Gravity.Y), new Vector3(0, 3, 0));
                    parent.resources.AddComponent(parent.Car);
                }

            if (keyboard.KeyPressed(Keys.F7))
                if (parent.duckActor == null)
                {
                    parent.duckActor = new XActor(X, new BoxObject(new Vector3(10), Matrix.Identity, new Vector3(0, 7, 0)), parent.duckModel, new Vector3(20), new Vector3(0, -2, 0), new Vector3(0), 10);
                    parent.duckActor.Immovable = true;
                }

            if (keyboard.KeyPressed(Keys.F8))
                if (parent.houseactor == null)
                {
                    parent.houseactor = new XActor(X, new BoxObject(new Vector3(10), Matrix.Identity, new Vector3(10, 10, 0)), parent.housemodel, new Vector3(/*0.05f*/1f), new Vector3(0, -1, 0), new Vector3(0), 10);
                    parent.houseactor.Immovable = false;
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
