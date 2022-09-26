using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using DataLibrary;
using MudBlazor;
using System.Reflection;

namespace LinkNoNetApp;
// Scoped
public sealed class AppState
{
    public string CrcId { get; set; } = string.Empty;
    public int UsrId { get; set; } = default;
    public string UsrTyp { get; set; } = "?";
    public int UsrRefId { get; set; } = default;
    public string UsrIp { get; set; } = string.Empty;
    public string UsrNN { get; set; } = string.Empty;

    public event EventHandler? UsrChanged;
    void OnUsrChanged() => UsrChanged?.Invoke(this, EventArgs.Empty);

    private readonly ProtectedLocalStorage _protectedLocalStorage;
    IDataAccess _db;

    public AppState(ProtectedLocalStorage protectedLocalStorage, IDataAccess db)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _db = db;
    }

    public async Task TryUsrEnter()
    {
        var toto = await _protectedLocalStorage.GetAsync<int>(Constants.BrowserUsrIdKey);
        //AppState.OnEnter(connectionInfo?.IPAddress!);
        if (toto.Success)
            UsrId = toto.Value;

        if (UsrId != 0)
        {
            var rtrn = _db.StoreProc<dynamic, dynamic>("Usr_Enter(@UsrId)", new {UsrId = UsrId});

            if (rtrn.STU == 0)
            {
                UsrId = rtrn.UTID;
                UsrTyp = rtrn.TYP;
                UsrRefId = rtrn.REFID;
                UsrNN = rtrn.NN;
            }
        }
        OnUsrChanged();
    }

    public void OnEnter(string ip)
    {
    }

    public void OnExit()
    {
    }

    public void LoginOk(int usrId, string usrTyp, int usrRefId, string usrNN)
    {
        UsrId = usrId;
        UsrTyp = usrTyp;
        UsrRefId = usrRefId;
        UsrNN = usrNN;
        //await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        OnUsrChanged();
    }

    public async Task Logout()
    {
        _db.StoreProc<dynamic, dynamic>("Usr_Logout(@UTid)", new { UTid = UsrId });
        UsrId = 0;
        UsrTyp = "?";
        UsrRefId = 0;
        UsrNN = "";
        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        
        OnUsrChanged();
    }
}
