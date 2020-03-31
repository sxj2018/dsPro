using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils.Tool
{
    public class DelayHelper
    {
        private static List<TimerElapsed> timers = new List<TimerElapsed>();

        public static void DelayExecute(int seconds, Action action)
        {
            Guid timerId = Guid.NewGuid();
            Timer timer = new Timer(TimerCallback, timerId, seconds * 1000, 0);
            TimerElapsed te = new TimerElapsed();
            te.Timer = timer;
            te.TimerId = timerId;
            te.AfterAction = action;
            timers.Add(te);
        }

        private static void TimerCallback(object timerId)
        {
            var tid = (Guid)timerId;
            TimerElapsed te = timers.Find(p => p != null && p.TimerId == tid);
            if (te != null)
            {
                te.AfterAction();
                //清理工作
                te.Timer.Dispose();
                timers.Remove(te);
            }
        }

        class TimerElapsed
        {
            public Guid TimerId { get; set; }
            public Timer Timer { get; set; }
            public Action AfterAction { get; set; }
        }
    }
}
