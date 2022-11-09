using System.Collections.Generic;
using System.Text;
using Fb2.Document.Models;
using Fb2.Document.UI.WinUi.Entities;
using Fb2.Document.UI.WinUi.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.NodeProcessors
{
    public class AuthorProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var authorInfo = context.CurrentNode as Author;
            var sb = new StringBuilder();

            var fName = authorInfo.GetFirstChild<FirstName>();
            if (fName != null)
                sb.Append(fName.Content);

            var nickName = authorInfo.GetFirstChild<Nickname>();
            if (nickName != null)
                sb.Append($" {nickName.Content}");

            var mName = authorInfo.GetFirstChild<MiddleName>();
            if (mName != null)
                sb.Append($" {mName.Content}");

            var lName = authorInfo.GetFirstChild<LastName>();
            if (lName != null)
                sb.Append($" {lName.Content}");

            var text = sb.ToString();

            var run = new Run { Text = text };

            return context.Utils.Paragraphize(run);
        }
    }
}
