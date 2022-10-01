using Godot;
using System;

public class PlayerFollowingEnemy : AbstractEnemy
{

    public override void Move(Physics2DDirectBodyState bodyState){
        Vector2 moveDir = (_playerNode.Position - Position).Normalized();
        moveDir = moveDir * moveSpeed;
        bodyState.LinearVelocity = moveDir;
    }
}
