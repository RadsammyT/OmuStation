using System.Numerics;
using Content.Server._Omu.Pinpointer;
using Robust.Shared.GameStates;

namespace Content.Shared._Omu.Pinpointer;

/// <summary>
/// Handles non-movement alerts of salvage crew pinpointers.
/// </summary>
[RegisterComponent]
[Access(typeof(SalvageCrewPinpointerSystem))]
public sealed partial class SalvageCrewPinpointerComponent : Component
{
    public TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

    public DateTime NextCheck = DateTime.Now;

    public int FailedChecks = 0;

    public Vector2 PreviousPositionWorld = Vector2.Zero;

    public Vector2i PreviousPositionTileLocal = Vector2i.Zero;

    public Vector2 PreviousVelocity =  Vector2.Zero;
}
