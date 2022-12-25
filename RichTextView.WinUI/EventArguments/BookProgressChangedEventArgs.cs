namespace RichTextView.WinUI.EventArguments
{
    public class BookProgressChangedEventArgs
    {
        public double VerticalOffset { get; }

        public double ScrollableHeight { get; }

        public BookProgressChangedEventArgs(double verticalOffset, double scrollableHeight)
        {
            VerticalOffset = verticalOffset;
            ScrollableHeight = scrollableHeight;
        }
    }
}
