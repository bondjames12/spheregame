using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XConsole : XComponent, XDrawable
    {
        List<string> log = new List<string>();
        public bool Visible = false;
        public int MaxLines = 10;

        XTexture backgroundTexture;
        static bool LastLineReturnValue = true;

        public XConsole(XMain X) : base(X) 
        {
            DrawOrder = 1000;
            backgroundTexture = new XTexture(X, @"Content\XEngine\Textures\ConsoleBackground");
        }

        public override void Load(ContentManager Content)
        {
            backgroundTexture.Load(Content);
            base.Load(Content);
        }

        static string line = "";

        private static Keys[] keys = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                                       Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
                                       Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
                                       Keys.OemComma, Keys.OemCloseBrackets, Keys.OemOpenBrackets, Keys.OemMinus};

        public void AcceptInput(XKeyboard keyboard)
        {
            if (Visible)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keyboard.KeyPressed(keys[i]))
                    {
                        string add = keys[i].ToString().Replace("D1", "1");
                        add = add.Replace("D2", "2");
                        add = add.Replace("D3", "3");
                        add = add.Replace("D4", "4");
                        add = add.Replace("D5", "5");
                        add = add.Replace("D6", "6");
                        add = add.Replace("D7", "7");
                        add = add.Replace("D8", "8");
                        add = add.Replace("D9", "9");
                        add = add.Replace("D0", "0");
                        add = add.Replace("OemComma", ",");
                        add = add.Replace("OemOpenBrackets", "{");
                        add = add.Replace("OemCloseBrackets", "}");
                        add = add.Replace("OemMinus", "-");

                        if (!keyboard.KeyDown(Keys.LeftShift))
                            line += add.ToLower();
                        else
                        {
                            add = add.Replace("9", "(");
                            add = add.Replace("0", ")");
                            line += add;
                        }
                    }
                }

                if (keyboard.KeyPressed(Keys.Back))
                {
                    if (line != "")
                        line = line.Remove(line.Length - 1, 1);
                }

                if (keyboard.KeyPressed(Keys.Enter))
                {
                    if (line != "")
                    {
                        log.Add(line);
                        line = "";
                        LastLineReturnValue = false;

                        if (log.Count > MaxLines)
                            log.Remove(log[0]);
                    }
                }

                if (keyboard.KeyPressed(Keys.Space))
                    line += " ";

                //CHANGE:add escape to get out of console
                if (keyboard.KeyPressed(Keys.Escape))
                    this.Visible = false;
            }
        }

        string lastvalue = "";

        public string GetCommand()
        {
            if (!LastLineReturnValue && log.Count > 0)
            {
                if (lastvalue != log[log.Count - 1])
                {
                    lastvalue = log[log.Count - 1];
                    return log[log.Count - 1];
                }
                else
                    return null;
            }
            else
                return null;
        }

        public void Execute(string Value)
        {
            log.Add(Value);
            line = "";
            LastLineReturnValue = false;

            if (log.Count > MaxLines)
                log.Remove(log[0]);
        }

        public void Return(string Value)
        {
            log.Add(Value);
            line = "";
            LastLineReturnValue = true;

            if (log.Count > MaxLines)
                log.Remove(log[0]);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (Visible)
            {
                backgroundTexture.Draw(gameTime, new Rectangle(0, 0, X.GraphicsDevice.Viewport.Width, X.Debug.RowHeight * log.Count + 30), 0, Color.White);
                
                for (int i = 0; i < log.Count; i++)
                    X.SystemFont.Draw(log[i], new Vector2(5, i * X.Debug.RowHeight + 4), Color.White);

                if (LastLineReturnValue)
                    LastLineReturnValue = false;

                X.SystemFont.Draw("> " + line, new Vector2(5, log.Count * X.Debug.RowHeight + 4), Color.White);
            }
        }

    }
}
