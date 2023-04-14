namespace Fb2.Document.WPF.Playground.Models;

public class BookModel
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }
    public string CoverPageBase64Image { get; set; } = string.Empty;
    public string BookName { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public Fb2Document? Fb2Document { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is BookModel model &&
               FileName == model.FileName &&
               FilePath == model.FilePath &&
               FileSizeInBytes == model.FileSizeInBytes &&
               CoverPageBase64Image == model.CoverPageBase64Image &&
               BookName == model.BookName &&
               BookAuthor == model.BookAuthor &&
               (Fb2Document == null && model.Fb2Document == null || (Fb2Document?.Equals(model.Fb2Document) ?? false));
    }

    public override int GetHashCode()
    {
        return (!string.IsNullOrEmpty(FileName) ? FileName.GetHashCode() : 0) ^
               (!string.IsNullOrEmpty(FilePath) ? FilePath.GetHashCode() : 0) ^
               FileSizeInBytes.GetHashCode() ^
               (!string.IsNullOrEmpty(CoverPageBase64Image) ? CoverPageBase64Image.GetHashCode() : 0) ^
               (!string.IsNullOrEmpty(BookName) ? BookName.GetHashCode() : 0) ^
               (!string.IsNullOrEmpty(BookAuthor) ? BookAuthor.GetHashCode() : 0) ^
               (Fb2Document != null ? Fb2Document.GetHashCode() : 0);
    }
}
