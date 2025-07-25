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
        private const string PlaceholderImagePath = "Images/Gun.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string relativePath = value?.ToString();

                // If path is null or empty, return the placeholder
                if (string.IsNullOrEmpty(relativePath))
                {
                    return LoadImage(PlaceholderImagePath);
                }

                // Try to load the image from the relative path
                var image = LoadImage(relativePath);
                
                // If successful, return the image
                if (image != null)
                {
                    return image;
                }
                
                // If image couldn't be loaded, return placeholder
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
                // First try as a project resource (for placeholder image that might be embedded)
                try
                {
                    Uri resourceUri = new Uri($"pack://application:,,,/{relativePath}", UriKind.Absolute);
                    BitmapImage resourceImage = new BitmapImage();
                    resourceImage.BeginInit();
                    resourceImage.UriSource = resourceUri;
                    resourceImage.CacheOption = BitmapCacheOption.OnLoad;
                    resourceImage.EndInit();
                    if (resourceImage.CanFreeze) resourceImage.Freeze();
                    return resourceImage;
                }
                catch
                {
                    // If that fails, try as a file relative to the executable
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string fullPath = Path.Combine(basePath, relativePath);
                    
                    // Check if the file exists
                    if (File.Exists(fullPath))
                    {
                        BitmapImage fileImage = new BitmapImage();
                        fileImage.BeginInit();
                        fileImage.CacheOption = BitmapCacheOption.OnLoad;
                        fileImage.UriSource = new Uri(fullPath);
                        fileImage.EndInit();
                        if (fileImage.CanFreeze) fileImage.Freeze();
                        return fileImage;
                    }
                    
                    // If file doesn't exist, try looking in the project directory
                    // This is useful during development when running from the IDE
                    string projectDirectory = GetProjectDirectory();
                    if (!string.IsNullOrEmpty(projectDirectory))
                    {
                        string projectPath = Path.Combine(projectDirectory, relativePath);
                        if (File.Exists(projectPath))
                        {
                            BitmapImage projectImage = new BitmapImage();
                            projectImage.BeginInit();
                            projectImage.CacheOption = BitmapCacheOption.OnLoad;
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
                    // Try to locate the project directory by looking for common project files
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                
                // Navigate up directories looking for WPFCatLoaf folder
                while (!string.IsNullOrEmpty(currentDir))
                {
                    // Check if this might be the WPFCatLoaf project directory
                    if (Directory.Exists(Path.Combine(currentDir, "Images")) && 
                        Directory.Exists(Path.Combine(currentDir, "Images", "Products")))
                    {
                        return currentDir;
                    }
                    
                    // Move up one directory
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