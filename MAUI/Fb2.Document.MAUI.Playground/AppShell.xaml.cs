using Fb2.Document.MAUI.Playground.Pages;

namespace Fb2.Document.MAUI.Playground
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Read", typeof(ReadPage));
            Routing.RegisterRoute("BookInfo", typeof(BookInfoPage));
        }
    }
}