using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IDE_UI.Helper
{
    class IdleExec
    {
        public event Action timeOutAction;

        private Timer timer;

        private int ms;

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
            timer.Interval = ms; //执行间隔时间,单位为毫秒
            timer.Start();
            
        }

        public IdleExec(int sec)
        {
            this.ms = sec * 1000;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeOutAction?.Invoke();
        }
    }
}
