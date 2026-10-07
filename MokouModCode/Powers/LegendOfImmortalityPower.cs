using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MokouMod.MokouModCode.Powers;

public class LegendOfImmortalityPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnTrigger()
    {
        Flash();
        await PowerCmd.Apply<TempEssencePower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
    }
}