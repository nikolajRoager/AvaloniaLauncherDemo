using System;
using Avalonia.Media.Imaging;
using AvaloniaLauncherDemo.Models;

namespace AvaloniaLauncherDemo.ViewModels;

public class GameWindowViewModel : ViewModelBase
{
    private Map _map=new();
    
    /// <summary>
    /// Stupit hack to get svg to something we can render, convert it to png first and then back to a bitmap
    /// </summary>
    public Bitmap MapBitmap
    {
        get
        {
            Console.WriteLine("CALL");
            var pngStream = _map.AsPngImage(); 
            return new Bitmap(pngStream);
        }
    }
}