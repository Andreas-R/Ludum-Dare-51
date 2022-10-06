using Godot;
using System.Collections.Generic;

public class TouchPad : Control {
    private int touchIndex = -1;
    private float minOffset = 5;
    private float minOffsetSquared;
    private float maxLength = 100;
    private float maxLengthSquared;

    private List<Vector2> lastTouches = new List<Vector2>();
    private int maxTouches = 15;

    public override void _Ready() {
        minOffsetSquared = minOffset * minOffset;
        maxLengthSquared = maxLength * maxLength;
    }

    public override void _Input(InputEvent inputEvent) {
        if (inputEvent is InputEventScreenTouch touchEvent) {
            // handle new touch event
            if (this.touchIndex == -1 && touchEvent.Pressed) {
                this.touchIndex = touchEvent.Index;
                this.lastTouches.Add(touchEvent.Position);
            }
            // release current touch event
            else if (!touchEvent.Pressed && touchEvent.Index == this.touchIndex) {
                this.touchIndex = -1;
                this.lastTouches.Clear();
            }
        }
        if (inputEvent is InputEventScreenDrag dragEvent) {
            // handle continuous touch event
            if (dragEvent.Index == this.touchIndex) {
                if ((dragEvent.Position - this.lastTouches[this.lastTouches.Count - 1]).LengthSquared() >= minOffsetSquared) {
                    this.lastTouches.Add(dragEvent.Position);
                    if (this.lastTouches.Count > maxTouches) {
                        this.lastTouches.RemoveAt(0);
                    }
                }
            }
        }
    }

    public Vector2 GetInput(bool normalized = true) {
        if (this.lastTouches.Count > 2) {
            Vector2 direction = this.lastTouches[this.lastTouches.Count - 1] - this.lastTouches[0];
            if (direction.LengthSquared() > maxLengthSquared) {
                direction = direction.Normalized() * maxLength;
            }
            return direction / maxLength;
        } else {
            return Vector2.Zero;
        }
    }
}
