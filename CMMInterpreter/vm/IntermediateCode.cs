﻿using System;
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

        public InstructionType type { get; set; }

        public IntermediateCodeInformation information { get; set; }

        public IntermediateCode()
        {
        }

        public IntermediateCode(InstructionType _type)
        {
            type = _type;
        }

        public IntermediateCode(Object _operant, InstructionType _type)
        {
            operant = _operant;
            type = _type;
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
            return this.type + " " + this.operant + "\n" ;
        }

    }
}
