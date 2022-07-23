using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class ImageProcessor : NodeProcessorBase
    {
        private const string InvalidImageBase64 = "/9j/4AAQSkZJRgABAQEA8ADwAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCABkAGQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDnf+CM/wDwRn/Zs8ef8E2fhv8AHL45fDf/AITrwt46/tNvEHiBvEGqafN4ClttUvLKO4njtLmKJ9HaG3iMs5TzbGXzJpnks5ZJNM/R/wD4hcf2E/8Aohv/AJefiD/5Oo/4Ncf+UFHwM/7j/wD6kGp17/8A8mAf9m//APqqv/wf/wDTV/2Df+QUAeAf8QuP7Cf/AEQ3/wAvPxB/8nUf8QuP7Cf/AEQ3/wAvPxB/8nV9/wBFAHwB/wAQuP7Cf/RDf/Lz8Qf/ACdR/wAQuP7Cf/RDf/Lz8Qf/ACdX3/RQB8Af8QuP7Cf/AEQ3/wAvPxB/8nUf8QuP7Cf/AEQ3/wAvPxB/8nV9/wBFAHwB/wAQuP7Cf/RDf/Lz8Qf/ACdR/wAQuP7Cf/RDf/Lz8Qf/ACdX3/RQB8Af8QuP7Cf/AEQ3/wAvPxB/8nVwHx0/4N5f2E/hZ/Zei6L+zx/wlXxA8Veanh3w6njzxBb/AGvytnnXdzN9sf7Lp9v5sRuLoo+zzYoo457m4traf7++Onx0/wCFWf2Xoui6X/wlXxA8Veanh3w6lz9m+2eVs867uZtj/ZdPt/NiNxdFH2ebFFHHPc3FtbTnwL+Bn/CrP7U1rWtU/wCEq+IHiryn8ReIntvs/wBr8rf5NpbQ73+y6fb+bKLe1Dvs82WWSSe5uLm5nAP44/8Agsx+x1oP7An/AAUm+JHwn8NzedpPhn+zJUKpIkKS3el2d7MkCSySypbrNcSLEks00qxLGsk0zhpXK9f/AODo7/lOv8c/+4B/6j+mUUAfv9/wa4/8oKPgZ/3H/wD1INTr7/r4A/4Ncf8AlBR8DP8AuP8A/qQanX3/AEAfP/8AyYB/2b//AOqq/wDwf/8ATV/2Df8AkFfQFFfP/wDyYB/2b/8A+qq//B//ANNX/YN/5BQB9AUUUUAFFFFABXn/AMdPjp/wqz+y9F0XS/8AhKviB4q81PDvh1Ln7N9s8rZ513czbH+y6fb+bEbi6KPs82KKOOe5uLa2nPjp8dP+FWf2Xoui6X/wlXxA8Veanh3w6lz9m+2eVs867uZtj/ZdPt/NiNxdFH2ebFFHHPc3FtbTnwL+Bn/CrP7U1rWtU/4Sr4geKvKfxF4ie2+z/a/K3+TaW0O9/sun2/myi3tQ77PNllkknubi5uZwA+BfwM/4VZ/amta1qn/CVfEDxV5T+IvET232f7X5W/ybS2h3v9l0+382UW9qHfZ5ssskk9zcXNzP6BRRQB/IF/wdHf8AKdf45/8AcA/9R/TKKP8Ag6O/5Tr/ABz/AO4B/wCo/plFAH7/AH/Brj/ygo+Bn/cf/wDUg1Ovv+vgD/g1x/5QUfAz/uP/APqQanX3/QAUUUUAfP8A/wAmAf8AZv8A/wCqq/8Awf8A/TV/2Df+QV9AUV8//wDJgH/Zv/8A6qr/APB//wBNX/YN/wCQUAfQFef/AB0+On/CrP7L0XRdL/4Sr4geKvNTw74dS5+zfbPK2edd3M2x/sun2/mxG4uij7PNiijjnubi2tpz46fHT/hVn9l6Loul/wDCVfEDxV5qeHfDqXP2b7Z5Wzzru5m2P9l0+382I3F0UfZ5sUUcc9zcW1tOfAv4Gf8ACrP7U1rWtU/4Sr4geKvKfxF4ie2+z/a/K3+TaW0O9/sun2/myi3tQ77PNllkknubi5uZwA+BfwM/4VZ/amta1qn/AAlXxA8VeU/iLxE9t9n+1+Vv8m0tod7/AGXT7fzZRb2od9nmyyyST3Nxc3M/oFFFABRRRQB/IF/wdHf8p1/jn/3AP/Uf0yij/g6O/wCU6/xz/wC4B/6j+mUUAfv9/wAGuP8Aygo+Bn/cf/8AUg1Ovv8Ar4A/4Ncf+UFHwM/7j/8A6kGp19/0AFFFFABXn/x0+On/AAqz+y9F0XS/+Eq+IHirzU8O+HUufs32zytnnXdzNsf7Lp9v5sRuLoo+zzYoo457m4trac+Onx0/4VZ/Zei6Lpf/AAlXxA8Veanh3w6lz9m+2eVs867uZtj/AGXT7fzYjcXRR9nmxRRxz3NxbW058C/gZ/wqz+1Na1rVP+Eq+IHiryn8ReIntvs/2vyt/k2ltDvf7Lp9v5sot7UO+zzZZZJJ7m4ubmcA8A+BfwM/4dW/2prWtap/wlXw/wDFXlP4i8RPbfZ/+Fa+Vv8AJtLaHe/2Xwfb+bKLe1Dv/Y/myyyST21xc3Nj9f0V8/8A/JgH/Zv/AP6qr/8AB/8A9NX/AGDf+QUAfQFFFFABRRRQB/IF/wAHR3/Kdf45/wDcA/8AUf0yij/g6O/5Tr/HP/uAf+o/plFAH7/f8GuP/KCj4Gf9x/8A9SDU6+/6+AP+DXH/AJQUfAz/ALj/AP6kGp19/wBABXn/AMdPjp/wqz+y9F0XS/8AhKviB4q81PDvh1Ln7N9s8rZ513czbH+y6fb+bEbi6KPs82KKOOe5uLa2nPjp8dP+FWf2Xoui6X/wlXxA8Veanh3w6lz9m+2eVs867uZtj/ZdPt/NiNxdFH2ebFFHHPc3FtbTnwL+Bn/CrP7U1rWtU/4Sr4geKvKfxF4ie2+z/a/K3+TaW0O9/sun2/myi3tQ77PNllkknubi5uZwA+BfwM/4VZ/amta1qn/CVfEDxV5T+IvET232f7X5W/ybS2h3v9l0+382UW9qHfZ5ssskk9zcXNzP6BRRQAUUUUAfP/8AyYB/2b//AOqq/wDwf/8ATV/2Df8AkFfQFFfP/wDyYB/2b/8A+qq//B//ANNX/YN/5BQB9AUUUUAfyBf8HR3/ACnX+Of/AHAP/Uf0yij/AIOjv+U6/wAc/wDuAf8AqP6ZRQB+/wB/wa4/8oKPgZ/3H/8A1INTr6/+Onx0/wCFWf2Xoui6X/wlXxA8Veanh3w6lz9m+2eVs867uZtj/ZdPt/NiNxdFH2ebFFHHPc3FtbT/AAD/AMG8vx0/4VZ/wQo/Z40XRdL/AOEq+IHir/hJE8O+HUufs32zyvEF/wCdd3M2x/sun2/mxG4uij7PNiijjnubi2tp/v74F/Az/hVn9qa1rWqf8JV8QPFXlP4i8RPbfZ/tflb/ACbS2h3v9l0+382UW9qHfZ5ssskk9zcXNzOAHwL+Bn/CrP7U1rWtU/4Sr4geKvKfxF4ie2+z/a/K3+TaW0O9/sun2/myi3tQ77PNllkknubi5uZ/QKKKACiiigAooooAKKKKAPn/AP5MA/7N/wD/AFVX/wCD/wD6av8AsG/8gr6Aor5//wCTAP8As3//ANVV/wDg/wD+mr/sG/8AIKAP5gv+Do7/AJTr/HP/ALgH/qP6ZRR/wdHf8p1/jn/3AP8A1H9MooA8g/Y6/wCCzH7Sn7AngSbw38J/iR/wjOkzbVKS+H9L1OZIlkmlS3Sa7tpZUt1muLqVYFcRLLd3MioHnlZ/X/8AiKN/bs/6Ll/5Znh//wCQaKKAD/iKN/bs/wCi5f8AlmeH/wD5Bo/4ijf27P8AouX/AJZnh/8A+QaKKAD/AIijf27P+i5f+WZ4f/8AkGj/AIijf27P+i5f+WZ4f/8AkGiigA/4ijf27P8AouX/AJZnh/8A+QaP+Io39uz/AKLl/wCWZ4f/APkGiigA/wCIo39uz/ouX/lmeH//AJBo/wCIo39uz/ouX/lmeH//AJBoooAP+Io39uz/AKLl/wCWZ4f/APkGj/iKN/bs/wCi5f8AlmeH/wD5BoooA+IPil8Ute+NHju+8SeJL7+0NW1Dy1d1hjt4YIoo1iht4IYlWKC3hhSOKKCJEihiijjjRERVBRRQB//Z";

        private const int EditingDistanceThreshold = 3;

        public override List<TextElement> Process(IRenderingContext context)
        {
            var imageNode = context.CurrentNode;

            if (!imageNode.TryGetAttribute(AttributeNames.XHref, true, out var xHref)) // if value (linked image id) was String.Empty
                return null;

            var linkedBinaries = GetLinkedBinaries(context);

            if (linkedBinaries == null || !linkedBinaries.Any())
                return null; // nothing to choose from

            var bestMatchImage = GetBestMatchImage(linkedBinaries, xHref.Value);

            if (bestMatchImage == null)
                return null;

            var bitmap = GetBitmapImage(bestMatchImage.Content) ?? GetBitmapImage(InvalidImageBase64);

            if (bitmap == null)
                return null;

            var viewPortWidth = context.ViewPortSize.Width;

            var image = new Image
            {
                Source = bitmap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0),
                Stretch = Stretch.Uniform,
                Width = Math.Min(bitmap.PixelWidth, viewPortWidth),
                MaxWidth = bitmap.PixelWidth,
                MaxHeight = bitmap.PixelHeight
            };

            if (imageNode.TryGetAttribute(AttributeNames.Alt, true, out var altAttribute))
                SetTooltip(image, altAttribute.Value);

            var result = new InlineUIContainer();

            // hacky fix for b/w images with transparent background,
            // image itself can get lost on black background

            // TODO: make it configurable - if user wants to have "fake" background or transparency
            var backgroundPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Colors.White),
                MaxWidth = bitmap.PixelWidth
                //BorderBrush = new SolidColorBrush(Colors.Red),
                //BorderThickness = new Thickness(2)
            };

            backgroundPanel.Children.Add(image);

            if (imageNode.IsInline)
                result.Child = backgroundPanel;
            else
            {
                // TODO: make configurable - if user wants to see image titles at all, and if yes - title location? Top/Bottom/Left/Right etc.?
                if (imageNode.TryGetAttribute(AttributeNames.Title, true, out var titleAttribute))
                {
                    var imageTitleTextBlock = new TextBlock
                    {
                        FontSize = Math.Max(context.RenderingConfig.BaseFontSize - 2, 10),
                        FontWeight = FontWeights.Light,
                        FontStretch = FontStretch.SemiCondensed,
                        FontStyle = FontStyle.Oblique,
                        Foreground = new SolidColorBrush(Colors.Black),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 2, 0, 2),
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        Text = titleAttribute.Value
                    };
                    SetTooltip(imageTitleTextBlock, titleAttribute.Value);

                    var imageTitleContainer = new Border
                    {
                        Background = new SolidColorBrush(Colors.White),
                        Child = imageTitleTextBlock,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    backgroundPanel.Children.Add(imageTitleContainer);
                }

                var container = GetCenteringBorder(viewPortWidth);
                container.Child = backgroundPanel;

                result.Child = container;
            }

            return new List<TextElement>(1) { result };
        }

        private IEnumerable<BinaryImage> GetLinkedBinaries(IRenderingContext context)
        {
            if (context is RenderingContext<Fb2Document> documentContext &&
                documentContext.Data.BinaryImages != null &&
                documentContext.Data.BinaryImages.Any())
            {
                return documentContext.Data.BinaryImages.Where(im => im.HasAttribute(AttributeNames.Id, true));
            }
            else if (context is RenderingContext<Fb2Node> containerContext)
            {
                var node = containerContext.Data;

                if (node is Fb2Container nodeContainer)
                    return nodeContainer.GetDescendants<BinaryImage>();
                if (node is Fb2Element nodeElement && nodeElement is BinaryImage binaryNode)
                    return new BinaryImage[1] { binaryNode };
            }
            else if (context is RenderingContext<IEnumerable<Fb2Node>> nodesContext)
            {
                var data = nodesContext.Data;
                var directImages = data.OfType<BinaryImage>().ToList();
                var decsendantImages = data.OfType<Fb2Container>().SelectMany(c => c.GetDescendants<BinaryImage>());

                if (decsendantImages.Any())
                    directImages.AddRange(decsendantImages);

                return directImages;
            }

            Debug.WriteLine($"UNEXPECTED - EMPTY NODE DAT DUE TO : {context.GetType()}");
            return null;
        }

        private BinaryImage GetBestMatchImage(IEnumerable<BinaryImage> linkedBinaries, string xHref)
        {
            var imageDistances = linkedBinaries.Select(im =>
                    new
                    {
                        Image = im,
                        Distance = GetEditingDistance(xHref, im.GetAttribute(AttributeNames.Id, true).Value)
                    });

            // check for distinction
            var distinctDistances = imageDistances.Select(t => t.Distance).Distinct();

            // we have 12 images, with same distances, so distinct instance.count wil be 1.
            // so if we have only 1 image at all it will still work
            if (distinctDistances.Count() == 1 && linkedBinaries.Count() != 1)
                return null; // we are fucked up - all referenes are equally good or bad at same time

            var bestMatch = imageDistances
                .OrderBy(t => t.Distance)
                .FirstOrDefault(); // choose the shortest distance

            if (bestMatch.Distance > EditingDistanceThreshold)
                return null;

            return bestMatch.Image;
        }

        private BitmapImage GetBitmapImage(string base64ImageContent)
        {
            try
            {
                var bitmap = new BitmapImage();

                using (var stream = new InMemoryRandomAccessStream())
                {
                    byte[] bytes = Convert.FromBase64String(base64ImageContent);

                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(bytes);

                    dataWriter.StoreAsync();
                    stream.FlushAsync();

                    stream.Seek(0);
                    bitmap.SetSource(stream);
                }

                if (bitmap.PixelHeight == 0 || bitmap.PixelWidth == 0)
                    return null;

                if (bitmap.IsAnimatedBitmap) // gifs and stuff
                    bitmap.AutoPlay = true;

                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private Border GetCenteringBorder(double viewPortWidth) =>
            new Border
            {
                Margin = new Thickness(0, 8, 0, 8), // TODO: configurable?
                Width = viewPortWidth // make sure image is centered in a page
            };

        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary> 
        private static int GetEditingDistance(string source, string target)
        {
            // corner cases
            if (source == target ||
                string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target)) return 0;

            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            int sourceCharCount = source.Length;
            int targetCharCount = target.Length;

            int[,] distance = new int[sourceCharCount + 1, targetCharCount + 1];

            // Step 2
            for (int i = 0; i <= sourceCharCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetCharCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceCharCount; i++)
            {
                for (int j = 1; j <= targetCharCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceCharCount, targetCharCount];
        }
    }
}
