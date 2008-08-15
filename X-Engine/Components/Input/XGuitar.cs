using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XEngine;

namespace XEngine
{
    public class XGuitar : XComponent, XUpdateable
    {
        XGamePad GamePad;

        public XGuitar(XMain X, int GamepadNumber) : base(X)
        {
            DrawOrder = 0;
            GamePad = new XGamePad(X, GamepadNumber);
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

        public enum GuitarFrets { Green = 1, Red = 2, Yellow = 3, Blue = 4, Orange = 5 }

        public bool GuitarFretUp(GuitarFrets Fret)
        {
            switch (Fret)
            {
                case GuitarFrets.Green:
                    {
                        return GamePad.ButtonUp(Buttons.A);
                    }
                case GuitarFrets.Red:
                    {
                        return GamePad.ButtonUp(Buttons.B);
                    }
                case GuitarFrets.Yellow:
                    {
                        return GamePad.ButtonUp(Buttons.Y);
                    }
                case GuitarFrets.Blue:
                    {
                        return GamePad.ButtonUp(Buttons.X);
                    }
                case GuitarFrets.Orange:
                    {
                        return GamePad.ButtonUp(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool GuitarFretDown(GuitarFrets Fret)
        {
            switch (Fret)
            {
                case GuitarFrets.Green:
                    {
                        return GamePad.ButtonDown(Buttons.A);
                    }
                case GuitarFrets.Red:
                    {
                        return GamePad.ButtonDown(Buttons.B);
                    }
                case GuitarFrets.Yellow:
                    {
                        return GamePad.ButtonDown(Buttons.Y);
                    }
                case GuitarFrets.Blue:
                    {
                        return GamePad.ButtonDown(Buttons.X);
                    }
                case GuitarFrets.Orange:
                    {
                        return GamePad.ButtonDown(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool GuitarFretHeld(GuitarFrets Fret)
        {
            switch (Fret)
            {
                case GuitarFrets.Green:
                    {
                        return GamePad.ButtonHeld(Buttons.A);
                    }
                case GuitarFrets.Red:
                    {
                        return GamePad.ButtonHeld(Buttons.B);
                    }
                case GuitarFrets.Yellow:
                    {
                        return GamePad.ButtonHeld(Buttons.Y);
                    }
                case GuitarFrets.Blue:
                    {
                        return GamePad.ButtonHeld(Buttons.X);
                    }
                case GuitarFrets.Orange:
                    {
                        return GamePad.ButtonHeld(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool GuitarFretPressed(GuitarFrets Fret)
        {
            switch (Fret)
            {
                case GuitarFrets.Green:
                    {
                        return GamePad.ButtonPressed(Buttons.A);
                    }
                case GuitarFrets.Red:
                    {
                        return GamePad.ButtonPressed(Buttons.B);
                    }
                case GuitarFrets.Yellow:
                    {
                        return GamePad.ButtonPressed(Buttons.Y);
                    }
                case GuitarFrets.Blue:
                    {
                        return GamePad.ButtonPressed(Buttons.X);
                    }
                case GuitarFrets.Orange:
                    {
                        return GamePad.ButtonPressed(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public bool GuitarFretReleased(GuitarFrets Fret)
        {
            switch (Fret)
            {
                case GuitarFrets.Green:
                    {
                        return GamePad.ButtonReleased(Buttons.A);
                    }
                case GuitarFrets.Red:
                    {
                        return GamePad.ButtonReleased(Buttons.B);
                    }
                case GuitarFrets.Yellow:
                    {
                        return GamePad.ButtonReleased(Buttons.Y);
                    }
                case GuitarFrets.Blue:
                    {
                        return GamePad.ButtonReleased(Buttons.X);
                    }
                case GuitarFrets.Orange:
                    {
                        return GamePad.ButtonReleased(Buttons.LeftShoulder);
                    }
            }

            return false;
        }

        public enum StrumType { Up, Down, Combined }
        public bool GuiterStrummed(StrumType strum)
        {
            if (strum == StrumType.Up)
                return GamePad.ButtonPressed(Buttons.DPadUp);
            else if (strum == StrumType.Down)
                return GamePad.ButtonPressed(Buttons.DPadDown);
            else if (strum == StrumType.Combined)
                if (GamePad.ButtonPressed(Buttons.DPadDown) || GamePad.ButtonPressed(Buttons.DPadUp))
                    return true;

            return false;
        }

        public float WhammyDelta()
        {
            return GamePad.ThumbStickDelta(XGamePad.Thumbsticks.Right).X;
        }

        public bool StarPower()
        {
            if (((GamePad.State(XGamePad.States.Current).ThumbSticks.Right.Y > .5f)
                    && !(GamePad.State(XGamePad.States.Last).ThumbSticks.Right.Y > .5f))
                || ((GamePad.State(XGamePad.States.Current).ThumbSticks.Right.Y < -.5f)
                    && !(GamePad.State(XGamePad.States.Last).ThumbSticks.Right.Y < -.5f)))
                return true;
            else
                return false;
        }
    }
}