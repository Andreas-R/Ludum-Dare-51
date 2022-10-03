using Godot;
using System.Collections.Generic;

public class UpgradeMenu : TextureRect {
    private AbilityUpgradeHandler abilityUpgradeHandler;
    private Chest chest;
    private TextureButton upgrade1Button;
    private TextureButton upgrade2Button;
    private TextureButton upgrade3Button;
    private ColorRect remainingTime;

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

        this.initialRemainingTimeMarginLeft = this.remainingTime.MarginLeft;
        this.initialRemainingTimeMarginRight = this.remainingTime.MarginRight;
    }

    public override void _Process(float delta) {
        if (!Visible) return;

        this.remainingTime.MarginRight = this.initialRemainingTimeMarginLeft + (this.initialRemainingTimeMarginRight - this.initialRemainingTimeMarginLeft) * (1f - (Metronome.instance.elapsedTime / Metronome.CYCLE_TIME));
    }

    public void LoadNewUpgrades() {
        this.currentUpgrades.Clear();
        this.currentUpgrades = this.abilityUpgradeHandler.GetPossibleUpgrades();
        Utils.Shuffle<AbilityUpgradeHandler.AbilityUpgrade>(this.currentUpgrades);

        if (currentUpgrades.Count > 0) {
            upgrade1Button.TextureNormal = abilityUpgradeHandler.abilityMenuImages[this.currentUpgrades[0].type][this.currentUpgrades[0].upgradeType];
            upgrade1Button.TextureHover = abilityUpgradeHandler.abilityMenuHoveredImages[this.currentUpgrades[0].type][this.currentUpgrades[0].upgradeType];
        } else {
            upgrade1Button.TextureNormal = null;
            upgrade1Button.TextureHover = null;
        }

        if (currentUpgrades.Count > 1) {
            upgrade2Button.TextureNormal = abilityUpgradeHandler.abilityMenuImages[this.currentUpgrades[1].type][this.currentUpgrades[1].upgradeType];
            upgrade2Button.TextureHover = abilityUpgradeHandler.abilityMenuHoveredImages[this.currentUpgrades[1].type][this.currentUpgrades[1].upgradeType];
        } else {
            upgrade2Button.TextureNormal = null;
            upgrade2Button.TextureHover = null;
        }

        if (currentUpgrades.Count > 2) {
            upgrade3Button.TextureNormal = abilityUpgradeHandler.abilityMenuImages[this.currentUpgrades[2].type][this.currentUpgrades[2].upgradeType];
            upgrade3Button.TextureHover = abilityUpgradeHandler.abilityMenuHoveredImages[this.currentUpgrades[2].type][this.currentUpgrades[2].upgradeType];
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
        if (currentUpgrades.Count > 1) {
            chest.OnLoot(this.currentUpgrades[1].type, this.currentUpgrades[1].upgradeType);
        }
    }

    public void Upgrade3Selected() {
        if (currentUpgrades.Count > 2) {
            chest.OnLoot(this.currentUpgrades[2].type, this.currentUpgrades[2].upgradeType);
        }
    }
}
