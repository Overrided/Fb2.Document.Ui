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
using Paragraph = System.Windows.Documents.Paragraph;

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
///     <MyNamespace:CustomInfoRendererControl/>
///
/// </summary>
public class CustomInfoRendererControl : Control
{
    FlowDocument? CustomInfoFlowDoc = null;

    public CustomInfo CustomInfo
    {
        get { return (CustomInfo)GetValue(CustomInfoProperty); }
        set { SetValue(CustomInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CustomInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomInfoProperty =
        DependencyProperty.Register(
            "CustomInfo",
            typeof(CustomInfo),
            typeof(CustomInfoRendererControl));

    static CustomInfoRendererControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomInfoRendererControl), new FrameworkPropertyMetadata(typeof(CustomInfoRendererControl)));
    }

    public CustomInfoRendererControl()
    {
        this.Loaded += CustomInfoRendererControl_Loaded;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        CustomInfoFlowDoc = GetTemplateChild("CustomInfoFlowDoc") as FlowDocument;
    }

    private void CustomInfoRendererControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (CustomInfo == null)
            return;

        var contents = new List<string>();
        var trimmedContent = CustomInfo.Content.Trim();

        if (!string.IsNullOrEmpty(trimmedContent))
            contents.Add(trimmedContent);

        if (CustomInfo.Attributes.Any())
            contents.AddRange(CustomInfo.Attributes.Select(a => $"{a.Key} {a.Value}"));

        if (contents.Count == 0)
            return;

        var customInfoContent = string.Join(Environment.NewLine, contents);

        var run = new Run { Text = customInfoContent };
        var paragraph = new Paragraph();
        paragraph.Inlines.Add(run); // dirty trick

        CustomInfoFlowDoc?.Blocks.Add(paragraph);
    }
}
