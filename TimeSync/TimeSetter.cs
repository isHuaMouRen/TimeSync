using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TimeSync
{
    public class TimeSetter
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);

        public static bool SetTime(DateTime newTime)
        {
            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (ushort)newTime.Year,
                wMonth = (ushort)newTime.Month,
                wDay = (ushort)newTime.Day,
                wHour = (ushort)newTime.Hour,
                wMinute = (ushort)newTime.Minute,
                wSecond = (ushort)newTime.Second,
                wMilliseconds = (ushort)newTime.Millisecond
            };

            return SetLocalTime(ref st);
        }
    }
}
