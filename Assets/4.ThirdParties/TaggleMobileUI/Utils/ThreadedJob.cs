using System;
using System.Collections;
using System.Threading;

public class ThreadedJob
{
    private bool m_isDone = false;
    private readonly object m_handle = new object();
    private Thread m_thread = null;

    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_handle)
            {
                tmp = m_isDone;
            }
            return tmp;
        }
        set
        {
            lock (m_handle)
            {
                m_isDone = value;
            }
        }
    }

    public virtual void Start()
    {
        m_thread = new Thread(Run);
        m_thread.Start();
    }

    public virtual void Abort()
    {
        m_thread.Abort();
    }

    protected virtual void DoJob()
    {
    }

    protected virtual void OnFinished()
    {
    }

    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    private void Run()
    {
        while (!IsDone)
        {
            DoJob();
            Finish();
            Thread.Sleep(100);
            Console.WriteLine("unity: ThreadedJob running...");
        }
        Console.WriteLine("unity: ThreadedJob end");
    }

    protected virtual void Finish()
    {
        IsDone = true;
        Console.WriteLine("unity: ThreadedJob end");
    }
}