using Godot;

public class AbilityDisplay : Control {
    [Export]
    public AbilityType abilityType;

    private AbilityHandler playerAbilityHandler;
    private TextureRect icon;
    private ColorRect[] leds;

    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    private Color inactiveColor = new Color(1f, 1f, 1f, 0.25f);

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");

        this.icon = GetNode<TextureRect>("Icon");

        this.leds = new ColorRect[10];

        for (int i = 0; i < 10; i += 1) {
            this.leds[i] = GetNode<ColorRect>("LED" + (i + 1));
        }
    }

    public override void _Process(float delta) {
        this.icon.Modulate = this.playerAbilityHandler.HasAbility(abilityType) ? activeColor : inactiveColor;

        int totalAbilityLevel = this.playerAbilityHandler.GetAbilityLevel(abilityType);

        for (int i = 0; i < 10; i += 1) {
            this.leds[i].Visible = totalAbilityLevel > i;
        }
    }
}
