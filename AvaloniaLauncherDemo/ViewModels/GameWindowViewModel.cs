using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaLauncherDemo.Game;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaLauncherDemo.ViewModels;

public partial class GameWindowViewModel : ViewModelBase
{
    private Map _map=new();
    /// <summary>
    /// WARNING, this encodes the SKIASHARP bitmap as PNG, which is slow (as in several milliseconds)
    /// This should only be called when stuff actually has change
    /// </summary>
    public Bitmap MapBitmap
    {
        get
        {
            Console.WriteLine("CALL PNG CONVERTER");
            var pngStream = _map.AsPngImage(SelectedCity); 
            return new Bitmap(pngStream);
        }
    }
    
    /// <summary>
    /// Number of brigades in the selected city
    /// </summary>
    public int SelectedBrigades => _map.Brigades(SelectedCity);
    
    private string _selectedCity = "Aarhus";

    public GameWindowViewModel()
    {
        Stream mapStream = AssetLoader.Open(new Uri("avares://AvaloniaLauncherDemo/Assets/mapData.xml"));
    }

    public string SelectedCity
    {
        get { return _selectedCity;}
        set
        {
            _selectedCity = value;
            OnPropertyChanged(nameof(SelectedCity));
            OnPropertyChanged(nameof(MapBitmap));
        }
        
    }


    public List<string> CityNames => _map.CityNames;
    

    [RelayCommand]
    private void ImageClicked(Point imagePoint)
    {
        Console.WriteLine($"Clicked image, received in VM at {imagePoint.X},{imagePoint.Y}");
        SelectedCity = _map.SelectCity(imagePoint.X, imagePoint.Y);
        Console.WriteLine($"Selected {SelectedCity}");
        //The selection has now changed ... maybe
        OnPropertyChanged(nameof(SelectedCity));
        OnPropertyChanged(nameof(MapBitmap));
        OnPropertyChanged(nameof(SelectedBrigades));
    }
}