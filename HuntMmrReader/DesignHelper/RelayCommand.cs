using System;
using System.Windows.Input;

namespace HuntMmrReader.DesignHelper;

internal class RelayCommand<T> : ICommand
{
    private readonly Predicate<T>? _canExecute;
    private readonly Action<T> _execute;

    internal RelayCommand(Action<T> execute)
        : this(execute, default)
    {
    }

    internal RelayCommand(Action<T> execute, Predicate<T>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
#pragma warning disable CS8604
#pragma warning disable CS8600
        return _canExecute == default || _canExecute((T) parameter);
#pragma warning restore CS8600
#pragma warning restore CS8604
    }

    public void Execute(object? parameter)
    {
#pragma warning disable CS8604
#pragma warning disable CS8600
        _execute((T) parameter);
#pragma warning restore CS8600
#pragma warning restore CS8604
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void NotifyCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}