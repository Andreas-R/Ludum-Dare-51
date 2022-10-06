using Godot;

public class RightAnalogPad : AnalogPad {
    protected override bool IsInBounds(Vector2 position) {
        return position.x > GetViewportRect().Size.x * 0.5f && position.y > GetViewportRect().Size.y * 0.5f;
    }
}
