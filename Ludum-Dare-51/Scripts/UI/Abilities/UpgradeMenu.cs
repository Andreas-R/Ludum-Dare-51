using Godot;
using System.Collections.Generic;

public class UpgradeMenu : TextureRect {
    [Export]
    public Texture hpRecoverMenuImage;
    [Export]
    public Texture hpRecoverMenuHoverImage;

    private AbilityUpgradeHandler abilityUpgradeHandler;
    private Chest chest;
    private TextureButton upgrade1Button;
    private TextureButton upgrade2Button;
    private TextureButton upgrade3Button;
    private ColorRect remainingTime;
    private LifePointManager playerLifePointManager;

    private float initialRemainingTimeMarginLeft;
    private float initialRemainingTimeMarginRight;

    private List<AbilityUpgradeHandler.AbilityUpgrade> currentUpgrades = new List<AbilityUpgradeHandler.AbilityUpgrade>();

    public override void _Ready() {
        this.abilityUpgradeHandler = GetTree().Root.GetNode<AbilityUpgradeHandler>("Main/AbilityUpgradeHandler");
        this.chest = GetParent().GetParent<Chest>();
        this.upgrade1Button = GetNode<TextureButton>("Upgrade1");
        this.upgrade2Button = GetNode<TextureButton>("Upgrade2");
        this.upgrade3Button = GetNode<TextureButton>("Upgrade3");
        this.remainingTime = GetNode<ColorRect>("RemainingTime");
        this.playerLifePointManager = GetTree().Root.GetNode<LifePointManager>("Main/Player/LifePointManager");

        this.initialRemainingTimeMarginLeft = this.remainingTime.MarginLeft;
        this.initialRemainingTimeMarginRight = this.remainingTime.MarginRight;

        ControllerTooltipManager.instance.registerTooltipNode(GetNode<Sprite>("Upgrade1/ControllerTooltip"), "r_left");
        ControllerTooltipManager.instance.registerTooltipNode(GetNode<Sprite>("Upgrade2/ControllerTooltip"), "r_up");
        ControllerTooltipManager.instance.registerTooltipNode(GetNode<Sprite>("Upgrade3/ControllerTooltip"), "r_right");
    }

    public override void _Process(float delta) {
        if (!Visible) return;

        if(Input.IsActionJustReleased("select_upgrade_1")){
            Upgrade1Selected();
        }
        else if(Input.IsActionJustReleased("select_upgrade_2")){
            Upgrade2Selected();
        }
        else if(Input.IsActionJustReleased("select_upgrade_3")){
            Upgrade3Selected();
        }

        this.remainingTime.MarginRight = this.initialRemainingTimeMarginLeft + (this.initialRemainingTimeMarginRight - this.initialRemainingTimeMarginLeft) * (1f - (Metronome.instance.elapsedTime / Metronome.CYCLE_TIME));
    }

    public void LoadNewUpgrades(List<AbilityUpgradeHandler.AbilityUpgrade> possibleUpgrades) {
        this.currentUpgrades.Clear();
        this.currentUpgrades = possibleUpgrades;

        if (currentUpgrades.Count > 0) {
            upgrade1Button.TextureNormal = abilityUpgradeHandler.abilityMenuImages[this.currentUpgrades[0].type][this.currentUpgrades[0].upgradeType];
            upgrade1Button.TextureHover = abilityUpgradeHandler.abilityMenuHoveredImages[this.currentUpgrades[0].type][this.currentUpgrades[0].upgradeType];
        } else {
            upgrade1Button.TextureNormal = null;
            upgrade1Button.TextureHover = null;
        }

        upgrade2Button.TextureNormal = hpRecoverMenuImage;
        upgrade2Button.TextureHover = hpRecoverMenuHoverImage;

        if (currentUpgrades.Count > 1) {
            upgrade3Button.TextureNormal = abilityUpgradeHandler.abilityMenuImages[this.currentUpgrades[1].type][this.currentUpgrades[1].upgradeType];
            upgrade3Button.TextureHover = abilityUpgradeHandler.abilityMenuHoveredImages[this.currentUpgrades[1].type][this.currentUpgrades[1].upgradeType];
        } else {
            upgrade3Button.TextureNormal = null;
            upgrade3Button.TextureHover = null;
        }
    }

    public void Upgrade1Selected() {
        if (currentUpgrades.Count > 0) {
            chest.OnLoot(this.currentUpgrades[0].type, this.currentUpgrades[0].upgradeType);
        }
    }

    public void Upgrade2Selected() {
        playerLifePointManager.Heal(playerLifePointManager.maxHealth * 0.4f);
        chest.OnHealLoot();
    }

    public void Upgrade3Selected() {
        if (currentUpgrades.Count > 1) {
            chest.OnLoot(this.currentUpgrades[1].type, this.currentUpgrades[1].upgradeType);
        }
    }
}
