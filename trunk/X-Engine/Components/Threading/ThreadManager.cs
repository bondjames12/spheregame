using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using System.Threading;

namespace Sphere
{
#if XBOX
    ///
    /// Processor affinity map.
    /// Index CPU CORE Comment
    /// -----------------------------------------------------------------------
    ///   0    1    1  Please avoid using. (used by 360)
    ///   1    1    2  Game runs here by default, so avoid this one too.
    ///   2    2    1  Please avoid using. (used by 360)
    ///   3    2    2  Part of Guide and Dashbaord live here so usable in game.
    ///   4    3    1  Live market place downloads use this so usable in game.
    ///   5    3    2  Part of Guide and Dashbaord live here so usable in game.
    /// -----------------------------------------------------------------------  
    ///
#endif
        /// <summary>
        /// This is the delegate to be used for passing the code to be called in the tread.
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        public delegate void ThreadCode(GameTime gameTime);

        /// <summary>
        /// This class holds the required data for the code to be called in the thread.
        /// </summary>
        internal class ThreadCodeObj
        {
            public ThreadCode CodeToCall = null;

            /// <summary>
            /// Mutex to stop thread clashes.
            /// </summary>
            private static Mutex mutex = new Mutex();
            /// <summary>
            /// Used to make the thread wait.
            /// </summary>
            private ManualResetEvent threadStopEvent = new ManualResetEvent(false);
            /// <summary>
            /// Bool to control imediate stopping of thread loop.
            /// </summary>        
            public bool stopThread = false;
            /// <summary>
            /// Interval thread will wait befoer next cycle.
            /// </summary>        
            private int threadIntervals = 1;

#if XBOX 
        int processorAffinity;
#endif

            public GameTime gameTime;

#if XBOX
        public ThreadCodeObj(ThreadCode code,int interval,int affinity)
        {
            CodeToCall = code;
            threadIntervals = interval;
            processorAffinity = affinity;
        }
#else
            public ThreadCodeObj(ThreadCode code, int interval)
            {
                CodeToCall = code;
                threadIntervals = interval;
            }
#endif
            public void Update(GameTime gameTime)
            {
                this.gameTime = gameTime;
            }
            public void Worker()
            {
#if XBOX
            Thread.CurrentThread.SetProcessorAffinity(new int[] { processorAffinity });
#endif
                do
                {
                    try
                    {
                        mutex.WaitOne();

                        if (gameTime != null)
                            CodeToCall(gameTime);

                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
                while (!threadStopEvent.WaitOne(threadIntervals, false) && !stopThread);
            }
            public void KillThread(Thread thread)
            {
                if (!stopThread)
                {
                    mutex.WaitOne();
                    stopThread = true;
                    thread.Join(0);
                    mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// This is the Thread manager.
        /// </summary>
        public class ThreadManager : GameComponent
        {

            /// <summary>
            /// List of ThreadStart used to start the treads.
            /// </summary>
            private Dictionary<int, ThreadStart> threadStarters = new Dictionary<int, ThreadStart>();
            private Dictionary<int, ThreadCodeObj> threadedCodeList = new Dictionary<int, ThreadCodeObj>();
            /// <summary>
            /// List of runnign threads.
            /// </summary>
            private Dictionary<int, Thread> threads = new Dictionary<int, Thread>();

            /// <summary>
            /// Managers GameTime to be passed onto the threads.
            /// </summary>
            static GameTime gameTime;
            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="game">Calling game class</param>
            public ThreadManager(Game game)
                : base(game)
            { }

            /// <summary>
            /// Overiden Update call, loads manager gameTime varaible.
            /// </summary>
            /// <param name="gameTime"></param>
            public override void Update(GameTime gameTime)
            {
                ThreadManager.gameTime = gameTime;

                for (int t = 0; t < threadedCodeList.Count; t++)
                    threadedCodeList[t].Update(gameTime);

                base.Update(gameTime);
            }

            /// <summary>
            /// Method to add a thread to the maanger.
            /// </summary>
            /// <param name="threadCode">Code to be executed in the thread.</param>
            /// <param name="threadInterval">Time period between each call in miliseconds</param>
            /// <returns>Index of thread, first one added will be 0 next 1 etc..</returns>
#if XBOX
        public int AddThread(ThreadCode threadCode, int threadInterval,int affinityIndex)
#else
            public int AddThread(ThreadCode threadCode, int threadInterval)
#endif
            {
                int retVal = threads.Count;

#if XBOX
            ThreadCodeObj thisThread = new ThreadCodeObj(threadCode, threadInterval,affinityIndex);
#else
                ThreadCodeObj thisThread = new ThreadCodeObj(threadCode, threadInterval);
#endif

                threadedCodeList.Add(threadedCodeList.Count, thisThread);
                threadStarters.Add(threadStarters.Count, new ThreadStart(thisThread.Worker));
                threads.Add(threads.Count, new Thread(threadStarters[threads.Count]));

                threads[threads.Count - 1].Start();

                return retVal;
            }

            /// <summary>
            /// Method to kill a single thread.
            /// </summary>
            /// <param name="index"></param>
            public void KillThread(int index)
            {
                threadedCodeList[index].KillThread(threads[index]);
            }

            /// <summary>
            /// Method to start a thread
            /// </summary>
            /// <param name="threadCode"></param>
            /// <param name="threadInterval"></param>
            /// <param name="index"></param>
            public void StartThread(ThreadCode threadCode, int threadInterval, int index)
            {
                if (threadedCodeList[index].stopThread)
                {
                    threads[index] = new Thread(threadStarters[index]);
                    threadedCodeList[index].stopThread = false;
                    threads[index].Start();
                }
            }

            /// <summary>
            /// Method to tidy up unfinished threads.
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                for (int t = 0; t < threads.Count; t++)
                    KillThread(t);

                base.Dispose(disposing);
            }
        }
}
