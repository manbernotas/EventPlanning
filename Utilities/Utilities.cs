using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace EventPlanning.Utilities
{
    public static class Utilities
    {
        public static object GetAsync(object serObj, string url, object deserObj)
        {
            var client = new HttpClient();
            var serializer = new DataContractJsonSerializer(serObj.GetType());
            var jsonInString = JsonConvert.SerializeObject(serObj);
            var response = client.GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject(responseContent, deserObj.GetType());

            return result;
        }
    }
}
