using Godot;
using System.Collections.Generic;

public class Timeline : Control {
    private static PackedScene largeTickPrefab = ResourceLoader.Load("res://Prefabs/UI/LargeTick.tscn") as PackedScene;
    private static PackedScene smallTickPrefab = ResourceLoader.Load("res://Prefabs/UI/SmallTick.tscn") as PackedScene;
    private static PackedScene imageTickPrefab = ResourceLoader.Load("res://Prefabs/UI/ImageTick.tscn") as PackedScene;
    private static PackedScene splitTickPrefab = ResourceLoader.Load("res://Prefabs/UI/SplitTick.tscn") as PackedScene;

    public static float timelineLength = 1470f;
    public static int numberOfBeatsOnTimeline = 8;

    [Export]
    public Texture fireBallTickImage;
    [Export]
    public Texture iceNovaTickImage;
    [Export]
    public Texture chainLightningTickImage;

    private AbilityHandler playerAbilityHandler;

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(0, 0, -Metronome.instance.BeatToTime(numberOfBeatsOnTimeline * 0.5f))) {
            this.SpawnSplitTick();
        } else if (Metronome.instance.IsBeat(-1, 0)) {
            this.SpawnTick(Timeline.largeTickPrefab);
        }

        if (Metronome.instance.IsBeat(-1, 0.5f)) {
            this.SpawnTick(Timeline.smallTickPrefab);
        }

        foreach (KeyValuePair<AbilityType, AbstractAbility> typeAndAbility in this.playerAbilityHandler.abilities) {
            if (Metronome.instance.IsBeat(typeAndAbility.Value.GetBeatFrequency(), typeAndAbility.Value.GetSubBeatFrequency(), -Metronome.instance.BeatToTime(numberOfBeatsOnTimeline * 0.5f))) {
                switch(typeAndAbility.Key) {
                    case AbilityType.FIREBALL: {
                        this.SpawnImageTick(fireBallTickImage);
                        break;
                    }
                    case AbilityType.ICE_NOVA: {
                        this.SpawnImageTick(iceNovaTickImage);
                        break;
                    }
                    case AbilityType.CHAIN_LIGHTNING: {
                        this.SpawnImageTick(chainLightningTickImage);
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

    private void SpawnSplitTick() {
        TextureRect tick = splitTickPrefab.Instance() as TextureRect;
        this.AddChild(tick);
    }
}
