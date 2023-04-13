using System.Collections.Generic;
using System.Windows.Documents;

namespace Fb2.Document.WPF.Entities;

public class Fb2ContentPage : List<TextElement>
{
    public Fb2ContentPage(IEnumerable<TextElement> textElements) : base(textElements) { }
}
