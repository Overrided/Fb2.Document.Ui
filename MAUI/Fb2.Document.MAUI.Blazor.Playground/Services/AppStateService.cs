using System.Collections.Concurrent;
using Fb2.Document.MAUI.Blazor.Playground.Data;
using Fb2.Document.MAUI.Blazor.Playground.Enums;

namespace Fb2.Document.MAUI.Blazor.Playground.Services;

public class AppStateService
{
    public ConcurrentBag<BookModel> AllBooks { get; set; } = new();
    public BookModel CurrentBookModel { get; set; }

    public AppThemeEnum AppTheme { get; set; } = AppThemeEnum.Light;
}
