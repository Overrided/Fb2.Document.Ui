﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Html.NodeProcessors.Base;

namespace Fb2.Document.Html.NodeProcessors;

public class SubTitleProcessor : DefaultFb2HtmlNodeProcessor
{
    public override string CorrespondingHtmlTag => "h4";
}