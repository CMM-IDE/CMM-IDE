using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDE_UI.Controls
{
    public partial class DebugPanel
    {
        //TODO：有时间就把这些合为一个
        private void continue_Click(object sender, RoutedEventArgs e)
        {
            requireDebugAction(DebugOperation.Continue);
        }

        private void stepOver_Click(object sender, RoutedEventArgs e)
        {
            requireDebugAction(DebugOperation.StepOver);
        }

        private void StepIn_Click(object sender, RoutedEventArgs e)
        {
            requireDebugAction(DebugOperation.StepInto);
        }

        private void StepOut_Click(object sender, RoutedEventArgs e)
        {
            requireDebugAction(DebugOperation.StepOut);
        }
    }
}
