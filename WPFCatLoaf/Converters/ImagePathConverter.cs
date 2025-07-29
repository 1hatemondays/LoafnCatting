using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPFCatLoaf.Converters
{
    //Fix
    /// <summary>
    /// Converts relative image paths (e.g., "Images/Products/Shiori.jpg") to BitmapImage objects.
    /// Handles images stored as content files in the project directory.
    /// </summary>
    public class ImagePathConverter : IValueConverter
    {
        // Define the path to your placeholder image
        private const string PlaceholderImagePath = "Images/placeholder.jpg";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string relativePath = value?.ToString();

                if (string.IsNullOrEmpty(relativePath))
                {
                    return LoadImage(PlaceholderImagePath);
                }

                var image = LoadImage(relativePath);
                
                if (image != null)
                {
                    return image;
                }
                
                System.Diagnostics.Debug.WriteLine($"Could not load image: {relativePath}. Using placeholder.");
                return LoadImage(PlaceholderImagePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ImagePathConverter: {ex.Message}");
                return LoadImage(PlaceholderImagePath);
            }
        }

        private BitmapImage LoadImage(string relativePath)
        {
            try
            {
                // First try as a file relative to the executable
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(basePath, relativePath);
                
                // Check if the file exists
                if (File.Exists(fullPath))
                {
                    BitmapImage fileImage = new BitmapImage();
                    fileImage.BeginInit();
                    fileImage.CacheOption = BitmapCacheOption.OnLoad;
                    fileImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache; 
                    fileImage.UriSource = new Uri(fullPath);
                    fileImage.EndInit();
                    if (fileImage.CanFreeze) fileImage.Freeze();
                    return fileImage;
                }
                
                // Then try as a project resource 
                try
                {
                    Uri resourceUri = new Uri($"pack://application:,,,/{relativePath}", UriKind.Absolute);
                    BitmapImage resourceImage = new BitmapImage();
                    resourceImage.BeginInit();
                    resourceImage.UriSource = resourceUri;
                    resourceImage.CacheOption = BitmapCacheOption.OnLoad;
                    resourceImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache; 
                    resourceImage.EndInit();
                    if (resourceImage.CanFreeze) resourceImage.Freeze();
                    return resourceImage;
                }
                catch
                {
                    // Try looking in the project directory
                    string projectDirectory = GetProjectDirectory();
                    if (!string.IsNullOrEmpty(projectDirectory))
                    {
                        string projectPath = Path.Combine(projectDirectory, relativePath);
                        if (File.Exists(projectPath))
                        {
                            BitmapImage projectImage = new BitmapImage();
                            projectImage.BeginInit();
                            projectImage.CacheOption = BitmapCacheOption.OnLoad;
                            projectImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            projectImage.UriSource = new Uri(projectPath);
                            projectImage.EndInit();
                            if (projectImage.CanFreeze) projectImage.Freeze();
                            return projectImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image '{relativePath}': {ex.Message}");
            }
            
            return null;
        }

        private string GetProjectDirectory()
        {
            try
            {
                    // Locate the project directory by looking for common project files
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                
                // Navigate up directories looking for WPFCatLoaf folder
                while (!string.IsNullOrEmpty(currentDir))
                {
                    if (Directory.Exists(Path.Combine(currentDir, "Images")) && 
                        Directory.Exists(Path.Combine(currentDir, "Images", "Products")))
                    {
                        return currentDir;
                    }
                    
                    DirectoryInfo parent = Directory.GetParent(currentDir);
                    if (parent == null) break;
                    currentDir = parent.FullName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding project directory: {ex.Message}");
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}