using CMMInterpreter.vm;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.debuger
{
    public class CMMDebuger
    {
        private int mode;
        private List<int> breakpoints;
        private Dictionary<int, IntermediateCodeInformation> intermediateCodeInformations;
        private Dictionary<int, IntermediateCode> savedInstructions;

        private IVirtualMachine vm;

        public CMMDebuger(List<IntermediateCode> codes, List<int> breakpointList)
        {
            breakpoints = breakpointList;
            vm.Load(codes);
            intermediateCodeInformations = vm.GetIntermediateCodeInformation();
            foreach(int breakpoint in breakpoints)
            {
                if (intermediateCodeInformations.ContainsKey(breakpoint))
                {

                }
            }
        }

        public void AddBreakpoint()
        {

        }

        public void RemoveBreakpoint()
        {

        }

        public int GetCurrentLine()
        {
            return 0;
        }

        public List<FrameInformation> GetCurrentFrame()
        {
            return vm.GetCurrentFrame();
        }

        public void StepInto()
        {

        }

        public void StepOver()
        {

        }
    }
}
