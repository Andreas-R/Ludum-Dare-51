using Godot;
using System;

public class Spawn : AnimatedSprite
{

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    private bool isSpawned = false;
    private AbstractEnemy enemy;

    public override void _Ready()
    {
        Frame = 0;
        Playing = true;
        Play("StartSpawn");
    }

    public override void _Process(float delta)
    {
        if (isSpawned) return;
        
        if (Metronome.instance.IsBeat(-1, 0)) {
            isSpawned = true;
            GetTree().Root.GetNode<Node>("Main").AddChild(enemy);
            Frame = 0;
            Play("Done");
        }
    }

    public void Kill() {
        if (isSpawned) QueueFree();
    }

    public void SetEnemy(AbstractEnemy enemy) {
        this.enemy = enemy;
    }
}
