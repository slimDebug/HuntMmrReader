using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HuntMmrReader.DesignHelper;

internal abstract class ViewModelBase : ObservableObject
{
    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = default)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}