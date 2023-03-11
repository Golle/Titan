using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private readonly IAppSettings _appSettings;
    private Grid GetGrid() => this.Get<Grid>("TheGrid");
    public MainWindow()
    : this(null)
    { }

    public MainWindow(IAppSettings? appSettings = null)
    {
        _appSettings = appSettings ?? Registry.GetRequiredService<IAppSettings>();
        AvaloniaXamlLoader.Load(this);
        SetWindowSize();
        
        HotKeyManager.SetHotKey(this.Get<MenuItem>("SaveAll"), new KeyGesture(Key.S, KeyModifiers.Control));

#if DEBUG
        this.AttachDevTools();
#endif
        ViewModel = new MainWindowViewModel();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        
        SetPanelSizes();

        var window = new SelectProjectWindow();
        var result = await window.ShowDialog<string>(this);
        if (result == null)
        {
            //NOTE(Jens):  Exit the app if the popup window is closed without selecting a project. Maybe we should change this later?
            App.Exit();
        }
        else
        {
            await ViewModel!.LoadProject(result);
        }
    }

    private void SetWindowSize()
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        var windowSize = _appSettings
            .GetSettings()
            .WindowSize;
        if (windowSize.HasValues)
        {
            Width = windowSize.Width;
            Height = windowSize.Height;
            if (windowSize.X >= 0 && windowSize.Y >= 0)
            {
                Position = new PixelPoint(windowSize.X, windowSize.Y);
            }
        }
    }

    // Change these values if we restructure the Grid. Can't name columns.

    // Cols
    private const int ManifestColumnIndex = 0;
    private const int MiddleColumnIndex = 2;
    private const int PropertiesColumnIndex = 4;

    // Rows
    private const int MainRowIndex = 0;
    private const int ContentRowIndex = 2;

    private void SetPanelSizes()
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        var settings = _appSettings.GetSettings();
        var grid = GetGrid();
        if (settings.ManifestPanelSize.Size != 0 && settings.PropertiesPanelSize.Size != 0)
        {
            var totalWidth = grid.ColumnDefinitions
                .Where(c => c.Width.IsStar)
                .Sum(c => c.ActualWidth);

            var columnDefinitions = grid.ColumnDefinitions;
            var manifestWidth = settings.ManifestPanelSize.Size;
            var propertiesWidth = settings.PropertiesPanelSize.Size;
            columnDefinitions[ManifestColumnIndex].Width = new GridLength(manifestWidth, GridUnitType.Star);
            columnDefinitions[PropertiesColumnIndex].Width = new GridLength(propertiesWidth, GridUnitType.Star);
            var middleWidth = totalWidth - manifestWidth - propertiesWidth;
            if (middleWidth <= 0)
            {
                middleWidth = 1;
            }
            columnDefinitions[MiddleColumnIndex].Width = new GridLength(middleWidth, GridUnitType.Star);
        }

        if (settings.ContentPanelSize.Size != 0)
        {
            var rowDefinitions = grid.RowDefinitions;
            var totalHeight = grid.RowDefinitions
                .Where(c => c.Height.IsStar)
                .Sum(c => c.ActualHeight);

            var height = settings.ContentPanelSize.Size;
            rowDefinitions[MainRowIndex].Height = new GridLength(totalHeight - height, GridUnitType.Star);
            rowDefinitions[ContentRowIndex].Height = new GridLength(height, GridUnitType.Star);
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        SavePanelAndWindowSizes();
        base.OnClosing(e);
    }
    
    private void SavePanelAndWindowSizes()
    {
        if (Design.IsDesignMode)
        {
            return;
        }

        var settings = _appSettings.GetSettings();
        var grid = GetGrid();

        var columnDefintions = grid.ColumnDefinitions;
        var rowDefinitions = grid.RowDefinitions;
        _appSettings.Save(settings with
        {
            ManifestPanelSize = new PanelSize(columnDefintions[ManifestColumnIndex].ActualWidth),
            PropertiesPanelSize = new PanelSize(columnDefintions[PropertiesColumnIndex].ActualWidth),
            ContentPanelSize = new PanelSize(rowDefinitions[ContentRowIndex].ActualHeight),
            WindowSize = WindowState is WindowState.Maximized ? settings.WindowSize :  new WindowSize(Width, Height, Position.X, Position.Y) //NOTE(Jens): If the window is maximized, don't record windowsizes (can't figure out how to get the size before the state change)
        });
    }
}
