// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GetWorkItems
{
    //https://dev.azure.com/{organization}/{project}/_apis/wit/workitems?ids={ids}&api-version=7.0
    class Program
    {
        public class WorkItemsList
        {
            public Workitem[] workItems { get; set; }
        }
        public class Workitem
        {
            public object rel { get; set; }
            public object source { get; set; }
            public Target target { get; set; }
        }
        public class Target
        {
            public int id { get; set; }
        }
        static class Azure
        {
            public const string BASE = "https://dev.azure.com";
            public const string PAT = "3ljqhiwlkowddiqscp2xd6nd3rwfs2g6vs5yfaeg2kcdzsgi264a";
            public const string ORG = "ariqt";
            public const string API = "api-version=7.0";
            public const string PROJECT = "interns";
            public const string PROJECT_TEAM = "interns";
           // public const string BACKLOG_ID = "Microsoft.RequirementCategory";
        }
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", Azure.PAT))));
            string BackLogWorkItemsURI = string.Join("?", string.Join("/", Azure.ORG, Azure.PROJECT, Azure.PROJECT_TEAM, "_apis/work/backlogs"), Azure.API);
            Console.WriteLine(BackLogWorkItemsURI);
            string result = SendRequest(client, BackLogWorkItemsURI).Result;
            WorkItemsList workitemlist = JsonConvert.DeserializeObject<WorkItemsList>(result);
            List<int> ids = new List<int>();
            foreach(Workitem wit in workitemlist.workItems)
            {
                ids.Add(wit.target.id);
            }
            string workItemListURI = string.Join("/", Azure.BASE, Azure.ORG, Azure.PROJECT, "_apis/wit/workitems?ids=");
            Console.WriteLine(workItemListURI);
            result = SendRequest(client, workItemListURI).Result;
            dynamic witem=JsonConvert.DeserializeObject<object>(result);
            Console.WriteLine(JsonConvert.SerializeObject(witem,Formatting.Indented));
            Console.ReadLine();
            client.Dispose();
        }
        public static async Task<string>SendRequest(HttpClient client,string uri)
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                   response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return String.Empty;
            }

        }
    }
}
    