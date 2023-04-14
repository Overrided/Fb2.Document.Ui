using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RichTextView.UWP.EventArguments
{
    public class RichHyperlinkActivatedEventArgs : EventArgs
    {
        public object OriginalSender { get; }

        public object OriginalArgs { get; }

        public RichHyperlinkActivatedEventArgs(object innerSender, object innerArgs)
        {
            OriginalSender = innerSender;
            OriginalArgs = innerArgs;
        }
    }
}
