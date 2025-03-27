using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView;



using Client.Entities;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace Client.Functions
{
    public static class EchelonneAxe
    {
        public static List<Axis> Echelonne(this Data[] data)
        {
            int lentot = data.Length;

            List<Axis> axis;
            string[] datelist = new string[lentot];

            for (int i = 0; i < lentot; i++) {
                datelist[i] = data[i].Date.ToString("HH-mm-ss");
            }

            axis = new List<Axis> {
                new Axis
                {
                    Name = "Temps",
                    NamePaint = new SolidColorPaint(SKColors.White),
                    TextSize = 14,
                    Labels = datelist
                }
             };
            return axis;
        }
    }
}
        
