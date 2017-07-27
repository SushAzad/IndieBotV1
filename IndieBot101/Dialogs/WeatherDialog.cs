namespace IndieBot101.Dialogs

{

    using Microsoft.Bot.Builder.Dialogs;

    using System;

    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    using Microsoft.Bot.Connector;



    [Serializable]

    public class WeatherDialog : IDialog<string>

    {

        private string city;
        private const string apiKey = "WnD203OR41wDy4360INAjwVC9WpCjl5Q";
        public WeatherDialog(string city)

        {

            this.city = city;

        }
        //callApi prepends the received json with a data object to enable serialisation when finding location key.
        public static async Task<JObject> callApi(string uri)
        {
            HttpClient httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(uri);
            var newjson = "{'data':" + json + "}";
            var jsonObj = (JObject)JsonConvert.DeserializeObject(newjson);
            return (jsonObj);

        }
        //callApi2 - the api call with location key already present returns correctly formatted json object - can directly deserialize.
        public static async Task<JObject> callApi2(string uri)
        {
            HttpClient httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(uri);
            var jsonObj = (JObject)JsonConvert.DeserializeObject(json);
            return (jsonObj);

        }

        public async Task StartAsync(IDialogContext context)

        {

            await context.PostAsync($" Getting weather data for {this.city} ...");
            string uri1 = string.Format("http://dataservice.accuweather.com/locations/v1/cities/search?q={0}&apikey={1}", this.city, apiKey);
            var result = await (callApi(uri1));
            var locationKey = result["data"][0]["Key"].ToString();
            string uri2 = string.Format("http://dataservice.accuweather.com/forecasts/v1/daily/5day/{0}?apikey={1}", locationKey, apiKey);
            result = await (callApi2(uri2));
            var weather = result["Headline"]["Text"];
            string weatherResult = "Weather in " + this.city + " is : " + weather;
            context.Done(weatherResult);


        }




    }

}