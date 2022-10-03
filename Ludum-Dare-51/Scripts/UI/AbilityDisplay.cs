using Godot;

public class AbilityDisplay : Control {
    [Export]
    public AbilityType abilityType;
    [Export]
    public int totalMaxLevels = 10;

    private AbilityHandler playerAbilityHandler;
    private TextureRect icon;
    private ColorRect[] leds;

    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    private Color inactiveColor = new Color(1f, 1f, 1f, 0.25f);

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");

        this.icon = GetNode<TextureRect>("Icon");

        this.leds = new ColorRect[totalMaxLevels];

        for (int i = 0; i < totalMaxLevels; i += 1) {
            this.leds[i] = GetNode<ColorRect>("LED" + (i + 1));
        }

        this.UpdateUI();
    }

    public override void _Process(float delta) {
        this.UpdateUI();
    }

    private void UpdateUI() {
        this.icon.Modulate = this.playerAbilityHandler.HasAbility(abilityType) ? activeColor : inactiveColor;

        int totalAbilityLevel = this.playerAbilityHandler.GetAbilityLevel(abilityType);

        for (int i = 0; i < totalMaxLevels; i += 1) {
            this.leds[i].Visible = totalAbilityLevel > i;
        }
    }
}
