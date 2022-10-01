using Godot;
using System;

public class Tick : ColorRect {
    private float timelineLength = 1600f;
    private float numberOfSecondsOnTimeline = 5f;

    private float thicknessX;
    private float thicknessY;
    private float elapsedTime;

    public override void _Ready() {
        thicknessX = MarginRight - MarginLeft;
        thicknessY = MarginBottom - MarginTop;

        elapsedTime = 0f;

        PlaceTick();
    }

    public override void _Process(float delta) {
        elapsedTime += delta;

        PlaceTick();
    }

    private void PlaceTick() {
        
    }
}
