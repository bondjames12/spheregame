using System.Diagnostics;

namespace X_Editor
{
    public class XEditorGameTime
    {
        public Stopwatch ElapsedGameTime;
        public Stopwatch TotalGameTime;

        public XEditorGameTime()
        {
            ElapsedGameTime = new Stopwatch();
            TotalGameTime = new Stopwatch();

            ElapsedGameTime.Start();
            TotalGameTime.Start();
        }

        public void Update()
        {
            ElapsedGameTime.Reset();
            ElapsedGameTime.Start();
        }
    }
}
