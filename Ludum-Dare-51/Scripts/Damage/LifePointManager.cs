using Godot;
using System;

public class LifePointManager : Node {
    [Export]
    public float maxHealth;

    [Signal]
    public delegate void OnHit(Vector2 direction, float knockbackForce);
    [Signal]
    public delegate void OnDeath();

    public float currentHealth;
    public bool isInvulnerable = false;

    public override void _Ready() {
        currentHealth = maxHealth;
    }

    public void Damage(float damage, Vector2 direction, float knockbackForce) {
        if (isInvulnerable) return;

        currentHealth = Math.Max(0, currentHealth - damage);

        if (currentHealth <= 0){
            EmitSignal(nameof(OnDeath));
        } else {
            EmitSignal(nameof(OnHit), direction, knockbackForce);
        }
    }
}
