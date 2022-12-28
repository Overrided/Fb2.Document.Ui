using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.Entities
{
    public class Fb2ContentPage : List<TextElement>
    {
        public Fb2ContentPage(IEnumerable<TextElement> textElements) : base(textElements) { }
    }
}
