using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IDataAccess
    {
        Task<T> LoadRec<T, U>(string sql, U parameters);
        Task<IEnumerable<T>> LoadData<T, U>(string sql, U parameters);
        Task<T> StoreProc<T, U>(string storeProc, U parameters);
        //Task<dynamic> StoreProc2<T>(string storeProc, T parameters);
        Task<bool> SaveData<T>(string sql, T parameters);
        Task<T> InsertRec<T>(IDictionary<string, object> newValue) where T : new();
        Task UpdateRec<T>(T dataItem, IDictionary<string, object> newValue);

        int GetTablePK(string tblName);
        int GetPK();
        void MyTableFieldsCopy<T, U>(T src, U dst, IDictionary<string, object> newValue);
        Task SaveDataProc<T>(string sp, T parameters);
    }
}
