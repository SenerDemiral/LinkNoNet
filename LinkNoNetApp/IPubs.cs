﻿using static LinkNoNetApp.Pubs;

namespace LinkNoNetApp;

public interface IPubs
{
    public event EventHandler? XxChanged;
    public void XxRaise();

    public event EventHandler? UsrChanged;
    public void UsrRaise();

    public event EventHandler<AdmMsgEventArgs>? AdmMsgChanged;
    public void AdmMsgRaise(string who, string msg);

    public event EventHandler<ChatEventArgs>? ChatChanged;
    public void ChatRaise(int grp);
}