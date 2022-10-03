using Godot;
using System.Collections.Generic;

public class Chest : Sprite {
    [Export]
    public float detectionRadius = 150f;

    private AbilityUpgradeHandler abilityUpgradeHandler;
    private Player player;
    private Sprite toolTip;
    private UpgradeMenu upgradeMenu;

    private bool looted;

    public override void _Ready() {
        this.abilityUpgradeHandler = GetTree().Root.GetNode<AbilityUpgradeHandler>("Main/AbilityUpgradeHandler");
        this.player = GetTree().Root.GetNode<Player>("Main/Player");
        this.toolTip = GetNode<Sprite>("ToolTip");
        this.upgradeMenu = GetNode<UpgradeMenu>("MenuParent/UpgradeMenu");
    }

    public override void _Process(float delta) {
        if (!this.Visible) return;
        if (this.looted) return;

        if ((this.player.GlobalPosition - this.GlobalPosition).LengthSquared() <= this.detectionRadius * this.detectionRadius) {
            if (!this.toolTip.Visible) this.toolTip.Visible = true;

            if (Input.IsActionJustPressed("interact")) {
                this.upgradeMenu.Visible = !this.upgradeMenu.Visible;
                this.UpdateChestFrame();
            }
        } else {
            if (toolTip.Visible) toolTip.Visible = false;
            if (this.upgradeMenu.Visible) {
                this.upgradeMenu.Visible = false;
                this.UpdateChestFrame();
            }
        }
    }

    private void UpdateChestFrame() {
        this.Frame = this.upgradeMenu.Visible ? 1 : 0;
    }

    public void Spawn(List<AbilityUpgradeHandler.AbilityUpgrade> possibleUpgrades) {
        this.Visible = true;
        this.upgradeMenu.LoadNewUpgrades(possibleUpgrades);
        this.looted = false;
    }

    public void Despawn() {
        this.looted = true;
        this.Visible = false;
        this.toolTip.Visible = false;
        this.upgradeMenu.Visible = false;
        this.UpdateChestFrame();
    }

    public void OnLoot(AbilityType abilityType, int upgradeType) {
        this.looted = true;
        this.toolTip.Visible = false;
        this.upgradeMenu.Visible = false;
        this.Frame = 1;

        this.abilityUpgradeHandler.playerAbilityHandler.AddAbility(abilityType, upgradeType);
    }

    public void OnHealLoot() {
        this.looted = true;
        this.toolTip.Visible = false;
        this.upgradeMenu.Visible = false;
        this.Frame = 1;
    }
}