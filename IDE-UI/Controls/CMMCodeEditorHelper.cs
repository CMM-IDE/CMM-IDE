using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IDE_UI.Controls
{
    public partial class CMMCodeEditor
    {

        /// <summary>
        /// Converts a Win32 colour to a Drawing.Color
        /// </summary>
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Converts a Win32 colour to a Media Color
        /// </summary>
        public static Color IntToMediaColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

    }
}
