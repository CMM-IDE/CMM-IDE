using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDE_UI.Helper
{
    /// <summary>
    /// 用于获取屏幕缩放比例的类。
    /// </summary>
    public static class WindowsMonitorAPI
    {
        private const string User32 = "user32.dll";

        [DllImport(User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        [DllImport(User32, ExactSpelling = true)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool EnumDisplayMonitors(HandleRef hdc, COMRECT rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            internal int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            internal RECT rcMonitor = new RECT();
            internal RECT rcWork = new RECT();
            internal int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal char[] szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(Rect r)
            {
                left = (int)r.Left;
                top = (int)r.Top;
                right = (int)r.Right;
                bottom = (int)r.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class COMRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public static readonly HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);
    }


    public class ScreenHelper
    {

        public static double ScalingRatio = GetScalingRatio();
        /// <summary>
        /// 获取缩放比例
        /// </summary>
        /// <returns></returns>
        public static double GetScalingRatio()
        {
            var logicalHeight = GetLogicalHeight();
            var actualHeight = GetActualHeight();

            if (logicalHeight > 0 && actualHeight > 0) {
                return logicalHeight / actualHeight;
            }

            return 1;
        }

        private static double GetActualHeight()
        {
            return SystemParameters.PrimaryScreenHeight;
        }

        private static double GetLogicalHeight()
        {
            var logicalHeight = 0.0;

            WindowsMonitorAPI.MonitorEnumProc proc = (m, h, lm, lp) => {
                WindowsMonitorAPI.MONITORINFOEX info = new WindowsMonitorAPI.MONITORINFOEX();
                WindowsMonitorAPI.GetMonitorInfo(new HandleRef(null, m), info);

                //是否为主屏
                if ((info.dwFlags & 0x00000001) != 0) {
                    logicalHeight = info.rcMonitor.bottom - info.rcMonitor.top;
                }

                return true;
            };
            WindowsMonitorAPI.EnumDisplayMonitors(WindowsMonitorAPI.NullHandleRef, null, proc, IntPtr.Zero);

            return logicalHeight;
        }
    }

}
