public class SwordDamageDealer : DamageDealer {
    private Player player;

    public override void _Ready() {
        this.player = GetTree().Root.GetNode<Player>("Main/Player");
    }

    public override void HandleDamage(DamageReceiver receiver, float damage) {
        receiver.Damage(damage, (receiver.GlobalPosition - this.player.GetCenter()).Normalized(), 500f);
    }
}
