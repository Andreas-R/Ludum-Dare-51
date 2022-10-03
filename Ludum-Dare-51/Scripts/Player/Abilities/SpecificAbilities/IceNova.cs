using Godot;

public class IceNova : DamageDealer {
    [Export]
    public float baseDamage;

    private Player player;
    private AnimatedSprite animatedSprite;
    private CollisionShape2D collider;
    private Timer attackTimer;
    
    public override void _Ready() {
        this.player = GetParent<Player>();
        this.animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        this.collider = GetNode<CollisionShape2D>("Collider");
        this.attackTimer = GetNode<Timer>("AttackTimer");

        animatedSprite.Visible = false;
        this.collider.SetDeferred("disabled", true);
    }

    public override void HandleDamage(DamageReceiver receiver, float damage) {
        receiver.Damage(damage, (receiver.GlobalPosition - this.player.GetCenter()).Normalized(), 200f);
    }

    public void OnAttack() {
        this.collider.SetDeferred("disabled", false);
        animatedSprite.Frame = 0;
        animatedSprite.Play("nova");
        animatedSprite.Visible = true;
        this.attackTimer.Start();
    }

    public void OnAttackEnd() {
        this.collider.SetDeferred("disabled", true);
        animatedSprite.Visible = false;
    }
}
