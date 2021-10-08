using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SauktiniaiUI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SauktiniaiUI
{
    public static class MilitantsProcessor
    {
        public static async Task<List<EnlistedPerson>> GetMilitantsPageDataAsync(City city, string range)
        {
            var uri = new Uri("https://sauktiniai.karys.lt/list.php");
            var uriWithParameters = QueryHelpers.AddQueryString(uri.OriginalString, "region", $"{(int)city}");
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriWithParameters))
            {
                requestMessage.Headers.Referrer = new Uri("https://sauktiniai.karys.lt/");
                requestMessage.Headers.TryAddWithoutValidation("range", range);

                var response = await ApiHelper.ApiClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var enlistedPeople = JsonConvert.DeserializeObject<List<EnlistedPerson>>(jsonResponse);
                    return enlistedPeople;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
