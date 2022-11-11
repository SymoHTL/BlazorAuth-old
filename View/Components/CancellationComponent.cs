using Microsoft.AspNetCore.Components;

namespace View.Components;

public abstract class CancellationComponent : ComponentBase, IDisposable {
    protected readonly CancellationTokenSource Cts = new();

    public virtual void Dispose() {
        Cts.Cancel();
        Cts.Dispose();
    }
}