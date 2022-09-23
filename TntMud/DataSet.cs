using DataLibrary;
using MailKit;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Crypto;
using System.Diagnostics;
using TntMud.Models;

namespace TntMud;

public sealed class DataSet : IDataSet
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
    
    private int[] str2intAry(string s)
    {
        // Max 50 token
        string[] fsa = s.Split(',', 50, StringSplitOptions.RemoveEmptyEntries);
        fsa = fsa.Distinct().ToArray(); // Duplicate olmasin
        int[] result = new int[fsa.Length];

        int i = 0;
        foreach (string str in fsa)
            result[i++] = ushort.Parse(str);
        Array.Sort(result);
        return result;
    }

    public async Task<List<UT>> UtLblSrch(string fndAnd, string fndOr)
    {
        // UtLblSrchYAVAS tan 10 kat hizli
        var utSet = await UTset();
        List<UT> output = new();

        int[] fa = str2intAry(fndAnd);
        int[] fo = str2intAry(fndOr);
        int fal = fa.Length;
        int fol = fo.Length;
        int sal;
        bool faOk, foOk;
        int bf, bs;

        foreach (var itm in utSet)
        {
            sal = itm.LblAry.Length;
            faOk = false;
            foOk = false;

            // fa dakilerin hepsi itm.LblAry de olmali (AND)
            if (fal > sal)  // Aranan mevcuttan cok fOk=false
                faOk = false;
            else if (fal == 0)   // Aranan yok
                faOk = true;
            else
            {
                bf = 0;
                bs = 0;
                for (int f = bf; f < fal; f++)
                {
                    faOk = false;
                    for (int s = bs; s < sal; s++)
                    {
                        if (fa[f] == itm.LblAry[s])
                        {
                            bf = f + 1;
                            bs = s + 1;
                            faOk = true;
                            break;
                        }
                        else if (itm.LblAry[s] > fa[f]) // s ascending oldugu icin sonrasindakilerde de bulamazsin
                            break;
                    }
                    if (!faOk)   // bulundugu surece devam et
                        break;
                }
            }

            if (faOk)
            {
                // fo dakilerin biri itm.LblAry de olmali (OR)
                if (fol == 0)   // Aranan yok
                    foOk = true;
                else
                {
                    for (int f = 0; f < fol; f++)
                    {
                        foOk = false;
                        for (int s = 0; s < sal; s++)
                        {
                            if (fo[f] == itm.LblAry[s])
                            {
                                foOk = true;
                                break;
                            }
                            else if (itm.LblAry[s] > fo[f]) // s ascending oldugu icin sonrasindakilerde de bulamazsin
                                break;
                        }
                        if (foOk)   // bulundugunda bitir
                            break;
                    }
                }
            }
            if (faOk && foOk)
                output.Add(itm);
        }
        return output;
    }
    
    public async Task<List<UT>> UtLblSrchYAVAS(string fndAnd, string fndOr)
    {
        var utSet = await UTset();
        List<UT> output = new();

        int[] fa = str2intAry(fndAnd);
        int[] fo = str2intAry(fndOr);
        int fal = fa.Length;
        int fol = fo.Length;
        bool faOk, foOk;

        foreach (var itm in utSet)
        {
            //utSet.Prepend(itm);
            faOk = foOk = false;
            // hic Lbl olmayabilir
            // itm icinde aranan kadar Lbl olmayabilir
            // fal sifir olabilir fa yok
            if (fal == 0 ||                                 // and find yok
                itm.LblAry.Length >= fal &&                 // Lbl sayisi >= aran and
                fa.Intersect(itm.LblAry).Count() == fal)    // Aranan sayida Lbl var
            { 
                faOk = true; 
            }

            if (faOk)
            {
                if (fol == 0 ||                             // or find yok
                    itm.LblAry.Length > 0 &&                // Lbl var
                    fo.Intersect(itm.LblAry).Count() > 0)   // Or dakilerden biri var
                {
                    foOk = true;
                }
            }

            if (faOk && foOk)
                output.Add(itm);
        }
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
    public Task<List<UT>> UtLblSrch(string fndAnd, string fndOr);
    public Task<List<UT>> UTsetSrch(string fndAnd, string fndOr);
}