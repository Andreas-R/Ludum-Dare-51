using Godot;
using System;

public class Metronome : Node {
    public static float CYCLE_TIME = 10;

    private float elappsedTime;
    private float lastTime;

    public override void _Ready() {
        elappsedTime = 0;
        lastTime = 0;
    }

    public override void _Process(float delta) {
        lastTime = elappsedTime;
        elappsedTime += delta;
        if (elappsedTime >= CYCLE_TIME) {
            elappsedTime -= CYCLE_TIME;
        }
    }

    public bool IsFrame(float cycleTime) {
        if (elappsedTime >= lastTime) {
            return cycleTime > lastTime && cycleTime <= elappsedTime;
        } else {
            return cycleTime > lastTime || cycleTime <= elappsedTime;
        }
    }
}
