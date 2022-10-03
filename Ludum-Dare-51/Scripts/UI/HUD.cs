using Godot;

public class HUD : Control {
    private AnimationPlayer introAnimationPlayer;
    private bool turnedOn = false;
    private bool gameRunning = false;

    public override void _Ready() {
        introAnimationPlayer = GetNode<AnimationPlayer>("BlackScreen/AnimationPlayer");
    }

    public override void _Process(float delta) {
        
    }

    public void OnPower() {
        if (!turnedOn) {
            turnedOn = true;
            introAnimationPlayer.Play("turnOn");
        } else {
            introAnimationPlayer.Play("turnOff");
        }
    }

    public void OnPause() {
        
    }

    public void OnAnimationFinished(string animationName) {
        if (animationName == "turnOn") {
            this.OnIntroFinished();
        } else if (animationName == "turnOff") {
            GetTree().ReloadCurrentScene();
        }
    }

    public void OnIntroFinished() {

    }
}
