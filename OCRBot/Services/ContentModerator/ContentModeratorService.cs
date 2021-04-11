using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCRBot.Services.ContentModerator
{
    public class ContentModeratorService: IContentModeratorService
    {
        public async Task<string> processImage(string imageUrl)
        {
            var client = new RestClient("https://westus.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessImage/OCR?language=spa");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Ocp-Apim-Subscription-Key", "YOUR_SUBSCRIPTION_KEY");
            var model = new ModelRequest();
            model.DataRepresentation = "URL";
            model.Value = imageUrl;

            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            var result = JsonConvert.DeserializeObject<ModelResult>(response.Content);
            return result.Text;
        }
    }

    public class ModelResult
    {
        public string Text { get; set; }
    }
    public class ModelRequest
    {
        public string DataRepresentation { get; set; }
        public string Value { get; set; }
    }
}
