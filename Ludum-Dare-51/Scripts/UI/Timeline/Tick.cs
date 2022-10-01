using Godot;
using System;

public class Tick : ColorRect {
    private float timelineLength = 1600f;
    private float numberOfSecondsOnTimeline = 4f;

    private float thicknessX;
    private float thicknessY;
    private float elapsedTime;

    public override void _Ready() {
        thicknessX = (this.MarginRight - this.MarginLeft) * 0.5f;
        thicknessY = (this.MarginBottom - this.MarginTop) * 0.5f;

        this.elapsedTime = 0f;

        PlaceTick();
    }

    public override void _Process(float delta) {
        this.elapsedTime += delta;

        PlaceTick();
    }

    private void PlaceTick() {
        float t = elapsedTime / numberOfSecondsOnTimeline;
        float xPos = ((-timelineLength) * t + (timelineLength) * (1f - t)) * 0.5f;

        this.MarginRight = xPos + thicknessX;
        this.MarginLeft = xPos - thicknessX;

        float alpha = 1f - (Mathf.Abs(t - 0.5f) * 2f);
        alpha = Mathf.SmoothStep(0f, 1f, alpha);

        Color color = this.Color;
        color.a = alpha;
        this.Color = color;

        if (t > 1f) {
            QueueFree();
        }
    }
}
