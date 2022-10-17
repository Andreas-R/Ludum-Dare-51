using Godot;

public class ChainLightning : Node2D {
    [Export]
    public float baseDamage = 50f;
    [Export]
    public float lengthInPixels = 32f;

    public Node2D target1;
    public Node2D target2;
    public int depth;
    public float damage;

    private Timer deplayTimer;
    private Timer attackTimer;
    private AnimatedSprite lightningConnector;
    private AnimatedSprite paralyzedEffect;

    private Vector2 start;
    private Vector2 end;

    public override void _Ready() {
        this.deplayTimer = GetNode<Timer>("DelayTimer");
        this.attackTimer = GetNode<Timer>("AttackTimer");
        this.lightningConnector = GetNode<AnimatedSprite>("LightningConnector");
        this.paralyzedEffect = GetNode<AnimatedSprite>("ParalyzedEffect");

        this.deplayTimer.WaitTime *= (this.depth + 1);
        this.deplayTimer.WaitTime += Metronome.instance.audioServerDelay;
        this.deplayTimer.Start();
        
        this.lightningConnector.Visible = false;
        this.paralyzedEffect.Visible = false;
    }

    public override void _Process(float delta) {
        this.UpdatePositions();
        this.PlaceChainLightning();
    }

    public void UpdatePositions() {
        if (this.target1 != null && IsInstanceValid(this.target1)) {
            this.start = this.target1.GlobalPosition;
        }
        if (this.target2 != null && IsInstanceValid(this.target2)) {
            this.end = this.target2.GlobalPosition;
        }
    }

    private void PlaceChainLightning() {
        this.GlobalPosition = (this.start + this.end) * 0.5f;
        this.lightningConnector.Rotation = -(this.end - this.start).AngleTo(Vector2.Right);
        this.lightningConnector.Scale = new Vector2((this.end - this.start).Length() / (lengthInPixels), 5f);
        this.paralyzedEffect.GlobalPosition = this.end;
    }

    public void OnAttack() {
        this.lightningConnector.Frame = 0;
        this.lightningConnector.Visible = true;
        this.paralyzedEffect.Visible = true;
        this.lightningConnector.Play("chain");
        if (this.target2 != null && IsInstanceValid(this.target2)) {
            if (this.target2.HasNode("DamageReceiver")) {
                DamageReceiver damageReceiver = this.target2.GetNode<DamageReceiver>("DamageReceiver");
                damageReceiver.Damage(damage, Vector2.Zero, 0f);
            }
        }
        this.attackTimer.Start();
    }

    public void OnAttackEnd() {
        this.QueueFree();
    }
}
