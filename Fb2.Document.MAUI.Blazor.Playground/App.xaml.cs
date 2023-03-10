using System.Text;

namespace Fb2.Document.MAUI.Blazor.Playground
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MainPage = new MainPage();
        }
    }
}