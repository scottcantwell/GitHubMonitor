using GitHubMonitor.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace GitHubMonitor.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class. 
    /// Sets up event handlers for DataContext changes and window closing events.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Closing += (_, _) =>
        {
            PersistLayout();
            if (Vm is not null)
                Vm.PropertyChanged -= OnViewModelPropertyChanged;
            Vm?.ClosingCommand.Execute(null);
        };
    }

    private MainViewModel? Vm => DataContext as MainViewModel;

    /// <summary>
    /// Handles the DataContextChanged event of the MainWindow. Subscribes to the PropertyChanged event of the new ViewModel and unsubscribes 
    /// from the old one. Applies the visibility layout when the DataContext changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MainViewModel oldVm)
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        if (e.NewValue is MainViewModel newVm)
            newVm.PropertyChanged += OnViewModelPropertyChanged;
        ApplyVisibilityLayout();
    }

    /// <summary>
    /// Handles the Loaded event of the MainWindow. Applies the visibility layout when the window is loaded.    
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyVisibilityLayout();
    }

    /// <summary>
    /// Handles the PropertyChanged event of the ViewModel. Applies the visibility layout when relevant properties change, such as IsCriteriaVisible, IsLogVisible, IsResultsVisible, or ShowAllSectionsCommand.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.IsCriteriaVisible)
            or nameof(MainViewModel.IsLogVisible)
            or nameof(MainViewModel.IsResultsVisible)
            or nameof(MainViewModel.ShowAllSectionsCommand))
        {
            ApplyVisibilityLayout();
        }
    }

    /// <summary>
    /// Applies the visibility layout based on the current state of the ViewModel. Adjusts the widths and heights of the criteria and log sections, as well as the splitter sizes, according to whether they are visible or not. Persists the layout when sections are hidden.
    /// </summary>
    private void ApplyVisibilityLayout()
    {
        if (Vm is null) return;

        if (Vm.IsCriteriaVisible)
        {
            CriteriaColumn.MinWidth = 220;
            CriteriaColumn.Width = new GridLength(Math.Max(220, Vm.CriteriaWidth));
            SplitterColumn.Width = new GridLength(8);
        }
        else
        {
            PersistLayout();
            CriteriaColumn.MinWidth = 0;
            CriteriaColumn.Width = new GridLength(0);
            SplitterColumn.Width = new GridLength(0);
        }

        if (Vm.IsLogVisible)
        {
            LogSplitterRow.Height = new GridLength(8);
            LogRow.MinHeight = 60;
            LogRow.Height = new GridLength(Math.Max(60, Vm.LogHeight));
        }
        else
        {
            PersistLayout();
            LogSplitterRow.Height = new GridLength(0);
            LogRow.MinHeight = 0;
            LogRow.Height = new GridLength(0);
        }
    }

    /// <summary>
    /// Handles the DragCompleted event for the column splitter. Persists the current layout when the user finishes dragging the splitter.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnColumnSplitterDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        => PersistLayout();

    /// <summary>
    /// Handles the DragCompleted event for the row splitter. Persists the current layout when the user finishes dragging the splitter.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnRowSplitterDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        => PersistLayout();

    /// <summary>
    /// Handles the MouseDoubleClick event for the repository list. Executes the OpenSelectedCommand of the ViewModel when a repository is double-clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnRepoDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => Vm?.OpenSelectedCommand.Execute(null);

    /// <summary>
    /// Persists the current layout state of the criteria and log sections. Saves the widths and heights of these sections to the ViewModel if they are 
    /// visible and meet minimum size requirements. This method is called when sections are hidden or when the user finishes dragging splitters.
    /// </summary>
    private void PersistLayout()
    {
        if (Vm is null) return;

        double? criteria = Vm.IsCriteriaVisible && CriteriaColumn.ActualWidth >= 220
            ? CriteriaColumn.ActualWidth
            : null;
        double? log = Vm.IsLogVisible && LogRow.ActualHeight >= 60
            ? LogRow.ActualHeight
            : null;

        Vm.ApplyLayoutCommand.Execute(new LayoutState(criteria, log));
    }
}