using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XEngine;

namespace XEngine
{
    public class XGamePad : XComponent, XUpdateable
    {
        GamePadState LastState;
        GamePadState CurrentState;
        int GamepadNumber;

        PlayerIndex index;

        XTimer idleTime;

        public XGamePad(ref XMain X, int GamepadNumber) : base(ref X)
        {
            DrawOrder = 0;
            this.GamepadNumber = GamepadNumber;

            if (GamepadNumber == 1)
                index = PlayerIndex.One;
            else if (GamepadNumber == 2)
                index = PlayerIndex.Two;
            else if (GamepadNumber == 3)
                index = PlayerIndex.Three;
            else if (GamepadNumber == 4)
                index = PlayerIndex.Four;

            GetState();

            idleTime = new XTimer(ref X);
            vibrationTimer = new XTimer(ref X);
        }

        XTimer vibrationTimer;
        float left;
        float right;
        float length;

        public override void Update(ref GameTime gameTime)
        {
            GetState();

            if (CurrentState == LastState)
                idleTime.Start();
            else
                idleTime.Reset();

            if (vibrationTimer.PassedTime > length)
            {
                vibrationTimer.Reset();
                left = 0;
                right = 0;
                length = 0;
                GamePad.SetVibration(index, 0, 0);
            }
        }

        private void GetState()
        {
            LastState = CurrentState;
            CurrentState = GamePad.GetState(index);
        }

        public enum States { Last, Current }

        public GamePadState State(States State)
        {
            if (State == States.Last)
                return LastState;
            else
                return CurrentState;
        }

        public enum Triggers { Left, Right }

        public float Trigger(Triggers Trigger)
        {
            if (Trigger == Triggers.Right)
                return CurrentState.Triggers.Left;
            else
                return CurrentState.Triggers.Right;
        }

        public enum Thumbsticks { Left, Right }

        public Vector2 Thumbstick(Thumbsticks Thumstick)
        {
            if (Thumstick == Thumbsticks.Left)
                return CurrentState.ThumbSticks.Left;
            else
                return CurrentState.ThumbSticks.Right;
        }

        public bool ButtonDown(Buttons Button)
        {
            return CurrentState.IsButtonDown(Button);
        }

        public bool ButtonUp(Buttons Button)
        {
            return CurrentState.IsButtonUp(Button);
        }

        public bool ButtonHeld(Buttons Button)
        {
            if (CurrentState.IsButtonDown(Button) && LastState.IsButtonDown(Button))
                return true;
            else
                return false;
        }

        public bool TriggerPulled(Triggers Trigger, float Amount)
        {
            if (Trigger == Triggers.Left)
            {
                if ((CurrentState.Triggers.Left > Amount) && !(LastState.Triggers.Left > Amount))
                    return true;
                else
                    return false;
            }
            else
            {
                if ((CurrentState.Triggers.Right > Amount) && !(LastState.Triggers.Right > Amount))
                    return true;
                else
                    return false;
            }
        }

        public bool TriggerReleased(Triggers Trigger, float Amount)
        {
            if (Trigger == Triggers.Left)
            {
                if (!(CurrentState.Triggers.Left > Amount) && (LastState.Triggers.Left > Amount))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!(CurrentState.Triggers.Right > Amount) && (LastState.Triggers.Right > Amount))
                    return true;
                else
                    return false;
            }
        }

        public bool TriggerDown(Triggers Trigger, float Amount)
        {
            if (Trigger == Triggers.Left)
            {
                if ((CurrentState.Triggers.Left > Amount))
                    return true;
                else
                    return false;
            }
            else
            {
                if ((CurrentState.Triggers.Right > Amount))
                    return true;
                else
                    return false;
            }
        }

        public bool TriggerUp(Triggers Trigger, float Amount)
        {
            if (Trigger == Triggers.Left)
            {
                if (!(CurrentState.Triggers.Left > Amount))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!(CurrentState.Triggers.Right > Amount))
                    return true;
                else
                    return false;
            }
        }

        public float TriggerDelta(Triggers Trigger)
        {
            if (Trigger == Triggers.Left)
                return CurrentState.Triggers.Left - LastState.Triggers.Left;
            else
                return CurrentState.Triggers.Right - LastState.Triggers.Right;
        }

        public bool ButtonPressed(Buttons Button)
        {
            if (CurrentState.IsButtonDown(Button) && !LastState.IsButtonDown(Button))
                return true;
            else
                return false;
        }

        public bool ButtonReleased(Buttons Button)
        {
            if (!CurrentState.IsButtonDown(Button) && LastState.IsButtonDown(Button))
                return true;
            else
                return false;
        }

        public void Rumble(float Left, float Right, float Time, GameTime gameTime)
        {
            left = Left;
            right = Right;
            length = Time;

            GamePad.SetVibration(index, Left, Right);
            vibrationTimer.Start();
        }

        public void Rumble(float Amount, float Time, GameTime gameTime)
        {
            Rumble(Amount, Amount, Time, gameTime);
        }

        public Vector2 ThumbStickDelta(Thumbsticks Thumbstick)
        {
            if (Thumbstick == Thumbsticks.Left)
                return CurrentState.ThumbSticks.Left - LastState.ThumbSticks.Left;
            else
                return CurrentState.ThumbSticks.Right - LastState.ThumbSticks.Right;
        }
    }
}