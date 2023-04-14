using System;
using Microsoft.UI.Xaml;
using WinRT;

namespace Fb2.Document.WinUI.Playground.Services
{
    public class PopupInitializerService
    {
        private static PopupInitializerService instance = new PopupInitializerService();

        public static PopupInitializerService Instance { get { return instance; } }

        private PopupInitializerService() { }

        private IntPtr rootWindowHandle = IntPtr.Zero;
        public bool IsServiceInitialized = false;

        public void Initialize(Window root)
        {
            rootWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(root);
            IsServiceInitialized = true;
        }

        public void InitializePopup(IWinRTObject popupWindow)
        {
            if (!IsServiceInitialized)
                throw new Exception("Service not initialized");

            WinRT.Interop.InitializeWithWindow.Initialize(popupWindow, rootWindowHandle);
        }
    }
}
