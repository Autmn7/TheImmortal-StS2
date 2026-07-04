using System.Reflection;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class KeinesSuppression : MokouModCard, DualSuppression.IChoosable
{
    public KeinesSuppression() : base(-1, CardType.Skill, CardRarity.Event, TargetType.None)
    {
        WithPower<DupHistoricalGapPower>(6);
    }

    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    public async Task OnChosen()
    {
        // 1. Check if KeineMod is actively loaded
        if (ModManager.GetLoadedMods().Any(mod => string.Equals(mod.manifest?.id, "KeineMod")))
            try
            {
                // 2. Locate Keine's assembly
                var keineAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => string.Equals(a.GetName().Name, "KeineMod"));

                if (keineAssembly != null)
                {
                    // 3. Find the native HistoricalGapPower type
                    var historicalGapPowerType = keineAssembly.GetTypes()
                        .FirstOrDefault(t => t.Name.Contains("HistoricalGapPower"));

                    if (historicalGapPowerType != null)
                    {
                        // 4. Target the multi-creature generic Apply method precisely (2nd parameter is IEnumerable<Creature>)
                        var applyMethod = typeof(PowerCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Where(m => m.Name == "Apply" && m.IsGenericMethod && m.GetGenericArguments().Length == 1)
                            .FirstOrDefault(m =>
                            {
                                var p = m.GetParameters();
                                return p.Length == 6 && p[1].ParameterType == typeof(IEnumerable<Creature>);
                            });

                        if (applyMethod != null)
                        {
                            // 5. Transform the generic method definition into PowerCmd.Apply<HistoricalGapPower>
                            var genericApply = applyMethod.MakeGenericMethod(historicalGapPowerType);

                            // 6. Invoke using the multi-target arrangement, passing all hittable enemies
                            var invokeResult = genericApply.Invoke(null, [new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState.HittableEnemies, DynamicVars["DupHistoricalGapPower"].BaseValue, Owner.Creature, this, false]);

                            if (invokeResult is Task task)
                            {
                                await task;
                                return;
                            }
                        }
                        else
                        {
                            Log.Info("[MokouMod] Reflection Error: Could not locate the 6-parameter multi-target Apply<T> method.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Info($"[MokouMod] Reflection Exception: {ex.Message}\n{ex.StackTrace}");
            }

        // Fallback to local duplicate power if KeineMod isn't present
        await PowerCmd.Apply<DupHistoricalGapPower>(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState.HittableEnemies, DynamicVars["DupHistoricalGapPower"].BaseValue, Owner.Creature, this);
    }
}