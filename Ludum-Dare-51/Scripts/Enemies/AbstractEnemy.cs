using Godot;
using System;

public class AbstractEnemy : RigidBody2D
{
    [Export]
    public float moveSpeed;
    [Export]
    public float damage;
    //[Export]
    //public NodePath playerNodePath;
    protected Player _playerNode;

    protected LifePointManager lifePointManager;
    public AnimatedSprite _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playerNode = GetNode<Player>("../Player");
        _sprite = GetNode<AnimatedSprite>("Sprite");
        lifePointManager = GetNode<LifePointManager>("LifePointManager");
        OnReady();
    }

    public virtual void OnReady(){}

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        Move(bodyState);
    }

    public void Damage(float damage){
        lifePointManager.Damage(damage);
        OnHit();
    }

    public virtual void CollisionEnter(Node body){
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
        QueueFree();
        GD.Print("Dead");
    }
}
