using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace ClientWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbCreateShapshot.IsChecked ?? false)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PostAsync("https://localhost:5001/snapshot", null))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }

            var serverOrchestrator = new WebClientOrchestrator("https://localhost:5001/sync");
            var database = Path.Combine(Environment.CurrentDirectory, "database.sqlite");
            File.Delete(database);
            var builder = new SqliteConnectionStringBuilder { DataSource = database };
            var clientProvider = new SqliteSyncProvider(builder);
            var syncSetup = new SyncSetup(new[] { "Account", "Category", "Currency", "Option" });
            var agent = new SyncAgent(clientProvider, serverOrchestrator, new SyncOptions() { UseVerboseErrors = true }, syncSetup);
            var progress = new Progress<ProgressArgs>(x => Debug.WriteLine($"{x.Context.SyncStage}: {x.Message} ({x.Hint})"));
            var s1 = await agent.SynchronizeAsync(
                SyncType.Reinitialize,
                default,
                progress);
            MessageBox.Show(s1.ToString());
        }
    }
}
