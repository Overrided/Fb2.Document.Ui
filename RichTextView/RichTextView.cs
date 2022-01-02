using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using RichTextView.Common;
using RichTextView.DTOs;
using RichTextView.EventArguments;
using RichTextView.Extensions;
using RichTextView.Services;
using Windows.Foundation;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace RichTextView
{
    // TODO : move to separate file
    public class RichTextViewModel : ObservableObject
    {
        public ObservableCollection<RichTextBlock> Pages { get; } = new ObservableCollection<RichTextBlock>();

        public bool IsRendered { get; set; } = false;

        public ConcurrentBag<double> PreviousWidths = new ConcurrentBag<double>();

        public int FirstVisiblePageIndex = 0;
        public int LastVisiblePageIndex = 0;
    }

    // TODO : update Rendered event - raise Rendered false on reset? 
    // TODO : add font size change?
    // TODO : add font family change?
    // TODO : add "go to position" method ??
    // TODO : add notes opening in popups or navigate there and back?
    // TODO : fix "select all" (ctrl + a)
    // TODO : add search? (or on book model side?)
    // TODO : add sane context menu
    // TODO : add pagination (ability to switch paged/scroll view)?
    [TemplatePart(Name = "viewPortContainer", Type = typeof(Grid))]
    [TemplatePart(Name = "itemsHost", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "loadingIndicator", Type = typeof(ProgressRing))]
    public sealed class RichTextView : Control
    {
        public RichTextViewModel RichTextViewModel { get; set; } = new RichTextViewModel();

        public event EventHandler<RichHyperlinkActivatedEventArgs> HyperlinkActivated;
        public event EventHandler<BookProgressChangedEventArgs> ReadingProgressChanged;
        public event EventHandler ContentRendered;

        private const string ItemsHostTemplateName = "itemsHost";

        private ScrollViewer scrollHost = null;
        private ItemsControl itemsHost = null;
        private ProgressBar bookProgressBar = null;

        private TappedEventHandler defaultLinkClickEventHandler;

        public RichTextView()
        {
            DefaultStyleKey = typeof(RichTextView);
            defaultLinkClickEventHandler = new TappedEventHandler(HyperlinkBtn_Tapped);
            this.Unloaded += RichTextView_Unloaded;
        }

        // TODO : revisit, attempt to force-clean memory from textures etc
        // marshall.blah was already attempted
        private void RichTextView_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("RichTextView_Unloaded");

            scrollHost.ViewChanging -= ScrollHost_ViewChanging;
            scrollHost.ViewChanged -= ScrollHost_ViewChanged;

            itemsHost.SizeChanged -= ItemsHost_SizeChanged;

            RichTextViewModel.PreviousWidths.Clear();

            if (RichTextViewModel.Pages.Count > 0)
            {
                HandleHyperlinksInVisibleArea(false);
                for (int i = 0; i < RichTextViewModel.Pages.Count; i++)
                {
                    var page = RichTextViewModel.Pages[i];
                    VisualTreeHelper.DisconnectChildrenRecursive(page);
                }

                RichTextViewModel.Pages.Clear();
                itemsHost.ItemsSource = null;
            }

            Unloaded -= RichTextView_Unloaded;
            VisualTreeHelper.DisconnectChildrenRecursive(this);

            var uiThreadBytesCount = GC.GetAllocatedBytesForCurrentThread();
            var allMemoryBytesCount = GC.GetTotalMemory(true);
            GC.AddMemoryPressure(allMemoryBytesCount + uiThreadBytesCount);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitElementHost();
        }

        public ChaptersContent RichTextContent
        {
            get { return (ChaptersContent)GetValue(RichTextContentProperty); }
            set { SetValue(RichTextContentProperty, value); }
        }

        public static readonly DependencyProperty RichTextContentProperty =
            DependencyProperty.Register(
                nameof(RichTextContent),
                typeof(ChaptersContent),
                typeof(RichTextView),
                new PropertyMetadata(null, new PropertyChangedCallback(RichTextContent_PropertyChangedCallback)));

        private static async void RichTextContent_PropertyChangedCallback(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs args)
        {
            Debug.WriteLine("Chapters_PropertyChangedCallback");

            // debatable
            if (args.NewValue == null)
                return;

            var chaptersContent = args.NewValue as ChaptersContent;

            if (chaptersContent?.IsEmpty() ?? true)
                return;

            var control = sender as RichTextView;
            var shouldResetView = args.OldValue != null;

            await control.RenderContent(shouldResetView, chaptersContent);
        }

        public Thickness PageMargin
        {
            get { return (Thickness)GetValue(PageMarginProperty); }
            set { SetValue(PageMarginProperty, value); }
        }

        public static readonly DependencyProperty PageMarginProperty =
            DependencyProperty.Register(
                nameof(PageMargin),
                typeof(Thickness),
                typeof(RichTextView),
                new PropertyMetadata(new Thickness(0), new PropertyChangedCallback(PageMargin_PropertyChangedCallback)));

        private static void PageMargin_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RichTextView;

            if (control == null || !control.RichTextViewModel.IsRendered)
                return;

            control.TryAdjustPagesMargin();
        }

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register(
                nameof(ShowProgress),
                typeof(bool),
                typeof(RichTextView),
                new PropertyMetadata(true, new PropertyChangedCallback(ShowProgress_PropertyChangedCallback)));

        // TODO : use visual states for progress bar on/off
        private static void ShowProgress_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = d as RichTextView;
            if (control == null || !control.RichTextViewModel.IsRendered)
                return;

            var newVisibilityValue = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            control.ToggleProgressBarVisibility(newVisibilityValue);
        }

        public bool ShowLoading
        {
            get { return (bool)GetValue(ShowLoadingProperty); }
            set { SetValue(ShowLoadingProperty, value); }
        }

        public static readonly DependencyProperty ShowLoadingProperty =
            DependencyProperty.Register(
                nameof(ShowLoading),
                typeof(bool),
                typeof(RichTextView),
                new PropertyMetadata(true));

        public Size GetViewHostSize()
        {
            var originalSize = itemsHost.ActualSize.ToSize();
            var expectedWidth = originalSize.Width - (PageMargin.Left + PageMargin.Right);

            var result = new Size
            {
                Height = originalSize.Height,
                Width = Math.Max(expectedWidth, 0)
            };
            return result;
        }

        // init stuff
        private void InitElementHost()
        {
            var itemsHostElement = GetTemplateChild(ItemsHostTemplateName) as ItemsControl;
            if (itemsHostElement == null)
                throw new Exception("template itemsHost is missing!");

            itemsHost = itemsHostElement;
            itemsHost.SizeChanged += ItemsHost_SizeChanged;
        }

        private void InitScrollViewer()
        {
            var scrollViewer = itemsHost.FindVisualChild<ScrollViewer>(); // hmmmm
            if (scrollViewer == null)
                throw new ApplicationException("template scrollHost is missing!");

            scrollHost = scrollViewer;

            HandleHyperlinksInVisibleArea(true);

            scrollHost.ViewChanging += ScrollHost_ViewChanging;
            scrollHost.ViewChanged += ScrollHost_ViewChanged;
        }

        private void InitProgressBar()
        {
            var progressBar = itemsHost.FindVisualChild<ProgressBar>(); // hmmmm
            if (progressBar == null)
                throw new Exception("template progressBar is missing!");

            bookProgressBar = progressBar;
        }

        // event handlers
        private void ItemsHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var actualSize = GetViewHostSize();

            SaveScreenWidth(actualSize);
            TryAdjustImageSizes();
        }

        private void ScrollHost_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            HandleHyperlinksChanged(false);
            TryAdjustPagesMargin();
        }

        private void ScrollHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollComplete = !e.IsIntermediate;
            if (scrollComplete)
            {
                ReadingProgressChanged?.Invoke(this, new BookProgressChangedEventArgs(scrollHost.VerticalOffset, scrollHost.ScrollableHeight));
                Debug.WriteLine($"Vertical offset: {scrollHost.VerticalOffset}, scrollablaHeight: {scrollHost.ScrollableHeight}");
            }

            HandleHyperlinksChanged(scrollComplete);
            TryAdjustImageSizes();
            TryAdjustPagesMargin();
        }

        private void HyperText_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Debug.WriteLine("hyperlink clicked");
            HyperlinkActivated?.Invoke(this, new RichHyperlinkActivatedEventArgs(sender, args));
        }

        private void HyperlinkBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("hyperlink button tapped");
            HyperlinkActivated?.Invoke(this, new RichHyperlinkActivatedEventArgs(sender, e));
        }

        // hyperlinks handling
        private void HandleHyperlinksInVisibleArea(bool subscribe)
        {
            (var fvi, var lvi) = GetVisiblePageIndexes();

            for (int i = fvi; i <= lvi; i++)
                if (TryGetPageByIndex(i, out var rtb))
                    HandleHyperlinksSubscription(rtb, subscribe);

            RichTextViewModel.FirstVisiblePageIndex = subscribe ? fvi : 0;
            RichTextViewModel.LastVisiblePageIndex = subscribe ? lvi : 0;
        }

        private void HandleHyperlinksChanged(bool shouldSubscribe = true)
        {
            (var actualFirstIndex, var actualLastIndex) = GetVisiblePageIndexes();

            // unsubscribing pages which just went out of view
            if (actualFirstIndex > RichTextViewModel.FirstVisiblePageIndex) // scrolling forward, 0 => 1
            {
                for (int i = RichTextViewModel.FirstVisiblePageIndex; i < actualFirstIndex; i++)
                    if (TryGetPageByIndex(i, out var rtb))
                        HandleHyperlinksSubscription(rtb, false);

                RichTextViewModel.FirstVisiblePageIndex = actualFirstIndex;
            }
            // unsubscribing pages which just went out of view
            if (actualLastIndex < RichTextViewModel.LastVisiblePageIndex)
            {
                for (int i = RichTextViewModel.LastVisiblePageIndex; i > actualLastIndex; i--)
                    if (TryGetPageByIndex(i, out var rtb))
                        HandleHyperlinksSubscription(rtb, false);

                RichTextViewModel.LastVisiblePageIndex = actualLastIndex;
            }

            if (actualLastIndex > RichTextViewModel.LastVisiblePageIndex && shouldSubscribe) // scrolling forward, 1 => 2
            {
                for (int i = RichTextViewModel.LastVisiblePageIndex + 1; i <= actualLastIndex; i++)
                    if (TryGetPageByIndex(i, out var rtb))
                        HandleHyperlinksSubscription(rtb, shouldSubscribe);

                RichTextViewModel.LastVisiblePageIndex = actualLastIndex;
            }

            if (actualFirstIndex < RichTextViewModel.FirstVisiblePageIndex && shouldSubscribe)
            {
                for (int i = RichTextViewModel.FirstVisiblePageIndex - 1; i >= actualFirstIndex; i--)
                    if (TryGetPageByIndex(i, out var rtb))
                        HandleHyperlinksSubscription(rtb, shouldSubscribe);

                RichTextViewModel.FirstVisiblePageIndex = actualFirstIndex;
            }
        }

        private void HandleHyperlinksSubscription(RichTextBlock richTextBlock, bool shouldSubscribe)
        {
            var hyperlinkButtons = richTextBlock.FindVisualChildren<HyperlinkButton>().Distinct();
            var hasHyperlinkButtons = hyperlinkButtons != null && hyperlinkButtons.Any();
            if (hasHyperlinkButtons)
                foreach (var hyperlinkBtn in hyperlinkButtons)
                {
                    hyperlinkBtn.RemoveHandler(TappedEvent, defaultLinkClickEventHandler);
                    if (shouldSubscribe)
                        hyperlinkBtn.AddHandler(TappedEvent, defaultLinkClickEventHandler, true);
                }

            var hyperlinks = richTextBlock.FindVisualChildren<Hyperlink>();

            var textHyperlinks = richTextBlock.GetAllTextElements<Hyperlink>();
            if (textHyperlinks != null && textHyperlinks.Any())
                hyperlinks.AddRange(textHyperlinks);

            var hasHyperlinks = hyperlinks.Any();

            if (hasHyperlinks)
                foreach (var hyperText in hyperlinks)
                {
                    hyperText.Click -= HyperText_Click;
                    if (shouldSubscribe)
                        hyperText.Click += HyperText_Click;
                }

            if (hasHyperlinkButtons || hasHyperlinks)
                Debug.WriteLine($"TrySubscribeHyperlinks");
        }

        private (int firstVisiblePageIndex, int lastVisiblePageIndex) GetVisiblePageIndexes()
        {
            var isp = itemsHost?.ItemsPanelRoot as ItemsStackPanel;
            if (isp == null)
                return (0, 0);

            var fvi = isp.FirstVisibleIndex;
            var lvi = isp.LastVisibleIndex;
            return (fvi, lvi);
        }

        private bool TryGetPageByIndex(int index, out RichTextBlock page)
        {
            var rtb = itemsHost.ContainerFromIndex(index) as RichTextBlock;
            page = rtb;

            return rtb != null;
        }

        // images handling
        private void TryAdjustImageSizes()
        {
            var actualSize = GetViewHostSize();
            (var fvi, var lvi) = GetVisiblePageIndexes();

            if (fvi < 0 && lvi < 0)
                return;

            for (int i = fvi; i <= lvi; i++)
            {
                if (TryGetPageByIndex(i, out var rtb))
                    TryResizeNotInlineImages(rtb, actualSize);
            }
        }

        private void TryResizeNotInlineImages(RichTextBlock richTextBlock, Size actualSize)
        {
            var anyImages = richTextBlock.FindVisualChildren<Image>().Distinct();

            if (!anyImages.Any())
                return;

            var actualWidth = actualSize.Width;

            // TODO : break it up & refactor!
            Func<FrameworkElement, bool> predicate = (fe) =>
            {
                var elementWidth = fe.ActualWidth;
                return elementWidth >= actualWidth ||
                       (elementWidth < actualWidth && fe.Tag != null && fe.Tag.ToString().Equals("fullWidthImage")) ||
                       RichTextViewModel.PreviousWidths.Any(s => s <= elementWidth);
            };

            var parentsByImage = anyImages
                .SelectMany(im => im.GetVisualParents()) // hacky af
                .OfType<FrameworkElement>()
                .Where(predicate)
                .ToList();

            var imagesToResize = anyImages.Where(predicate);

            if (imagesToResize.Any())
                parentsByImage.AddRange(imagesToResize);

            if (!parentsByImage.Any())
                return;

            foreach (var pbi in parentsByImage)
            {
                if (pbi.Tag == null)
                    pbi.Tag = "fullWidthImage";

                pbi.Width = actualWidth;
                pbi.UpdateLayout();
            }
        }

        // rendering
        private async Task RenderContent(bool shouldResetView, ChaptersContent chaptersContent)
        {
            if (shouldResetView)
                ResetView();

            if (ShowLoading)
            {
                await GoToVisualStateAsync("Loading");
                await Task.Delay(20); // lol yiiiis
            }

            var renderStopwatch = Stopwatch.StartNew();

            this.DispatcherQueue.TryEnqueue(async () =>
            {
                var size = GetViewHostSize();
                var contentPages = chaptersContent.RichContentPages;

                SaveScreenWidth(size);

                var result = contentPages.Select(section =>
                {
                    Debug.WriteLine("instantiating rich text block");
                    return CreateRichTextBlock(section, size);
                });

                foreach (var item in result)
                    RichTextViewModel.Pages.Add(item);

                await this.FinishLayoutAsync();
            });

            renderStopwatch.Stop();

            InitScrollViewer();
            InitProgressBar();

            if (ShowProgress)
                ToggleProgressBarVisibility(Visibility.Visible);

            if ((chaptersContent?.LeftOffPosition ?? 0) > 0)
                scrollHost.ChangeView(null, chaptersContent.LeftOffPosition, null);

            await GoToVisualStateAsync("Rendered");

            RichTextViewModel.IsRendered = true;
            ContentRendered?.Invoke(this, null);

            Debug.WriteLine($"Rendering control time: {renderStopwatch.Elapsed}");
        }

        private void TryAdjustPagesMargin()
        {
            var actualSize = GetViewHostSize();
            (var fvi, var lvi) = GetVisiblePageIndexes();

            if (fvi < 0 && lvi < 0)
                return;

            for (int i = fvi; i <= lvi; i++)
            {
                if (TryGetPageByIndex(i, out var rtb) && rtb.Margin != PageMargin)
                {
                    rtb.Margin = PageMargin;
                    TryResizeNotInlineImages(rtb, actualSize);
                    rtb.UpdateLayout();
                }
            }

            UpdateLayout();
        }

        private RichTextBlock CreateRichTextBlock(List<TextElement> content, Size viewHostSize)
        {
            var richTextBlock = ContainerBuilder.BuildRichTextBlock(FontSize, viewHostSize, PageMargin);

            if (content.Any(te => !(te is Block)))
            {
                var paragraps = PaginationUtils.Paragraphize(content);
                richTextBlock.Blocks.AddRange(paragraps);
            }
            else
                richTextBlock.Blocks.AddRange(content);

            OverrideRichTextBlockContextMenu(richTextBlock);

            return richTextBlock;
        }

        private void OverrideRichTextBlockContextMenu(RichTextBlock richTextBlock)
        {
            var menu = new MenuFlyout();
            var pageMarginFlyoutItem = new MenuFlyoutSubItem { Text = "Page Margin" };

            var increasePageMarginSubitem = new MenuFlyoutItem { Text = "Increase" };
            increasePageMarginSubitem.Click += IncreasePageMarginFlyoutItem_Click;

            var decreasePageMarginSubitem = new MenuFlyoutItem { Text = "Decrease" };
            decreasePageMarginSubitem.Click += DecreasePageMarginFlyoutItem_Click;

            pageMarginFlyoutItem.Items.Add(increasePageMarginSubitem);
            pageMarginFlyoutItem.Items.Add(decreasePageMarginSubitem);

            var toggleProgressFlyoutItem = new MenuFlyoutItem { Text = "Toggle Progress" };
            toggleProgressFlyoutItem.Click += ToggleProgressFlyoutItem_Click;

            menu.Items.Add(pageMarginFlyoutItem);
            menu.Items.Add(toggleProgressFlyoutItem);
            richTextBlock.ContextFlyout = menu;

            var inlineContainers = richTextBlock.GetAllTextElements<InlineUIContainer>();

            foreach (var container in inlineContainers)
                container.SetValue(FlyoutBase.AttachedFlyoutProperty, menu);
        }

        private void ToggleProgressFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ShowProgress = !ShowProgress;
        }

        private void DecreasePageMarginFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            PageMargin = new Thickness(
                Math.Max(PageMargin.Left - 10, 0),
                Math.Max(PageMargin.Top - 10, 0),
                Math.Max(PageMargin.Right - 10, 0),
                Math.Max(PageMargin.Bottom - 10, 0));
        }

        private void IncreasePageMarginFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            PageMargin = new Thickness(
                PageMargin.Left + 10,
                PageMargin.Top + 10,
                PageMargin.Right + 10,
                PageMargin.Bottom + 10);
        }

        // miscellaneous
        private void SaveScreenWidth(Size actualSize)
        {
            if (!RichTextViewModel.PreviousWidths.Contains(actualSize.Width))
                RichTextViewModel.PreviousWidths.Add(actualSize.Width);
        }

        private bool ToggleProgressBarVisibility(Visibility visibility)
        {
            if (bookProgressBar == null)
                return false;

            bookProgressBar.Visibility = visibility;
            return true;
        }

        private async Task GoToVisualStateAsync(string stateName)
        {
            VisualStateManager.GoToState(this, stateName, true);
            await this.FinishLayoutAsync();
            UpdateLayout();
        }

        //TODO : make public again?
        private void ResetView()
        {
            scrollHost.ViewChanging -= ScrollHost_ViewChanging;
            scrollHost.ViewChanged -= ScrollHost_ViewChanged;

            RichTextViewModel.IsRendered = false;
            ToggleProgressBarVisibility(Visibility.Collapsed);
            RichTextViewModel.PreviousWidths.Clear();

            if (RichTextViewModel.Pages.Any())
            {
                HandleHyperlinksInVisibleArea(false);
                RichTextViewModel.Pages.Clear();
            }
        }
    }
}
