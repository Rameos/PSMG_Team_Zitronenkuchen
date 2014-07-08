
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace iViewX
{
    public static class Win32HelperClass
    {

        /// <summary>
        /// Pointer to the GameWindow;
        /// </summary>
        private static IntPtr pointerToGameWindow;

        /// <summary>
        /// Point struct to save the Position of the GameView
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr window, ref Point lPPoint);

        /// <summary>
        /// Get the Position of the GameView. (Use this informations if you have some troubles with windowed Applications)
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetGameViewPosition()
        {
            Win32HelperClass.Point position = new Win32HelperClass.Point();
            pointerToGameWindow = Win32HelperClass.GetForegroundWindow();
            Win32HelperClass.ClientToScreen(pointerToGameWindow, ref position);
            return new Vector2(position.x, position.y);
        }
    }
}
