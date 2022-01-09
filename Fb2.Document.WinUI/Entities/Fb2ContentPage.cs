using System.Collections.Generic;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.Entities
{
    public class Fb2ContentPage : List<TextElement>
    {
        public Fb2ContentPage(IEnumerable<TextElement> textElements) : base(textElements) { }
    }
}
