using Godot;
using System;

public class LifePointManager : Node {
    [Export]
    public float maxHealth;

    [Signal]
    public delegate void OnHit(Vector2 direction, float knockbackForce);
    [Signal]
    public delegate void OnDeath();

    private float currentHealth;

    public override void _Ready() {
        currentHealth = maxHealth;
    }

    public void Damage(float damage, Vector2 direction, float knockbackForce) {
        currentHealth = Math.Max(0, currentHealth - damage);

        if (currentHealth <= 0){
            EmitSignal(nameof(OnDeath));
        } else {
            EmitSignal(nameof(OnHit), direction, knockbackForce);
        }
    }
}
