using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Client.Entities;
using System.Threading.Tasks;

namespace Client.Functions;

public static class ButtonChangeFonctions
{
    public static async void ApplyButtonTextChange(this Button button, MainWindow window, TextBox textBox, Device device)
    {
        if (!string.IsNullOrWhiteSpace(textBox.Text))
        {
            button.Content = textBox.Text;
        }
        await window.ReplaceTextBoxWithButton(button, textBox, device);
    }

    public static async void CancelButtonTextEdit(this Button button, MainWindow window, TextBox textBox, Device device)
    {
        await window.ReplaceTextBoxWithButton(button, textBox, device);
    }
    
    public static void StartRenameButton(this Button button, MainWindow window, Device device)
    {
        var textBox = new TextBox
        {
            Text = button.Content.ToString(),
            MinWidth = 100,
            Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
            Foreground = Brushes.White,
            CaretBrush = Brushes.White,
            SelectionBrush = new SolidColorBrush(Colors.DodgerBlue),
            BorderThickness = new Thickness(0),
            Padding = button.Padding,
            Tag = device // Stocker l'appareil dans le Tag du TextBox
        };

        textBox.KeyDown += (sender, args) =>
        {
            if (args.Key == Avalonia.Input.Key.Enter)
            {
                button.ApplyButtonTextChange(window, textBox, device);
            }
            else if (args.Key == Avalonia.Input.Key.Escape)
            {
                button.CancelButtonTextEdit(window, textBox, device);
            }
        };

        textBox.LostFocus += (sender, args) => { button.ApplyButtonTextChange(window, textBox, device); };

        if (button.Parent is Panel panel)
        {
            var index = panel.Children.IndexOf(button);
            panel.Children.RemoveAt(index);
            panel.Children.Insert(index, textBox);
            textBox.Focus();
            textBox.SelectAll();
        }
    }
}
