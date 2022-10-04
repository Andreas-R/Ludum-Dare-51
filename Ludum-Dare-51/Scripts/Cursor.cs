using Godot;

public class Cursor : Sprite {
    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Process(float delta) {
        GlobalPosition = GetGlobalMousePosition();
    }
}
