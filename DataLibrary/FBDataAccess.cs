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

namespace DataLibrary
{

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
            var aaa = await cnct.QueryAsync<T>(sql, parameters);
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
        /// <returns></returns>
        public async Task<T> StoreProc<T, U>(string storeProc, U parameters)
        {
            // var params = new { UserName = username, Password = password };
            using IDbConnection cnct = new FbConnection(cnctStr);
            //var aaa = await connection.QueryAsync<T>(storeProc, parameters, commandType: CommandType.StoredProcedure).Sing
            //var aaa = await connection.QueryFirstOrDefault<T>(storeProc, parameters, commandType: CommandType.StoredProcedure);
            //var bbb = connection.Query<T>(storeProc, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return await cnct.QueryFirstOrDefaultAsync<T>(storeProc, parameters, commandType: CommandType.StoredProcedure);
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

        public async Task UpdateRec<T>(T dataItem, IDictionary<string, object> newValue)
        {
            StringBuilder sql = new System.Text.StringBuilder();
            int dc = newValue.Count;
            bool first = true;
            if (dc > 0)
            {
                Type t = typeof(T);

                var (tblName, keyName) = MyTableNameAndKey(t);


                sql.Append($"update {tblName} set ");
                foreach (var n in newValue)
                {
                    sql.Append($"{(first ? "" : ",")} {n.Key} = @{n.Key} ");
                    first = false;
                    // newValue yu dataItem a da koy, UI de degissin
                    dataItem?.GetType().GetProperty(n.Key)?.SetValue(dataItem, n.Value);
                }
                var objVal = dataItem?.GetType().GetProperty(keyName)?.GetValue(dataItem);
                sql.Append($" where {keyName} = {objVal}");  // PK
                Console.WriteLine(sql.ToString());
                await SaveData(sql.ToString(), newValue);
            }
        }

        /*
        https://makolyte.com/csharp-adding-dynamic-query-parameters-with-dapper/
        //Built dynamically somewhere
        var query = "SELECT * FROM Movies WHERE Name=@Name AND YearOfRelease=@Year";
        var parameters = new Dictionary<string, object>()
        {
            ["Year"] = 1999
        };

        //Using hardcoded (known) parameters  + dynamic parameters
        using (var con = new SqlConnection(connectionString))
        {
            var dynamicParameters = new DynamicParameters(parameters);

            dynamicParameters.AddDynamicParams(new { name = "The Matrix" });

            var results = con.Query<Movie>(query, dynamicParameters);
            return results;
        }
         */

        public async Task<T> InsertRec<T>(IDictionary<string, object> newValue) where T : new()
        {
            //string sql = "insert into UST (UstID, Ad) values (@UstID, @Ad)";
            // PK ile ugrasma
            // Parameter T 
            //string sql = "insert into UST (UstID, Ad) values (Gen_ID(Id_Gen,1), @Ad)";
            /*
            */
            var dataItem = new T();

            int dc = newValue.Count;
            if (dc > 0)
            {
                bool first = true;
                StringBuilder sql = new StringBuilder();
                StringBuilder prm = new StringBuilder();
                var (tblName, keyName) = MyTableNameAndKey(typeof(T));

                newValue.Add(keyName, GetPK()); // Get PK

                sql.Append($"insert into {tblName}(");
                prm.Append("values(");

                foreach (var n in newValue)
                {
                    sql.Append($"{(first ? "" : ",")}{n.Key} ");
                    prm.Append($"{(first ? "" : ",")}@{n.Key} ");
                    first = false;

                    dataItem.GetType().GetProperty(n.Key)?.SetValue(dataItem, n.Value); // Populate new rec
                }
                sql.Append(")");
                prm.Append(")");

                await SaveData($"{sql} {prm}", newValue);
            }

            return dataItem;
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

        public (string tblName, string keyName) MyTableNameAndKey(Type t)
        {
            string tblName = "", keyName = "";
            TableAttribute ta = (TableAttribute)t.GetCustomAttribute(typeof(TableAttribute));
            if (ta != null)
                tblName = ta.Name;

            var properties = t.GetProperties();
            foreach (var property in properties)
            {
                var pAttributes = property.GetCustomAttributes(false);
                KeyAttribute ka = (KeyAttribute)property.GetCustomAttribute(typeof(KeyAttribute));
                if (ka != null)
                {
                    keyName = property.Name;
                    break;
                }
            }
            return (tblName, keyName);
        }

        public Dictionary<string, string> MyTableFields(Type t)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            //foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                //Console.WriteLine($"{prop.Name} {prop.PropertyType.Name} - {prop.CanWrite}");
                d.Add(prop.Name, prop.PropertyType.Name);
            }

            return d;
        }

        public void MyTableFieldsCopy<T, U>(T edtCtx, U oldRow, IDictionary<string, object> newValue)
        {
            // edtCtx::EditFormContext (oldRow disinda baska alanlar da var)
            // oldRow::??model bunun uzerinden git
            foreach (var fld in oldRow.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                NotMappedAttribute nma = (NotMappedAttribute)fld.GetCustomAttribute(typeof(NotMappedAttribute));

                //if (fld.Name != "SelectedTGs") // SelectedTGs Table da yok: NotMapped olarak isaretlendi
                if (nma == null)
                {
                    if (fld.CanWrite)
                    {
                        var newVal = edtCtx.GetType().GetProperty(fld.Name)?.GetValue(edtCtx);
                        var oldVal = oldRow.GetType().GetProperty(fld.Name)?.GetValue(oldRow);

                        if (!object.Equals(newVal, oldVal))
                        {
                            newValue.Add(fld.Name, newVal);
                            //oldRow.GetType().GetProperty(fld.Name)?.SetValue(oldRow, newVal); // Gerek yok, cagiran yapiyor
                        }
                    }
                }
            }
        }
    }
}
