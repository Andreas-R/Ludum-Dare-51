using Godot;

public class Metronome : Node {
    public static Metronome instance;

    public static int CYCLE_TIME = 10;
    public static int BEATS_PER_CYCLE = 8;

    public float elapsedTime;
    private float lastTime;
    
    public float currentBeat;
    public float lastBeat;

    public override void _Ready() {
        instance = this;
        elapsedTime = -1f;
        lastTime = 0f;
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

        lastBeat = TimeToBeat(lastTime);
        currentBeat = TimeToBeat(elapsedTime);
    }

    public bool IsBeat(int[] beats, float[] timesInBeat) {
        for (int i = 0; i < beats.Length; i++) {
            for (int j = 0; j < timesInBeat.Length; j++) {
                if (this.IsBeat(beats[i], timesInBeat[j])) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsBeat(int beat, float timeInBeat, float delayInSecond = 0f) {
        timeInBeat -= TimeToBeat(delayInSecond);

        if (timeInBeat < 0f) {
            int negativeBeats = Mathf.FloorToInt(timeInBeat);
            timeInBeat -= negativeBeats;
            beat += negativeBeats;
            
            while (beat < 0) {
                beat += CYCLE_TIME;
            }
        }

        if (beat != -1) {
            if (Mathf.FloorToInt(currentBeat) != beat && Mathf.FloorToInt(lastBeat) != beat) {
                return false;
            }
        }

        if (beat == -1) {
            float currentBeat_timeInSecond = currentBeat - Mathf.Floor(currentBeat);
            float lastBeat_timeInSecond = lastBeat - Mathf.Floor(lastBeat);

            if (currentBeat_timeInSecond >= lastBeat_timeInSecond) {
                return timeInBeat > lastBeat_timeInSecond && timeInBeat <= currentBeat_timeInSecond;
            } else {
                return timeInBeat > lastBeat_timeInSecond || timeInBeat <= currentBeat_timeInSecond;
            }
        } else {
            float cycleTime = beat + timeInBeat;

            if (currentBeat >= lastBeat) {
                return cycleTime > lastBeat && cycleTime <= currentBeat;
            } else {
                return cycleTime > lastBeat || cycleTime <= currentBeat;
            }
        }
    }

    public float TimeToBeat(float time) {
        return time * ((float) BEATS_PER_CYCLE / (float) CYCLE_TIME);
    }
}
