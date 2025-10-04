using System.Linq;
using Content.Server.Chat.Systems;
using Content.Shared._Omu.Pinpointer;
using Content.Shared.Access.Components;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Pinpointer;
using Robust.Server.GameObjects;

namespace Content.Server._Omu.Pinpointer;

/// <summary>
/// This handles non-movement alerts of salvage crew IDs.
/// </summary>
public sealed class SalvageCrewPinpointerSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _speech = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SalvageCrewPinpointerComponent, ExaminedEvent>(OnExamined);
    }

    public override void Update(float delta)
    {
        base.Update(delta);
        var query = EntityQueryEnumerator<PinpointerComponent, SalvageCrewPinpointerComponent>();
        while (query.MoveNext(out var uid, out var pin, out var salv))
        {
            if (pin.Targets.Count < 1)
                continue;
            if (salv.NextCheck >= DateTime.Now)
                continue;

            salv.NextCheck =  DateTime.Now + salv.CheckInterval;
            if (salv.PreviousPositionWorld == _transform.GetWorldPosition(pin.Targets.First()) ||
                salv.PreviousPositionTileLocal == _transform.GetGridOrMapTilePosition(pin.Targets.First()))
            {
                if (!TryComp<IdCardComponent>(pin.Targets.First(), out var card))
                    continue;
                _speech.TrySendInGameICMessage(uid,
                    $"Salvager {card.FullName ?? "ERROR"} remained stationary for {FormatFailedChecks(salv)} minute(s).",
                    InGameICChatType.Speak,
                    false);
                salv.FailedChecks++;
            }
            else
            {
                salv.FailedChecks = 0;
            }
            salv.PreviousPositionWorld = _transform.GetWorldPosition(pin.Targets.First());
            salv.PreviousPositionTileLocal = _transform.GetGridOrMapTilePosition(pin.Targets.First());
        }
    }

    private void OnExamined(EntityUid entity, SalvageCrewPinpointerComponent comp, ExaminedEvent args)
    {
        if (!TryComp<PinpointerComponent>(entity, out var pinpointer))
            return;
        if (pinpointer.Targets.Count == 0)
            return;
        if (!TryComp<IdCardComponent>(pinpointer.Targets.First(), out var idCard))
            return;


        if (!args.IsInDetailsRange || pinpointer.TargetName == null)
            return;
        args.PushMarkup(Loc.GetString("examine-pinpointer-linked", ("target", idCard.FullName ?? "ERROR")));
        args.PushMarkup($"Salvager remained stationary for: {FormatFailedChecks(comp)} minute(s)");
    }

    private string FormatFailedChecks(SalvageCrewPinpointerComponent pin) =>
        ((double) pin.FailedChecks * pin.CheckInterval.TotalMinutes).ToString("N1");

}
