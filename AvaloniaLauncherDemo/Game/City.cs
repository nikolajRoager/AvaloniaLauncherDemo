using System.Collections.Generic;

namespace AvaloniaLauncherDemo.Game;

public class City
{ 
    public string Name { get; set; }
    
    public string Controller {get; set;}
        
    public float x { get; set; }
    public float y { get; set; }
    
    public int Brigades { get; set; }
        
    public List<City> Neighbors { get; set; }
    public City(float x, float y, string controller,int brigades=5)
    {
        this.x = x;
        this.y = y;
        this.Controller = controller;
        this.Brigades = brigades;
        Neighbors = new List<City>();
    }
}
