using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace Fb2.Document.WinUI.Playground.PageNavigation
{
    internal class ReadPageNavigationParams
    {
        public Window BaseWindow { get; set; }

        public ReadPageNavigationParams(Window baseWindow)
        {
            BaseWindow = baseWindow;
        }
    }
}
