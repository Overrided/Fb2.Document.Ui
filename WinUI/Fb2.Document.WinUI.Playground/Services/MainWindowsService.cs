using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fb2.Document.WinUI.Playground.Services
{
    public class MainWindowsService
    {
        private static MainWindowsService instance = new MainWindowsService();

        private MainWindowsService() { }

        public static MainWindowsService Instance { get { return instance; } }

        private Window rootWindow;

        public Window RootWindow
        {
            get
            {
                if (!IsInitialized)
                    throw new Exception("Not Initialized");

                return rootWindow;
            }
        }

        public Frame ContentFrame { get; private set; }

        public bool IsInitialized { get; private set; } = false;

        public void Init(Window rootWindow, Frame contentFrame)
        {
            this.rootWindow = rootWindow;
            ContentFrame = contentFrame;
            IsInitialized = true;
        }
    }
}
