using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using AvaloniaLauncherDemo.ViewModels;

namespace AvaloniaLauncherDemo.Views;

public partial class GameWindow : Window
{
    public GameWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets the location we just pressed on
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMapPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var image = sender as Image;
        //We will also need to talk to the pixels of the bitmap to scale this correctly
        if (image?.Source is Bitmap bitmap)
        {
            var point = e.GetPosition(image);
            
            //To rescale the point, get the widths before and after scaling
            double renderedWidth = image.Bounds.Width;
            double renderedHeight= image.Bounds.Height;
            double imageWidth = bitmap.PixelSize.Width;
            double imageHeight = bitmap.PixelSize.Height;
            
            Console.WriteLine($"Got width {renderedWidth} and height {renderedHeight}");
            
            //Scale back to image pixels
            double x =point.X * imageWidth / renderedWidth;
            double y = point.Y * imageHeight / renderedHeight;
            var newPoint = new Point(x, y);

            //Pass on this new 
            if (DataContext is GameWindowViewModel vm)
            {
                vm.ImageClickedCommand.Execute(newPoint);
            }
        }
    }
}