using Godot;
using System.Collections.Generic;

public class AbilityHandler : Node {
    private static int ABILITY_MAX_LEVEL = 5;
    private static int MAX_EQUIPPED_ABILITIES = 3;

    private Player player;
    public Dictionary<AbilityType, AbstractAbility> abilities = new Dictionary<AbilityType, AbstractAbility>();

    public override void _Ready() {
        this.player = GetParent<Player>();
    }

    public override void _Process(float delta) {
        foreach (AbstractAbility ability in this.abilities.Values) {
            ability.OnProcess(player, delta);
        }
    }

    public bool HasAbility(AbilityType abilityType) {
        return this.abilities.ContainsKey(abilityType);
    }

    public bool CanIncreaseAbility(AbilityType abilityType, int upgradeType) {
        if (!this.abilities.ContainsKey(abilityType)) {
            return true;
        }

        switch (upgradeType) {
            case 1: {
                return this.abilities[abilityType].level1 < this.abilities[abilityType].level1Max;
            }
            case 2: {
                return this.abilities[abilityType].level2 < this.abilities[abilityType].level2Max;
            }
            case 3: {
                return this.abilities[abilityType].level3 < this.abilities[abilityType].level3Max;
            }
        }

        return false;
    }

    public bool AddAbility(AbilityType abilityType, int upgradeType) {
        if (this.abilities.ContainsKey(abilityType)) {
            if (this.abilities[abilityType].GetTotalLevel() >= ABILITY_MAX_LEVEL) {
                //return false; // TODO? - limit ability level
            }
            return IncreaseAbilityLevel(abilityType, upgradeType);
        } else if (upgradeType == 0 && this.abilities.Count < MAX_EQUIPPED_ABILITIES) {
            switch (abilityType) {
                case AbilityType.FIREBALL: {
                    this.abilities[abilityType] = new FireballAbility();
                    return true;
                }
                case AbilityType.ICE_NOVA: {
                    this.abilities[abilityType] = new IceNovaAbility();
                    return true;
                }
                default: {
                    return false;
                }
            }
        } else {
            return false;
        }
    }

    private bool IncreaseAbilityLevel(AbilityType abilityType, int upgradeType) {
        switch (upgradeType) {
            case 1: {
                if (this.abilities[abilityType].level1 < this.abilities[abilityType].level1Max) {
                    this.abilities[abilityType].level1 += 1;
                    return true;
                }
                break;
            }
            case 2: {
                if (this.abilities[abilityType].level2 < this.abilities[abilityType].level2Max) {
                    this.abilities[abilityType].level2 += 1;
                    return true;
                }
                break;
            }
            case 3: {
                if (this.abilities[abilityType].level3 < this.abilities[abilityType].level3Max) {
                    this.abilities[abilityType].level3 += 1;
                    return true;
                }
                break;
            }
        }

        return false;
    }

    public bool RemoveAbility(AbilityType abilityType) {
        if (this.abilities.ContainsKey(abilityType)) {
            this.abilities.Remove(abilityType);
            return true;
        } else {
            return false;
        }
    }
}
