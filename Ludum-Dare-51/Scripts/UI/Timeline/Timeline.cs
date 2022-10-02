using Godot;
using System.Collections.Generic;

public class Timeline : Control {
    private static PackedScene largeTickPrefab = ResourceLoader.Load("res://Prefabs/UI/LargeTick.tscn") as PackedScene;
    private static PackedScene smallTickPrefab = ResourceLoader.Load("res://Prefabs/UI/SmallTick.tscn") as PackedScene;
    private static PackedScene imageTickPrefab = ResourceLoader.Load("res://Prefabs/UI/ImageTick.tscn") as PackedScene;

    public static float timelineLength = 1600f;
    public static int numberOfBeatsOnTimeline = 8;

    [Export]
    public Texture fireBallTickImage;

    private AbilityHandler playerAbilityHandler;

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(-1, 0)) {
            this.SpawnTick(Timeline.largeTickPrefab);
        }
        if (Metronome.instance.IsBeat(-1, 0.5f)) {
            this.SpawnTick(Timeline.smallTickPrefab);
        }

        foreach (KeyValuePair<AbilityType, AbstractAbility> typeAndAbility in this.playerAbilityHandler.abilities) {
            if (Metronome.instance.IsBeat(typeAndAbility.Value.GetBeatFrequency(), typeAndAbility.Value.GetSubBeatFrequency())) {
                switch(typeAndAbility.Key) {
                    case AbilityType.FIREBALL: {
                        this.SpawnImageTick(fireBallTickImage);
                        break;
                    }
                }
            }
        }
    }

    private void SpawnTick(PackedScene tickPrefab) {
        ColorRect tick = tickPrefab.Instance() as ColorRect;
        this.AddChild(tick);
    }

    private void SpawnImageTick(Texture tickImage) {
        TextureRect tick = imageTickPrefab.Instance() as TextureRect;
        tick.Texture = tickImage;
        this.AddChild(tick);
    }
}
