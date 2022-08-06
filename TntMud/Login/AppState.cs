using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TntMud.Login;
// Scoped
public class AppState
{
    public string CrcId { get; set; } = string.Empty;
    public int UsrId { get; set; } = default;
    public string UsrNme { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;

    public event EventHandler? UsrChanged;
    void OnUsrChanged() => UsrChanged?.Invoke(this, EventArgs.Empty);

    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly CircuitHandler _circuitHandler;
    ICircuitUserService _userService;

    public AppState(ProtectedLocalStorage protectedLocalStorage, CircuitHandler circuitHandler, ICircuitUserService userService)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _circuitHandler = circuitHandler!;
        _userService = userService!;
    }

    public async Task OnEnter(string ip)
    {
        CircuitHandlerService handler = (CircuitHandlerService)_circuitHandler;
        CrcId = handler.CircuitId;
        Ip = ip;
        var toto = await _protectedLocalStorage.GetAsync<int>(Constants.BrowserUsrIdKey);
        if (toto.Success)
        {
            UsrId = toto.Value;
        }

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
        await _protectedLocalStorage.SetAsync(Constants.BrowserUsrIdKey, UsrId);
        // Set UsrId = 0
        // setLocal("uid")
        // Remove from GlbState
        // Invoke Listeners
        _userService.Connect(CrcId, UsrId);
        //OnUsrChanged();
    }
}
