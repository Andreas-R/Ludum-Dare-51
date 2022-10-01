using Godot;
using System;

public class Tick : ColorRect {
    private float timelineLength = 1600f;
    private float numberOfSecondsOnTimeline = 4f;

    private float thicknessX;
    private float thicknessY;
    private float elapsedTime;

    public override void _Ready() {
        thicknessX = (MarginRight - MarginLeft) * 0.5f;
        thicknessY = (MarginBottom - MarginTop) * 0.5f;

        elapsedTime = 0f;

        PlaceTick();
    }

    public override void _Process(float delta) {
        elapsedTime += delta;

        PlaceTick();
    }

    private void PlaceTick() {
        float t = elapsedTime / numberOfSecondsOnTimeline;
        float xPos = ((-timelineLength) * t + (timelineLength) * (1f - t)) * 0.5f;

        MarginRight = xPos + thicknessX;
        MarginLeft = xPos - thicknessX;
    }
}
