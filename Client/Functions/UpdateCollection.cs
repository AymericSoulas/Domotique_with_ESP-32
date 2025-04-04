using System.Collections.ObjectModel;
using System.Linq;
using Client.Entities;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Client.Functions;

public static class UpdateCollection
{
    public static ObservableCollection<ISeries> ToCollection(this Data[] data, bool type = false)
    {
        ObservableCollection<ISeries> collection = new ObservableCollection<ISeries>();
        
        collection.Add(new LineSeries<float>
        {
                
            Values = data.Select(x => x.Humidite).ToArray(),
            Name = "Humidité"
        });
        
        collection.Add(new LineSeries<float>
        {
            Values = data.Select(x => x.Temperature).ToArray(),
            Name = "Température"
        });
        /*
        if (type)
        {
            collection.Add(new LineSeries<float>
            {
                
                Values = data.Select(x => x.Humidite).ToArray(),
                Name = "Temperature"
            });
        }
        else
        {
            collection.Add(new LineSeries<float>
            {
                Values = data.Select(x => x.Temperature).ToArray(),
            });
        }*/
        return collection;
    }
}