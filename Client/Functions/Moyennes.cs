using Client.Entities;

namespace Client.Functions;

public static class Moyennes
{
    public static float MoyenneTemp(this Data[] datas)
    {
        float somme = 0;
        for (int i = 0; i < datas.Length; i++)
        {
            somme += datas[i].Temperature;
        }
        return somme / datas.Length;
    }
    
    public static float MoyenneHum(this Data[] datas)
    {
        float somme = 0;
        for (int i = 0; i < datas.Length; i++)
        {
            somme += datas[i].Humidite;
        }
        return somme / datas.Length;
    }
}