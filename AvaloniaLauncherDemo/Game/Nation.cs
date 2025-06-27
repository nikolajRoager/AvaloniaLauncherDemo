using SkiaSharp;

namespace AvaloniaLauncherDemo.Game;

public class Nation
{
    public string Name { get; set; }
    public SKColor Color { get; set; }

    public Nation(string name, SKColor color)
    {
        this.Name = name;
        this.Color = color;
    }
}