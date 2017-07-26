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

        public static async Task<JObject> callApi(string uri)
        {
            HttpClient httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(uri);
            if (json[json.Length - 1] == ']')
                json = json.Remove(json.Length - 1, 1);
            if (json[0] == '[')
                json = json.Remove(0, 1);

            //string trimmedjson = json.Substring(21,14);
            //Console.WriteLine(trimmedjson);
            var jsonObj = (JObject)JsonConvert.DeserializeObject(json);
            Console.WriteLine(jsonObj["Key"]);
            return (jsonObj);

        }


        public async Task StartAsync(IDialogContext context)

        {

            await context.PostAsync($" Getting weather data for {this.city} ...");
            string uri1 = string.Format("http://dataservice.accuweather.com/locations/v1/cities/search?q={0}&apikey={1}", this.city, apiKey);
            var result = await (callApi(uri1));
            var locationKey = result["Key"];
            string uri2 = string.Format("http://dataservice.accuweather.com/forecasts/v1/daily/1day/{0}?apikey={1}", locationKey, apiKey);
            result = await (callApi(uri2));
            var weather = result["Headline"]["Text"];
            string weatherResult = "Weather in " + this.city + " is : " + weather;

            //($"In Main {locationKey} \n Weather in {city}: {weather}");
            

            context.Done(weatherResult);


        }




    }

}