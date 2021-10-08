using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SauktiniaiUI
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeApiClient()
        {
            ApiClient = new HttpClient();
        }
    }
}
