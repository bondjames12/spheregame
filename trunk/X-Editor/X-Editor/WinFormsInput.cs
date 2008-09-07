using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace X_Editor
{
    using Point = System.Drawing.Point;
    using XEngine;

    public class WinFormsInput : XIInputProvider
    {
        private Control     mControl;

        private Vector2     mStart;
        private Vector2     mEnd;
        private Vector2     mDelta;

        private Vector2     mLastMouse;

        private bool        mMouseDown;
        private bool        mMouseClick;
        private bool        mMouseRelease;

        public Vector2 Start
        {
            get { return mStart; }
        }

        public Vector2 End
        {
            get { return mEnd; }
        }

        public Vector2 Delta
        {
            get { return mDelta; }
        }

        public bool LeftButton
        {
            get { return mMouseDown; }
        }

        public bool LeftClick
        {
            get { return mMouseClick; }
        }

        public bool LeftRelease
        {
            get { return mMouseRelease; }
        }

        public float Length
        {
            get { return mDelta.Length(); }
        }

        public int X
        {
            get { return (int)mEnd.X; }
        }

        public int Y
        {
            get { return (int)mEnd.Y; }
        }

        public bool Focused
        {
            get { return mControl.Focused; }
        }

        public Viewport Viewport
        {
            get
            {
                Viewport vp = new Viewport();

                int w = mControl.ClientRectangle.Width;
                int h = mControl.ClientRectangle.Height;

                vp.X = vp.Y = 0;

                vp.Width = w;
                vp.Height = h;

                vp.MinDepth = 0;
                vp.MaxDepth = 1;

                return vp;
            }
        }

        public WinFormsInput(Control ctrl)
        {
            mControl = ctrl;

            mControl.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                mLastMouse = new Vector2(e.X, e.Y);

                //mStart = mEnd;
                //mEnd = new Vector2(e.X, e.Y);
                //mDelta = mEnd - mStart;
            };

            mControl.MouseDown += delegate(object sender, MouseEventArgs e)
            {
                mControl.Focus();

                mMouseDown = true;
                mMouseRelease = false;
                mMouseClick = true;
            };

            mControl.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                mMouseDown = false;
                mMouseRelease = false;
                mMouseClick = false;
            };

            mControl.MouseLeave += delegate(object sender, EventArgs e)
            {
                mMouseDown = false;
                mMouseRelease = true;
                mMouseClick = false;
            };
        }

        public void Cycle()
        {
            mStart = mEnd;
            mEnd = mLastMouse;
            mDelta = mEnd - mStart;
        }

        public void Reset()
        {
            Point mouse = Cursor.Position;

            mStart = mEnd = new Vector2(mouse.X, mouse.Y);
            mDelta = Vector2.Zero;
            mMouseDown = mMouseClick = mMouseRelease = false;
        }
    }
}