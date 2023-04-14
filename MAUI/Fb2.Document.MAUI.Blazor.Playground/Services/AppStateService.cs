using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fb2.Document.MAUI.Blazor.Playground.Data;

namespace Fb2.Document.MAUI.Blazor.Playground.Services;

public class AppStateService
{
    public ConcurrentBag<BookModel> AllBooks { get; set; } = new();
    public BookModel CurrentBookModel { get; set; }
}
