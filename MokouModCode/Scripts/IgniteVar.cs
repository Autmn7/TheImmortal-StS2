using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MokouMod.MokouModCode.Scripts;

public class IgniteVar : DynamicVar
{
    public const string DefaultName = "Ignite";

    public IgniteVar(decimal value)
        : base(DefaultName, value)
    {
    }

    public IgniteVar(string name, decimal value)
        : base(name, value)
    {
    }
}