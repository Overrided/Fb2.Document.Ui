using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.WinUI.Playground.Common;
using RichTextView.DTOs;

namespace Fb2.Document.WinUI.Playground.ViewModels
{
    public class BookInfoViewModel : ObservableObject
    {
        private string coverpageBase64Image = String.Empty;

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

        private ChaptersContent chaptersContent;

        public ChaptersContent ChaptersContent
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
