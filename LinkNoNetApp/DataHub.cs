using DataLibrary;
using System.Collections.Concurrent;
using System.Reflection;

namespace LinkNoNetApp;

public sealed class DataHub
{
    public ConcurrentDictionary<int, List<EC>> ECd;
    private readonly IDataAccess db;
    private readonly IPubs pub
        ;
    public DataHub(IDataAccess dataAccess, IPubs pub)
    {
        this.db = dataAccess;
        ECd = new ConcurrentDictionary<int, List<EC>>();
        this.pub = pub;
    }

    public async Task ECdInit(int etId)
    {
        if (!ECd.ContainsKey(etId))
        {
            ECd[etId] = (await db.LoadData<EC, dynamic>("select * from EC_GET(@ETid)", new { ETid = etId })).ToList();
        }
    }
    public async Task ECdAdd(int etId, int utId, string info)
    {
        if (!ECd.ContainsKey(etId))
        {
            //            ECd[etId] = new List<EC>();
            ECd[etId] = (await db.LoadData<EC, dynamic>("select * from EC_GET(@ETid)", new { ETid = etId })).ToList();
            pub.EcRaise();
        }
        var ec = new EC()
        {
            ECid = 0,
            ETid = etId,
            UTid = utId,
            Info = info
        };

        var res = await db.StoreProcAsync<dynamic, EC>("EC_INS(@ETid, @UTid, @Info)", ec);
        ec.ECid = res.ECID;
        ec.InsTS = res.INSTS;

        ECd[etId].Insert(0, ec);

        pub.EcRaise();
        // Publish EC nin ETid sine ekleme yapildi
        // Buna Abone olanlar Refresh yapmali 
        // Aslinda bu Dict uzerinden yapsin tum islemini
    }

    public List<EC> ECdGet(int etId)
    {
        return ECd[etId];
    }
}

public sealed class EC
{
    public int ECid { get; set; }
    public int ETid { get; set; }
    public int UTid { get; set; }
    public string? Info { get; set; }
    public DateTime? InsTS { get; set; }

}
