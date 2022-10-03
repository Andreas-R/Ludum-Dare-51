using Godot;
using System;

public class LifeAndAbilitiesPanel : TextureRect {
    private ColorRect lifebar;
    private Label roomNumber;
    private LifePointManager playerLifePointManager;
    private RoomHandler roomHandler;

    private float lifeBarStartRectSizeX;

    public override void _Ready() {
        lifebar = GetNode<ColorRect>("Lifebar");
        roomNumber = GetNode<Label>("Number");
        playerLifePointManager = GetTree().Root.GetNode<LifePointManager>("Main/Player/LifePointManager");
        roomHandler = GetTree().Root.GetNode<RoomHandler>("Main/RoomHandler");

        lifeBarStartRectSizeX = lifebar.RectSize.x;
    }

    public override void _Process(float delta) {
        lifebar.RectSize = new Vector2(
            (playerLifePointManager.currentHealth / playerLifePointManager.maxHealth) * lifeBarStartRectSizeX,
            lifebar.RectSize.y
        );

        int number = roomHandler.roomCounter;
        string label = number.ToString();

        if (number < 10) {
            label = "00" + label;
        } else if (number < 100) {
            label = "0" + label;
        }

        roomNumber.Text = label;
    }
}
