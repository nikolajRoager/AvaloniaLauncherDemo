using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaLauncherDemo.ViewModels;

public class ViewModelBase : ObservableObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}