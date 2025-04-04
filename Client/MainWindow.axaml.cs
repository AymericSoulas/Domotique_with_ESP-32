using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using SkiaSharp;
using System.Net.Http;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Configuration;

using Client.Controls;
using Client.Dtos;
using Client.Entities;
using Client.Functions;
using Client.httpRequests;


namespace Client
{
    public partial class MainWindow : Window
    {
        #region Variables
        // Variables contenant des éléments graphiques
        private readonly ContentControl _pageContent;
        private readonly Border _coloredFrame;
        private readonly WrapPanel _buttonPanel;
        private readonly StackPanel _addButtonContainer;
        
        // Variables de sélection de dates
        private DateTime _startDate;
        private DateTime _endDate;
        private readonly DatePicker _startDatePicker;
        private readonly DatePicker _endDatePicker;
        private readonly TimePicker _startTimePicker;
        private readonly TimePicker _endTimePicker;
        
        //Variables contenant les données récoltées depuis l'API
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
        public HttpClient Client => _client;
        private string _serverAddress = "http://localhost:5262";
        public string ServerAddress => _serverAddress;
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
            
            // Initialisation des sélecteurs de dates et d'heures
            _endDate = DateTime.Now;
            _startDate = DateTime.Now.AddHours(-1);
            
            _startDatePicker = new DatePicker
            {
                SelectedDate = _startDate,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0)
            };
            
            _endDatePicker = new DatePicker
            {
                SelectedDate = _endDate,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0)
            };

            _startTimePicker = new TimePicker
            {
                SelectedTime = new TimeSpan(_startDate.Hour, _startDate.Minute, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0),
                ClockIdentifier = "24HourClock"
            };
            
            _endTimePicker = new TimePicker
            {
                SelectedTime = new TimeSpan(_endDate.Hour, _endDate.Minute, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0),
                ClockIdentifier = "24HourClock"
            };
            
            // Correction des gestionnaires d'événements avec des lambdas pour adapter les signatures
            _startDatePicker.SelectedDateChanged += (sender, e) => OnDateTimeSelectionChanged(sender, EventArgs.Empty);
            _endDatePicker.SelectedDateChanged += (sender, e) => OnDateTimeSelectionChanged(sender, EventArgs.Empty);
            _startTimePicker.SelectedTimeChanged += (sender, e) => OnDateTimeSelectionChanged(sender, EventArgs.Empty);
            _endTimePicker.SelectedTimeChanged += (sender, e) => OnDateTimeSelectionChanged(sender, EventArgs.Empty);
            
            LoadConfiguration();
            InitializeComponent();
        }

        private void AddInitialButtons()
        {
            _buttonPanel.Children.Clear();
            // Ajoute 4 boutons initiaux
            for (int i = 0; i < _devices.Length; i++)
            {
                var button = CreateRoomButton(_devices[i].Nom, (uint)i);
                _buttonPanel.Children.Add(button);
            }
        }

        private Button CreateRoomButton(string content, uint id)
        {
            var button = new Button
            {
                Content = content,
                Padding = new Thickness(50, 20),
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = Brushes.White,
                Tag = id // Stocker l'ID de l'appareil dans le Tag
            };

            var contextMenu = new ContextMenu();
            var renameItem = new MenuItem { Header = "Renommer" };
            var deleteItem = new MenuItem { Header = "Supprimer" };

            renameItem.Click += (s, e) => button.StartRenameButton(this, _devices[(int)id]);
            deleteItem.Click += (s, e) => DeleteButton(button);

            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(deleteItem);
            button.ContextMenu = contextMenu;

            button.Click += (s, e) => LoadSensorPage(button, id);
            return button;
        }
        #endregion

        #region Date Selection Handling
        private void OnDateTimeSelectionChanged(object? sender, EventArgs e)
        {
            UpdateSelectedDateTimes();
            
            // Si on est sur une page capteur, on recharge les données
            if (_pageContent.Content is Grid grid && grid.Children.Count > 0 && grid.Children[0] is TextBlock titleBlock)
            {
                var button = _buttonPanel.Children.OfType<Button>().FirstOrDefault(b => b.Content.ToString() == titleBlock.Text);
                if (button != null && button.Tag is uint id)
                {
                    LoadSensorPage(button, id);
                }
            }
        }

        private void UpdateSelectedDateTimes()
        {
            // Convertir DateTimeOffset en DateTime et combiner avec l'heure sélectionnée
            if (_startDatePicker.SelectedDate.HasValue && _startTimePicker.SelectedTime.HasValue)
            {
                DateTime date = _startDatePicker.SelectedDate.Value.DateTime;
                TimeSpan time = _startTimePicker.SelectedTime.Value;
                _startDate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            }
            
            if (_endDatePicker.SelectedDate.HasValue && _endTimePicker.SelectedTime.HasValue)
            {
                DateTime date = _endDatePicker.SelectedDate.Value.DateTime;
                TimeSpan time = _endTimePicker.SelectedTime.Value;
                _endDate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            }
        }
        #endregion

        #region Button Management

        private void DeleteButton(Button button)
        {
            if (button.Parent is Panel panel)
            {
                panel.Children.Remove(button);
            }
            uint deviceId = (uint)button.Tag;
            this.DeleteDevice(_devices[deviceId]);
        }
        
        public async Task ReplaceTextBoxWithButton(Button button, TextBox textBox, Device device)
        {
            if (textBox.Parent is Panel panel)
            {
                var index = panel.Children.IndexOf(textBox);
                panel.Children.RemoveAt(index);
                panel.Children.Insert(index, button);
            }
            
            device.Nom = textBox.Text;
            // Attendre que la modification soit terminée avant d'actualiser la page
            await this.ModifyDevice(device);
            LoadingHomePage();
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
        private async void LoadSensorPage(Button sourceButton, uint id)
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
            var sensorpagecontent = await CreateSensorPage(pageName, id);
            _pageContent.Content = sensorpagecontent;
        }

        private async Task<Grid> CreateSensorPage(string pageName, uint id)
        {
            var content = new Grid()
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            
            content.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            content.RowDefinitions.Add(new RowDefinition(GridLength.Parse("10")));
            content.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            content.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            content.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            content.Children.Add(new TextBlock
            {
                Text = pageName,
                FontSize = 24,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            TextBlockCustom description;
            var chartsGrid = await CreateChartsGrid(id);
            if (_devices != null && _data.Length != 0)
            {
                description = new TextBlockCustom(_devices[id].Description);
                var datagrid = new Grid()
                {
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                
                
                
                //Création de la partie de 
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("10")));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("10")));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Parse("10")));
                datagrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                
                
                //Crée les valeurs instantanées
                var temp =new TextBlockCustom("Temperature: " + _data.Last().Temperature.ToString() + "°C");
                var hum = new TextBlockCustom("Humidité: " + _data.Last().Humidite.ToString() + "%");
                var tempmoy = new TextBlockCustom("Temperature moyenne: " + _data.MoyenneTemp().ToString() + "°C");
                var hummoy = new TextBlockCustom("Humidité moyenne: " + _data.MoyenneHum().ToString() + "%");
                
                Grid.SetColumn(temp, 0);
                Grid.SetColumn(tempmoy, 4);
                Grid.SetColumn(hum, 2);
                Grid.SetColumn(hummoy, 6);
                
                datagrid.Children.Add(temp);
                datagrid.Children.Add(hum);
                datagrid.Children.Add(tempmoy);
                datagrid.Children.Add(hummoy);
                
                Grid.SetRow(datagrid, 3);
                Grid.SetRow(description, 2);
                Grid.SetRow(chartsGrid, 4);
                
                content.Children.Add(description);
                content.Children.Add(datagrid);
                content.Children.Add(chartsGrid);
            }
            else
            {
                Console.WriteLine("Pas de données trouvées.");
                content = new Grid();
                content.Children.Add(new TextBlockCustom("Pas de données trouvées."));
            }

            return content;
        }
        #endregion

        #region Charts
        private async Task<Grid> CreateChartsGrid(uint id)
        {
            // Get data and wait for it to complete
            _data = await this.GetData(_client, _serverAddress, _data, id, _startDate, _endDate);
            
            var chartsGrid = new Grid
            {
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            //chartsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            CartesianChart tempChart;
            if (_devices != null)
            {
                tempChart = CreateTemperatureChart();
                //var humChart = CreateHumidityChart();
                                                     
                Grid.SetColumn(tempChart, 0);
                //Grid.SetColumn(humChart, 1);

                chartsGrid.Children.Add(tempChart);
                //chartsGrid.Children.Add(humChart);
            }
            else
            {
                chartsGrid.Children.Add(new TextBlockCustom("Il n'y a pas de données disponibles pour cet appareil durant cette période"));
            }
            
            

            return chartsGrid;
        }

        private CartesianChart CreateTemperatureChart()
        {
            return new CartesianChart
            {
                Series = _data.ToCollection(),
                XAxes = _data.Echelonne(),
                YAxes = CreateAxis(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 40))
            };
        }

        private CartesianChart CreateHumidityChart()
        {
            var series = _data.ToCollection();
            foreach (var serie in series)
            {
                if (serie is LineSeries<Data> lineSeries)
                {
                    lineSeries.GeometrySize = 10; // Taille des points en pixels
                    lineSeries.GeometryFill = new SolidColorPaint(SKColors.White);
                }
            }
            return new CartesianChart
            {
                Series = series,
                XAxes = _data.Echelonne(),
                YAxes = CreateHumidityAxis(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
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
            },
        };
        
        private Axis[] CreateAxis() => new[] 
        {
            new Axis 
            {
                Name = "Température (°C)",
                NamePaint = new SolidColorPaint(SKColors.Blue),
                TextSize = 14
            },
            new Axis
            {
                Name = "Humidité (%)",
                NamePaint = new SolidColorPaint(SKColors.Red),
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
            Icon = new WindowIcon("favicon.ico");
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
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Titre
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Label Date début
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Sélecteur Date début
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Sélecteur Heure début
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Label Date fin
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Sélecteur Date fin
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Sélecteur Heure fin
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Bouton accueil
            
            // Titre de la page
            var titleLabel = new TextBlock
            {
                Text = "DomoTiX",
                Foreground = Brushes.White,
                FontSize = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0)
            };
            
            // Label pour la date de début
            var startDateLabel = new TextBlock
            {
                Text = "Du :",
                Foreground = Brushes.White,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0)
            };
            
            // Label pour la date de fin
            var endDateLabel = new TextBlock
            {
                Text = "Au :",
                Foreground = Brushes.White,
                FontSize = 16,
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
            //Action lors du click sur le titre On charge la page d'accueil
            homeButton.Click += (s, e) => LoadingHomePage();
            
            //Positionnement des éléments
            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(startDateLabel, 1);
            Grid.SetColumn(_startDatePicker, 2);
            Grid.SetColumn(_startTimePicker, 3);
            Grid.SetColumn(endDateLabel, 4);
            Grid.SetColumn(_endDatePicker, 5);
            Grid.SetColumn(_endTimePicker, 6);
            Grid.SetColumn(homeButton, 7);
            
            //Ajout au parent
            headerPanel.Children.Add(titleLabel);
            headerPanel.Children.Add(startDateLabel);
            headerPanel.Children.Add(_startDatePicker);
            headerPanel.Children.Add(_startTimePicker);
            headerPanel.Children.Add(endDateLabel);
            headerPanel.Children.Add(_endDatePicker);
            headerPanel.Children.Add(_endTimePicker);
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
            
            //Ajout du bouton contenu dans le stackpanel du conteneur de bouton
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
        //Fonction de chargement de la configuration contenue dans appsettings.json
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

