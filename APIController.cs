using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace YTS_Downloader
{
    public static class APIController
    {
        public static HttpClient client { get; set; }

        public static void APIInitializer()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://yts.mx/api/v2/");
            //clear default headers
            client.DefaultRequestHeaders.Accept.Clear();

            //give data with type json
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }



    }
}
