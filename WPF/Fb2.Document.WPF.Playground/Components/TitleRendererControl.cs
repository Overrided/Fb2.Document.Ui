using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Fb2.Document.Models.Base;
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
///     <MyNamespace:TitleRendererControl/>
///
/// </summary>
public class TitleRendererControl : Control
{
    static TitleRendererControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(TitleRendererControl),
            new FrameworkPropertyMetadata(typeof(TitleRendererControl)));
    }

    private FlowDocument? TitleDoc = null;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleDoc = GetTemplateChild("TitleDoc") as FlowDocument;
    }

    public TitleInfoBase TitleInfo
    {
        get { return (TitleInfoBase)GetValue(TitleInfoProperty); }
        set { SetValue(TitleInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TitleInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleInfoProperty =
        DependencyProperty.Register(
            "TitleInfo",
            typeof(TitleInfoBase),
            typeof(TitleRendererControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleInfoPropertyChangedCallback)));

    private static void OnTitleInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as TitleRendererControl;
        if (sender == null)
            return;

        var titleInfo = sender.TitleInfo;
        if (titleInfo == null)
        {
            return;
        }

        var authors = titleInfo.GetDescendants<Author>().Where(a =>
        {
            var result = a != null && a.HasContent &&
                ((a.TryGetFirstDescendant(ElementNames.FirstName, out var fName) && fName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.MiddleName, out var mName) && mName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.LastName, out var lName) && lName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.NickName, out var nName) && nName!.HasContent));
            return result;
        });
        var titleInfoBookName = titleInfo.GetFirstDescendant<BookTitle>();
        var subTitle = titleInfo.GetFirstDescendant<SubTitle>();
        var annotation = titleInfo.GetFirstDescendant<Annotation>();
        //var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && !s.IsEmpty && s.HasAttributes);
        var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && s.HasContent && s.HasAttributes);
        var keywords = titleInfo.GetDescendants<Keywords>().Where(k => k != null && k.HasContent);

        var nodes = new List<Fb2Node>();

        nodes.AddRange(authors);
        nodes.AddRange(sequences);

        if (titleInfoBookName != null)
            nodes.Add(titleInfoBookName);

        if (subTitle != null)
            nodes.Add(subTitle);

        if (annotation != null)
            nodes.Add(annotation);

        nodes.AddRange(keywords.Where(k => k != null && k.HasContent));

        var mappedTitle = Fb2Mapper.Instance.MapNodes(nodes);
        var allTextElements = mappedTitle
            .SelectMany(c => c)
            .Where(c => c != null)
            .ToList();

        var blockTextElements = Utils.Instance.Paragraphize(allTextElements);

        var doc = sender.FindName("TitleDoc");

        sender.TitleDoc?.Blocks.AddRange(blockTextElements);
    }
}
