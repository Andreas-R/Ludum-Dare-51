using Godot;

public class Metronome : Node {
    public static Metronome instance;

    public static int CYCLE_TIME = 10;
    public static int BEATS_PER_CYCLE = 8;

    public float elapsedTime;
    private float lastTime;
    
    public float currentBeat;
    public float lastBeat;
    public float audioServerDelay;
    public bool isPlaying;

    public override void _Ready() {
        instance = this;
        elapsedTime = 0f;
        lastTime = -1f;
        currentBeat = 0f;
        lastBeat = -1f;
        audioServerDelay = (float) (AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency());
        isPlaying = false;
    }

    public override void _ExitTree() {
        instance = null;
    }

    public override void _Process(float delta) {
        if (!isPlaying) return;

        lastTime = elapsedTime;
        elapsedTime += delta;

        if (elapsedTime >= CYCLE_TIME) {
            elapsedTime -= CYCLE_TIME;
        }

        lastBeat = TimeToBeat(lastTime);
        currentBeat = TimeToBeat(elapsedTime);
    }

    public void Start() {
        isPlaying = true;
    }

    public bool IsBeat(int[] beats, float[] timesInBeat, float delayInSecond = 0f) {
        for (int i = 0; i < beats.Length; i++) {
            for (int j = 0; j < timesInBeat.Length; j++) {
                if (this.IsBeat(beats[i], timesInBeat[j], delayInSecond)) {
                    return true;
                }
            }
        }
        return false;
    }

    
    public bool IsBeatWithAudioDelay(int[] beats, float[] timesInBeat) {
        for (int i = 0; i < beats.Length; i++) {
            for (int j = 0; j < timesInBeat.Length; j++) {
                if (this.IsBeat(beats[i], timesInBeat[j], Mathf.Max(0, audioServerDelay))) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsBeatWithAudioDelay(int beat, float timeInBeat) {
        return this.IsBeat(beat, timeInBeat, Mathf.Max(0, audioServerDelay));
    }

    public bool IsBeat(int beat, float timeInBeat, float delayInSecond = 0f) {
        if (!isPlaying) return false;

        timeInBeat -= TimeToBeat(delayInSecond);

        if (timeInBeat < 0f) {
            int negativeBeats = Mathf.FloorToInt(timeInBeat);
            timeInBeat -= negativeBeats;

            if (beat != -1) {
                beat += negativeBeats;
                
                while (beat < 0) {
                    beat += BEATS_PER_CYCLE;
                }
            }
        }
        if (timeInBeat > 1f) {
            int positiveBeats = Mathf.FloorToInt(timeInBeat);
            timeInBeat -= positiveBeats;

            if (beat != -1) {
                beat += positiveBeats;
                
                while (beat >= BEATS_PER_CYCLE) {
                    beat -= BEATS_PER_CYCLE;
                }
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

    public float BeatToTime(float beat) {
        return beat * ((float) CYCLE_TIME / (float) BEATS_PER_CYCLE);
    }
}
