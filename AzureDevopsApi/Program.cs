// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AzureDevopsApi
{
    static class Azure
    {
        public const string BASE= "https://dev.azure.com";
        public const string PAT = "ggbdmg74fkizi5q6e2xlwyhkvq6uxctvd33kv6nchgx6m42c7l7a";
        public const string ORG = "ariqt";
        public const string PROJECT = "interns";
        public const string API= "api-version=7.0";
        public const string WIT_TYPE = "$Issue";
        //https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/${type}?api-version=7.0
    }
    class Program
    {
        public static async Task<string>CreateWIT(HttpClient client,string uri,HttpContent content)
        {
            try
            {
                using (HttpResponseMessage response = await client.PatchAsync(uri, content))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return String.Empty;
            }
        }
        static void Main(string[]args)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", Azure.PAT))));
            string uri=string.Join("?",string.Join("/",Azure.BASE,Azure.ORG,Azure.PROJECT,"_apis/wit/workitems",Azure.WIT_TYPE),Azure.API);
            string json = "[{ \"op\": \"add\", \"path\": \"/fields/System.Title\", \"from\": null, \"value\": \"REST API Demo issue\"}]";
            HttpContent content= new StringContent(json,Encoding.UTF8,"application/json-patch+json");
            string result=CreateWIT(client, uri, content).Result;
            if(!string.IsNullOrEmpty(result))
            {
                dynamic wit=JsonConvert.DeserializeObject<object>(result);
                Console.WriteLine(JsonConvert.SerializeObject(wit,Formatting.Indented));
            }
            Console.ReadLine();
            client.Dispose();
        }

    }
}
