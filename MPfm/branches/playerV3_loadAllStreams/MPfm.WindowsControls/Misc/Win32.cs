//
// Win32.cs: This file contains structured based on the Win32 kernel.
//
// Copyright © 2011 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{


    //    <StructLayout(LayoutKind.Sequential)> _
    //    Public Structure NMHDR
    //        Public hwndFrom As IntPtr
    //        Public idFrom As Integer
    //        Public code As Integer
    //    End Structure

    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public IntPtr hwndFrom;
        public int idFrom;
        public int code;
    }

    //    <StructLayout(LayoutKind.Sequential)> _
    //    Public Structure RECT
    //        Public left As Integer
    //        Public top As Integer
    //        Public right As Integer
    //        Public bottom As Integer
    //    End Structure

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    //    <StructLayout(LayoutKind.Sequential)> _
    //    Public Structure NMCUSTOMDRAW
    //        Public hdr As NMHDR
    //        Public dwDrawStage As Integer
    //        Public hdc As IntPtr
    //        Public rc As RECT
    //        Public dwItemSpec As Integer
    //        Public uItemState As Integer
    //        Public lItemlParam As IntPtr
    //    End Structure
    //End Class

    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        public NMHDR ndr;
        public int dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public int dwItemSpec;
        public int uItemState;
        public IntPtr lItemlParam;
    }

    public static class Win32
    {
        //    Private Class Win32
        //    Public Enum Consts
        //        ' messages
        //        WM_NCPAINT = &H85
        //        WM_ERASEBKGND = &H14
        //        WM_NOTIFY = &H4E
        //        OCM_BASE = &H2000
        //        OCM_NOTIFY = OCM_BASE + WM_NOTIFY
        //        NM_CUSTOMDRAW = -12
        //        NM_SETFOCUS = -7
        //        LVN_ITEMCHANGED = -101

        //        ' custom draw return flags
        //        CDRF_DODEFAULT = &H0
        //        CDRF_SKIPDEFAULT = &H4
        //        CDRF_NOTIFYITEMDRAW = &H20

        //        ' custom draw state flags
        //        CDDS_PREPAINT = &H1
        //        CDDS_ITEM = &H10000
        //        CDDS_ITEMPREPAINT = CDDS_ITEM Or CDDS_PREPAINT
        //    End Enum

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_MOUSELEAVE = 0x2A3;
        public const int WH_MOUSE_LL = 14;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_LBUTTONDBLCLK = 0x203;  

        public const int WM_PAINT = 0xF;
        public const int WM_ERASEBKGND = 0x14;

        public const int WM_PRINT = 0x317;

        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;

        public const int WM_PRINTCLIENT = 0x318;

        public const long PRF_CHECKVISIBLE = 0x1L;
        public const long PRF_NONCLIENT = 0x2L;
        public const long PRF_CLIENT = 0x4L;
        public const long PRF_ERASEBKGND = 0x8L;
        public const long PRF_CHILDREN = 0x10L;
        public const long PRF_OWNED = 0x20L;

        public const long WM_NCPAINT = 0x85;
        //public const long WM_ERASEBKGND = 0x14;
        public const long WM_NOTIFY = 0x4E;
        public const long OCM_BASE = 0x2000;
        public const long OCM_NOTIFY = OCM_BASE + WM_NOTIFY;
        public const long NM_CUSTOMDRAW = -12;
        public const long NM_SETFOCUS = -7;
        public const long LVN_ITEMCHANGED = -101;

        //custom draw return flags
        public const long CDRF_DODEFAULT = 0x0;
        public const long CDRF_SKIPDEFAULT = 0x4;
        public const long CDRF_NOTIFYITEMDRAW = 0x20;

        //custom draw state flags
        public const long CDDS_PREPAINT = 0x1;
        public const long CDDS_ITEM = 0x10000;
        public const long CDDS_ITEMPREPAINT = CDDS_ITEM | CDDS_PREPAINT;

        [DllImport("user32.dll")]
        public static extern int SendMessage(
             IntPtr hWnd,      // handle to destination window
             uint Msg,       // message
             IntPtr wParam,  // first message parameter
             IntPtr lParam   // second message parameter
             );

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private const int MK_LBUTTON = 0x0001;
        private const int MK_RBUTTON = 0x0002;
        private const int MK_SHIFT = 0x0004;
        private const int MK_CONTROL = 0x0008;
        private const int MK_MBUTTON = 0x0010;
        private const int MK_XBUTTON1 = 0x0020;
        private const int MK_XBUTTON2 = 0x0040;

        public static int GetWheelDeltaWParam(int wparam) { return HighWord(wparam); }

        public static MouseButtons GetMouseButtonWParam(int wparam)
        {
            int mask = LowWord(wparam);

            if ((mask & MK_LBUTTON) == MK_LBUTTON) return MouseButtons.Left;
            if ((mask & MK_RBUTTON) == MK_RBUTTON) return MouseButtons.Right;
            if ((mask & MK_MBUTTON) == MK_MBUTTON) return MouseButtons.Middle;
            if ((mask & MK_XBUTTON1) == MK_XBUTTON1) return MouseButtons.XButton1;
            if ((mask & MK_XBUTTON2) == MK_XBUTTON2) return MouseButtons.XButton2;

            return MouseButtons.None;
        }

        public static bool IsCtrlKeyPressedWParam(int wparam)
        {
            int mask = LowWord(wparam);
            return (mask & MK_CONTROL) == MK_CONTROL;
        }

        public static bool IsShiftKeyPressedWParam(int wparam)
        {
            int mask = LowWord(wparam);
            return (mask & MK_SHIFT) == MK_SHIFT;
        }

        public static int GetXLParam(int lparam) { return LowWord(lparam); }

        public static int GetYLParam(int lparam) { return HighWord(lparam); }

        public static int LowWord(int word) { return word & 0xFFFF; }

        public static int HighWord(int word) { return word >> 16; }

        #region Icons
        
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        #endregion
    }
}
