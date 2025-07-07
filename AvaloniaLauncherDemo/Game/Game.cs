using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SkiaSharp;

namespace AvaloniaLauncherDemo.Game;

/// <summary>
/// The class which handles the game, and the display of the map
/// </summary>
public class Game
{
    /// <summary>
    /// What order do the players come in
    /// </summary>
    private List<string> _nationsPlayOrder;

    private int _activePlayerIndex;
    /// <summary>
    /// Whose turn is it to give commands, commands will be executed in random order
    /// </summary>
    public string ActivePlayer => _nationsPlayOrder[_activePlayerIndex];
    
    
    /// <summary>
    /// the radius used to display the cities
    /// </summary>
    public static int CityMarkerRadius { get; set; } = 10;
    
    /// <summary>
    /// The radius of the selection marker around selected cities, and the radius you should click within to select
    /// </summary>
    public static int CitySelectionRadius { get; set; } = 20;

    
    private Dictionary<string,City> _cities;
    private Dictionary<string,Nation> _nations;
    
    /// <summary>
    /// List of queued orders which will be executed
    /// </summary>
    private List<Order> _queuedOrders;
    
    public List<string> CityNames
    {
        get
        {
            var keys = _cities.Keys.ToList();
            keys.Add("None");
            return keys;
        }
    }

    public void NextTurn()
    {
        _activePlayerIndex=(_activePlayerIndex+1)%_nationsPlayOrder.Count;

        foreach (var order in _queuedOrders)
        {
            if (_cities.ContainsKey(order.From) && _cities.ContainsKey(order.To) && _cities[order.From].Controller==order.Player && _cities[order.From].Brigades>0)
            {
                int number = Math.Min(_cities[order.From].Brigades,order.Brigades);
                if (_cities[order.To].Controller == order.Player)
                {
                    Console.WriteLine($"{order.Player} moves {number} brigades from "+order.From+" to "+order.To);
                    _cities[order.From].Brigades -= number;
                    _cities[order.To].Brigades += number;
                }
                else
                {
                    Console.WriteLine($"{order.Player} attacks {_cities[order.To].Controller} in {order.To} from {order.From} with {number} brigades");
                    _cities[order.From].Brigades -= number;
                    if (_cities[order.To].Brigades >= number)
                    {
                        _cities[order.To].Brigades -= number;
                    }
                    else
                    {
                        _cities[order.To].Brigades = number-_cities[order.To].Brigades;
                        _cities[order.To].Controller = order.Player;
                    }
                }
            }
            else
            {
                Console.WriteLine("Ignores invalid order");
            }
        }
        _queuedOrders = new List<Order>();
    }

    public int Brigades(string city)
    {
        if (_cities.TryGetValue(city, out var city1))
            return city1.Brigades;
        return 0;
    }


    public string SelectCity(double x, double y)
    {

        foreach (var (name, city) in _cities)
        {
            //If distance to the city is less than 10 pixels, we select it
            if (Math.Pow(city.x - x, 2) + Math.Pow(city.y - y, 2) < Math.Pow(CitySelectionRadius,2))
            {
                return name;
            }
        }
        return "None";
    }

    public Game(Stream stream)
    {
        _cities = new Dictionary<string, City>();
        _nations = new Dictionary<string,Nation>();
        
        XmlDocument doc = new();
        doc.Load(stream);
        var root = doc.DocumentElement;
        var nations = root?["Nations"];
        var cities = root?["Cities"];
        if (nations == null)
            throw new Exception("No Nations found in mapData.xml");
        if (cities== null)
            throw new Exception("No Nations found in mapData.xml");

        foreach (XmlNode nation in nations.ChildNodes)
        {
            Console.WriteLine(nation.Name);
            _nations[nation.Name] = new Nation(
                nation.Name, new SKColor(byte.Parse(nation.Attributes?["Red"]?.Value ?? "0"),byte.Parse(nation.Attributes?["Green"]?.Value ?? "0"),byte.Parse(nation.Attributes?["Blue"]?.Value ?? "0")));
        }
        
        
        
        foreach (XmlNode city in cities.ChildNodes)
        {
            _cities.Add(city.Name, new City(city.Name,int.Parse(city.Attributes?["x"]?.Value ?? "0"), int.Parse(city.Attributes?["y"]?.Value ?? "0"),city.Attributes?["controller"]?.Value ?? "Danmark"));

            foreach (XmlNode neighbour in city.ChildNodes)
            {
                //The xml may contain cities which have not yet been added, just ignore them and add two-way connection later
                if (_cities.ContainsKey(neighbour.Name))
                {
                    //Add two-way connection
                    _cities[city.Name].Neighbors.Add(_cities[neighbour.Name]);
                    _cities[neighbour.Name].Neighbors.Add(_cities[city.Name]);
                }
                
            }
        }
        _nationsPlayOrder = new List<string>() { "Danmark", "Sverige", "Tyskland" };
        
        _queuedOrders = new List<Order>();
        
        _activePlayerIndex = 0;
    }
    
    /// <summary>
    /// Convert internal svg document to a png image
    /// </summary>
    /// <returns></returns>
    public MemoryStream AsPngImage(string selectedCity)
    {
        //Convert SVG document to a stream
        /*MemoryStream outStream = new MemoryStream();
        _SVGDocument.Save(outStream);
        outStream.Position = 0;
        
        //Then read the stream as a Skia SVG
        var skSvg = new SKSvg();
        skSvg.Load(outStream);
        */
        
        // var picture = skSvg.Picture;
        // if (picture == null)
        //     throw new ArgumentException("Picture converter to null in svg converter, maybe svg is empty");

        //Auto-generate city markers for each nation
        Dictionary<string, SKPaint> cityMarkers = new();
        foreach (var (name, nation) in _nations)
        {
            cityMarkers[name] = new SKPaint()
            {
                Color = nation.Color,
                IsAntialias = true,
            };
        }
        
        
        SKPaint cityMarker = new SKPaint()
        {
            Color = SKColors.Red,
            IsAntialias = true,
        };
        SKPaint selectedCityMarker = new SKPaint()
        {
            Color = SKColors.Green,
            IsStroke = true,
            IsAntialias = true,
        };
        SKPaint selectedCityNeighbourMarker = new SKPaint()
        {
            Color = SKColors.Red,
            IsStroke = true,
            IsAntialias = true,
        };
        SKPaint TextPaint = new SKPaint()
        {
            Color = SKColors.Yellow,
            IsAntialias = true,
            TextSize = 64
        };
        
        //float scale = picture.CullRect.Width / 250;
        //It is easier for me to work with a hardcoded expected image size, this works well with the UI on a 1920 by 1080 windows computer
        var image = new SKImageInfo(1720,880); //new SKImageInfo((int)picture.CullRect.Width, (int)picture.CullRect.Height);
        using var bitmap = new SKBitmap(image);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.LightBlue);

            foreach (var (name, city) in _cities)
            {
                canvas.DrawCircle(city.x, city.y, CityMarkerRadius, cityMarkers[city.Controller]);
                foreach (var neighbor in city.Neighbors)
                {
                    canvas.DrawLine(city.x, city.y, neighbor.x, neighbor.y, cityMarker);
                }
            }
            foreach (var (name, city) in _cities)
                canvas.DrawText($"{city.Brigades}",city.x,city.y,TextPaint);

            if (_cities.ContainsKey(selectedCity))
            {
                canvas.DrawCircle((int)_cities[selectedCity].x,(int)_cities[selectedCity].y,CitySelectionRadius ,selectedCityMarker);
                foreach (var neighbour in _cities[selectedCity].Neighbors)
                {
                    canvas.DrawCircle((int)neighbour.x,(int)neighbour.y,CitySelectionRadius ,selectedCityNeighbourMarker);
                    
                }
            }
            //canvas.DrawPicture(picture);
        }
        var imageStream = new MemoryStream();
        bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
        imageStream.Position = 0;
        
        return imageStream;
    }
}