using System.Collections.Generic;
using Fb2.Document.Models;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class AuthorProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var authorInfo = context.CurrentNode as Author;

            var names = new List<string>();

            var fName = authorInfo.GetFirstChild<FirstName>();
            if (fName != null)
                names.Add(fName.Content);

            var nickName = authorInfo.GetFirstChild<Nickname>();
            if (nickName != null)
                names.Add(nickName.Content);

            var mName = authorInfo.GetFirstChild<MiddleName>();
            if (mName != null)
                names.Add(mName.Content);

            var lName = authorInfo.GetFirstChild<LastName>();
            if (lName != null)
                names.Add(lName.Content);

            var finalName = string.Join(' ', names);

            var p = new Windows.UI.Xaml.Documents.Paragraph();
            p.Inlines.Add(new Run { Text = finalName });

            return new List<TextElement>(1) { p };
        }
    }
}
