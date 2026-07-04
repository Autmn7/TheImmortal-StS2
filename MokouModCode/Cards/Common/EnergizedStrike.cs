using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Common;

public class EnergizedStrike : MokouModCard
{
    public EnergizedStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(11, 2);
        WithVars(new HpLossVar(2));
        WithCards(1, 1);
        WithTip(typeof(VigorousEnchantment));
        WithTags(CardTag.Strike);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackUpKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nonLethal = CalculateNonLethal(Owner.Creature, DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, nonLethal, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards =
            (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                PileType.Draw.GetPile(Owner).Cards
                    .Where(model => enchantment.CanEnchant(model))
                    .OrderBy(c => c.Rarity)
                    .ThenBy(c => c.Id)
                    .ToList(),
                Owner,
                new CardSelectorPrefs(new LocString("card_selection", "TO_ENCHANT_VIGOROUS"), DynamicVars.Cards.IntValue)
            )).ToList();
        if (cards.Count != 0)
            foreach (var card in cards)
            {
                var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
                Enchant(newEnchant, card);
            }
    }
}