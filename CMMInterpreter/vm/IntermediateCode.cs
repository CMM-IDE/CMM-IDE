using System;
using System.Collections.Generic;
using System.Text;
using CMMInterpreter.debuger;
namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * 中间代码
     * 
     */
    public class IntermediateCode
    {

        public Object operant { get; set; }

        public int lineNum { get; set; }

        public InstructionType type { get; set; }

        public IntermediateCodeInformation information { get; set; }

        public IntermediateCode(int line)
        {
            information = new IntermediateCodeInformation();
            lineNum = line;
        }

        public IntermediateCode(InstructionType _type, int line)
        {
            type = _type;
            information = new IntermediateCodeInformation();
            lineNum = line;
        }

        public IntermediateCode(Object _operant, InstructionType _type, int line)
        {
            operant = _operant;
            type = _type;
            lineNum = line;
            information = new IntermediateCodeInformation();
        }


        public void setOperant(Object _operant)
        {
            operant = _operant;
        }

        public InstructionType getType()
        {
            return type;
        }
        
        public string toString()
        {
            return this.type + "\t\t" + this.operant;
        }

    }
}
