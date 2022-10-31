using System;
using System.Collections.Generic;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Playground.Common;

namespace Fb2.Document.WinUI.Playground.ViewModels
{
    public class BinaryImageViewModel
    {
        public string Content { get; set; }

        public string Id { get; set; }

        public string ContentType { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BinaryImageViewModel model &&
                Content == model.Content &&
                Id == model.Id &&
                ContentType == model.ContentType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Content, Id, ContentType);
        }
    }

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
            set
            {
                if (srcTitleInfo != value)
                {
                    OnPropertyChanging();
                    srcTitleInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        private PublishInfo publishInfo;

        public PublishInfo PublishInfo
        {
            get { return publishInfo; }
            set
            {
                if (publishInfo != value)
                {
                    OnPropertyChanging();
                    publishInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        private CustomInfo customInfo;

        public CustomInfo CustomInfo
        {
            get { return customInfo; }
            set
            {
                if (customInfo != value)
                {
                    OnPropertyChanging();
                    customInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<BinaryImageViewModel> binaryImages;

        public List<BinaryImageViewModel> BookImages
        {
            get { return binaryImages; }
            set
            {
                OnPropertyChanging();
                binaryImages = value;
                OnPropertyChanged();
            }
        }

    }
}
