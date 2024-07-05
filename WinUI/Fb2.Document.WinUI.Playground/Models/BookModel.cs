using System;

namespace Fb2.Document.WinUI.Playground.Models;

public class BookModel
{
    //private const string system = "book_model_system";

    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSizeInBytes { get; set; }
    public string CoverpageBase64Image { get; set; } = string.Empty;
    public string BookName { get; set; }
    public string BookAuthor { get; set; }
    public Fb2Document? Fb2Document { get; set; }

    //public static BookModel AddBookModel;

    //static BookModel()
    //{
    //    AddBookModel = new BookModel
    //    {
    //        FileName = system,
    //        FilePath = system,
    //        FileSizeInBytes = -1,
    //        BookName = system,
    //        BookAuthor = system
    //    };
    //}

    public override bool Equals(object? obj)
    {
        return obj is BookModel model &&
               FileName == model.FileName &&
               FilePath == model.FilePath &&
               FileSizeInBytes == model.FileSizeInBytes &&
               CoverpageBase64Image == model.CoverpageBase64Image &&
               BookName == model.BookName &&
               BookAuthor == model.BookAuthor &&
               (Fb2Document == null && model.Fb2Document == null || (Fb2Document?.Equals(model.Fb2Document) ?? false));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, FilePath, FileSizeInBytes, CoverpageBase64Image, BookName, BookAuthor, Fb2Document);
    }
}
