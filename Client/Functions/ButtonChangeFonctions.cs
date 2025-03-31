using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Client.Functions;

public static class ButtonChangeFonctions
{
    public static void ApplyButtonTextChange(this Button button, TextBox textBox)
    {
        if (!string.IsNullOrWhiteSpace(textBox.Text))
        {
            button.Content = textBox.Text;
        }

        ReplaceTextBoxWithButton(button, textBox);
    }

    public static void CancelButtonTextEdit(this Button button, TextBox textBox)
    {
        ReplaceTextBoxWithButton(button, textBox);
    }

    public static void ReplaceTextBoxWithButton(this Button button, TextBox textBox)
    {
        if (textBox.Parent is Panel panel)
        {
            var index = panel.Children.IndexOf(textBox);
            panel.Children.RemoveAt(index);
            panel.Children.Insert(index, button);
        }
    }

    public static void StartRenameButton(this Button button)
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
            Padding = button.Padding
        };

        textBox.KeyDown += (sender, args) =>
        {
            if (args.Key == Avalonia.Input.Key.Enter)
            {
                button.ApplyButtonTextChange(textBox);
            }
            else if (args.Key == Avalonia.Input.Key.Escape)
            {
                button.CancelButtonTextEdit(textBox);
            }
        };

        textBox.LostFocus += (sender, args) => { button.ApplyButtonTextChange(textBox); };

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