﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepeatingTask;

public interface IDataAccess
{
    Task<T> LoadRec<T, U>(string sql, U parameters);
    Task<IEnumerable<T>> LoadData<T, U>(string sql, U parameters);
    Task<T> StoreProc<T, U>(string storeProc, U parameters);
    //Task<dynamic> StoreProc2<T>(string storeProc, T parameters);
    Task<bool> SaveData<T>(string sql, T parameters);
    int GetTablePK(string tblName);
    int GetPK();
}
