using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace AvaloniaLauncherDemo.Game;

/// <summary>
/// The class which handles the map, and the display of the map
/// </summary>
public class Map
{
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
    
    public List<string> CityNames
    {
        get
        {
            var keys = _cities.Keys.ToList();
            keys.Add("None");
            return keys;
        }
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

    public Map()
    {
        _cities = new Dictionary<string, City>();
        _cities.Add("Aalborg", new City(140, 100 ,"Danmark"));
        _cities.Add("Aarhus", new City(200, 200 , "Danmark" ));
        _cities.Add("Esbjerg", new City(100, 450 , "Tyskland"));
        _cities.Add("København", new City(600, 300, "Sverige"));
        _cities.Add("Odense", new City(400, 400, "Sverige"));
        _cities.Add("Fredericia", new City(200, 400, "Tyskland"));
        
        _cities["Aarhus"].Neighbors = new List<City>{_cities["Aalborg"], _cities["Esbjerg"],_cities["Fredericia"]};
        _cities["Aalborg"].Neighbors = new List<City>{_cities["Aarhus"] };
        _cities["Esbjerg"].Neighbors = new List<City>{_cities["Aarhus"], _cities["Fredericia"]};
        _cities["Fredericia"].Neighbors = new List<City>{_cities["Aarhus"], _cities["Esbjerg"], _cities["Odense"]};
        _cities["Odense"].Neighbors = new List<City>{_cities["Fredericia"], _cities["København"] };
        _cities["København"].Neighbors = new List<City>{_cities["Odense"] };
        
        _nations = new Dictionary<string,Nation>();
        _nations["Danmark"] = new Nation("Danmark",SKColors.Red);
        _nations["Sverige"] = new Nation("Sverige",SKColors.Blue);
        _nations["Tyskland"] = new Nation("Tyskland",SKColors.Gray);
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
                
            }
            //canvas.DrawPicture(picture);
        }
        var imageStream = new MemoryStream();
        bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
        imageStream.Position = 0;
        
        return imageStream;
    }
}