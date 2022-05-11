using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

// ReSharper disable once IdentifierTypo
namespace HuntMmrReader.ViewAddons;

public sealed class BubbleScrollEvent : Behavior<UIElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        base.OnDetaching();
    }

    private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        var newEvent = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {RoutedEvent = UIElement.MouseWheelEvent};
        AssociatedObject.RaiseEvent(newEvent);
    }
}