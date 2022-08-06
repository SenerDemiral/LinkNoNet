public interface ICircuitUserService
{
    ConcurrentDictionary<string, int> Circuits { get; }
    int NoC { get; }
    int NoU { get; }
    int No0 { get; }

    event EventHandler CircuitsChanged;
    event UserRemovedEventHandler UserRemoved;
    void Connect(string CircuitId, int UserId);
    void Disconnect(string CircuitId);
}