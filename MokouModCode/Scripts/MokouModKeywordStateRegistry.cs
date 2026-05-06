using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MokouMod.MokouModCode.Scripts;

public static class MokouKeywordStateRegistry
{
    private static readonly Dictionary<CombatStateTracker, Dictionary<Player, MokouModKeywordState>> states = new();

    public static MokouModKeywordState Get(Player owner)
    {
        var tracker = CombatManager.Instance.StateTracker;

        if (!states.TryGetValue(tracker, out var perPlayer))
        {
            perPlayer = new Dictionary<Player, MokouModKeywordState>();
            states[tracker] = perPlayer;
        }

        if (!perPlayer.TryGetValue(owner, out var state))
        {
            state = new MokouModKeywordState();
            perPlayer[owner] = state;
        }

        return state;
    }

    public static void Clear(CombatStateTracker tracker)
    {
        states.Remove(tracker);
    }
}