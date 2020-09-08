using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Microsoft.Data.Sqlite;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Creating snapshot
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://localhost:5001/snapshot", null);
            response.EnsureSuccessStatusCode();

            var serverOrchestrator = new WebClientOrchestrator("https://localhost:5001/sync");
            var database = Path.Combine(Environment.CurrentDirectory, "database.sqlite");
            File.Delete(database);
            var builder = new SqliteConnectionStringBuilder { DataSource = database };
            var clientProvider = new SqliteSyncProvider(builder);
            var syncSetup = new SyncSetup(new[] { "Account", "Category", "Currency", "Option" });
            var agent = new SyncAgent(clientProvider, serverOrchestrator, new SyncOptions() { UseVerboseErrors = true }, syncSetup);
            var progress = new Progress<ProgressArgs>(x => Console.WriteLine($"{x.Context.SyncStage}: {x.Message} ({x.Hint})"));
            var s1 = await agent.SynchronizeAsync(
                SyncType.Reinitialize,
                default,
                progress);
            Console.WriteLine(s1);
        }
    }
}
