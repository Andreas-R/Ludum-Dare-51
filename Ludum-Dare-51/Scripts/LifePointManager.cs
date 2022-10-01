using Godot;
using System;

public class LifePointManager : Node
{
    [Export]
    public float maxHealth;
    public float currentHealth;

    [Signal]
    public delegate void OnDeath();

    private Node _parent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentHealth = maxHealth;
        _parent = GetParent();
    }

    public void Damage(float damage){
        currentHealth -= damage;
        if(currentHealth <= 0){
            Die();
        }
    }

    public void Die(){
        EmitSignal(nameof(OnDeath));
    }

}
