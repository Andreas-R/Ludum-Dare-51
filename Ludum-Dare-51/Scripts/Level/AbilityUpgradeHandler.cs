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
    [Export]
    public Texture[] fireBallUpgradeMenuHoveredImages;
    [Export]
    public Texture[] iceNovaUpgradeMenuImages;
    [Export]
    public Texture[] iceNovaUpgradeMenuHoveredImages;
    [Export]
    public Texture[] chainLightningUpgradeMenuImages;
    [Export]
    public Texture[] chainLightningUpgradeMenuHoveredImages;

    public AbilityHandler playerAbilityHandler;
    public Dictionary<AbilityType, Texture[]> abilityMenuImages = new Dictionary<AbilityType, Texture[]>();
    public Dictionary<AbilityType, Texture[]> abilityMenuHoveredImages = new Dictionary<AbilityType, Texture[]>();

    public override void _Ready() {
        this.playerAbilityHandler = GetTree().Root.GetNode<AbilityHandler>("Main/Player/AbilityHandler");

        // add all ability images here
        this.abilityMenuImages.Add(AbilityType.FIREBALL, fireBallUpgradeMenuImages);
        this.abilityMenuImages.Add(AbilityType.ICE_NOVA, iceNovaUpgradeMenuImages);
        this.abilityMenuImages.Add(AbilityType.CHAIN_LIGHTNING, chainLightningUpgradeMenuImages);

        this.abilityMenuHoveredImages.Add(AbilityType.FIREBALL, fireBallUpgradeMenuHoveredImages);
        this.abilityMenuHoveredImages.Add(AbilityType.ICE_NOVA, iceNovaUpgradeMenuHoveredImages);
        this.abilityMenuHoveredImages.Add(AbilityType.CHAIN_LIGHTNING, chainLightningUpgradeMenuHoveredImages);
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
