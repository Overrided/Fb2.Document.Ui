using Fb2.Document.WinUI.Playground.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fb2.Document.WinUI.Playground.TemplateSelectors
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
