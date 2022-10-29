using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Playground.Common;
using RichTextView.DTOs;

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

        //private RichContent titleContent;

        //public RichContent TitleContent
        //{
        //    get { return titleContent; }
        //    set
        //    {
        //        OnPropertyChanging();
        //        titleContent = value;
        //        OnPropertyChanged();
        //    }
        //}

        private TitleInfoBase titleInfo;

        public TitleInfoBase TitleInfo
        {
            get { return titleInfo; }
            set
            {
                if (titleInfo != value)
                {
                    OnPropertyChanging();
                    titleInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        private TitleInfoBase srcTitleInfo;

        public TitleInfoBase SrcTitleInfo
        {
            get { return srcTitleInfo; }
            set { srcTitleInfo = value; }
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
