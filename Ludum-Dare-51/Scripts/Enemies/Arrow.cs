using Godot;
using System;

public class Arrow : KinematicBody2D
{
    public Vector2 target;
    [Export]
    public float moveSpeed;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Vector2 move = (target - Position);
        if (move.Length() > 2f){
            var collision = MoveAndCollide(move.Normalized() * delta * moveSpeed);
            if(collision != null){
                //GD.Print("Hit");
                //TODO: Check if hit was Player & damage them
                QueueFree();
            }
        }
        else{
            QueueFree();
        }
    }
}
