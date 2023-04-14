using System;

namespace RichTextView.WinUI.EventArguments
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
