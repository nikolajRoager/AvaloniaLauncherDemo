using System;
using System.IO;
using SkiaSharp;

namespace AvaloniaLauncherDemo.Models;

public class Map
{
    
    /// <summary>
    /// Convert internal svg document to a png image
    /// </summary>
    /// <returns></returns>
    public MemoryStream AsPngImage()
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
        
     //   float scale = picture.CullRect.Width / 250;
     var image = new SKImageInfo(4000,2500); //new SKImageInfo((int)picture.CullRect.Width, (int)picture.CullRect.Height);
        using var bitmap = new SKBitmap(400,250);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors. LightBlue);
         //   canvas.DrawPicture(picture);
        }

        var imageStream = new MemoryStream();
        bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
        imageStream.Position = 0;
        
        return imageStream;
    }
}