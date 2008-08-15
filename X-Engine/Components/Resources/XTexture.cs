using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XTexture : XComponent, XUpdateable
    {
        Texture2D texture;
        string filename;
        XTimer timer;
        public bool Animated;

        int NumColumns;
        int NumRows;
        float AnimationTime;
        int CurrentCell;
        bool IsRunning;
        int TotalCells;

        Cell[] cells;

        public XTexture(XMain X, string Filename) : base(X)
        {
            filename = Filename;
            timer = new XTimer(X);
        }

        public void Animate(int NumRows, int NumColumns, int NumCells, float AnimationTime, bool Start)
        {
            this.NumRows = NumRows;
            this.NumColumns = NumColumns;
            this.AnimationTime = AnimationTime;

            int CellWidth = texture.Width / NumColumns;
            int CellHeight = texture.Height / NumRows;

            cells = new Cell[NumCells];

            for (int y = 0; y < NumRows; y++)
            {
                for (int x = 0; x < NumColumns; x++)
                {
                    if (x + (y * texture.Width / CellWidth) == NumCells)
                        break;

                    cells[x + (y * texture.Width / CellWidth)] = new Cell(CellWidth, CellHeight, y, x);
                }
            }

            TotalCells = NumCells;
            CurrentCell = 1;

            IsRunning = Start;
            Animated = true;
        }

        public void Start()
        {
            IsRunning = true;
            CurrentCell = 0;
        }

        public void Stop()
        {
            IsRunning = false;
            CurrentCell = 0;
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }

        public void Next()
        {
            if (CurrentCell < TotalCells - 1)
                CurrentCell++;
            else
                CurrentCell = 0;
        }

        public void Previous()
        {
            if (CurrentCell > 0)
                CurrentCell--;
            else
                CurrentCell = TotalCells - 1;
        }

        public override void  Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(filename);
            base.Load(Content);
        }

        public void Draw(GameTime gameTime, Rectangle rectangle, float Rotation, Color color)
        {
            if (loaded)
            {
                if (!Animated)
                    X.spriteBatch.Draw(texture, rectangle, Color.White);
                else
                {
                    if (timer.PassedTime > AnimationTime && IsRunning)
                    {
                        timer.Reset();
                        timer.Start(gameTime);

                        if (CurrentCell < TotalCells - 1)
                            CurrentCell++;
                        else
                            CurrentCell = 0;
                    }

                    X.spriteBatch.Draw(texture, rectangle, new Rectangle(
                        cells[CurrentCell].Column * cells[CurrentCell].CellWidth,
                        cells[CurrentCell].Row * cells[CurrentCell].CellWidth,
                        cells[CurrentCell].CellWidth,
                        cells[CurrentCell].CellHeight
                    ), color, Rotation, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
                }
            }
        }
    }

    class Cell
    {
        public int CellWidth;
        public int CellHeight;
        public int Row;
        public int Column;

        public Cell(int Width, int Height, int Row, int Column)
        {
            this.CellWidth = Width;
            this.CellHeight = Height;
            this.Row = Row;
            this.Column = Column;
        }
    }
}