using Godot;
using System;

public class DeathEffect : AnimatedSprite
{
    public void Kill() {
       QueueFree();
    }
}
