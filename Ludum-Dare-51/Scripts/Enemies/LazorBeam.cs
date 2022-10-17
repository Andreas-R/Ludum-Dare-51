using Godot;

public class LazorBeam : Node2D {
    public AnimatedSprite beamSprite;
    public AnimatedSprite beamEndSprite;

    public override void _Ready() {
        this.beamSprite = GetNode<AnimatedSprite>("BeamSprite");
        this.beamEndSprite = GetNode<AnimatedSprite>("BeamEndSprite");
    }
}
