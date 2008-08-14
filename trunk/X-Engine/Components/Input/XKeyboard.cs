﻿using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XKeyboard : XComponent, XUpdateable
    {
        KeyboardState CurrentState;
        KeyboardState LastState;

        XTimer idleTime;

        public XKeyboard(XMain X) : base(X)
        {
            UpdateStates();
            idleTime = new XTimer(X);
        }

        public override void Update(ref GameTime gameTime)
        {
            UpdateStates();

            if (CurrentState == LastState)
                idleTime.Start(gameTime);
            else
                idleTime.Reset();
        }

        void UpdateStates()
        {
            LastState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        public enum States { Last, Current }

        public KeyboardState State(States State)
        {
            if (State == States.Last)
                return LastState;
            else
                return CurrentState;
        }

        public bool KeyDown(Keys Key)
        {
            return (CurrentState.IsKeyDown(Key));
        }

        public bool KeyUp(Keys Key)
        {
            return (CurrentState.IsKeyUp(Key));
        }

        public bool KeyHeld(Keys Key)
        {
            if (CurrentState.IsKeyDown(Key) && LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }

        public bool KeyPressed(Keys Key)
        {
            if (CurrentState.IsKeyDown(Key) && !LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }

        public bool KeyReleased(Keys Key)
        {
            if (!CurrentState.IsKeyDown(Key) && LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }
    }
}