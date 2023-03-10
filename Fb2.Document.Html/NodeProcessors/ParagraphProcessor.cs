using Fb2.Document.Html.NodeProcessors.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class ParagraphProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string CorrespondingHtmlTag => "p";
}
