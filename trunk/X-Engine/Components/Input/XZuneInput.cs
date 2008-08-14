using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XEngine;

namespace XEngine
{
    public class XZuneInput : XComponent, XUpdateable
    {
        XGamePad GamePad;

        public XZuneInput(XMain X) : base(X)
        {
            GamePad = new XGamePad(X, 1);
        }

        public override void Update(ref GameTime gameTime)
        {
            GamePad.Update(ref gameTime);
        }

        public enum States { Last, Current }

        public GamePadState State(States State)
        {
            if (State == States.Last)
                return GamePad.State(XGamePad.States.Last);
            else
                return GamePad.State(XGamePad.States.Current);
        }

        public enum ZuneButtons { Up, Down, Left, Right, Back, Start, TouchCenter, PressCenter }

        public bool ButtonUp(ZuneButtons Button)
        {
            switch (Button)
            {
                case ZuneButtons.Up:
                    {
                        return GamePad.ButtonUp(Buttons.DPadUp);
                    }
                case ZuneButtons.Down:
                    {
                        return GamePad.ButtonUp(Buttons.DPadDown);
                    }
                case ZuneButtons.Right:
                    {
                        return GamePad.ButtonUp(Buttons.DPadRight);
                    }
                case ZuneButtons.Left:
                    {
                        return GamePad.ButtonUp(Buttons.DPadLeft);
                    }
                case ZuneButtons.Start:
                    {
                        return GamePad.ButtonUp(Buttons.B);
                    }
                case ZuneButtons.Back:
                    {
                        return GamePad.ButtonUp(Buttons.Back);
                    }
                case ZuneButtons.TouchCenter:
                    {
                        return GamePad.ButtonUp(Buttons.LeftStick);
                    }
                case ZuneButtons.PressCenter:
                    {
                        return GamePad.ButtonUp(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool ButtonDown(ZuneButtons Button)
        {
            switch (Button)
            {
                case ZuneButtons.Up:
                    {
                        return GamePad.ButtonDown(Buttons.DPadDown);
                    }
                case ZuneButtons.Down:
                    {
                        return GamePad.ButtonDown(Buttons.DPadDown);
                    }
                case ZuneButtons.Right:
                    {
                        return GamePad.ButtonDown(Buttons.DPadRight);
                    }
                case ZuneButtons.Left:
                    {
                        return GamePad.ButtonDown(Buttons.DPadLeft);
                    }
                case ZuneButtons.Start:
                    {
                        return GamePad.ButtonDown(Buttons.B);
                    }
                case ZuneButtons.Back:
                    {
                        return GamePad.ButtonDown(Buttons.Back);
                    }
                case ZuneButtons.TouchCenter:
                    {
                        return GamePad.ButtonDown(Buttons.LeftStick);
                    }
                case ZuneButtons.PressCenter:
                    {
                        return GamePad.ButtonDown(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool ButtonHeld(ZuneButtons Button)
        {
            switch (Button)
            {
                case ZuneButtons.Up:
                    {
                        return GamePad.ButtonHeld(Buttons.DPadUp);
                    }
                case ZuneButtons.Down:
                    {
                        return GamePad.ButtonHeld(Buttons.DPadDown);
                    }
                case ZuneButtons.Right:
                    {
                        return GamePad.ButtonHeld(Buttons.DPadRight);
                    }
                case ZuneButtons.Left:
                    {
                        return GamePad.ButtonHeld(Buttons.DPadLeft);
                    }
                case ZuneButtons.Start:
                    {
                        return GamePad.ButtonHeld(Buttons.B);
                    }
                case ZuneButtons.Back:
                    {
                        return GamePad.ButtonHeld(Buttons.Back);
                    }
                case ZuneButtons.TouchCenter:
                    {
                        return GamePad.ButtonHeld(Buttons.LeftStick);
                    }
                case ZuneButtons.PressCenter:
                    {
                        return GamePad.ButtonHeld(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool ButtonPressed(ZuneButtons Button)
        {
            switch (Button)
            {
                case ZuneButtons.Up:
                    {
                        return GamePad.ButtonPressed(Buttons.DPadUp);
                    }
                case ZuneButtons.Down:
                    {
                        return GamePad.ButtonPressed(Buttons.DPadDown);
                    }
                case ZuneButtons.Right:
                    {
                        return GamePad.ButtonPressed(Buttons.DPadRight);
                    }
                case ZuneButtons.Left:
                    {
                        return GamePad.ButtonPressed(Buttons.DPadLeft);
                    }
                case ZuneButtons.Start:
                    {
                        return GamePad.ButtonPressed(Buttons.B);
                    }
                case ZuneButtons.Back:
                    {
                        return GamePad.ButtonPressed(Buttons.Back);
                    }
                case ZuneButtons.TouchCenter:
                    {
                        return GamePad.ButtonPressed(Buttons.LeftStick);
                    }
                case ZuneButtons.PressCenter:
                    {
                        return GamePad.ButtonPressed(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool ButtonReleased(ZuneButtons Button)
        {
            switch (Button)
            {
                case ZuneButtons.Up:
                    {
                        return GamePad.ButtonReleased(Buttons.DPadUp);
                    }
                case ZuneButtons.Down:
                    {
                        return GamePad.ButtonReleased(Buttons.DPadDown);
                    }
                case ZuneButtons.Right:
                    {
                        return GamePad.ButtonReleased(Buttons.DPadRight);
                    }
                case ZuneButtons.Left:
                    {
                        return GamePad.ButtonReleased(Buttons.DPadLeft);
                    }
                case ZuneButtons.Start:
                    {
                        return GamePad.ButtonReleased(Buttons.B);
                    }
                case ZuneButtons.Back:
                    {
                        return GamePad.ButtonReleased(Buttons.Back);
                    }
                case ZuneButtons.TouchCenter:
                    {
                        return GamePad.ButtonReleased(Buttons.LeftStick);
                    }
                case ZuneButtons.PressCenter:
                    {
                        return GamePad.ButtonReleased(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public Vector2 TouchPad()
        {
            return GamePad.State(XGamePad.States.Current).ThumbSticks.Left;
        }

        public Vector2 TouchPadDelta()
        {
            return GamePad.State(XGamePad.States.Current).ThumbSticks.Left - GamePad.State(XGamePad.States.Last).ThumbSticks.Left;
        }
    }
}