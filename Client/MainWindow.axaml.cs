using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.Defaults;
using SkiaSharp;
using System.Linq;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Client
{
    public partial class MainWindow : Window
    {
        private ContentControl pageContent = new ContentControl();
        private Border coloredFrame;

        private WrapPanel buttonPanel;
        private int newButtonCount = 1;

        public MainWindow()
        {
            pageContent = new ContentControl();
            coloredFrame = new Border();
            buttonPanel = new WrapPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(30),
            };

            // Ajout du bouton "Pièce 1" initial
            var initialButton = new Button
            {
                Content = "Pièce 1",
                Padding = new Thickness(50, 20),
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = Brushes.White
            };
            initialButton.Click += (s, e) => LoadPage2();
            MakeButtonTextEditable(initialButton);
            buttonPanel.Children.Add(initialButton);

            InitializeComponent();
        }

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

            mainGrid.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));

            

            // Barre de titre personnalisée
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10),
                Spacing = 10,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            var titleLabel = new TextBlock
            {
                Text = "DomoTiX",
                Foreground = Brushes.White,
                FontSize = 30,
            };

            // Créer un conteneur pour aligner le bouton home à droite
            var rightAlignedPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var button1 = new Button
            {
                Content = "home",
                Padding = new Thickness(10, 10),
                Background = new SolidColorBrush(Color.FromRgb(33,33,33)),
                Foreground = Brushes.White
            };
            button1.Click += (s, e) => LoadPage1();

            rightAlignedPanel.Children.Add(button1);
            headerPanel.Children.Add(titleLabel);
            headerPanel.Children.Add(rightAlignedPanel);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            // Zone de contenu principal
            pageContent.Margin = new Thickness(20);
            Grid.SetRow(pageContent, 1);
            mainGrid.Children.Add(pageContent);

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
                Content = "+",
                Padding = new Thickness(20, 10),
                Background = new SolidColorBrush(Colors.Green),
                Foreground = Brushes.White,
                Margin = new Thickness(0, 20, 0, 0)
            };
            addButton.Click += AddNewButton;
            page1Content.Children.Add(addButton);

            // Retirer le buttonPanel de son parent actuel s'il en a un
            if (buttonPanel.Parent is Panel parent)
            {
                parent.Children.Remove(buttonPanel);
            }

            page1Content.Children.Add(buttonPanel);
            pageContent.Content = page1Content;
        }


        private void MakeButtonTextEditable(Button button)
        {
            button.DoubleTapped += (s, e) =>
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
                        ApplyButtonTextChange(button, textBox);
                    }
                    else if (args.Key == Avalonia.Input.Key.Escape)
                    {
                        CancelButtonTextEdit(button, textBox);
                    }
                };

                textBox.LostFocus += (sender, args) =>
                {
                    ApplyButtonTextChange(button, textBox);
                };

                if (button.Parent is Panel panel)
                {
                    var index = panel.Children.IndexOf(button);
                    panel.Children.RemoveAt(index);
                    panel.Children.Insert(index, textBox);
                    textBox.Focus();
                    textBox.SelectAll();
                }
            };
        }

        private void ApplyButtonTextChange(Button button, TextBox textBox)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                button.Content = textBox.Text;
            }
            ReplaceTextBoxWithButton(button, textBox);
        }

        private void CancelButtonTextEdit(Button button, TextBox textBox)
        {
            ReplaceTextBoxWithButton(button, textBox);
        }

        private void ReplaceTextBoxWithButton(Button button, TextBox textBox)
        {
            if (textBox.Parent is Panel panel)
            {
                var index = panel.Children.IndexOf(textBox);
                panel.Children.RemoveAt(index);
                panel.Children.Insert(index, button);
            }
        }
        // Ajoutez cette nouvelle méthode pour gérer l'ajout de boutons
        private void AddNewButton(object sender, EventArgs e)
        {
        var newButton = new Button
        {
            Content = $"Pièce {newButtonCount + 1}",
            Padding = new Thickness(50, 20),
            Margin = new Thickness(10), // Ajoute de l'espacement entre les boutons
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
            Foreground = Brushes.White
        };

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
                    Text = newButton.Content.ToString(),
                    FontSize = 24,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                pageContent.Content = content;
            };

            MakeButtonTextEditable(newButton);
            buttonPanel.Children.Add(newButton);
            newButtonCount++;
        }
        
        private void LoadPage2()
        {
            var button = buttonPanel.Children.OfType<Button>().FirstOrDefault(b => b.IsPointerOver);
            string pageName = button?.Content?.ToString() ?? "Nouvelle pièce";

            var page2Content = new StackPanel
            {
                Spacing = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            page2Content.Children.Add(new TextBlock
            {
                Text = pageName,
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // Création d'une grille pour les graphiques
            var chartsGrid = new Grid
            {
                Margin = new Thickness(20)
            };
            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var tempChart = new CartesianChart
            {
                Series = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = new double[] { 21, 22, 20, 23, 24, 21 },
                        Name = "Température",
                        Fill = null
                    }
                },
                XAxes = new[] 
                {
                    new Axis 
                    {
                        Name = "Temps",
                        NamePaint = new SolidColorPaint(SKColors.White),
                        TextSize = 14
                    }
                },
                YAxes = new[] 
                {
                    new Axis 
                    {
                        Name = "Température (°C)",
                        NamePaint = new SolidColorPaint(SKColors.White),
                        TextSize = 14
                    }
                },
                Height = 300,
                Width = 350,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };

            // Deuxième graphique (Humidité)
            var humChart = new CartesianChart
            {
                Series = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = new double[] { 45, 48, 50, 46, 44, 47 },
                        Name = "Humidité",
                        Fill = null
                    }
                },
                XAxes = new[] 
                {
                    new Axis 
                    {
                        Name = "Temps",
                        NamePaint = new SolidColorPaint(SKColors.White),
                        TextSize = 14
                    }
                },
                YAxes = new[] 
                {
                    new Axis 
                    {
                        Name = "Humidité (%)",
                        NamePaint = new SolidColorPaint(SKColors.White),
                        TextSize = 14
                    }
                },
                Height = 300,
                Width = 350,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };
            Grid.SetColumn(tempChart, 0);
            Grid.SetColumn(humChart, 1);

            chartsGrid.Children.Add(tempChart);
            chartsGrid.Children.Add(humChart);

            page2Content.Children.Add(chartsGrid);
            pageContent.Content = page2Content;
        }

    }
}