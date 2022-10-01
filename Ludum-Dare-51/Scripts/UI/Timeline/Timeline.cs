using Godot;

public class Timeline : Control {
    private static PackedScene largeTickPrefab = ResourceLoader.Load("res://Prefabs/UI/LargeTick.tscn") as PackedScene;
    private static PackedScene smallTickPrefab = ResourceLoader.Load("res://Prefabs/UI/SmallTick.tscn") as PackedScene;

    public override void _Process(float delta) {
        if (Metronome.instance.IsFrame(-1, 0)) {
            this.SpawnTick(largeTickPrefab);
        }
        if (Metronome.instance.IsFrame(-1, 0.5f)) {
            this.SpawnTick(smallTickPrefab);
        }
    }

    private void SpawnTick(PackedScene tickPrefab) {
        ColorRect tick = tickPrefab.Instance() as ColorRect;
        AddChild(tick);
    }
}
