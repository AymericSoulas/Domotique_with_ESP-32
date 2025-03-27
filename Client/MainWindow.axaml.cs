using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using System.IO;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using SkiaSharp;
using System.Net.Http;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Painting;

using Client.Entities;
using Client.Functions;
using Client.httpRequests;
using Microsoft.Extensions.Configuration;

namespace Client
{
    public partial class MainWindow : Window
    {
        #region Fields
        private readonly ContentControl _pageContent;
        private readonly Border _coloredFrame;
        private readonly WrapPanel _buttonPanel;
        private readonly StackPanel _addButtonContainer;
        private int _newButtonCount = 4; // Changé à 4 pour commencer avec 4 boutons
        private Data[] _data = new Data[]
        {
            new Data
            {
                Date = DateTime.Now,
                Temperature = 20,
                Humidite = 50,
                Id = 10,
                Appareil = 0
            }
        };
        private readonly HttpClient _client = new HttpClient();
        private string _serverAddress = "http://localhost:5262";
        #endregion

        #region Initialization
        public MainWindow()
        {
            _pageContent = new ContentControl();
            _coloredFrame = new Border();
            _buttonPanel = new WrapPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(30),
            };
            _addButtonContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5)
            };
            
            LoadConfiguration();
            AddInitialButtons(); // Nouvelle méthode
            InitializeComponent();
        }

        private void AddInitialButtons()
        {
            // Ajoute 4 boutons initiaux
            for (int i = 1; i <= 4; i++)
            {
                var button = CreateRoomButton($"Pièce {i}");
                _buttonPanel.Children.Add(button);
            }
        }

        private Button CreateRoomButton(string content)
        {
            var button = new Button
            {
                Content = content,
                Padding = new Thickness(50, 20),
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = Brushes.White
            };

            var contextMenu = new ContextMenu();
            var renameItem = new MenuItem { Header = "Renommer" };
            var deleteItem = new MenuItem { Header = "Supprimer" };

            renameItem.Click += (s, e) => StartRenameButton(button);
            deleteItem.Click += (s, e) => DeleteButton(button);

            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(deleteItem);
            button.ContextMenu = contextMenu;

            button.Click += (s, e) => LoadPage2(button);
            return button;
        }
        #endregion

        #region Button Management
        private void StartRenameButton(Button button)
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
        }

        private void DeleteButton(Button button)
        {
            if (button.Parent is Panel panel)
            {
                panel.Children.Remove(button);
            }
        }

        private void AddNewButton(object sender, EventArgs e)
        {
            var newButton = CreateRoomButton($"Pièce {_newButtonCount + 1}");
            _buttonPanel.Children.Add(newButton);
            _newButtonCount++;
        }
        #endregion

        #region Page Management
        private void LoadPage1()
        {
            // Afficher le bouton d'ajout
            _addButtonContainer.IsVisible = true;

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

            // Retirer le buttonPanel de son parent actuel s'il en a un
            if (_buttonPanel.Parent is Panel parent)
            {
                parent.Children.Remove(_buttonPanel);
            }

            page1Content.Children.Add(_buttonPanel);
            _pageContent.Content = page1Content;
        }
        
        private async void LoadPage2(Button sourceButton)
        {
            // Cacher le bouton d'ajout
            _addButtonContainer.IsVisible = false;

            string pageName = sourceButton.Content?.ToString() ?? "Nouvelle pièce";
            
            // Show loading indicator or message
            var loadingContent = new StackPanel
            {
                Spacing = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            loadingContent.Children.Add(new TextBlock
            {
                Text = "Chargement des données...",
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            _pageContent.Content = loadingContent;
            
            var page2Content = await CreatePage2Content(pageName);
            _pageContent.Content = page2Content;
        }

        private async Task<StackPanel> CreatePage2Content(string pageName)
        {
            var content = new StackPanel
            {
                Spacing = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            content.Children.Add(new TextBlock
            {
                Text = pageName,
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var chartsGrid = await CreateChartsGrid();
            content.Children.Add(chartsGrid);

            return content;
        }
        #endregion

        #region Charts
        private async Task<Grid> CreateChartsGrid()
        {
            // Get data and wait for it to complete
            _data = await this.GetData(_client, _serverAddress, _data);
            
            var chartsGrid = new Grid { Margin = new Thickness(20) };
            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var tempChart = CreateTemperatureChart();
            var humChart = CreateHumidityChart();

            Grid.SetColumn(tempChart, 0);
            Grid.SetColumn(humChart, 1);

            chartsGrid.Children.Add(tempChart);
            chartsGrid.Children.Add(humChart);

            return chartsGrid;
        }

        private CartesianChart CreateTemperatureChart()
        {
            return new CartesianChart
            {
                Series = _data.ToCollection(),
                XAxes = _data.Echelonne(),
                YAxes = CreateTemperatureAxis(),
                Height = 300,
                Width = 350,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };
        }

        private CartesianChart CreateHumidityChart()
        {
            return new CartesianChart
            {
                Series = _data.ToCollection(true),
                XAxes = _data.Echelonne(),
                YAxes = CreateHumidityAxis(),
                Height = 300,
                Width = 350,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };
        }

        private Axis[] CreateTemperatureAxis() => new[] 
        {
            new Axis 
            {
                Name = "Température (°C)",
                NamePaint = new SolidColorPaint(SKColors.White),
                TextSize = 14
            }
        };

        private Axis[] CreateHumidityAxis() => new[] 
        {
            new Axis 
            {
                Name = "Humidité (%)",
                NamePaint = new SolidColorPaint(SKColors.White),
                TextSize = 14
            }
        };
        #endregion

        #region Actions d'initialisation
        
        private void InitializeComponent()
        {
            Title = "DomoTiX";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            

            var mainGrid = new Grid();
            mainGrid.RowDefinitions = new RowDefinitions();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Header
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Add Button
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));  // Content
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));

            // En-tête avec titre et bouton home
            var headerPanel = new Grid
            {
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                Height = 50
            };
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            var titleLabel = new TextBlock
            {
                Text = "DomoTiX",
                Foreground = Brushes.White,
                FontSize = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0)
            };

            var homeButton = new Button
            {
                Content = "🏠",
                FontSize = 20,
                Padding = new Thickness(15, 5),
                Background = new SolidColorBrush(Color.FromRgb(33,33,33)),
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            homeButton.Click += (s, e) => LoadPage1();

            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(homeButton, 1);
            headerPanel.Children.Add(titleLabel);
            headerPanel.Children.Add(homeButton);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            // Bouton d'ajout sous l'en-tête
            var addButtonContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5)
            };

            var addButton = new Button
            {
                Content = "Ajouter une pièce",
                FontSize = 15,
                Padding = new Thickness(15, 5),
                Background = new SolidColorBrush(Colors.DeepSkyBlue),
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };
            addButton.Click += AddNewButton;

            addButtonContainer.Children.Add(addButton);
            Grid.SetRow(addButtonContainer, 1);
            mainGrid.Children.Add(addButtonContainer);

            // Zone de contenu principal
            _pageContent.Margin = new Thickness(20);
            Grid.SetRow(_pageContent, 2);
            mainGrid.Children.Add(_pageContent);

            Content = mainGrid;

            // Charger la page 1 par défaut
            LoadPage1();
        }
        //Fonction de chargement de la configuration
        private void LoadConfiguration()
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                
                string? configAddress = configuration["ServerAddress"];
                if (!string.IsNullOrEmpty(configAddress))
                {
                    _serverAddress = configAddress;
                }
                else
                {
                    Console.WriteLine("Server address not found in configuration!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }
        }
        #endregion

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
    }
}
