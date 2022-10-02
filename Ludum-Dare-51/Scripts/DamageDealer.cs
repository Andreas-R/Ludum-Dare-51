using Godot;

public class DamageDealer : Area2D {
    [Export]
    public float damage;

    public void OnArea2DEnter(Area2D area2D) {
        DamageReceiver receiver = area2D as DamageReceiver;

        if (receiver != null) {
            receiver.Damage(damage);
        }
    }
}
