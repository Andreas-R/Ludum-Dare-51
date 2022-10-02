using Godot;

public class LazorBeam : RigidBody2D {
    public AnimatedSprite sprite;
    public override void _Ready() {
        this.sprite = GetNode<AnimatedSprite>("Sprite");
    }
}
