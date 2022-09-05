namespace RepeatingTask;

public class RepeatingService: BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(60));
    private readonly BasicModel _basicModel;    // Ornek
    private readonly GetOrders _getOrders;
    
    public RepeatingService(BasicModel basicModel, GetOrders getOrders)
    {
        _basicModel = basicModel; // Ornek
        _getOrders = getOrders;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stopingToken)
    {
        // DoWorkAsync icine koy, simdilik burda
        await _basicModel.OnGet();  // Ornek
        await _getOrders.OnGet(accessToken: "", mgzId: 5, mgzUri: "", startUpdatedAt: DateTime.Now.AddDays(-1), endUpdatedAt: DateTime.Now);  // Kayitli aktif her Magaza icin yap

        while (await _timer.WaitForNextTickAsync(stopingToken) && !stopingToken.IsCancellationRequested)
        {
            await DoWorkAsync();
        }
    }

    private async Task DoWorkAsync()
    {
        Console.WriteLine(DateTime.Now.ToString("O"));
        await Task.Delay(500);

        // for every Magaza get Siparis listesi from IdeaSoft API
        // if accessToken expired GetNewToken, update db and return new accessToken
        // read aktif Magaza
        // foreach(var itm in Magazalar)
        //      accessToken = itm.AccessToken
        //      if(DateTime.Now > itm.TokenExpiredTS) // TokenExpiredTS alindigi zamana 23 saat ekleyerek yaz, aslinda 24 saat gecerli
        //      {
        //          GetNewToken.OnGet(itm.mgzId, itm.mgzUri, itm.client_id, itm.client_secret, itm.refresh_token);
        //          accessToken = GetNewToken.Token;
        //      }
        //      startUpdatedAt = itm.EndUpdatedAt;
        //      await _getOrders.OnGet(accessToken: accessToken, mgzId: 5, mgzUri: itm.MgzUri, startUpdatedAt: DateTime.Now.AddDays(-1), endUpdatedAt: DateTime.Now);  // Kayitli aktif her Magaza icin yap
        //
    }
}
