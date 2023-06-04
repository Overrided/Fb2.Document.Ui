using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fb2.Document.MAUI.Playground.Common;

public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler PropertyChanged;
    public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
    }
}
