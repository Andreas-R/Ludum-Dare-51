using Godot;

public class Sword : Node2D {
    [Export]
    public float attackArchAngle = 200f;

    private Timer attackTimer;
    private Sprite swordSprite;
    public AnimatedSprite slash;
    public CollisionShape2D swordCollider;
    private AudioStreamPlayer audioPlayer;
    private Player player;

    private bool isAttacking = false;
    private float startRotation;
    public float attackArchAngleRadians;
    private float lastSwingDirection = 1f;
    private BgMusicHandler bgMusicHandler;

    public override void _Ready() {
        this.attackTimer = GetNode<Timer>("AttackTimer");
        this.swordSprite = GetNode<Sprite>("Sword/Sprite");
        this.slash = GetNode<AnimatedSprite>("Sword/Slash");
        this.swordCollider = GetNode<CollisionShape2D>("Sword/Collider");
        this.audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
        this.player = GetParent<Player>();
        this.bgMusicHandler = GetTree().Root.GetNode<BgMusicHandler>("Main/BgMusicHandler");

        this.attackArchAngleRadians = Mathf.Deg2Rad(this.attackArchAngle);
    }

    public override void _Process(float delta) {
        if (player.IsDead()) return;
        if (Metronome.instance.IsBeatWithAudioDelay(-1, 1f)) {
           this.audioPlayer.Play();
        }

        if (Metronome.instance.IsBeat(-1, 1f - Metronome.instance.TimeToBeat(this.attackTimer.WaitTime * 0.5f))) {
            this.InitAttack();
        }

        if (isAttacking) {
            this.ExecuteAttack();
        }
    }

    private void InitAttack() {
        Vector2 attackDir = (GetGlobalMousePosition() - this.player.GetCenter()).Normalized();
        this.startRotation = -attackDir.AngleTo(Vector2.Up) + this.attackArchAngleRadians * 0.5f * this.lastSwingDirection;
        this.Rotation = this.startRotation;

        this.isAttacking = true;
        this.swordSprite.Visible = true;
        this.swordCollider.SetDeferred("disabled", false);

        this.attackTimer.Start();

        this.slash.Frame = 0;
        this.slash.Playing = true;
        if (this.lastSwingDirection > 0) {
            this.slash.FlipV = true;
        } else {
            this.slash.FlipV = false;
        }
        this.slash.Play();
    }

    private void StopAttack() {
        this.isAttacking = false;
        this.swordSprite.Visible = false;
        this.swordCollider.SetDeferred("disabled", true);
        this.lastSwingDirection *= -1f;
    }

    private void ExecuteAttack() {
        float t = 1f - (attackTimer.TimeLeft / attackTimer.WaitTime);
        t = Mathf.Clamp(t * 1.8f - 0.4f, 0f, 1f);
        t = Mathf.SmoothStep(0f, 1f, t);
        this.Rotation = startRotation - attackArchAngleRadians * t * lastSwingDirection;
    }

    public void setSprite(Texture texture){
        swordSprite.Texture = texture;
    }
}
