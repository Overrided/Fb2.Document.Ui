using Fb2.Document.WinUI.Playground.Common;
using Microsoft.UI.Xaml;
using RichTextView.WinUI.DTOs;

namespace Fb2.Document.WinUI.Playground.ViewModels
{
    public class ReadViewModel : ObservableObject
    {
        private bool showBookProgress;

        private Thickness pageMargin;

        private RichContent chaptersContent;

        public bool ShowBookProgress
        {
            get { return showBookProgress; }
            set
            {
                if (showBookProgress != value)
                {
                    OnPropertyChanging();
                    showBookProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public Thickness PageMargin
        {
            get { return pageMargin; }
            set
            {
                if (pageMargin != value)
                {
                    OnPropertyChanging();
                    pageMargin = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ReadViewModel(
            RichContent chaptersContent = null,
            Thickness? suggestedPageMargin = null,
            bool? showBookProgress = null)
        {
            if (chaptersContent != null)
                ChaptersContent = chaptersContent;

            if (suggestedPageMargin.HasValue)
                PageMargin = suggestedPageMargin.Value;

            if (showBookProgress.HasValue)
                ShowBookProgress = showBookProgress.Value;
        }
    }
}
