using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XEngine
{

#if XBOX == FALSE
    public class XMouse : XComponent, XIUpdateable
    {
        MouseState CurrentState;
        MouseState LastState;

        public Vector2 InitialPosition;
        public Vector2 CurrentPosition;
        public Vector2 Delta;

        public float ScrollPosition;
        public float ScrollDelta;

        public bool Reset = true;

        XTimer idleTime;

        public XMouse(ref XMain X) : base(ref X)
        {
            DrawOrder = 0;
            Delta = Vector2.Zero;
            ScrollPosition = 0;
            ScrollDelta = 0;

            if (Reset)
            {
                Mouse.SetPosition(X.GraphicsDevice.Viewport.Width / 2, X.GraphicsDevice.Viewport.Height / 2);
            }

            CurrentState = Mouse.GetState();
            LastState = CurrentState;

            InitialPosition = new Vector2(CurrentState.X, CurrentState.Y);
            CurrentPosition = InitialPosition;

            idleTime = new XTimer(ref X);
        }

        public override void Update(ref GameTime gameTime)
        {
            LastState = CurrentState;
            CurrentState = Mouse.GetState();

            CurrentPosition = new Vector2(CurrentState.X, CurrentState.Y);

            if (Reset)
                Delta = InitialPosition - CurrentPosition;
            else
                Delta = CurrentPosition - new Vector2(LastState.X, LastState.Y);

            if (Reset)
                Mouse.SetPosition((int)InitialPosition.X, (int)InitialPosition.Y);

            ScrollPosition = CurrentState.ScrollWheelValue;
            ScrollDelta = ScrollPosition - LastState.ScrollWheelValue;

            if (CurrentState == LastState)
                idleTime.Start();
            else
                idleTime.Reset();
        }

        public enum States { Last, Current }

        public MouseState State(States State)
        {
            if (State == States.Last)
                return LastState;
            else
                return CurrentState;
        }

        public enum Buttons { Left, Middle, Right, XButton1, XButton2 }

        public bool ButtonDown(Buttons Button)
        {
            switch (Button)
            {
                case Buttons.Left:
                    {
                        if (CurrentState.LeftButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Middle:
                    {
                        if (CurrentState.MiddleButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Right:
                    {
                        if (CurrentState.RightButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton1:
                    {
                        if (CurrentState.XButton1 == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton2:
                    {
                        if (CurrentState.XButton2 == ButtonState.Pressed)
                            return true;
                    } break;
            }

            return false;
        }

        public bool ButtonUp(Buttons Button)
        {
            switch (Button)
            {
                case Buttons.Left:
                    {
                        if (CurrentState.LeftButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Middle:
                    {
                        if (CurrentState.MiddleButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Right:
                    {
                        if (CurrentState.RightButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton1:
                    {
                        if (CurrentState.XButton1 != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton2:
                    {
                        if (CurrentState.XButton2 != ButtonState.Pressed)
                            return true;
                    } break;
            }

            return false;
        }

        public bool ButtonHeld(Buttons Button)
        {
            switch (Button)
            {
                case Buttons.Left:
                    {
                        if (CurrentState.LeftButton == ButtonState.Pressed && LastState.LeftButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Middle:
                    {
                        if (CurrentState.MiddleButton == ButtonState.Pressed && LastState.MiddleButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Right:
                    {
                        if (CurrentState.RightButton == ButtonState.Pressed && LastState.RightButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton1:
                    {
                        if (CurrentState.XButton1 == ButtonState.Pressed && LastState.XButton1 == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton2:
                    {
                        if (CurrentState.XButton2 == ButtonState.Pressed && LastState.XButton2 == ButtonState.Pressed)
                            return true;
                    } break;
            }

            return false;
        }

        public bool ButtonPressed(Buttons Button)
        {
            switch (Button)
            {
                case Buttons.Left:
                    {
                        if (CurrentState.LeftButton == ButtonState.Pressed && LastState.LeftButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Middle:
                    {
                        if (CurrentState.MiddleButton == ButtonState.Pressed && LastState.MiddleButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Right:
                    {
                        if (CurrentState.RightButton == ButtonState.Pressed && LastState.RightButton != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton1:
                    {
                        if (CurrentState.XButton1 == ButtonState.Pressed && LastState.XButton1 != ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton2:
                    {
                        if (CurrentState.XButton2 == ButtonState.Pressed && LastState.XButton2 != ButtonState.Pressed)
                            return true;
                    } break;
            }

            return false;
        }

        public bool ButtonReleased(Buttons Button)
        {
            switch (Button)
            {
                case Buttons.Left:
                    {
                        if (CurrentState.LeftButton != ButtonState.Pressed && LastState.LeftButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Middle:
                    {
                        if (CurrentState.MiddleButton != ButtonState.Pressed && LastState.MiddleButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.Right:
                    {
                        if (CurrentState.RightButton != ButtonState.Pressed && LastState.RightButton == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton1:
                    {
                        if (CurrentState.XButton1 != ButtonState.Pressed && LastState.XButton1 == ButtonState.Pressed)
                            return true;
                    } break;
                case Buttons.XButton2:
                    {
                        if (CurrentState.XButton2 != ButtonState.Pressed && LastState.XButton2 == ButtonState.Pressed)
                            return true;
                    } break;
            }

            return false;
        }
    }
#endif
}
