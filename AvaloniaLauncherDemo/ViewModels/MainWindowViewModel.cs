using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using AvaloniaLauncherDemo.Messages;
using AvaloniaLauncherDemo.Models;
using AvaloniaLauncherDemo.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaLauncherDemo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    public string Title = "unset";
    /// <summary>
    /// Start the game with current settings selected
    /// </summary>
    [RelayCommand]
    private async Task PlayGame()
    {
        
        var newViewModel = new GameWindowViewModel();
        var newWindow = new GameWindow()
        {
            DataContext = newViewModel
        };
        
        // Show the new window
        newWindow.Show();
        //Send the message which closes the window
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage());
    }
}