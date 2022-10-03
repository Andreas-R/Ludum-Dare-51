using Godot;
using System;

public class LifeAndAbilitiesPanel : TextureRect {
    private ColorRect lifebar;
    private LifePointManager playerLifePointManager;

    private float lifeBarStartRectSizeX;

    public override void _Ready() {
        lifebar = GetNode<ColorRect>("Lifebar");
        playerLifePointManager = GetTree().Root.GetNode<LifePointManager>("Main/Player/LifePointManager");

        lifeBarStartRectSizeX = lifebar.RectSize.x;
    }

    public override void _Process(float delta) {
        lifebar.RectSize = new Vector2(
            (playerLifePointManager.currentHealth / playerLifePointManager.maxHealth) * lifeBarStartRectSizeX,
            lifebar.RectSize.y
        );
    }
}
