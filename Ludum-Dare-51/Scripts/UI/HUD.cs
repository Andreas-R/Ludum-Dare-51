using Godot;

public class HUD : Control {
    private Metronome metronome;
    private AnimationPlayer introAnimationPlayer;
    private ColorRect pauseScreen;
    private bool turnedOn = false;
    private bool gameRunning = false;

    public override void _Ready() {
        metronome = GetTree().Root.GetNode<Metronome>("Main/Metronome");
        introAnimationPlayer = GetNode<AnimationPlayer>("BlackScreen/AnimationPlayer");
        pauseScreen = GetNode<ColorRect>("PauseScreen");
        GetTree().Paused = true;
    }

    public override void _Process(float detla) {
        bool pauseVisible = gameRunning && GetTree().Paused;
        GD.Print(pauseVisible);
        if (pauseScreen.Visible != pauseVisible) pauseScreen.Visible = pauseVisible;
    }

    public void OnPower() {
        if (!turnedOn) {
            GetTree().Paused = true;
            turnedOn = true;
            introAnimationPlayer.Play("turnOn");
        } else {
            GetTree().Paused = true;
            gameRunning = false;
            introAnimationPlayer.Play("turnOff");
        }
    }

    public void OnPause() {
        if (gameRunning) {
            GetTree().Paused = !GetTree().Paused;
        }
    }

    public void OnAnimationFinished(string animationName) {
        if (animationName == "turnOn") {
            this.OnIntroFinished();
        } else if (animationName == "turnOff") {
            GetTree().ReloadCurrentScene();
        }
    }

    public void OnIntroFinished() {
        GetTree().Paused = false;
        metronome.Start();
        gameRunning = true;
    }
}
