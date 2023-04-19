using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fb2.Document.Models;
using Fb2.Document.WPF.Common;
using Fb2.Document.WPF.Playground.Models;

namespace Fb2.Document.WPF.Playground.Pages;

/// <summary>
/// Interaction logic for ReadPage.xaml
/// </summary>
public partial class ReadPage : Page
{
    private bool isContentRendered = false;
    private readonly BookModel? BookModel = null;

    public ReadPage(BookModel bookModel)
    {
        InitializeComponent();
        BookModel = bookModel;
        this.Loaded += ReadPage_Loaded;
    }

    private void ReadPage_Loaded(object sender, RoutedEventArgs e)
    {
        doc.ColumnWidth = double.MaxValue; // overkill

        if (!isContentRendered)
        {
            var uiContent = Fb2Mapper.Instance.MapDocument(BookModel.Fb2Document!);
            var allTextElements = uiContent
                .SelectMany(c => c)
                .Where(c => c != null)
                .ToList();

            var blockTextElements = Utils.Instance.Paragraphize(allTextElements);

            doc.Blocks.AddRange(blockTextElements);

            isContentRendered = true;
        }
    }
}
