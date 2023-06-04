namespace Fb2.Document.MAUI.Playground.Controls;

public partial class ExpanderHeaderControl : ContentView
{
    public static readonly BindableProperty HeaderContentProperty =
        BindableProperty.Create(
            "HeaderContent",
            typeof(IView),
            typeof(ExpanderHeaderControl));

    public IView? HeaderContent
    {
        get => (IView?)GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public ExpanderHeaderControl()
    {
        InitializeComponent();
    }
}