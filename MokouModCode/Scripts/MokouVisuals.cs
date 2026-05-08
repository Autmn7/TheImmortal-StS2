using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MokouMod.MokouModCode.Scripts;

[GlobalClass]
public partial class MokouVisuals : NCreatureVisuals
{
    public AnimationTree AnimationTree { get; private set; }

    public AnimationNodeStateMachinePlayback Playback { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        AnimationTree = GetCurrentBody()
            .GetNode<AnimationTree>("AnimationTree");

        Playback = (AnimationNodeStateMachinePlayback)
            AnimationTree.Get("parameters/playback");
    }
}