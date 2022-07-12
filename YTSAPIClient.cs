using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static YTS_Downloader.MovieModel;

namespace YTS_Downloader
{
    internal class YTSAPIClient
    {
        public static async Task<Root> GetTaskAsync(string query, int page, string genre, string sort)
        {
            Debug.WriteLine(sort);
            if (string.IsNullOrEmpty(query))
            {
                using (HttpResponseMessage response = await APIController.client.GetAsync($"list_movies.json?sort_by={sort}&page={page}&genre={genre}"))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        var movie_info = await response.Content.ReadAsStringAsync();
                        Root movie = JsonConvert.DeserializeObject<Root>(movie_info);

                        return movie;

                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
            else
            {
                using (HttpResponseMessage response = await APIController.client.GetAsync($"list_movies.json?query_term={query}&page={page}&genre={genre}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var movie_info = await response.Content.ReadAsStringAsync();
                        Root movie = JsonConvert.DeserializeObject<Root>(movie_info);

                        return movie;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
        }

        //private async Task<MovieModel.Root> LoadData(string query = null, int PageNumber = 1)
        //{
        //    MovieModel.Root movie = new MovieModel.Root();
        //    movie = await GetTaskAsync(query, PageNumber);

        //    try
        //    {
        //        if (movie != null)
        //        {
        //            if (movie.Data.Movies == null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                foreach (var data in movie.Data.Movies)
        //                {
        //                    var result = await APIController.client.GetAsync(data.MediumCoverImage);

        //                    if (result.IsSuccessStatusCode)
        //                    {
                                
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    return ;


        //}
    }
}
