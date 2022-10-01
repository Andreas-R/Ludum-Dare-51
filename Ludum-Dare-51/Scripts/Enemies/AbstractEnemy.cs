using Godot;
using System;

public class AbstractEnemy : RigidBody2D
{
    [Export]
    public float maxHealth;
    [Export]
    public float moveSpeed;
    [Export]
    public float damage;
    [Export]
    public NodePath playerNodePath;
    protected RigidBody2D _playerNode;

    public float _currentHealth;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playerNode = GetNode<RigidBody2D>(playerNodePath);
        _currentHealth = maxHealth;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    /* public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Move(delta);
    } */

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        Move(bodyState);
    }

    public void BeAttacked(float damage){
        GD.Print("ouch");
        OnHit();
        GD.Print(_currentHealth);
        _currentHealth -= damage;
        if(_currentHealth <= 0){
            Die();
        }
        
    }

    public void CollisionEnter(){
        GD.Print("collision detected");
    }

    public void Die(){
        OnDeath();
        GD.Print("Oh no. I am dead");
        QueueFree();
    }

    public virtual void Move(Physics2DDirectBodyState bodyState){
    }

    public virtual void OnHit(){
    }

    public virtual void OnDeath(){
    }
}
