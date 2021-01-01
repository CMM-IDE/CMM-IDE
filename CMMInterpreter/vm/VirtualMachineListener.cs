using System;

namespace CMMInterpreter.vm {

    public interface VirtualMachineListener {
        void write(Object o);
    }


}