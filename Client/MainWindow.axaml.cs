using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using System.IO;
using System.Linq;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using SkiaSharp;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Dtos;
using LiveChartsCore.SkiaSharpView.Painting;

using Client.Entities;
using Client.Functions;
using Client.httpRequests;
using Microsoft.Extensions.Configuration;

namespace Client
{
    public partial class MainWindow : Window
    {
        #region Variables
        private readonly ContentControl _pageContent;
        private readonly Border _coloredFrame;
        private readonly WrapPanel _buttonPanel;
        private readonly StackPanel _addButtonContainer;
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
        private Device[]? _devices;
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
            InitializeComponent();
        }

        private void AddInitialButtons()
        {
            _buttonPanel.Children.Clear();
            // Ajoute 4 boutons initiaux
            for (int i = 0; i < _devices.Length; i++)
            {
                var button = CreateRoomButton(_devices[i].Nom, _devices[i].Id);
                _buttonPanel.Children.Add(button);
            }
        }

        private Button CreateRoomButton(string content, uint Id)
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

            renameItem.Click += (s, e) => button.StartRenameButton();
            deleteItem.Click += (s, e) => DeleteButton(button);

            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(deleteItem);
            button.ContextMenu = contextMenu;

            button.Click += (s, e) => LoadPage2(button, Id);
            return button;
        }
        #endregion

        #region Button Management

        private void DeleteButton(Button button)
        {
            if (button.Parent is Panel panel)
            {
                panel.Children.Remove(button);
            }
        }

        private void AddNewButton(object sender, EventArgs e)
        {
            CreateAppareilDto newdevice = new CreateAppareilDto("ESP-32", "ESP-32", 1, "Un nouvel appareil.");
            this.CreateDevice(_client, _serverAddress, newdevice);
            LoadingHomePage();
        }
        #endregion

        #region Page Management
        
        //Page de chargement 
        
        // Chargement de la page d'accueil
        private async void LoadingHomePage()
        {
            //on cache le bouton d'ajout
            _addButtonContainer.IsVisible = false;
            
            // Stackpanel pour le texte de chargement
            var loadingContent = new StackPanel
            {
                Spacing = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            // On ajoute le texte de chargement des données
            loadingContent.Children.Add(new TextBlock
            {
                Text = "Chargement des données...",
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            // On remplace le contenu actuel de la page par notre écran de chargement
            _pageContent.Content = loadingContent;
            
            // On crée une nouvelle page qui récupère les données 
            _devices = await this.GetDevice(_client, _serverAddress);
            if (_devices == null)
            {
                _devices = new Device[]
                {
                    new Device()
                    {
                        Id = 1,
                        Localisation = 1,
                        Nom = "Erreur",
                        Type = "Erreur"
                    }
                };
            }
            LoadHomePage();

        }
        
        private void LoadHomePage()
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
            AddInitialButtons();
            _pageContent.Content = page1Content;
        }
        
        // Page de chargement lors de la récupération des données, sert à éviter des erreurs dues
        // au délai de récupération des données
        private async void LoadPage2(Button sourceButton, uint Id)
        {
            // Cacher le bouton d'ajout
            _addButtonContainer.IsVisible = false;

            string pageName = sourceButton.Content?.ToString() ?? "Nouvelle pièce";
            
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
            
            // On attend la récupération des données avant de charger la page finale
            var page2Content = await CreatePage2Content(pageName, Id);
            _pageContent.Content = page2Content;
        }

        private async Task<StackPanel> CreatePage2Content(string pageName, uint Id)
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

            var chartsGrid = await CreateChartsGrid(Id);
            content.Children.Add(chartsGrid);

            return content;
        }
        #endregion

        #region Charts
        private async Task<Grid> CreateChartsGrid(uint Id)
        {
            // Get data and wait for it to complete
            _data = await this.GetData(_client, _serverAddress, _data, Id = Id);
            
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
            Width = 1600;
            Height = 800;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var mainGrid = new Grid();
            mainGrid.RowDefinitions = new RowDefinitions();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Header
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));  // Add Button
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));  // Content
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));

            // Création de l'en-tête
            // Création du conteneur
            var headerPanel = new Grid
            {
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                Height = 50
            };
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            
            // Titre de la page
            var titleLabel = new TextBlock
            {
                Text = "DomoTiX",
                Foreground = Brushes.White,
                FontSize = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0)
            };

            // Bouton d'accueil
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
            //Action lors du click sur le titre
            homeButton.Click += (s, e) => LoadingHomePage();
            
            //Positionnement des éléments
            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(homeButton, 1);
            
            //Ajout au parent
            headerPanel.Children.Add(titleLabel);
            headerPanel.Children.Add(homeButton);
            
            // Ajout de l'en-tête au Grid principal
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
            LoadingHomePage();
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


    }
}
