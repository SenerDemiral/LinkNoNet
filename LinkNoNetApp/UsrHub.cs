using DataLibrary;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LinkNoNetApp;

public sealed class UsrHub
{
    public ConcurrentDictionary<int, Usr> Usrs;
    private readonly IPubs pub
        ;
    public UsrHub(IPubs pub)
    {
        Usrs = new ConcurrentDictionary<int, Usr>();
        this.pub = pub;
    }

    public void UsrAdd(int usrId, string usrNN)
    {
        if (!Usrs.ContainsKey(usrId))
        {
            Usrs[usrId] = new()
            {
                UsrId = usrId,
                UsrNN = usrNN,
                EXD = DateTime.Now,
            };
            pub.UsrRaise();
        }
        Usrs[usrId].Cnt++;
    }

    public void UsrRemove(int usrId)
    {
        Usr? usr;
        if (Usrs.ContainsKey(usrId))
        {
            Usrs[usrId].Cnt--;

            if (Usrs[usrId].Cnt == 0)
            {
                if(Usrs.TryRemove(usrId, out usr))
                {
                    pub.UsrRaise();
                    // User Cikti
                }
            }
        }
    }
}

public sealed class Usr
{
    public int UsrId;
    public string? UsrNN;
    public DateTime? EXD;
    public int Cnt;
}
