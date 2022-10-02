using Godot;

public class DamageReceiver : Area2D {
    [Signal]
    public delegate void OnDamage(float damage, Vector2 direction, float knockbackForce);

    public void Damage(float damage) {
        HandleDamage(damage);
    }

    // Overwrite in child class if knockback is required
    public virtual void HandleDamage(float damage) {
        EmitSignal(nameof(OnDamage), damage, Vector2.Zero, 0f);
    }
}
