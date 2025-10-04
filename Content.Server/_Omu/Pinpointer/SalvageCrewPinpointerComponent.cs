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
    /// <summary>
    /// How much time it takes until the pinpointer checks for movement again
    /// </summary>
    [DataField]
    public TimeSpan CheckInterval = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Exactly when the next movement check is ran
    /// </summary>
    public DateTime NextCheck = DateTime.Now;

    /// <summary>
    /// How many checks has the target failed in a row?
    /// </summary>
    public int FailedChecks = 0;

    public Vector2 PreviousPositionWorld = Vector2.Zero;

    public Vector2i PreviousPositionTileLocal = Vector2i.Zero;
}
