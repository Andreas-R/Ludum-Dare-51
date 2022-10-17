using Godot;

public class Fireball : RigidBody2D {
    [Export]
    public float speed;

    public Vector2 direction;

    public override void _Ready() {
        CPUParticles2D particles = GetNode<CPUParticles2D>("CPUParticles2D");
        particles.ScaleAmount *= 1f + (this.Scale.x - 1f) * 0.5f;
        particles.Lifetime *= 1f + (this.Scale.x - 1f) * 0.4f;
    }

    public override void _Process(float delta) {
        this.GlobalPosition += this.direction * this.speed * delta;
    }

    public void OnBodyEntered(Node body) {
        this.OnDestroy();
    }

    public void OnDestroy() {
        QueueFree();
    }
}
