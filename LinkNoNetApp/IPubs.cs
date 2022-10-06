using static LinkNoNetApp.Pubs;

namespace LinkNoNetApp;

public interface IPubs
{
    public event XxEventHandler? XxChanged;
    public void XxRaise(string msg);

    public event AdmMsgEventHandler? AdmMsgChanged;
    public void AdmMsgRaise(string who, string msg);

    public event EcEventHandler? EcChanged;
    public void EcRaise();
}