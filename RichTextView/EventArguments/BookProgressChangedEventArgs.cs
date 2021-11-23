namespace RichTextView.EventArguments
{
    public class BookProgressChangedEventArgs
    {
        public double VerticalOffset { get; }

        public double ScrollableOffset { get; }

        public BookProgressChangedEventArgs(double verticalOffset, double scrollableOffset)
        {
            VerticalOffset = verticalOffset;
            ScrollableOffset = scrollableOffset;
        }
    }
}
