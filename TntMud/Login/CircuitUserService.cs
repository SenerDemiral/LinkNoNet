using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
// Singleton
public class CircuitUserService : ICircuitUserService
{
    public record _record2 (int UsrId, string IP);
    public class _record
    {
        public int UsrId;
        public string IP;
    }

    //public ConcurrentDictionary<string, _record> deneme { get; private set; }
    public ConcurrentDictionary<string, _record> deneme { get; set; }

    //private ProtectedLocalStorage protectedLocalStorage;

    public ConcurrentDictionary<string, int> Circuits { get; private set; }
    public event EventHandler? CircuitsChanged;
    public event UserRemovedEventHandler? UserRemoved;
    public int NoC { get; set; } = 0;
    public int NoU { get; set; } = 0;
    public int No0 { get; set; } = 0;   // Not logged in

    void OnCircuitsChanged()
    {
        //NoC = Circuits.Count();
        //NoU = Circuits.Values.Where(z => z != 0).Distinct().Count();
        HashSet<int> uHS = new HashSet<int>();
        NoC = 0;
        NoU = 0;
        No0 = 0;

        //foreach(var v in Circuits.Values)
        //{
        //    NoC++;
        //    if (v != 0)
        //    {
        //        if (!uHS.Contains(v))
        //        {
        //            uHS.Add(v);
        //            NoU++;
        //        }
        //    }
        //    else
        //        No0++;
        //}

        foreach (var v in deneme.Values)
        {
            int uid = v.UsrId;
            NoC++;
            if (uid != 0)
            {
                if (!uHS.Contains(uid))
                {
                    uHS.Add(uid);
                    NoU++;
                }
            }
            else
                No0++;
        }

        CircuitsChanged?.Invoke(this, EventArgs.Empty); 
    }

    void OnUserRemoved(int UserId)
    {
        var args = new UserRemovedEventArgs();
        args.UserId = UserId.ToString();
        UserRemoved?.Invoke(this, args);
    }

    public CircuitUserService()
    {
        Circuits = new ConcurrentDictionary<string, int>();
        deneme = new ConcurrentDictionary<string, _record>();
    }

    public void Connect(string CircuitId, int UserId)
    {
        Circuits[CircuitId] = UserId;
        //deneme[CircuitId] = new _record(UserId, "");
        deneme[CircuitId] = new _record() { UsrId = UserId, IP = "" };

        OnCircuitsChanged();
    }
    /*
    public bool TryConnect(string CircuitId, string UserId)
    {
        // Check user can coonect with UserId
        
        return true;
        
        OnCircuitsChanged();
    }
    */
    public void Disconnect(string CircuitId)
    {
        //int circuitRemoved;
        //if (deneme.TryRemove(CircuitId, out int circuitRemoved))
        if (deneme.TryRemove(CircuitId, out _record? circuitRemoved))    //(CircuitId))
        {
                OnCircuitsChanged();
        }
    }

}
