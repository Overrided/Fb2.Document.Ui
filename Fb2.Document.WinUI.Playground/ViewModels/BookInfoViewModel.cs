using Fb2.Document.WinUI.Playground.Common;
using RichTextView.WinUI.DTOs;

namespace Fb2.Document.WinUI.Playground.ViewModels
{
    public class BookInfoViewModel : ObservableObject
    {
        private string coverpageBase64Image = string.Empty;

        public string CoverpageBase64Image
        {
            get { return coverpageBase64Image; }
            set
            {
                if (coverpageBase64Image != value)
                {
                    OnPropertyChanging();
                    coverpageBase64Image = value;
                    OnPropertyChanged();
                }
            }
        }

        private RichContent chaptersContent;

        public RichContent ChaptersContent
        {
            get { return chaptersContent; }
            set
            {
                OnPropertyChanging();
                chaptersContent = value;
                OnPropertyChanged();
            }
        }
    }
}
