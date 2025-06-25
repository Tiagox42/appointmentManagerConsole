// See https://aka.ms/new-console-template for more information
using DotNetEnv;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

var currentDir = Directory.GetCurrentDirectory();
var root = Directory.GetParent(currentDir)?.Parent?.Parent?.FullName;
var dotenv = Path.Combine(root!, ".env");
Env.Load(dotenv);

////Env VARIAVEL

var urlAmbiente = Environment.GetEnvironmentVariable("urlAmbienteDevOps");
var pat = Environment.GetEnvironmentVariable("PAT");

//var credentials = new VssClientCredentials();
//credentials.PromptType = CredentialPromptType.PromptIfNeeded;

var credentials = new VssBasicCredential(string.Empty, pat!);
var connection = new VssConnection(new Uri(urlAmbiente!), credentials);
await connection.ConnectAsync();

var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

var wiql = new Wiql
{
    Query = "select * from WorkItems where [System.Id] in (33257) order by [System.ChangedDate] desc"
};

var result = await witClient.QueryByWiqlAsync(wiql);

if (result.WorkItems.Any())
{
    var ids = result.WorkItems.Select(wi => wi.Id).ToArray();
    var workItems = await witClient.GetWorkItemsAsync(ids);

    foreach (var item in workItems)
    {
        Console.WriteLine($"--- Work Item #{item.Id} ---");

        foreach (var field in item.Fields)
        {
            Console.WriteLine($"{field.Key}: {field.Value}");
        }

        Console.WriteLine(); // linha em branco para separar os itens
    }
}
else
{
    Console.WriteLine("Nenhuma história encontrada.");
}

