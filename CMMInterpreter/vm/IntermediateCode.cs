using System;
using System.Collections.Generic;
using System.Text;

namespace CMMInterpreter.vm
{
    /**
     * @author 谭惠日 杨翔
     * @since 12-07-2020
     * 中间代码
     * 
     */
    class IntermediateCode
    {

        private Object operant;

        private InstructionType type;

        IntermediateCode(InstructionType _type)
        {
            type = _type;
        }

        IntermediateCode(Object _operant, InstructionType _type)
        {
            operant = _operant;
            type = _type;
        }


    }
}
