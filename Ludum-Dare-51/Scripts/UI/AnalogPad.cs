using Godot;

public abstract class AnalogPad : Control {
    private TextureRect analogKnob1;
    private TextureRect analogKnob2;

    private int touchIndex = -1;
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private float deadZone = 10;
    private float maxAnalogDistance = 100;
    private float deadZoneSquared;
    private float maxAnalogDistanceSquared;

    public override void _Ready() {
        analogKnob1 = GetNode<TextureRect>("AnalogKnob1");
        analogKnob2 = GetNode<TextureRect>("AnalogKnob2");

        analogKnob1.Visible = false;
        analogKnob2.Visible = false;

        deadZoneSquared = deadZone * deadZone;
        maxAnalogDistanceSquared = maxAnalogDistance * maxAnalogDistance;
    }

    public override void _Process(float delta) {
        if (touchIndex != -1) {
            if (!analogKnob1.Visible) analogKnob1.Visible = true;
            if (!analogKnob2.Visible) analogKnob2.Visible = true;

            analogKnob1.RectPosition = RectPosition + startTouchPosition;
            analogKnob2.RectPosition = RectPosition + currentTouchPosition;
        } else {
            if (analogKnob1.Visible) analogKnob1.Visible = false;
            if (analogKnob2.Visible) analogKnob2.Visible = false;
        }
    }

    public override void _Input(InputEvent inputEvent) {
        if (inputEvent is InputEventScreenTouch touchEvent) {
            // handle new touch event
            if (this.touchIndex == -1 && touchEvent.Pressed && this.IsInBounds(touchEvent.Position)) {
                this.touchIndex = touchEvent.Index;
                this.startTouchPosition = touchEvent.Position;
                this.currentTouchPosition = touchEvent.Position;
            }
            // release current touch event
            else if (!touchEvent.Pressed && touchEvent.Index == this.touchIndex) {
                this.touchIndex = -1;
                this.startTouchPosition = Vector2.Zero;
                this.currentTouchPosition = Vector2.Zero;
            }
        }
        if (inputEvent is InputEventScreenDrag dragEvent) {
            // handle continuous touch event
            if (dragEvent.Index == this.touchIndex) {
                this.currentTouchPosition = dragEvent.Position;
                if ((this.currentTouchPosition - this.startTouchPosition).LengthSquared() > maxAnalogDistanceSquared) {
                    this.currentTouchPosition = this.startTouchPosition + (this.currentTouchPosition - this.startTouchPosition).Normalized() * maxAnalogDistance;
                }
            }
        }
    }

    protected abstract bool IsInBounds(Vector2 position);

    public Vector2 GetInput(bool normalized = true) {
        Vector2 offset = this.currentTouchPosition - this.startTouchPosition;
        if (offset.LengthSquared() < deadZoneSquared) {
            return Vector2.Zero;
        } else {
            if (normalized) {
                return offset.Normalized();
            } else {
                return offset / maxAnalogDistance;
            }
        }
    }
}
