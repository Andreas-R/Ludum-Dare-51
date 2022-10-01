using Godot;

public class Metronome : Node {
    public static Metronome instance;

    public static int CYCLE_TIME = 10;

    public float elapsedTime;
    private float lastTime;

    public override void _Ready() {
        instance = this;
        elapsedTime = 0;
        lastTime = 0;
    }

    public override void _ExitTree() {
        instance = null;
    }

    public override void _Process(float delta) {
        lastTime = elapsedTime;
        elapsedTime += delta;
        if (elapsedTime >= CYCLE_TIME) {
            elapsedTime -= CYCLE_TIME;
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

    public bool IsFrame(int cycleSecond, float timeInSecond, float delay = 0f) {
        timeInSecond -= delay;

        if (timeInSecond < 0f) {
            int negativeSconds = Mathf.FloorToInt(timeInSecond);
            timeInSecond -= negativeSconds;
            cycleSecond += negativeSconds;
            
            while (cycleSecond < 0) {
                cycleSecond += CYCLE_TIME;
            }
        }

        if (cycleSecond != -1) {
            if (Mathf.FloorToInt(elapsedTime) != cycleSecond && Mathf.FloorToInt(lastTime) != cycleSecond) {
                return false;
            }
        }

        if (cycleSecond == -1) {
            float elapsedTime_timeInSecond = elapsedTime - Mathf.Floor(elapsedTime);
            float lastTime_timeInSecond = lastTime - Mathf.Floor(lastTime);

            if (elapsedTime_timeInSecond >= lastTime_timeInSecond) {
                return timeInSecond > lastTime_timeInSecond && timeInSecond <= elapsedTime_timeInSecond;
            } else {
                return timeInSecond > lastTime_timeInSecond || timeInSecond <= elapsedTime_timeInSecond;
            }
        } else {
            float cycleTime = cycleSecond + timeInSecond;

            if (elapsedTime >= lastTime) {
                return cycleTime > lastTime && cycleTime <= elapsedTime;
            } else {
                return cycleTime > lastTime || cycleTime <= elapsedTime;
            }
        }
    }
}
