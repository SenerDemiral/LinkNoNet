// Scoped
public class CircuitHandlerService : CircuitHandler
{
    public string CircuitId { get; private set; }

    ICircuitUserService _userService;

    public CircuitHandlerService(ICircuitUserService userService)
    {
        _userService = userService;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitId = circuit.Id;
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _userService.Disconnect(circuit.Id);
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}