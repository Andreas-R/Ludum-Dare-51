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

    public AnimatedSprite _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playerNode = GetNode<RigidBody2D>(playerNodePath);
        _currentHealth = maxHealth;
        _sprite = GetNode<AnimatedSprite>("Sprite");
        OnReady();
    }

    public virtual void OnReady(){}

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

    public virtual void CollisionEnter(Node body){
    }

    public void Die(){
        OnDeath();
        GD.Print("Oh no. I am dead");
        QueueFree();
    }

    public void StartMoveAnimation() {
        _sprite.Frame = 0;
        _sprite.Playing = true;
        _sprite.Play();
    }

    protected void HandleSpriteFlip(Vector2 movementInput) {
        if (movementInput.x > 0) {
            _sprite.FlipH = true;
        }
        if (movementInput.x < 0) {
            _sprite.FlipH = false;
        }
    }


    protected virtual void Move(Physics2DDirectBodyState bodyState){
    }

    protected virtual void OnHit(){
    }

    protected virtual void OnDeath(){
    }
}
