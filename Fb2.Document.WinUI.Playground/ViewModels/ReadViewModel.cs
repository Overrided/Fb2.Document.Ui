using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using RichTextView.DTOs;

namespace Fb2.Document.WinUI.Playground.ViewModels
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }

    public class ReadViewModel : ObservableObject
    {
        private bool showBookProgress;

        private Thickness pageMargin;

        private ChaptersContent chaptersContent;

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

        public ReadViewModel(
            ChaptersContent chaptersContent = null,
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
