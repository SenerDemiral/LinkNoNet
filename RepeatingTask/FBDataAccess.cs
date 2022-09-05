using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;

namespace RepeatingTask;

public class FBDataAccess : IDataAccess
{
    string cnctStr = "";
    //private readonly IConfiguration _config;

    public FBDataAccess(IConfiguration config)
    {
        //_config = config;
        cnctStr = config.GetConnectionString("Default");
        FbConnectionStringBuilder csb = new FbConnectionStringBuilder(cnctStr);
        //csb.WireCrypt = FbWireCrypt.Disabled;
        //csb.ConnectionTimeout = 15;
        //csb.Pooling = false;
        csb.PacketSize = 16384;
        //csb.ServerType = FbServerType.Embedded;   // Hata
        //csb.Charset = FbCharset.Utf8.ToString(); //"WIN1254"; //Hata geliyor default: Utf8
        csb.ClientLibrary = "fbclient.dll"; //Default: fbembed
        cnctStr = csb.ConnectionString;
    }
    public FBDataAccess(string cnctString)
    {
        cnctStr = cnctString;
        //TableDefs.initTableDefs();

    }

    public async Task<T> LoadRec<T, U>(string sql, U parameters)
    {
        using (IDbConnection cnct = new FbConnection(cnctStr))
        {
            var row = await cnct.QueryFirstOrDefaultAsync<T>(sql, parameters);
            return row;
        }
    }

    public async Task<IEnumerable<T>> LoadData<T, U>(string sql, U parameters)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();  
        using IDbConnection cnct = new FbConnection(cnctStr);
        //return await cnct.QueryAsync<T>(sql, parameters);
        var aaa = await cnct.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
        stopWatch.Stop();
        Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
        return aaa;
    }

    /// <summary>
    /// Ins/Upd/Del StoreProcedure returns Row (multiple fields)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="storeProc"></param>
    /// <param name="parameters"></param>
    /// SP input params db de tanimlandigi sirasiyla gelmeli. Her ikisi de calisiyor
    /// "select * from Usr_Login(@LgnNme, @LgnPwd, @Ip)"
    /// "execute procedure Usr_Login(@LgnNme, @LgnPwd, @Ip)"
    /// https://www.tabsoverspaces.com/227021-firebird-net-provider-and-calling-stored-procedures-with-parameters
    /// <returns></returns>
    public async Task<T> StoreProc<T, U>(string storeProc, U parameters)
    {
        // var params = new { UserName = username, Password = password };
        using IDbConnection cnct = new FbConnection(cnctStr);
        //var aaa = await connection.QueryAsync<T>(storeProc, parameters, commandType: CommandType.StoredProcedure).Sing
        //var aaa = await connection.QueryFirstOrDefault<T>(storeProc, parameters, commandType: CommandType.StoredProcedure);
        //var bbb = connection.Query<T>(storeProc, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
        //return await cnct.QueryFirstOrDefaultAsync<T>(storeProc, parameters, commandType: CommandType.StoredProcedure);
        return await cnct.QueryFirstOrDefaultAsync<T>("execute procedure "+storeProc, parameters);
        //return await cnct.QueryFirstOrDefaultAsync<T>("execute procedure Usr_Login(@LgnNme, @LgnPwd, @Ip)", parameters);
    }

    public async Task<dynamic> StoreProc2<T>(string storeProc, T parameters)
    {
        // anonymous object
        // var paras = new { UserName = username, Password = password };
        // FB StoreProc u Table olarak gordugu icin CommandType == StoredProcedure olmasina gerekyok
        using IDbConnection cnct = new FbConnection(cnctStr);
        //var aaa = await connection.QueryAsync<T>(storeProc, parameters, commandType: CommandType.StoredProcedure).Sing

        var aaa = cnct.Query(storeProc, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();

        //var ccc = aaa as IDictionary<string, object>;
        //var ddd = ccc["FIRMA"];
        //var eee = ccc["PORT"];
        //var bbb = aaa.FIRMA;

        //foreach (var f in ccc)
        //{
        //    var fff = f.Key;
        //}
        return aaa;
    }

    public async Task<bool> SaveData<T>(string sql, T parameters)
    {
        using IDbConnection cnct = new FbConnection(cnctStr);
        var nora = await cnct.ExecuteAsync(sql, parameters);
        if (nora == 0)
            return false;   // Affected NOR yok, Save edilemedi
        return true;
    }
    public async Task SaveDataProc<T>(string sp, T parameters)
    {
        using IDbConnection cnct = new FbConnection(cnctStr);
        
        Console.WriteLine("connectionString");
        Console.WriteLine(cnctStr);
        
        await cnct.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
    }

      public int GetTablePK(string tblName)
    {
        int pk = 0;
        using (IDbConnection connection = new FbConnection(cnctStr))
        {
            pk = connection.ExecuteScalar<int>($"select ID from Get_PK('{tblName}')");
        }
        return pk;
    }
    public int GetPK()
    {
        int pk = 0;
        using (IDbConnection connection = new FbConnection(cnctStr))
        {
            pk = connection.ExecuteScalar<int>($"select ID from Get_PK");
        }
        return pk;
    }

}
