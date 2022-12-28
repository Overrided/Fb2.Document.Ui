using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Fb2.Document.UWP.Playground.Models;

namespace Fb2.Document.UWP.Playground.TemplateSelectors
{
    public class BookTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AddBookTemplate { get; set; }
        public DataTemplate ViewBookTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is BookModel book)
                return book.Equals(BookModel.AddBookModel) ? AddBookTemplate : ViewBookTemplate;

            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BookModel book)
                return book.Equals(BookModel.AddBookModel) ? AddBookTemplate : ViewBookTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
