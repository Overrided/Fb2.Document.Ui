﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fb2.Document.UWP.Playground.Models
{
    public class BookModel
    {
        //private const string system = "book_model_system";

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSizeInBytes { get; set; }
        public string CoverpageBase64Image { get; set; } = string.Empty;
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public Fb2Document Fb2Document { get; set; }

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

        public override bool Equals(object obj)
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
            return (!string.IsNullOrEmpty(FileName) ? FileName.GetHashCode() : 0) ^
                   (!string.IsNullOrEmpty(FilePath) ? FilePath.GetHashCode() : 0) ^
                   FileSizeInBytes.GetHashCode() ^
                   (!string.IsNullOrEmpty(CoverpageBase64Image) ? CoverpageBase64Image.GetHashCode() : 0) ^
                   (!string.IsNullOrEmpty(BookName) ? BookName.GetHashCode() : 0) ^
                   (!string.IsNullOrEmpty(BookAuthor) ? BookAuthor.GetHashCode() : 0) ^
                   (Fb2Document != null ? Fb2Document.GetHashCode() : 0);
        }
    }
}
