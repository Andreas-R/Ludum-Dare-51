using Godot;

public class Timeline : Control {
    public static float timelineLength = 1600f;
    public static int numberOfBeatsOnTimeline = 8;

    private static PackedScene largeTickPrefab = ResourceLoader.Load("res://Prefabs/UI/LargeTick.tscn") as PackedScene;
    private static PackedScene smallTickPrefab = ResourceLoader.Load("res://Prefabs/UI/SmallTick.tscn") as PackedScene;

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(-1, 0)) {
            this.SpawnTick(Timeline.largeTickPrefab);
        }
        if (Metronome.instance.IsBeat(-1, 0.5f)) {
            this.SpawnTick(Timeline.smallTickPrefab);
        }
    }

    private void SpawnTick(PackedScene tickPrefab) {
        ColorRect tick = tickPrefab.Instance() as ColorRect;
        AddChild(tick);
    }
}
