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
        int i;
        Stopwatch sw = Stopwatch.StartNew();
        List<UT> output = new List<UT>();

        int NOF; // NumberOfFound 

        //--SEARCH AND------------Token larin hepsi itm da olmali
        string[] fsa = fndAnd.Split(',', 50, StringSplitOptions.RemoveEmptyEntries);
        fsa = fsa.Distinct().ToArray(); // Duplicate olmasin

        ushort[] fndAryAnd = new ushort[fsa.Length];
        bool hasAndToken = fndAryAnd.Length == 0 ? false : true;
        int NOS = fsa.Length;   // Bulunmasi gereken NumberOfSearchToken
        
        i = 0;
        foreach (string str in fsa)
            fndAryAnd[i++] = ushort.Parse(str);
        Array.Sort(fndAryAnd);

        //--SEARCH OR------------Token larin bir tanesi itm da olmali
        string[] fso = fndOr.Split(',', 50, StringSplitOptions.RemoveEmptyEntries);
        fso = fso.Distinct().ToArray(); // Duplicate olmasin

        ushort[] fndAryOr = new ushort[fso.Length];
        bool hasOrToken = fndAryOr.Length == 0 ? false : true;

        i = 0;
        foreach (string str in fso)
            fndAryOr[i++] = ushort.Parse(str); 
        Array.Sort(fndAryOr);
        //--------------

        bool isAndTokensFound;
        bool isOrTokenFound;
        int ILAL;   // ItemLblAryLength

        foreach (var itm in utSet)
        {
            NOF = 0;        // Bulunan AndToken sayisi
            isAndTokensFound = false;
            isOrTokenFound = false;
            ILAL = itm.LblAry.Length;   // Simdilik kullanilmiyor

            isAndTokensFound = !hasAndToken;
            // FndAry token count > itm.LblAry token count -> hic arama. Bosver 
            if (hasAndToken)
            {
                foreach (int s in itm.LblAry)
                {
                    foreach (int f in fndAryAnd)
                    {
                        if (s == f)
                            NOF++;
                    }
                    if (NOF == NOS) // Bulunmasi gereken sayiya ulasildi
                    {
                        isAndTokensFound = true;
                        break;  // digerlerine bakmaya gerek yok
                    }
                }
            }
            if (isAndTokensFound) // And bulundu Or dakilerden birtanesini daha bul
            {
                isOrTokenFound = !hasOrToken;

                if (hasOrToken)
                {
                    foreach (int s in itm.LblAry)
                    {
                        foreach (int f in fndAryOr)
                        {
                            if (s == f)
                            {
                                isOrTokenFound = true; // Birinin bulunmasi yeterli
                                break;
                            }
                        }
                        if (isOrTokenFound)
                            break;
                    }
                }
            }

            if (isAndTokensFound && isOrTokenFound)
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