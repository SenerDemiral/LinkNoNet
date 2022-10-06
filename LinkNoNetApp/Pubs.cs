namespace LinkNoNetApp;

public sealed class Pubs : IPubs
{
    public delegate void XxEventHandler(object sender, XxEventArgs e);
    public event XxEventHandler? XxChanged; // Subscribe to this
    public void XxRaise(string msj)
    {
        var args = new XxEventArgs();
        args.Msj = msj;

        XxChanged?.Invoke(this, args);
    }

    public delegate void AdmMsgEventHandler(object sender, AdmMsgEventArgs e);
    public event AdmMsgEventHandler? AdmMsgChanged; 
    public void AdmMsgRaise(string who, string msg)
    {
        // who: H/A/M/T/#  Herkes, Adm, Mgz, Tnt veya number olabilir
        var args = new AdmMsgEventArgs();
        args.Who = who.ToUpper();
        args.Msg = $"Admin'den duyuru<br/>{msg}";

        AdmMsgChanged?.Invoke(this, args);
    }

    public delegate void EcEventHandler(object sender, EcEventArgs e);
    public event EcEventHandler? EcChanged;
    public void EcRaise()
    {
        var args = new EcEventArgs();
        EcChanged?.Invoke(this, args);
    }
}

public sealed class XxEventArgs : EventArgs
{
    public int Id;
    public string Msj;
}
public sealed class AdmMsgEventArgs : EventArgs
{
    public string Who;
    public string Msg;
}
public sealed class EcEventArgs : EventArgs
{
    public string Who;
    public string Msg;
}
