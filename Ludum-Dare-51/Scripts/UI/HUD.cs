using Godot;

public class HUD : Control {
    private Metronome metronome;
    private AnimationPlayer introAnimationPlayer;
    private AnimationPlayer outroAnimationPlayer;
    private AnimationPlayer powerButtonAnimationPlayer;
    private AudioStreamPlayer buttonAudioStreamPlayer;
    private ColorRect pauseScreen;
    private TextureButton powerButton;
    private bool turnedOn = false;
    private bool gameRunning = false;
    private bool pressedPowerButtonAtLeastOnce = false;

    public override void _Ready() {
        metronome = GetTree().Root.GetNode<Metronome>("Main/Metronome");
        introAnimationPlayer = GetNode<AnimationPlayer>("BlackScreen/AnimationPlayer");
        outroAnimationPlayer = GetNode<AnimationPlayer>("GameOverScreen/AnimationPlayer");
        powerButtonAnimationPlayer = GetNode<AnimationPlayer>("PowerButton/AnimationPlayer");
        buttonAudioStreamPlayer = GetNode<AudioStreamPlayer>("ButtonAudioStreamPlayer");
        pauseScreen = GetNode<ColorRect>("PauseScreen");
        powerButton = GetNode<TextureButton>("PowerButton");
        GetTree().Paused = true;
    }

    public override void _Process(float detla) {
        bool pauseVisible = gameRunning && GetTree().Paused;
        if (pauseScreen.Visible != pauseVisible) pauseScreen.Visible = pauseVisible;
        if (Input.IsActionJustReleased("menu")){
            if(!turnedOn){
                OnPower();
            }
            else{
                OnPause();
            }
        }
    }

    public void OnPower() {
        buttonAudioStreamPlayer.Play();
        pressedPowerButtonAtLeastOnce = true;

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
        buttonAudioStreamPlayer.Play();
        if (gameRunning) {
            GetTree().Paused = !GetTree().Paused;
        }
    }

    public void OnFullScreen() {
        buttonAudioStreamPlayer.Play();
        OS.WindowFullscreen = !OS.WindowFullscreen;
    }

    public void OnAnimationFinished(string animationName) {
        if (animationName == "turnOn") {
            this.OnIntroFinished();
        } else if (animationName == "turnOff") {
            GetTree().ReloadCurrentScene();
        } else if (animationName == "outro") {
            GetTree().ReloadCurrentScene();
        }
    }

    public void OnIntroFinished() {
        GetTree().Paused = false;
        metronome.Start();
        gameRunning = true;
    }

    public void OnGameOver() {
        outroAnimationPlayer.Play("outro");
    }

    public void OnPowerButtonTip() {
        if (!pressedPowerButtonAtLeastOnce) {
            powerButtonAnimationPlayer.Play("pulse");
        }
    }
}
