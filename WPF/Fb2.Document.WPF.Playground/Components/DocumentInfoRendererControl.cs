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
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.WPF.Common;

namespace Fb2.Document.WPF.Playground.Components;

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Fb2.Document.WPF.Playground.Components"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Fb2.Document.WPF.Playground.Components;assembly=Fb2.Document.WPF.Playground.Components"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:DocumentInfoRendererControl/>
///
/// </summary>
public class DocumentInfoRendererControl : Control
{
    static DocumentInfoRendererControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentInfoRendererControl), new FrameworkPropertyMetadata(typeof(DocumentInfoRendererControl)));
    }

    private FlowDocument? DocumentInfoViewer = null;



    public DocumentInfo DocumentInfo
    {
        get { return (DocumentInfo)GetValue(DocumentInfoProperty); }
        set { SetValue(DocumentInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DocumentInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DocumentInfoProperty =
        DependencyProperty.Register(
            "DocumentInfo",
            typeof(DocumentInfo),
            typeof(DocumentInfoRendererControl));

    public DocumentInfoRendererControl()
    {
        this.Loaded += DocumentInfoRendererControl_Loaded;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        DocumentInfoViewer = GetTemplateChild("DocumentInfoFlowDoc") as FlowDocument;
    }

    private void DocumentInfoRendererControl_Loaded(object sender, RoutedEventArgs e)
    {
        //var documentInfo = DocumentInfo;
        if (DocumentInfo == null)
        {
            return;
        }

        // drop "empty" authors
        DocumentInfo.RemoveContent(n =>
        {
            var isAuthor = n is Author;
            if (!isAuthor)
                return false;

            var authorNode = (Author)n;
            var hasSomeName = authorNode.HasContent &&
                ((authorNode.TryGetFirstDescendant(ElementNames.FirstName, out var fName) && fName!.HasContent) ||
                (authorNode.TryGetFirstDescendant(ElementNames.MiddleName, out var mName) && mName!.HasContent) ||
                (authorNode.TryGetFirstDescendant(ElementNames.LastName, out var lName) && lName!.HasContent) ||
                (authorNode.TryGetFirstDescendant(ElementNames.NickName, out var nName) && nName!.HasContent));

            return !hasSomeName;
        });

        var mappedNodes = Fb2Mapper.Instance.MapNode(
            DocumentInfo,
            new(useStyles: false));

        var normalizedContent = mappedNodes.SelectMany(uic => uic);
        var blockContent = Utils.Instance.Paragraphize(normalizedContent);
        DocumentInfoViewer?.Blocks.AddRange(blockContent);

        //var contentPage = new RichContentPage(normalizedContent);
        //var content = new RichContent(new List<RichContentPage>(1) { contentPage });
        //sender.ViewModel.DocumentInfoContent = content;
    }
}
