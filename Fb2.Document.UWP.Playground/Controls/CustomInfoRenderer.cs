using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fb2.Document.Models;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.Playground.Common;
using RichTextView.UWP.DTOs;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Fb2.Document.UWP.Playground.Controls
{
    //public sealed class CustomInfoRenderer : Control
    //{
    //    public CustomInfoRenderer()
    //    {
    //        this.DefaultStyleKey = typeof(CustomInfoRenderer);
    //    }
    //}

    public class CustomInfoRendererViewModel : ObservableObject
    {
        private RichContent customInfoContent;

        public RichContent CustomInfoContent
        {
            get { return customInfoContent; }
            set
            {
                OnPropertyChanging();
                customInfoContent = value;
                OnPropertyChanged();
            }
        }
    }

    public sealed class CustomInfoRenderer : Control
    {
        public CustomInfoRendererViewModel ViewModel { get; set; }

        public CustomInfoRenderer()
        {
            this.DefaultStyleKey = typeof(CustomInfoRenderer);
            ViewModel = new CustomInfoRendererViewModel();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        public CustomInfo CustomInfo
        {
            get { return (CustomInfo)GetValue(CustomInfoProperty); }
            set { SetValue(CustomInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomInfoProperty =
            DependencyProperty.Register(
                nameof(CustomInfo),
                typeof(CustomInfo),
                typeof(CustomInfoRenderer),
                new PropertyMetadata(null, new PropertyChangedCallback(OnCustomInfoPropertyChangedCallback)));

        private static void OnCustomInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as CustomInfoRenderer;

            if (sender == null)
                return;

            var customInfo = sender.CustomInfo;
            if (customInfo == null)
                return;

            var mappedNodes = Fb2Mapper.Instance.MapNode(
                customInfo,
                Size.Empty,
                new Fb2MappingConfig(useStyles: false));

            var normalizedContent = mappedNodes.SelectMany(uic => uic);

            var contentPage = new RichContentPage(normalizedContent);
            var content = new RichContent(new List<RichContentPage>(1) { contentPage });
            sender.ViewModel.CustomInfoContent = content;
        }
    }
}
