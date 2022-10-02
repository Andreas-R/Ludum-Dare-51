using Godot;

public class DamageDealer : Area2D {
    [Export]
    public float damage;

    public void OnArea2DEnter(Area2D area2D) {
        DamageReceiver receiver = area2D as DamageReceiver;

        if (receiver != null) {
            this.HandleDamage(receiver, damage);
        }
    }

    // Overwrite in child class if knockback is required
    public virtual void HandleDamage(DamageReceiver receiver, float damage) {
        receiver.Damage(damage, Vector2.Zero, 0f);
    }
}
