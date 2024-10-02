namespace GetUrlsAsync
{
     class Program
    {
        static async Task Main(string[] args)
        {
            
            List<string> urls = new List<string>
        {
            "https://www.microsoft.com/uk-ua",
            "https://rozetka.com.ua/ua/retail/odesa/"
            
        };

            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Press 'c' to cancel the operation.\n");

            // Старт таска для отмены через нажатие клавиши
            Task.Run(() =>
            {
                if (Console.ReadKey().KeyChar == 'c')
                {
                    cancellationTokenSource.Cancel();
                }
            });

            try
            {
               
                await DownloadPagesAsync(urls, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nOperation was canceled.");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            Console.WriteLine("Program has finished.");
        }

        static async Task DownloadPagesAsync(List<string> urls, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = new HttpClient();

            
            List<Task> downloadTasks = new List<Task>();

            foreach (var url in urls)
            {
                downloadTasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested(); 
                    Console.WriteLine($"Starting download: {url}");

                    HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
                    string content = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Completed download: {url}, {content.Length} characters loaded.");
                }, cancellationToken));
            }

           
            await Task.WhenAll(downloadTasks);
        }
    }
}
