using Godot;

public class LevelManager : Node {
    private RoomHandler roomHandler;

    public override void _Ready() {
        this.roomHandler = GetTree().Root.GetNode<RoomHandler>("Main/RoomHandler");
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(0, 0)) {
            this.roomHandler.ChangeToRandomRoom();
        }
    }
}
