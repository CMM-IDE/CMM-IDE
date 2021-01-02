using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IDE_UI.Helper
{
    class IdleExecutor
    {
        public event Action timeOutAction;

        private Timer timer;

        private int idleTimeInMS;

        public void MarkActive()
        {
            if(timer != null) {
                timer.Elapsed -= Timer_Elapsed;
            }
            
            timer = new Timer {
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Interval = idleTimeInMS; //执行间隔时间,单位为毫秒
            timer.Start();
            
        }

        public IdleExecutor(int sec)
        {
            this.idleTimeInMS = sec * 1000;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeOutAction?.Invoke();
        }
    }
}
