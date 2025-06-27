using System;
using System.Diagnostics;
using Avalonia.Controls;
using AvaloniaLauncherDemo.Messages;
using AvaloniaLauncherDemo.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaLauncherDemo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<CloseWindowMessage>(this, (r, m) =>
        {
            this.Close();
        });
    }
}