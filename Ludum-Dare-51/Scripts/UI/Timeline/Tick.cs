using Godot;
using System;

public class Tick : ColorRect {
    private float thicknessX;
    private float thicknessY;
    private float elapsedBeats;

    public override void _Ready() {
        thicknessX = (this.MarginRight - this.MarginLeft) * 0.5f;
        thicknessY = (this.MarginBottom - this.MarginTop) * 0.5f;

        this.elapsedBeats = 0f;

        PlaceTick();
    }

    public override void _Process(float delta) {
        this.elapsedBeats += Metronome.instance.TimeToBeat(delta);

        PlaceTick();
    }

    private void PlaceTick() {
        float t = elapsedBeats / Timeline.numberOfBeatsOnTimeline;
        float xPos = ((-Timeline.timelineLength) * t + (Timeline.timelineLength) * (1f - t)) * 0.5f;

        this.MarginRight = xPos + thicknessX;
        this.MarginLeft = xPos - thicknessX;

        float s = Mathf.Abs((Metronome.instance.currentBeat % 1) - 0.5f) * 2f;
        float u = 0.8f;
        s = (Mathf.Max(u, s) - u) / (1f - u);
        float v = 0.75f;
        s = (v + (1 - v) * s);

        this.MarginBottom = thicknessY * s;
        this.MarginTop = -thicknessY * s;

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
