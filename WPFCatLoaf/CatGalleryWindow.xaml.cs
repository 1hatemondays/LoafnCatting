using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFCatLoaf.Converters;

namespace WPFCatLoaf
{
    public partial class CatGalleryWindow : Window
    {
        private ICatService _catService;
        private readonly int _tableId;
        private readonly User _loggedInUser;
        private List<Cat> _allCats;
        private readonly ImagePathConverter _imagePathConverter = new ImagePathConverter();

        public CatGalleryWindow(int tableId)
        {
            InitializeComponent();
            _tableId = tableId;
            _loggedInUser = null;
            InitializeServices();
            LoadCats();
        }

        public CatGalleryWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            _tableId = 0;
            InitializeServices();
            LoadCats();
        }

        private void InitializeServices()
        {
            var context = new LoafNcattingDbContext();
            _catService = new CatService(new CatRepository(context));
        }

        private void LoadCats()
        {
            try
            {
                _allCats = _catService.GetAllCats();
                if (_allCats?.Any() == true)
                {
                    DisplayCats();
                }
                else
                {
                    var messageTextBlock = new TextBlock
                    {
                        Text = "No cats available at the moment. Please check back later!",
                        FontSize = 18,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    CatsPanel.Children.Add(messageTextBlock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cats: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayCats()
        {
            CatsPanel.Children.Clear();
            var catsGrid = new UniformGrid
            {
                Columns = 4,
                Margin = new Thickness(20)
            };

            foreach (var cat in _allCats)
            {
                var catCard = CreateCatCard(cat);
                catsGrid.Children.Add(catCard);
            }
            CatsPanel.Children.Add(catsGrid);
        }

        private Border CreateCatCard(Cat cat)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(15),
                Margin = new Thickness(12),
                Padding = new Thickness(20),
                Width = 320,
                Height = 540,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 5,
                    BlurRadius = 15,
                    Opacity = 0.3
                }
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(200) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Image section
            var imageContainer = new Border
            {
                CornerRadius = new CornerRadius(12),
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                Height = 180,
                Margin = new Thickness(0, 0, 0, 18)
            };

            var catImage = new Image
            {
                Stretch = Stretch.UniformToFill,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            if (!string.IsNullOrEmpty(cat.Picture))
            {
                try
                {
                    catImage.Source = _imagePathConverter.Convert(cat.Picture, typeof(ImageSource), null, CultureInfo.CurrentCulture) as ImageSource;
                }
                catch
                {
                    catImage.Source = null;
                }
            }

            imageContainer.Child = catImage;
            Grid.SetRow(imageContainer, 0);
            mainGrid.Children.Add(imageContainer);

            // Header section
            var headerPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 15) };
            
            var nameGrid = new Grid();
            nameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            nameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var nameTextBlock = new TextBlock
            {
                Text = cat.Name,
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 53))
            };
            Grid.SetColumn(nameTextBlock, 0);
            nameGrid.Children.Add(nameTextBlock);

            var genderTextBlock = new TextBlock
            {
                Text = cat.Gender?.GenderName ?? "Unknown",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Grid.SetColumn(genderTextBlock, 1);
            nameGrid.Children.Add(genderTextBlock);
            
            headerPanel.Children.Add(nameGrid);

            var infoPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 0)
            };

            if (cat.Age.HasValue)
            {
                var ageTextBlock = new TextBlock
                {
                    Text = $"Age: {cat.Age} years old",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 53)),
                    Margin = new Thickness(0, 0, 20, 0)
                };
                infoPanel.Children.Add(ageTextBlock);
            }

            if (!string.IsNullOrEmpty(cat.Breed))
            {
                var breedTextBlock = new TextBlock
                {
                    Text = cat.Breed,
                    FontSize = 14,
                    FontWeight = FontWeights.Medium,
                    Foreground = Brushes.Gray
                };
                infoPanel.Children.Add(breedTextBlock);
            }

            headerPanel.Children.Add(infoPanel);
            Grid.SetRow(headerPanel, 1);
            mainGrid.Children.Add(headerPanel);

            // Content section
            var contentPanel = new StackPanel();
            
            var ratingTitleTextBlock = new TextBlock
            {
                Text = "Personality Ratings",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 53)),
                Margin = new Thickness(0, 0, 0, 12)
            };
            contentPanel.Children.Add(ratingTitleTextBlock);

            contentPanel.Children.Add(CreateRatingRow("Friendliness", cat.FriendlinessRating ?? 0));
            contentPanel.Children.Add(CreateRatingRow("Cuteness", cat.CutenessRating ?? 0));
            contentPanel.Children.Add(CreateRatingRow("Playfulness", cat.PlayfulnessRating ?? 0));

            if (!string.IsNullOrEmpty(cat.Description))
            {
                var descriptionTextBlock = new TextBlock
                {
                    Text = cat.Description,
                    FontSize = 14,
                    Foreground = Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 18, 0, 0),
                    LineHeight = 18
                };
                contentPanel.Children.Add(descriptionTextBlock);
            }

            Grid.SetRow(contentPanel, 2);
            mainGrid.Children.Add(contentPanel);
            card.Child = mainGrid;
            
            return card;
        }

        private StackPanel CreateRatingRow(string label, int rating)
        {
            var ratingPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 4, 0, 4)
            };

            var labelTextBlock = new TextBlock
            {
                Text = label,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Width = 110,
                Foreground = Brushes.Black
            };
            ratingPanel.Children.Add(labelTextBlock);

            var starsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(12, 0, 0, 0)
            };

            for (int i = 1; i <= 5; i++)
            {
                var starContainer = new Viewbox
                {
                    Width = 18,
                    Height = 18,
                    Margin = new Thickness(1, 0, 1, 0)
                };

                var star = new Polygon
                {
                    Points = new PointCollection(new[]
                    {
                        new Point(10, 0), new Point(12.24, 6.91), new Point(20, 6.91),
                        new Point(14.12, 11.18), new Point(16.36, 18.09), new Point(10, 13.82),
                        new Point(3.64, 18.09), new Point(5.88, 11.18), new Point(0, 6.91), new Point(7.76, 6.91)
                    }),
                    Fill = i <= rating ? new SolidColorBrush(Color.FromRgb(255, 193, 7)) : Brushes.LightGray,
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 0.5
                };

                starContainer.Child = star;
                starsPanel.Children.Add(starContainer);
            }

            ratingPanel.Children.Add(starsPanel);
            return ratingPanel;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser != null)
            {
                var tableOrderWindow = new TableOrderWindow(_loggedInUser);
                tableOrderWindow.Show();
            }
            else
            {
                var tableOrderWindow = new TableOrderWindow(_tableId);
                tableOrderWindow.Show();
            }
            this.Close();
        }
    }
}