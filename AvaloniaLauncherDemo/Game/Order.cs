namespace AvaloniaLauncherDemo.Game;

/// <summary>
/// Base class for orders given to the game
/// </summary>
public class Order
{
    public string Player {get; set;}
    public string From {get; set;}
    public string To {get; set;}
    public int Brigades {get; set;}

    public Order(string player, string from, string to, int brigades)
    {
        Player = player;
        From = from;
        To = to;
        Brigades = brigades;
    }
}