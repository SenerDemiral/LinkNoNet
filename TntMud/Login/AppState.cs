using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TntMud.Login;
// Scoped
public class AppState
{
    public string CrcId { get; set; } = string.Empty;
    public int UsrId { get; set; } = default;
    public int UsrRefId { get; set; } = default;
    public string UsrTyp { get; set; } = string.Empty;
    public string UsrIp { get; set; } = string.Empty;
    public string UsrMoniker { get; set; } = string.Empty;

    public event EventHandler? UsrChanged;
    void OnUsrChanged() => UsrChanged?.Invoke(this, EventArgs.Empty);

    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly CircuitHandler _circuitHandler;
    ICircuitUserService _userService;
    DataLibrary.IDataAccess _db;

    public AppState(ProtectedLocalStorage protectedLocalStorage, CircuitHandler circuitHandler, ICircuitUserService userService, DataLibrary.IDataAccess DB)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _circuitHandler = circuitHandler!;
        _userService = userService!;
        _db = DB;
    }

    public async Task OnEnter(string ip)
    {
        CircuitHandlerService handler = (CircuitHandlerService)_circuitHandler;
        CrcId = handler.CircuitId;
        UsrIp = ip;
        //var toto = await _protectedLocalStorage.GetAsync<int>(Constants.BrowserUsrIdKey);
        //if (toto.Success)
        //{
        //    UsrId = toto.Value;
        //}

        // Check uid
        // Add to GlbState
        _userService.Connect(CrcId, UsrId);
        // Set UsrId
        // Invoke listeners
        OnUsrChanged();
    }

    public void OnExit()
    {
        // Remove from GlbState
        // CircuitHandlerService de yapiliyor zaten
        // UsrId = 0
        UsrId = 0;

        // Invoke listeners
        OnUsrChanged();
    }

    public async Task LoginOk(int usrId, string usrTyp, int usrRefId, string usrMoniker)
    {
        UsrId = usrId;
        UsrTyp = usrTyp;
        UsrRefId = usrRefId;
        UsrMoniker = usrMoniker;
        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        _userService.Connect(CrcId, UsrId);
    }

    public async Task Login(string usrNme, string usrPwd)
    {
        if(usrNme == usrPwd)
            UsrId = Convert.ToInt32(usrNme);

        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);

        // Check Nme & Pwd 
        // Set UsrId
        // setLocal("uid")
        // Add to GlbState
        _userService.Connect(CrcId, UsrId);
        // Invoke Listeners
        //OnUsrChanged();
    }

    public async Task Logout()
    {
        UsrId = 0;
        UsrTyp = "";
        UsrRefId = 0;
        UsrMoniker = "";
        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        // Set UsrId = 0
        // setLocal("uid")
        // Remove from GlbState
        // Invoke Listeners
        _userService.Connect(CrcId, UsrId);
        //OnUsrChanged();
    }
}
