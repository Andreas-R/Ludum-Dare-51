using Godot;

public class Sword : Node2D {
    [Export]
    public float attackArchAngle = 200f;

    private Timer attackTimer;
    private Sprite swordSprite;
    private CollisionShape2D swordCollider;
    private Player player;
    private bool isAttacking = false;
    private float startRotation;
    private float attackArchAnglRadians;
    private float lastSwingDirection = 1f;


    public override void _Ready() {
        this.attackTimer = GetNode<Timer>("AttackTimer");
        this.swordSprite = GetNode<Sprite>("Sword/Sprite");
        this.swordCollider = GetNode<CollisionShape2D>("Sword/Collider");
        this.player = GetParent<Player>();

        this.attackArchAnglRadians = Mathf.Deg2Rad(attackArchAngle);
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsFrame(-1, 1f - this.attackTimer.WaitTime * 0.5f)) {
            this.InitAttack();
        }

        if (isAttacking) {
            this.ExecuteAttack();
        }
    }

    private void InitAttack() {
        Vector2 attackDir = player.lastNonZeroMoveDir.Normalized();
        this.startRotation = -attackDir.AngleTo(Vector2.Up) + attackArchAnglRadians * 0.5f * lastSwingDirection;
        this.Rotation = startRotation;

        this.isAttacking = true;
        this.swordSprite.Visible = true;
        this.swordCollider.Disabled = false;
        this.attackTimer.Start();
    }

    private void StopAttack() {
        this.isAttacking = false;
        this.swordSprite.Visible = false;
        this.swordCollider.Disabled = true;
        this.lastSwingDirection *= -1f;
    }

    private void ExecuteAttack() {
        float t = 1f - (attackTimer.TimeLeft / attackTimer.WaitTime);
        t = Mathf.Clamp(t * 1.8f - 0.4f, 0f, 1f);
        t = Mathf.SmoothStep(0f, 1f, t);
        this.Rotation = startRotation - attackArchAnglRadians * t * lastSwingDirection;
    }
}
