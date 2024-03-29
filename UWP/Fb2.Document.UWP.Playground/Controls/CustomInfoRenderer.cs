﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models;
using Fb2.Document.UWP.Playground.Common;
using RichTextView.UWP.DTOs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Fb2.Document.UWP.Playground.Controls
{
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

            var contents = new List<string>();
            var trimmedContent = customInfo.Content.Trim();

            if (!string.IsNullOrEmpty(trimmedContent))
                contents.Add(trimmedContent);

            if (customInfo.Attributes.Any())
                contents.AddRange(customInfo.Attributes.Select(a => $"{a.Key} {a.Value}"));

            if (contents.Count == 0)
                return;

            var customInfoContent = string.Join(Environment.NewLine, contents);

            var run = new Run { Text = customInfoContent };
            var paragraph = new Windows.UI.Xaml.Documents.Paragraph();
            paragraph.Inlines.Add(run); // dirty trick

            var contentPage = new RichContentPage(new List<TextElement>(1) { paragraph });
            var content = new RichContent(new List<RichContentPage>(1) { contentPage });
            sender.ViewModel.CustomInfoContent = content;
        }
    }
}
