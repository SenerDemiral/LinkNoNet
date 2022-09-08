using DataLibrary;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using TntMud.Models;

namespace TntMud;

public class DataSet : IDataSet
{
    private readonly IMemoryCache cache;
    private readonly IDataAccess db;
    public DataSet(IMemoryCache cache, IDataAccess dataAccess)
    {
        this.cache = cache;
        db = dataAccess;
    }

    public async Task<IEnumerable<UT>> UTset()
    {
        Stopwatch sw = Stopwatch.StartNew();
        IEnumerable<UT> output;

        output = cache.Get<IEnumerable<UT>>("utSet");
        if (output == null)
        {
            output = await db.LoadData<UT, dynamic>("select * from UT", new { });
            cache.Set("utSet", output, TimeSpan.FromMinutes(10));
        }

        //output = await db.LoadData<UT, dynamic>("select * from UT", new { });
        sw.Stop();
        Console.WriteLine($"UTset:     {sw.ElapsedMilliseconds}ms {sw.ElapsedTicks}tick");

        return output;
    }

    public async Task<List<UT>> UTsetSrch(string fndAnd, string fndOr)
    {
        var utSet = await UTset();

        Stopwatch sw = Stopwatch.StartNew();
        List<UT> output = new List<UT>();

        int NOF;                // NumberOfFound 
        string[] fsa = fndAnd.Split(',', 50, StringSplitOptions.RemoveEmptyEntries);
        int NOS = fsa.Length;   // NumberOfSearch
        ushort[] fndAryAnd = new ushort[NOS];
        bool fndInAnd = fndAryAnd.Length == 0 ? false : true;

        string[] fso = fndOr.Split(',', 50, StringSplitOptions.RemoveEmptyEntries);
        ushort[] fndAryOr = new ushort[fso.Length];
        bool fndInOr = fndAryOr.Length == 0 ? false : true;

        int i = 0;
        foreach (string str in fsa)
            fndAryAnd[i++] = ushort.Parse(str);
        i = 0;
        foreach (string str in fso)
            fndAryOr[i++] = ushort.Parse(str);

        // AND search
        bool OKA = false, OKO = false;
        foreach (var itm in utSet)
        {
            NOF = 0;
            OKA = false;
            OKO = false;

            OKA = !fndInAnd;
            if (fndInAnd)
            {
                foreach (int s in itm.LblAry)
                {
                    foreach (int f in fndAryAnd)
                    {
                        if (s == f)
                        {
                            NOF++;
                        }
                    }
                    if (NOF == NOS)
                        OKA = true;
                }
            }
            if (OKA) // And bulundu Or dakilerden birtanesini daha bul
            {
                OKO = !fndInOr;

                if (fndInOr)
                {
                    foreach (int s in itm.LblAry)
                    {
                        foreach (int f in fndAryOr)
                        {
                            if (s == f)
                            {
                                OKO = true;
                                break;
                            }
                        }
                        if (OKO)
                            break;
                    }
                }
            }

            if (OKA && OKO)
            {
                output.Add(itm);
                //itm.fndAnd = true;
            }
        }
        sw.Stop();
        Console.WriteLine($"UTsetSrch: {sw.ElapsedMilliseconds}ms {sw.ElapsedTicks}tick fndAnd:{fndAnd}");


        return output;
    }

}

public interface IDataSet
{
    public Task<IEnumerable<UT>> UTset();
    public Task<List<UT>> UTsetSrch(string fndAnd, string fndOr);
}