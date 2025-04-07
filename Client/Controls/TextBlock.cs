using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Client.Controls;

public class TextBlockCustom:  Border
{
    TextBlock _textBlock;
    public TextBlockCustom(string? text, int borderthickness = 3)
    {
        _textBlock = new TextBlock()
        {
            Text = text,
            TextAlignment = TextAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            FontSize = 20
        };
        this.Child = _textBlock;
        this.BorderThickness = new Thickness(borderthickness);
        this.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.BorderBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        this.CornerRadius = new CornerRadius(borderthickness);
        this.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        this.Padding = new Thickness(borderthickness);
    }

    public void UpdateText(string text)
    {
        _textBlock.Text = text;
    }
}