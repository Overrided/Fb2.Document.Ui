﻿using Fb2.Document.Html.NodeProcessors.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class DivFb2HtmlProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string CorrespondingHtmlTag => "div";
}
