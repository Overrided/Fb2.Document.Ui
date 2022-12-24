using System.Text;

namespace Fb2.Document.MAUI.Playground
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MainPage = new AppShell();
        }
    }
}