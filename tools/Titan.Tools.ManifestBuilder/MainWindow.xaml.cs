using System;
using System.Windows;

namespace Titan.Tools.ManifestBuilder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Platform.Text = $"Platform: {(Environment.Is64BitProcess ? "x64" : "x86")}\nThis is titan!";
        
    }
}
