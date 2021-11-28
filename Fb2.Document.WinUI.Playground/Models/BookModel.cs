using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fb2.Document.WinUI.Playground.Models
{
    public class BookModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSizeInBytes { get; set; }
        public string CoverpageBase64Image { get; set; } = string.Empty;
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public Fb2Document? Fb2Document { get; set; }

        //public BookModel(string fileName, string filePath, long FileSizeInBytes, string coverpage, string bookName, string bookAuthor, Fb2Document? doc = null)
        //{

        //}
    }
}
