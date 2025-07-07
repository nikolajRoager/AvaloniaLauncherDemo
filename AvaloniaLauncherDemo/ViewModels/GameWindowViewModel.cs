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
    private Game.Game _game;
    /// <summary>
    /// WARNING, this encodes the SKIASHARP bitmap as PNG, which is slow (as in several milliseconds)
    /// This should only be called when stuff actually has change
    /// </summary>
    public Bitmap MapBitmap
    {
        get
        {
            Console.WriteLine("CALL PNG CONVERTER");
            var pngStream = _game.AsPngImage(SelectedCity); 
            return new Bitmap(pngStream);
        }
    }

    public string PlayerTurn => _game.ActivePlayer;
    
    /// <summary>
    /// Number of brigades in the selected city
    /// </summary>
    public int SelectedBrigades => _game.Brigades(SelectedCity);
    
    private string _selectedCity = "Aarhus";

    public GameWindowViewModel()
    {
        Stream mapStream = AssetLoader.Open(new Uri("avares://AvaloniaLauncherDemo/Assets/mapData.xml"));
        
        _game=new Game.Game(mapStream);
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


    public List<string> CityNames => _game.CityNames;
    

    [RelayCommand]
    private void ImageClicked(Point imagePoint)
    {
        SelectedCity = _game.SelectCity(imagePoint.X, imagePoint.Y);
        //The selection has now changed ... maybe
        OnPropertyChanged(nameof(SelectedCity));
        OnPropertyChanged(nameof(MapBitmap));
        OnPropertyChanged(nameof(SelectedBrigades));
    }

    [RelayCommand]
    private void NextTurn()
    {
        Console.WriteLine("--Turn--");
        _game.NextTurn();
        SelectedCity = "None";
        OnPropertyChanged(nameof(SelectedCity));
        OnPropertyChanged(nameof(MapBitmap));
        OnPropertyChanged(nameof(SelectedBrigades));
        OnPropertyChanged(nameof(PlayerTurn));
    }
}