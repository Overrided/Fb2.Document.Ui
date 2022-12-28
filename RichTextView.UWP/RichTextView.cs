using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using RichTextView.UWP.Common;
using RichTextView.UWP.DTOs;
using RichTextView.UWP.EventArguments;
using RichTextView.UWP.Extensions;
using RichTextView.UWP.Services;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace RichTextView.UWP
{
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
        // local constants
        private const string ViewPortContainerName = "viewPortContainer";

        // private members
        private HashSet<double> previousWidths = new HashSet<double>();
        private ScrollViewer scrollHost = null;
        //private ItemsControl itemsHost = null;
        private Grid viewPortContainer = null;
        private ProgressBar bookProgressBar = null;
        private TappedEventHandler defaultLinkClickEventHandler;
        private MenuFlyout menuFlyout = null;

        // events
        public event EventHandler<RichHyperlinkActivatedEventArgs> HyperlinkActivated;
        public event EventHandler<BookProgressChangedEventArgs> BookProgressChanged;
        public event EventHandler<bool> BookRendered;

        // public properties
        public ObservableCollection<RichTextBlock> Pages { get; private set; } = new ObservableCollection<RichTextBlock>();
        public bool IsRendered { get; private set; } = false;

        // ctor + OnApplyTemplate init
        public RichTextView()
        {
            DefaultStyleKey = typeof(RichTextView);
            defaultLinkClickEventHandler = new TappedEventHandler(HyperlinkBtn_Tapped);
            Unloaded += RichTextView_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitElementHost();
            menuFlyout = BuildMenuFlyout();
        }

        // public methods
        public Size GetViewHostSize()
        {
            var originalSize = viewPortContainer.ActualSize.ToSize();
            var expectedWidth = originalSize.Width - (PageMargin.Left + PageMargin.Right);

            var result = new Size
            {
                Height = originalSize.Height,
                Width = Math.Max(expectedWidth, 0)
            };
            return result;
        }

        public async Task ResetView()
        {
            IsRendered = false;
            BookRendered?.Invoke(this, IsRendered);
            previousWidths.Clear();

            if (Pages.Any())
            {
                foreach (var page in Pages)
                {
                    page.Loaded -= RichTextBlock_Loaded;
                    //page.EffectiveViewportChanged -= RichTextBlock_EffectiveViewportChanged;
                    page.ClearValue(ContextFlyoutProperty);
                }
                Pages.Clear();
            }

            await GoToVisualStateAsync("Empty");
            GC.Collect();
        }

        // Dependency properties
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
            var shouldResetView = args.OldValue != null && control.IsRendered;

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
                new PropertyMetadata(new Thickness(0)));

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
                new PropertyMetadata(true));

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

        // init stuff
        private void InitElementHost()
        {
            var viewPortContainerElement = GetTemplateChild(ViewPortContainerName) as Grid;
            if (viewPortContainerElement == null)
                throw new Exception($"Template {ViewPortContainerName} is missing!");

            viewPortContainer = viewPortContainerElement;
            viewPortContainer.SizeChanged += ItemsHost_SizeChanged;
        }

        private void InitScrollViewer()
        {
            var scrollViewer = viewPortContainer.FindVisualChild<ScrollViewer>(); // hmmmm
            if (scrollViewer == null)
                throw new Exception("template scrollHost is missing!");

            scrollHost = scrollViewer;
            scrollHost.ViewChanged += ScrollHost_ViewChanged;
        }

        private void ScrollHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var verticalOffset = scrollHost.VerticalOffset;
            var scrollableHeight = scrollHost.ScrollableHeight;

            this.BookProgressChanged?.Invoke(this, new BookProgressChangedEventArgs(verticalOffset, scrollableHeight));
        }

        private void InitProgressBar()
        {
            var progressBar = viewPortContainer.FindVisualChild<ProgressBar>(); // hmmmm
            if (progressBar == null)
                throw new Exception("template progressBar is missing!");

            bookProgressBar = progressBar;
        }

        private MenuFlyout BuildMenuFlyout()
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

            return menu;
        }

        // inner events handlers
        private void ItemsHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsRendered)
                return;

            var actualSize = GetViewHostSize();

            SaveScreenWidth(actualSize);
        }

        private void RichTextView_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("RichTextView_Unloaded");

            previousWidths.Clear();
            //previousWidths = null;

            if (Pages.Count > 0)
            {
                foreach (var page in Pages)
                {
                    page.Loaded -= RichTextBlock_Loaded;
                    page.EffectiveViewportChanged -= RichTextBlock_EffectiveViewportChanged;
                    page.SizeChanged -= RichTextBlock_SizeChanged;
                    page.ClearValue(ContextFlyoutProperty);
                    page.ClearValue(MarginProperty);
                }

                Pages.Clear();
                Pages = null;
            }

            bookProgressBar = null;
            scrollHost = null;

            viewPortContainer.SizeChanged -= ItemsHost_SizeChanged;

            viewPortContainer = null;

            Unloaded -= RichTextView_Unloaded;

            var uiThreadBytes = GC.GetAllocatedBytesForCurrentThread();
            var allAllocatedBytes = GC.GetTotalMemory(true);
            var totalMemotyAllocation = uiThreadBytes + allAllocatedBytes;

            GC.AddMemoryPressure(totalMemotyAllocation);
            GC.Collect();
        }

        // rendering
        private async Task RenderContent(bool shouldResetView, ChaptersContent chaptersContent)
        {
            if (shouldResetView)
                await ResetView();

            if (ShowLoading)
                await GoToVisualStateAsync("Loading");

            var renderStopwatch = Stopwatch.StartNew();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var size = GetViewHostSize();
                var contentPages = chaptersContent.RichContentPages;

                SaveScreenWidth(size);

                var result = contentPages.Select((section, i) =>
                {
                    Debug.WriteLine("instantiating rich text block");
                    return CreateRichTextBlock(section, size, i);
                });

                foreach (var item in result)
                    Pages.Add(item);

                await this.FinishLayoutAsync();
            });

            renderStopwatch.Stop();

            if ((chaptersContent?.LeftOffPosition ?? 0) > 0)
                scrollHost.ChangeView(null, chaptersContent.LeftOffPosition, null);

            await GoToVisualStateAsync("Rendered");

            InitScrollViewer();
            InitProgressBar();

            IsRendered = true;
            BookRendered?.Invoke(this, IsRendered);

            Debug.WriteLine($"Rendering control time: {renderStopwatch.Elapsed}");
        }

        private RichTextBlock CreateRichTextBlock(List<TextElement> content, Size viewHostSize, int dataPageIndex)
        {
            var richTextBlock = ContainerBuilder.BuildRichTextBlock(FontSize, viewHostSize);

            if (content.Any(te => !(te is Block)))
            {
                content = PaginationUtils.Paragraphize(content);
            }

            richTextBlock.Blocks.AddRange(content);

            // page margins
            richTextBlock.SetBinding(MarginProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(PageMargin))
            });

            richTextBlock.Tag = $"RichTextBlock {dataPageIndex}";
            richTextBlock.Loaded += RichTextBlock_Loaded;
            richTextBlock.EffectiveViewportChanged += RichTextBlock_EffectiveViewportChanged;
            richTextBlock.SizeChanged += RichTextBlock_SizeChanged;

            Debug.WriteLine($"{nameof(CreateRichTextBlock)} on {richTextBlock.Tag}");

            return richTextBlock;
        }

        // rendered page events
        private async void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var richTextBlock = sender as RichTextBlock;
                UpdateVisiblePage(richTextBlock, GetViewHostSize(), true, true, false);
                Debug.WriteLine($"{nameof(RichTextBlock_Loaded)} on {richTextBlock.Tag}");
            });
        }

        private async void RichTextBlock_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var richTextBlock = (RichTextBlock)sender;
                if (args.BringIntoViewDistanceY < richTextBlock.ActualHeight && richTextBlock.IsLoaded)
                {
                    UpdateVisiblePage(richTextBlock, GetViewHostSize(), false, true, true);
                    Debug.WriteLine($"{nameof(RichTextBlock_EffectiveViewportChanged)} on {richTextBlock.Tag}");
                }
            });
        }

        private async void RichTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var richTextBlock = (RichTextBlock)sender;

                UpdateVisiblePage(richTextBlock, GetViewHostSize(), false, true, true);
                Debug.WriteLine($"{nameof(RichTextBlock_SizeChanged)} on {richTextBlock.Tag}");
            });
        }

        // context menu
        private void OverrideContextMenu(DependencyObject uiElement)
        {
            uiElement.ClearValue(ContextFlyoutProperty);
            uiElement.SetValue(ContextFlyoutProperty, menuFlyout);
        }

        // context menu event handlers
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

        // hyperlinks events
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

        private void HyperlinkBtn_Unloaded(object sender, RoutedEventArgs e)
        {
            // todo: cleanup context menu also?
            var hyperlinkButton = (HyperlinkButton)sender;
            hyperlinkButton.RemoveHandler(TappedEvent, defaultLinkClickEventHandler);
            hyperlinkButton.Unloaded -= HyperlinkBtn_Unloaded;

            Debug.WriteLine("Hyperlink unloaded");
        }

        // page state & updates
        private void UpdateVisiblePage(
            RichTextBlock richTextBlock,
            Size viewHostSize,
            bool shouldOverrideContextMenu,
            bool shouldHandleHyperlinks,
            bool shouldAlignImages)
        {
            if (shouldOverrideContextMenu && shouldHandleHyperlinks)
            {
                var uiElementsToHandle = richTextBlock
                    .FindVisualChildren<DependencyObject>()
                    .Where(el => !(el is Panel))
                    .Distinct();

                // overkill
                foreach (var item in uiElementsToHandle) // every "control" down ui tree
                {
                    OverrideContextMenu(item);
                    if (item is HyperlinkButton hyperlinkButton)
                    {
                        hyperlinkButton.Unloaded -= HyperlinkBtn_Unloaded;
                        hyperlinkButton.Unloaded += HyperlinkBtn_Unloaded;

                        hyperlinkButton.RemoveHandler(TappedEvent, defaultLinkClickEventHandler);
                        hyperlinkButton.AddHandler(TappedEvent, defaultLinkClickEventHandler, true);
                    }
                }
            }
            else if (!shouldOverrideContextMenu && shouldHandleHyperlinks)
            {
                var hyperlinkButtons = richTextBlock.FindVisualChildren<HyperlinkButton>();

                foreach (var hyperlinkButton in hyperlinkButtons)
                {
                    hyperlinkButton.Unloaded -= HyperlinkBtn_Unloaded;
                    hyperlinkButton.Unloaded += HyperlinkBtn_Unloaded;

                    hyperlinkButton.RemoveHandler(TappedEvent, defaultLinkClickEventHandler);
                    hyperlinkButton.AddHandler(TappedEvent, defaultLinkClickEventHandler, true);
                }
            }

            if (shouldHandleHyperlinks)
            {
                var hyperlinks = richTextBlock
                  .GetAllTextElements<Hyperlink>()
                  .Concat(richTextBlock.FindVisualChildren<Hyperlink>())
                  .Distinct();

                foreach (var hyperlink in hyperlinks)
                {
                    hyperlink.Click -= HyperText_Click;
                    hyperlink.Click += HyperText_Click;
                }
            }

            if (shouldAlignImages)
            {
                TryResizeNotInlineImages(richTextBlock, viewHostSize);
            }

            richTextBlock.UpdateLayout();
        }

        // hacky af
        // look for elements that are bigger than screen and tag those, resize & repeat)
        private void TryResizeNotInlineImages(RichTextBlock richTextBlock, Size actualSize)
        {
            var anyImages = richTextBlock.FindVisualChildren<Image>();

            if (!anyImages.Any())
                return;

            var actualWidth = actualSize.Width;

            // TODO : break it up & refactor!
            Func<FrameworkElement, bool> predicate = (fe) =>
            {
                var elementWidth = fe.ActualWidth;
                return elementWidth >= actualWidth ||
                       (elementWidth < actualWidth && fe.Tag != null && fe.Tag.ToString().Equals("fullWidthImage")) ||
                       previousWidths.Any(s => s <= elementWidth);
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

        private void SaveScreenWidth(Size actualSize)
        {
            if (!previousWidths.Contains(actualSize.Width))
                previousWidths.Add(actualSize.Width);
        }

        // miscellaneous
        private async Task GoToVisualStateAsync(string stateName)
        {
            Func<Task> hideProgress = async () =>
            {
                VisualStateManager.GoToState(this, stateName, false);
                await this.FinishLayoutAsync();
            };
            await hideProgress();
        }
    }
}
