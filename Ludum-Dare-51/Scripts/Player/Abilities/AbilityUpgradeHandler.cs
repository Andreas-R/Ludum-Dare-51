using Godot;
using System;
using System.Collections.Generic;

public class AbilityUpgradeHandler : Node {
    public class AbilityUpgrade {
        public AbilityType type;
        public int upgradeType;

        public AbilityUpgrade(AbilityType type, int upgradeType) {
            this.type = type;
            this.upgradeType = upgradeType;
        }
    }

    [Export]
    public Texture[] fireBallUpgradeMenuImages;

    private AbilityHandler playerAbilityHandler;
    private Dictionary<AbilityType, Texture[]> abilityMenuImages = new Dictionary<AbilityType, Texture[]>();

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");

        // add all ability images here
        this.abilityMenuImages.Add(AbilityType.FIREBALL, fireBallUpgradeMenuImages);
    }

    public List<AbilityUpgrade> GetPossibleUpgrades() {
        List<AbilityUpgrade> possibleUpgrades = new List<AbilityUpgrade>();

        foreach (AbilityType abilityType in Enum.GetValues(typeof(AbilityType))) {
            if (!playerAbilityHandler.HasAbility(abilityType)) {
                possibleUpgrades.Add(new AbilityUpgrade(abilityType, 0));
            } else {
                for (int i = 1; i <= 3; i += 1) {
                    if (playerAbilityHandler.CanIncreaseAbility(abilityType, i)) {
                        possibleUpgrades.Add(new AbilityUpgrade(abilityType, i));
                    }
                }
            }
        }

        return possibleUpgrades;
    }
}