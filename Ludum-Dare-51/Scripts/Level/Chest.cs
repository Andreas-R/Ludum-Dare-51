using Godot;
using System.Collections.Generic;

public class Chest : Node2D {
    [Export]
    public float detectionRadius = 150f;

    private AbilityUpgradeHandler abilityUpgradeHandler;
    private Player player;
    private Sprite chestSprite;
    private UpgradeMenu upgradeMenu;

    private bool looted;

    public override void _Ready() {
        this.abilityUpgradeHandler = GetTree().Root.GetNode<AbilityUpgradeHandler>("Main/AbilityUpgradeHandler");
        this.player = GetTree().Root.GetNode<Player>("Main/Player");
        this.chestSprite = GetNode<Sprite>("Sprite");
        this.upgradeMenu = GetNode<UpgradeMenu>("MenuParent/UpgradeMenu");
    }

    public override void _Process(float delta) {
        if (!this.Visible) return;
        if (this.looted) return;

        if ((this.player.GlobalPosition - this.GlobalPosition).LengthSquared() <= this.detectionRadius * this.detectionRadius) {
            if (!this.upgradeMenu.Visible) {
                this.upgradeMenu.Visible = true;
                this.UpdateChestFrame();
            }
        } else {
            if (this.upgradeMenu.Visible) {
                this.upgradeMenu.Visible = false;
                this.UpdateChestFrame();
            }
        }
    }

    private void UpdateChestFrame() {
        this.chestSprite.Frame = this.upgradeMenu.Visible ? 1 : 0;
    }

    public void Spawn(List<AbilityUpgradeHandler.AbilityUpgrade> possibleUpgrades) {
        this.Visible = true;
        this.upgradeMenu.LoadNewUpgrades(possibleUpgrades);
        this.looted = false;
    }

    public void Despawn() {
        this.looted = true;
        this.Visible = false;
        this.upgradeMenu.Visible = false;
        this.UpdateChestFrame();
    }

    public void OnLoot(AbilityType abilityType, int upgradeType) {
        this.looted = true;
        this.upgradeMenu.Visible = false;
        this.chestSprite.Frame = 1;

        this.abilityUpgradeHandler.playerAbilityHandler.AddAbility(abilityType, upgradeType);
    }

    public void OnHealLoot() {
        this.looted = true;
        this.upgradeMenu.Visible = false;
        this.chestSprite.Frame = 1;
    }
}