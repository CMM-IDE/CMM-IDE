using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE_UI.Helper
{
    class IDEState
    {
        public bool isRunning = false;

        public bool fileOpened = false;

        public string openedFilePath = null;

        public bool fileModified = false;

        public bool consoleShowed = true;

        public bool debugWindowShowed = false;

    }
}
