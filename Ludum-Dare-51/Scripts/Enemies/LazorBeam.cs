using Godot;

public class LazorBeam : Node2D {
    public AnimatedSprite sprite;
    public override void _Ready() {
        this.sprite = GetNode<AnimatedSprite>("Sprite");
    }
}
