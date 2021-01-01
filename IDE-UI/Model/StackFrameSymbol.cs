using CMMInterpreter.debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE_UI.Model
{
    public class StackFrameSymbol
    {

        public string FunctionName { get; set; }

        public int lineNum { get; set; }

        public List<FrameInformation> CurrentFrame { get; set; }

        public StackFrameSymbol(string funcName, int lineNum, List<FrameInformation> CurrentFrame)
        {
            this.FunctionName = funcName;
            this.lineNum = lineNum;
            this.CurrentFrame = CurrentFrame;
        }
    }
}
