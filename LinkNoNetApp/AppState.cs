using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using DataLibrary;
using MudBlazor;

namespace LinkNoNetApp;
// Scoped
public sealed class AppState
{
    public string CrcId { get; set; } = string.Empty;
    public int UsrId { get; set; } = default;
    public string UsrTyp { get; set; } = string.Empty;
    public int UsrRefId { get; set; } = default;
    public string UsrIp { get; set; } = string.Empty;
    public string UsrMoniker { get; set; } = string.Empty;

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
            var rtrn = _db.StoreProc<dynamic, dynamic>("Usr_Enter(@UsrId, @Ip)", new {UsrId = UsrId, Ip = UsrIp});

            if (rtrn.STU == 0)
            {
                UsrId = rtrn.UTID;
                UsrTyp = rtrn.TYP;
                UsrRefId = rtrn.REFID;
                UsrMoniker = rtrn.MONIKER;
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

    public void LoginOk(int usrId, string usrTyp, int usrRefId, string usrMoniker)
    {
        UsrId = usrId;
        UsrTyp = usrTyp;
        UsrRefId = usrRefId;
        UsrMoniker = usrMoniker;
        //await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        OnUsrChanged();
    }

    public async Task Logout()
    {
        UsrId = 0;
        UsrTyp = "";
        UsrRefId = 0;
        UsrMoniker = "";
        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        
        OnUsrChanged();
    }
}
