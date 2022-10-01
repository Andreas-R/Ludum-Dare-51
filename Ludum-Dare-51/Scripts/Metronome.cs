using Godot;

public class Metronome : Node {
    public static Metronome instance;

    public static float CYCLE_TIME = 10;

    private float elappsedTime;
    private float lastTime;

    public override void _Ready() {
        instance = this;
        elappsedTime = 0;
        lastTime = 0;
    }

    public override void _ExitTree() {
        instance = null;
    }

    public override void _Process(float delta) {
        lastTime = elappsedTime;
        elappsedTime += delta;
        if (elappsedTime >= CYCLE_TIME) {
            elappsedTime -= CYCLE_TIME;
        }
    }

    public bool IsFrame(int[] cycleSeconds, float[] timesInSecond) {
        for (int i = 0; i < cycleSeconds.Length; i++) {
            for (int j = 0; j < timesInSecond.Length; j++) {
                if (this.IsFrame(cycleSeconds[i], timesInSecond[j])) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFrame(int cycleSecond, float timeInSecond) {
        if (cycleSecond != -1) {
            if (Mathf.FloorToInt(elappsedTime) != cycleSecond && Mathf.FloorToInt(lastTime) != cycleSecond) {
                return false;
            }
        }

        if (cycleSecond == -1) {
            float elappsedTime_timeInSecond = elappsedTime - Mathf.Floor(elappsedTime);
            float lastTime_timeInSecond = lastTime - Mathf.Floor(lastTime);

            if (elappsedTime_timeInSecond >= lastTime_timeInSecond) {
                return cycleSecond > lastTime && cycleSecond <= elappsedTime;
            } else {
                return cycleSecond > lastTime || cycleSecond <= elappsedTime;
            }
        } else {
            float cycleTime = cycleSecond + timeInSecond;

            if (elappsedTime >= lastTime) {
                return cycleTime > lastTime && cycleTime <= elappsedTime;
            } else {
                return cycleTime > lastTime || cycleTime <= elappsedTime;
            }
        }
    }
}
