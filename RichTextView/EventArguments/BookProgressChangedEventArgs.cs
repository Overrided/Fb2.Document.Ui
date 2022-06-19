namespace RichTextView.EventArguments
{
    public class BookProgressChangedEventArgs
    {
        public double OldValue { get; }

        public double NewValue { get; }

        public BookProgressChangedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
