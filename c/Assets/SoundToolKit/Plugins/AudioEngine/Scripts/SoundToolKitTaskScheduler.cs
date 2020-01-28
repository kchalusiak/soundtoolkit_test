using System;
using System.Collections.Generic;
using System.Threading;

namespace SoundToolKit.Unity
{
    public sealed class SoundToolKitTaskScheduler
    {
        public SoundToolKitTaskScheduler()
        {
            m_stkThread = new Thread(ThreadFunction);
            m_stkThread.Start();
        }

        public int Schedule(Action task)
        {
            int tasksCount = 0;
            lock (m_tasks)
            {
                if (!m_isStopping)
                {
                    m_tasks.Enqueue(task);
                    tasksCount = m_tasks.Count;
                    Monitor.Pulse(m_tasks);
                }
                else
                {
                    task();
                }
            }
            return tasksCount;
        }

        public void Stop()
        {
            lock (m_tasks)
            {
                m_isStopping = true;
                Monitor.Pulse(m_tasks);
            }
            m_stkThread.Join();
        }

        private void ThreadFunction()
        {
            Action task = null;
            while (true)
            {
                lock (m_tasks)
                {
                    while (m_tasks.Count == 0 && !m_isStopping)
                    {
                        Monitor.Wait(m_tasks);
                    }
                    if (m_tasks.Count > 0)
                    {
                        task = m_tasks.Dequeue();
                    }
                    else
                    {
                        return;
                    }
                }
                task();
            }
        }

        private bool m_isStopping = false;
        private Thread m_stkThread;
        private Queue<Action> m_tasks = new Queue<Action>();
    }
}