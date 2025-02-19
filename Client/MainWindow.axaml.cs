using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuration de base de la fenêtre
            Title = "DomoTiX";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Création du contenu principal
            var mainGrid = new Grid();
            mainGrid.RowDefinitions = new RowDefinitions();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
            gradient.EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative);
            gradient.GradientStops.Add(new GradientStop(Colors.DarkBlue, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.LightBlue, 1.0));
            mainGrid.Background = gradient;

            // Barre de titre personnalisée
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10),
                Spacing = 10
            };

            var titleLabel = new TextBlock
            {
                Text = "regarde bien ",
                Foreground = Brushes.White,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(titleLabel);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            // Zone de contenu principal
            var contentPanel = new DockPanel
            {
                Margin = new Thickness(20)
            };

            var welcomeText = new TextBlock
            {
                Text = "Bienvenue !",
                Foreground = Brushes.White,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            contentPanel.Children.Add(welcomeText);
            Grid.SetRow(contentPanel, 1);
            mainGrid.Children.Add(contentPanel);

            // Ajout des boutons de contrôle
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10),
                Spacing = 10
            };

            var button = new Button
            {
                Content = "bouton qui sert à rien",
                Padding = new Thickness(20, 10),
                Background = new SolidColorBrush(Colors.Green),
                Foreground = Brushes.White
            };

            buttonPanel.Children.Add(button);
            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;
        }
    }
}