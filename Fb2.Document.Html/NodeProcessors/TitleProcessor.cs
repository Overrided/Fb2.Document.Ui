using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Html.Entities;
using Fb2.Document.Html.NodeProcessors.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class TitleProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string CorrespondingHtmlTag => "h2";

    //public override string Process(RenderingContext context)
    //{
    //    this.CorrespondingHtmlTag = "sfd";

    //    return base.Process(context);
    //}
}
