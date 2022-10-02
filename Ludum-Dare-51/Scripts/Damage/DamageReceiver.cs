using Godot;

public class DamageReceiver : Area2D {
    [Signal]
    public delegate void OnDamage(float damage, Vector2 direction, float knockbackForce);

    public void Damage(float damage, Vector2 direction, float knockbackForce) {
        EmitSignal(nameof(OnDamage), damage, direction, knockbackForce);
    }
}
