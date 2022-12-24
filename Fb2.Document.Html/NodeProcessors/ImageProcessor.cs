using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.Html.Entities;
using Fb2.Document.Html.NodeProcessors.Base;
using Fb2.Document.Models.Base;
using Fb2.Document.Models;
using Fb2.Document.Constants;
using System.Net;

namespace Fb2.Document.Html.NodeProcessors;

public class ImageProcessor : DefaultFb2HtmlNodeProcessor
{
    private Dictionary<string, string> imageSignatures = new()
    {
        ["R0lGODdh"] = "image/gif",
        ["R0lGODlh"] = "image/gif",
        ["iVBORw0KGgo"] = "image/png",
        ["/9j/"] = "image/jpeg",
        ["SUkqAA"] = "image/tiff",
        ["TU0AKg"] = "image/tiff",
        ["Qk0"] = "image/bmp"
    };

    private const int EditingDistanceThreshold = 3;

    //public override string CorrespondingHtmlTag => string.Empty; // whatewer

    public override string Process(RenderingContext context)
    {
        var imageNode = context.CurrentNode;

        if (!imageNode.TryGetAttribute(AttributeNames.XHref, true, out var xHref)) // if value (linked image id) was String.Empty
            return string.Empty;

        var linkedBinaries = GetLinkedBinaries(context);
        if (linkedBinaries == null || !linkedBinaries.Any())
            return string.Empty; // nothing to choose from

        var bestMatchImage = GetBestMatchImage(linkedBinaries, xHref.Value);
        if (bestMatchImage == null)
            return string.Empty;

        var contentType = bestMatchImage.TryGetAttribute(AttributeNames.ContentType, out var contentTypeAttr) ?
                                contentTypeAttr!.Value :
                                string.Empty;

        var finalContentType = string.IsNullOrEmpty(contentType) ?
            TryGetContentTypeFromBase64Content(bestMatchImage.Content) :
            contentType;

        var centerForNotInlineImages = "style=\"margin: auto;display: block; max-width:100%\"";
        var srcData = $"src=\"data:{finalContentType};base64, {bestMatchImage.Content}\"";

        var isImageInline = imageNode.IsInline;

        var result = isImageInline ?
            $"<img {srcData} />" :
            $"<img {centerForNotInlineImages} {srcData} />";

        var htmlEncoded = WebUtility.HtmlEncode(result);

        return result;
    }

    private string TryGetContentTypeFromBase64Content(string base64Content)
    {
        var mime = imageSignatures.FirstOrDefault(k => base64Content.StartsWith(k.Key)).Value;
        if (string.IsNullOrEmpty(mime))
            mime = "application/octet-stream";

        return mime;
    }

    private IEnumerable<BinaryImage> GetLinkedBinaries(RenderingContext context)
    {
        var data = context.Data;

        var directImages = data.OfType<BinaryImage>().ToList();
        var decsendantImages = data.OfType<Fb2Container>().SelectMany(c => c.GetDescendants<BinaryImage>());

        if (decsendantImages.Any())
            directImages.AddRange(decsendantImages);

        return directImages;
    }

    private BinaryImage GetBestMatchImage(IEnumerable<BinaryImage> linkedBinaries, string xHref)
    {
        var imageDistances = linkedBinaries.Select(im =>
                new
                {
                    Image = im,
                    Distance = GetEditingDistance(xHref, im.GetAttribute(AttributeNames.Id, true).Value)
                }).ToList();

        // check for distinction
        var distinctDistances = imageDistances
                .DistinctBy(t => t.Distance)
                .ToList();

        // we have 12 images, with same distances, so distinct instance.count wil be 1.
        // so if we have only 1 image at all it will still work
        if (distinctDistances.Count == 1 && imageDistances.Count != 1)
            return null; // we are fucked up - all referenes are equally good or bad at same time

        var bestMatch = imageDistances
            .OrderBy(t => t.Distance)
            .FirstOrDefault(); // choose the shortest distance

        if (bestMatch.Distance > EditingDistanceThreshold)
            return null;

        return bestMatch.Image;
    }

    private static int GetEditingDistance(string source, string target)
    {
        // corner cases
        if (source == target || string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target))
            return 0;

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
                int cost = target[j - 1] == source[i - 1] ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceCharCount, targetCharCount];
    }
}
