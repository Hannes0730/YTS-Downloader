using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YTS_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private int pageNumber = 1;
        private int maxPageNumber;
        private string chosenGenre = null;
        private string chosenSort;

        public MainWindow()
        {
            InitializeComponent();
            APIController.APIInitializer();
            this.Title = $"YTS Downloader ({Assembly.GetExecutingAssembly().GetName().Version})";
        }

        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(movie_query.Text))
                {
                    DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
                }
                else
                {
                    DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
                }

                pageNumber = 1;
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            if (string.IsNullOrEmpty(movie_query.Text))
            {
                DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
            }
            else
            {
                DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
            }
        }

        public void GenreListDisplay()
        {
            string[] genres =
            {
                " ", "Action", "Adventure","Animation","Biography","Comedy","Crime","Documentary","Drama","Family","Fantasy","Film Noir","History","Horror","Music","Musical","Mystery","Romance","Sci-Fi","Short Film","Sport","Thriller","War","Western"
            };

            foreach(var genre in genres)
            {
                GenreList.Items.Add(genre);
            }
            
        }

        private void GenreList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            var selectedItem = GenreList.SelectedItem;
            chosenGenre = (string)selectedItem;
        }

        public void SortListDisplay()
        {

            //string[] sortListName =
            //{
            //    " ", "Latest", "Oldest","Featured","Seeds","Peers","Year","IMDB Rating","Date Added", "Download Count"
            //};

            //string[] sortListValue =
            //{
            //    " ", "Latest", "Oldest","Featured","Seeds","Peers","Year","IMDB Rating","Date Added", "Download Count"
            //};

            var sortListName = new Dictionary<string, string>
            {
                {"seeds", "Featured"},
                {"year", "Year"},
                {"rating", "IMDB Rating"},
                {"date_added", "Date Added"},
                {"download_count", "Download Count"},
            };

            SortList.SelectedValuePath = "Key";
            SortList.DisplayMemberPath = "Value";

            foreach (var sort in sortListName)
            {
                SortList.Items.Add(new KeyValuePair<string, string>(sort.Key, sort.Value));
            }
        }

        private void SortList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = SortList.SelectedValue;
            chosenSort = selectedItem.ToString();
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber > 1)
            {
                PreviousBtn.IsEnabled = true;
                pageNumber--;
                if (pageNumber == 1)
                {
                    PreviousBtn.IsEnabled = false;
                }
            }
            NextBtn.IsEnabled = true;
            DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (maxPageNumber != pageNumber)
            {
                pageNumber++;
                PreviousBtn.IsEnabled = true;
                DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
            }
            
        }


        private void YTS_MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GenreListDisplay();
            SortListDisplay();
            DisplayInfo(movie_query.Text, pageNumber, chosenGenre, chosenSort);
        }

        public async void DisplayInfo(string query = null, int pageNumber = 1, string genre = null, string sort = null)
        {
            var movieInfo = await YTSAPIClient.GetTaskAsync(query, pageNumber, genre, sort);
            if (movieInfo != null)
            {
                movieList.Children.Clear();
                if (movieInfo.Data.Movies == null)
                {
                    maxPageNumber = pageNumber;
                    NextBtn.IsEnabled = false;

                    StackPanel noMovie = new StackPanel();
                    TextBlock NoData = new TextBlock();
                    NoData.Text = "No More Movies to Show...";
                    NoData.Foreground = Brushes.White;
                    NoData.FontSize = 48;
                    NoData.FontWeight = FontWeights.Bold;
                    NoData.HorizontalAlignment = HorizontalAlignment.Center;
                    movieList.Columns = 1;

                    movieList.Children.Add(noMovie);
                    noMovie.Children.Add(NoData);

                }
                else
                {
                    
                    NextBtn.IsEnabled = true;
                    foreach (var movie in movieInfo.Data.Movies)
                    {
                        
                        Image movieImage = new Image();
                        TextBlock movieTitle = new TextBlock();
                        BitmapImage src = new BitmapImage();
                        StackPanel singleMovie = new StackPanel();

                        var response = await APIController.client.GetAsync(movie.MediumCoverImage);

                        movieList.Columns = 4;
                        src.BeginInit();
                        if (response.IsSuccessStatusCode)
                        {

                            src.UriSource = new Uri(movie.MediumCoverImage, UriKind.Absolute);



                            movieImage.Source = src;
                            movieImage.Stretch = Stretch.None;
                            movieImage.HorizontalAlignment = HorizontalAlignment.Center;
                            movieImage.MouseLeftButtonUp += ((sender, e) =>
                            {
                                // Movie Details
                                MovieInfo movieInfoWindow = new MovieInfo();
                                TextBlock movieInfoTitle = new TextBlock();
                                TextBlock movieInfoYear = new TextBlock();
                                TextBlock movieInfoGenre = new TextBlock();
                                TextBlock movieInfoRating = new TextBlock();
                                TextBlock movieInfoSynopsis = new TextBlock();
                                Button trailer = new Button();
                                var trailerbackgroundColor = new BrushConverter();

                                movie.Synopsis = movie.Synopsis.Replace("&#39;", "'").Replace("&quot;", "\"");

                                movieInfoWindow.Title = movie.Title;
                                movieInfoWindow.MovieInfoImage.Source = src;
                                movieInfoWindow.MovieInfoImage.Stretch = Stretch.None;
                                movieInfoWindow.MovieInfoImage.HorizontalAlignment = HorizontalAlignment.Center;

                                movieInfoTitle.Text = movie.Title;
                                movieInfoTitle.FontSize = 24;
                                movieInfoTitle.TextWrapping = TextWrapping.Wrap;
                                movieInfoTitle.Foreground = Brushes.White;

                                movieInfoYear.Text = $"({movie.Year.ToString()})";
                                movieInfoYear.FontSize = 14;
                                movieInfoYear.TextWrapping = TextWrapping.Wrap;
                                movieInfoYear.Foreground = Brushes.White;

                                movieInfoRating.Text = $"(Rating: {movie.Rating.ToString()}/10)";
                                movieInfoRating.FontSize = 14;
                                movieInfoRating.TextWrapping = TextWrapping.Wrap;
                                movieInfoRating.Margin = new Thickness(0, 10, 0, 0);
                                movieInfoRating.Foreground = Brushes.White;

                                string trailerUrl = $@"https://www.youtube.com/watch?v={movie.YtTrailerCode}";
                                var trailerBuilder = new UriBuilder(trailerUrl);
                                trailer.Content = "Watch Trailer";
                                trailer.FontSize = 16;
                                trailer.Background = (Brush)trailerbackgroundColor.ConvertFrom("#FF6AC045");
                                trailer.Margin = new Thickness(50, 25, 50, 50);
                                trailer.Width = 100;
                                //trailer.HorizontalContentAlignment = HorizontalAlignment.Left;
                                //trailer.VerticalAlignment = VerticalAlignment.Top;
                                trailer.Cursor = Cursors.Hand;
                                trailer.Click += ((sender, e) => {
                                    ProcessStartInfo trailer_psi = new ProcessStartInfo
                                    {
                                        FileName = "cmd",
                                        UseShellExecute = false,
                                        CreateNoWindow = true,
                                        Arguments = $"/c start {trailerBuilder.ToString()}"
                                    };
                                    Process.Start(trailer_psi);
                                });
                        

                                foreach (var genre in movie.Genres)
                                {
                                    movieInfoGenre.Text += $"{genre}, ";
                                }

                                movieInfoGenre.Text = $"Genre: {movieInfoGenre.Text.Remove(movieInfoGenre.Text.Length - 2, 1)}";
                                movieInfoGenre.FontSize = 14;
                                movieInfoGenre.TextWrapping = TextWrapping.Wrap;
                                movieInfoGenre.Margin = new Thickness(0, 10, 0, 0);
                                movieInfoGenre.Foreground = Brushes.White;

                                movieInfoSynopsis.Text = $"Plot:\n{movie.Synopsis}";
                                movieInfoSynopsis.FontSize = 16;
                                movieInfoSynopsis.TextWrapping = TextWrapping.Wrap;
                                movieInfoSynopsis.Foreground = Brushes.White;
                                movieInfoSynopsis.TextAlignment = TextAlignment.Justify;
                                movieInfoSynopsis.Margin = new Thickness(0, 20, 0, 50);

                                movieInfoWindow.MovieInfoDetails.Children.Add(movieInfoTitle);
                                movieInfoWindow.MovieInfoDetails.Children.Add(movieInfoYear);
                                movieInfoWindow.MovieInfoDetails.Children.Add(movieInfoRating);
                                movieInfoWindow.MovieInfoDetails.Children.Add(movieInfoGenre);
                                movieInfoWindow.MovieInfoDetails.Children.Add(movieInfoSynopsis);
                                movieInfoWindow.MovieInfoDetails.Children.Add(trailer);


                                foreach (var torrent in movie.Torrents)
                                {
                                    //Movie Torrent
                                    Button DownloadButton = new Button();
                                    TextBlock TorrentSize = new TextBlock();
                                    TextBlock DownloadText = new TextBlock();
                                    StackPanel DownloadPanel = new StackPanel();

                                    var backgroundColor = new BrushConverter();

                                    DownloadPanel.Orientation = Orientation.Horizontal;

                                    DownloadText.Text = "Download: ";
                                    DownloadText.FontSize = 16;
                                    DownloadText.Foreground = Brushes.White;
                                    DownloadText.VerticalAlignment = VerticalAlignment.Center;
                                    DownloadText.Margin = new Thickness(0, 25, 0, 0);

                                    DownloadButton.Content = $"{torrent.Quality}";
                                    DownloadButton.FontSize = 16;
                                    DownloadButton.Background = (Brush)backgroundColor.ConvertFrom("#FF6AC045");
                                    DownloadButton.Margin = new Thickness(50,25,50,0);
                                    DownloadButton.Width = 100;
                                    DownloadButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    DownloadButton.VerticalAlignment = VerticalAlignment.Top;
                                    DownloadButton.Cursor = Cursors.Hand;
                                    DownloadButton.Click += ((sender, e) =>
                                    {
                                        var torrentBuilder = new UriBuilder(torrent.Url);
                                        //string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                        //string path = System.IO.Path.GetTempPath();
                                        //var torrentDownloadPath = path + $@"\TorrentFiles";
                                        MessageBoxResult result = MessageBox.Show($"Do you want to Download {movie.Title} ({torrent.Quality})?", "Download File", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                        

                                        try
                                        {
                                            //if (!Directory.Exists(torrentDownloadPath))
                                            //{
                                            //    DirectoryInfo di = Directory.CreateDirectory(torrentDownloadPath);
                                            //}
                                            if (result == MessageBoxResult.Yes)
                                            {

                                                ProcessStartInfo trailer_psi = new ProcessStartInfo
                                                {
                                                    FileName = "cmd",
                                                    UseShellExecute = false,
                                                    CreateNoWindow = true,
                                                    Arguments = $"/c start iexplore {torrentBuilder.ToString()}"
                                                };
                                                Process.Start(trailer_psi);
                                                //HttpDownloader downloader = new HttpDownloader(torrent.Url,torrentDownloadPath);
                                                //downloader.Start();
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    });

                                    TorrentSize.Text = $" - {torrent.Size} ({torrent.Type})";
                                    TorrentSize.FontSize = 16;
                                    TorrentSize.Foreground = Brushes.White;
                                    TorrentSize.VerticalAlignment = VerticalAlignment.Center;
                                    TorrentSize.Margin = new Thickness(0, 25, 0, 0);

                                    movieInfoWindow.MovieInfoDetails.Children.Add(DownloadPanel);
                                    DownloadPanel.Children.Add(DownloadText);
                                    DownloadPanel.Children.Add(DownloadButton);
                                    DownloadPanel.Children.Add(TorrentSize);
                                    

                                }

                                movieInfoWindow.ShowDialog();
                            });

                            movieTitle.Text = movie.Title;
                            movieTitle.Foreground = Brushes.White;

                            movieTitle.TextAlignment = TextAlignment.Center;
                            movieTitle.FontSize = 16;
                            movieTitle.TextWrapping = TextWrapping.Wrap;


                            singleMovie.VerticalAlignment = VerticalAlignment.Center;
                            singleMovie.Cursor = Cursors.Hand;

                            movieList.Children.Add(singleMovie);
                            singleMovie.Children.Add(movieImage);
                            singleMovie.Children.Add(movieTitle);

                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();
                        }
                        

                    }
                }
                
            }

        }
    }
}
