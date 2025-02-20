using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;

namespace Client
{
    public partial class MainWindow : Window
    {
        private ContentControl pageContent = new ContentControl();
        private Border coloredFrame;

        public MainWindow()
        {
            InitializeComponent();
        }

        private StackPanel buttonPanel;
        private void InitializeComponent()
        {
            Title = "DomoTiX";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            

            var mainGrid = new Grid();
            mainGrid.RowDefinitions = new RowDefinitions();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Header
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));  // Content
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Colored Frame
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Buttons

            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
            gradient.EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative);
            gradient.GradientStops.Add(new GradientStop(Colors.DarkBlue, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.Black, 1.0));
            mainGrid.Background = gradient;

            

            // Barre de titre personnalisée
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10),
                Spacing = 10,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            var titleLabel = new TextBlock
            {
                Text = "DomoTiX",
                Foreground = Brushes.White,
            
                FontSize = 30,

            };

            headerPanel.Children.Add(titleLabel);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);


            // Zone de contenu principal
            pageContent.Margin = new Thickness(20);
            Grid.SetRow(pageContent, 1);
            mainGrid.Children.Add(pageContent);

            // Zone des boutons
            buttonPanel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(33, 33, 33)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(30),
                Spacing = 20
            };

            var button1 = new Button
            {
                Content = "home",
                Padding = new Thickness(10, 10),
                Background = new SolidColorBrush(Color.FromRgb(33,33,33)),
                Foreground = Brushes.White
                
            };
            button1.Click += (s, e) => LoadPage1();

            var button2 = new Button
            {
                Content = "Pièce 1",
                Padding = new Thickness(100, 20),
                Background = new SolidColorBrush(Colors.MediumOrchid),
                Foreground = Brushes.White
            };
            button2.Click += (s, e) => LoadPage2();


            buttonPanel.Children.Add(button2);
            Grid.SetRow(buttonPanel, 3);
            mainGrid.Children.Add(buttonPanel);
            headerPanel.Children.Add(button1);
            Content = mainGrid;

            // Charger la page 1 par défaut
            LoadPage1();
        }

        private void LoadPage1()
{
    var page1Content = new StackPanel
    {
        Spacing = 20,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };

    page1Content.Children.Add(new TextBlock
    {
        Text = "Menu principal",
        FontSize = 24,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center
    });

    // Ajout du bouton pour créer de nouveaux boutons
    var addButton = new Button
    {
        Content = "Ajouter un bouton",
        Padding = new Thickness(20, 10),
        Background = new SolidColorBrush(Colors.Green),
        Foreground = Brushes.White,
        Margin = new Thickness(0, 20, 0, 0)
    };
    addButton.Click += AddNewButton;

    page1Content.Children.Add(addButton);
    pageContent.Content = page1Content;
}

// Ajoutez cette nouvelle méthode pour gérer l'ajout de boutons
private int newButtonCount = 1;
private void AddNewButton(object sender, EventArgs e)
{
    var newButton = new Button
    {
        Content = $"Pièce {newButtonCount +1}",
        Padding = new Thickness(100, 20),
        Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
        Foreground = Brushes.White
    };
    
    // Ajout d'un gestionnaire d'événements pour le nouveau bouton
    int buttonNumber = newButtonCount;
    newButton.Click += (s, args) => 
    {
        var content = new StackPanel
        {
            Spacing = 20,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        content.Children.Add(new TextBlock
        {
            Text = $"Pièce {buttonNumber+1}",
            FontSize = 24,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        pageContent.Content = content;
    };

    buttonPanel.Children.Add(newButton);
    newButtonCount++;
}

        
        private void LoadPage2()
        {
            var page2Content = new StackPanel
            {
                Spacing = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            page2Content.Children.Add(new TextBlock
            {
                Text = "Pièce 1",
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            pageContent.Content = page2Content;
        }

    }
}