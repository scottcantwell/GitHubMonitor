using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GitHubMonitor.Behaviors;

/// <summary>
/// A behavior that automatically scrolls a ListBox to the end when new items are added.
/// </summary>
public static class ListBoxAutoScrollBehavior
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(ListBoxAutoScrollBehavior),
            new PropertyMetadata(false, OnChanged));

    /// <summary>
    /// Gets the value of the AutoScroll attached property for the specified DependencyObject.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool GetAutoScroll(DependencyObject o) => (bool)o.GetValue(AutoScrollProperty);

    /// <summary>
    /// Sets the value of the AutoScroll attached property for the specified DependencyObject.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="v"></param>
    public static void SetAutoScroll(DependencyObject o, bool v) => o.SetValue(AutoScrollProperty, v);

    /// <summary>
    /// Called when the AutoScroll attached property changes. Hooks or unhooks the Loaded event based on the new value.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ListBox listBox) return;
        listBox.Loaded -= Loaded;
        if (e.NewValue is true)
        {
            listBox.Loaded += Loaded;
            if (listBox.IsLoaded) Hook(listBox);
        }
    }

    /// <summary>
    /// Handles the Loaded event of the ListBox. Hooks the collection changed event to enable auto-scrolling when new items are added.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void Loaded(object sender, RoutedEventArgs e) => Hook((ListBox)sender);

    private static void Hook(ListBox listBox)
    {
        if (listBox.ItemsSource is not INotifyCollectionChanged notify) return;
        notify.CollectionChanged -= Handler;
        notify.CollectionChanged += Handler;

        void Handler(object? _, NotifyCollectionChangedEventArgs args)
            => OnItemsChanged(listBox, args);
    }

    /// <summary>
    /// Handles the collection changed event of the ListBox's items. Scrolls to the last item if new items are added and the ListBox is already scrolled to the bottom.
    /// </summary>
    /// <param name="listBox">The ListBox whose items have changed.</param>
    /// <param name="e">The event data for the collection changed event.</param>
    private static void OnItemsChanged(ListBox listBox, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Reset))
            return;
        if (listBox.Items.Count == 0)
            return;

        var viewer = FindScrollViewer(listBox);
        var stickToEnd = viewer is null
                         || viewer.ScrollableHeight <= 0
                         || viewer.VerticalOffset >= viewer.ScrollableHeight - 12
                         || e.Action == NotifyCollectionChangedAction.Reset;
        if (!stickToEnd)
            return;

        listBox.Dispatcher.InvokeAsync(() =>
        {
            if (listBox.Items.Count > 0)
                listBox.ScrollIntoView(listBox.Items[^1]);
        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// Recursively searches the visual tree for a ScrollViewer starting from the specified root DependencyObject. Returns the first ScrollViewer found, or null if none is found.
    /// </summary>
    /// <param name="root">The root DependencyObject to start the search from.</param>
    /// <returns>The first ScrollViewer found, or null if none is found.</returns>
    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        if (root is ScrollViewer sv) return sv;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var found = FindScrollViewer(VisualTreeHelper.GetChild(root, i));
            if (found is not null) return found;
        }
        return null;
    }
}