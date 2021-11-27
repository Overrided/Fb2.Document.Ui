﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using WinRT;

namespace Fb2.Document.WinUI.Playground.Services
{
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    internal interface IWindowNative
    {
        IntPtr WindowHandle { get; }
    }

    public class PopupInitializerService
    {
        private static PopupInitializerService instance = new PopupInitializerService();

        public static PopupInitializerService Instance { get { return instance; } }

        private PopupInitializerService() { }

        private Window rootHandleWindow;
        public bool IsServiceInitialized = false;

        public void Initialize(Window root)
        {
            this.rootHandleWindow = root;
            this.IsServiceInitialized = true;
        }

        public void InitializePopup(IWinRTObject popupWindow)
        {
            if (!IsServiceInitialized)
                throw new Exception("Service not initialized");

            var hwnd = rootHandleWindow.As<IWindowNative>().WindowHandle;

            var initializeWithWindow = popupWindow.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);
        }
    }
}
