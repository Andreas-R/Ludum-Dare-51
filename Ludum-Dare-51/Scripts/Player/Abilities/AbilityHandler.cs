using Godot;
using System.Collections.Generic;

public class AbilityHandler : Node {
    private Player player;
    public Dictionary<AbilityType, AbstractAbility> abilities = new Dictionary<AbilityType, AbstractAbility>();

    public override void _Ready() {
        this.player = GetParent<Player>();
        AddAbility(AbilityType.MOVE_SPEED, 0);
        AddAbility(AbilityType.SWORD, 0);

        // AddAbility(AbilityType.MOVE_SPEED, 1);
        // AddAbility(AbilityType.MOVE_SPEED, 1);
        // AddAbility(AbilityType.MOVE_SPEED, 1);
        // AddAbility(AbilityType.MOVE_SPEED, 1);
        // AddAbility(AbilityType.MOVE_SPEED, 1);
        // AddAbility(AbilityType.SWORD, 1);
        // AddAbility(AbilityType.SWORD, 1);
        // AddAbility(AbilityType.SWORD, 1);
        // AddAbility(AbilityType.SWORD, 1);
        // AddAbility(AbilityType.SWORD, 2);
        // AddAbility(AbilityType.SWORD, 2);
        // AddAbility(AbilityType.SWORD, 2);
        // AddAbility(AbilityType.SWORD, 2);
        // AddAbility(AbilityType.SWORD, 3);
        // AddAbility(AbilityType.SWORD, 3);
        // AddAbility(AbilityType.FIREBALL, 0);
        // AddAbility(AbilityType.FIREBALL, 1);
        // AddAbility(AbilityType.FIREBALL, 1);
        // AddAbility(AbilityType.FIREBALL, 1);
        // AddAbility(AbilityType.FIREBALL, 1);
        // AddAbility(AbilityType.FIREBALL, 2);
        // AddAbility(AbilityType.FIREBALL, 2);
        // AddAbility(AbilityType.FIREBALL, 2);
        // AddAbility(AbilityType.FIREBALL, 2);
        // AddAbility(AbilityType.FIREBALL, 3);
        // AddAbility(AbilityType.FIREBALL, 3);
        // AddAbility(AbilityType.ICE_NOVA, 0);
        // AddAbility(AbilityType.ICE_NOVA, 1);
        // AddAbility(AbilityType.ICE_NOVA, 1);
        // AddAbility(AbilityType.ICE_NOVA, 1);
        // AddAbility(AbilityType.ICE_NOVA, 1);
        // AddAbility(AbilityType.ICE_NOVA, 2);
        // AddAbility(AbilityType.ICE_NOVA, 2);
        // AddAbility(AbilityType.ICE_NOVA, 2);
        // AddAbility(AbilityType.ICE_NOVA, 2);
        // AddAbility(AbilityType.ICE_NOVA, 3);
        // AddAbility(AbilityType.ICE_NOVA, 3);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 0);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 1);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 1);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 1);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 1);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 2);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 2);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 2);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 2);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 3);
        // AddAbility(AbilityType.CHAIN_LIGHTNING, 3);
    }

    public override void _Process(float delta) {
        if (player.IsDead()) return;

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
            return IncreaseAbilityLevel(abilityType, upgradeType);
        } else if (upgradeType == 0) {
            switch (abilityType) {
                case AbilityType.FIREBALL: {
                    this.abilities[abilityType] = new FireballAbility();
                    return true;
                }
                case AbilityType.ICE_NOVA: {
                    this.abilities[abilityType] = new IceNovaAbility();
                    return true;
                }
                case AbilityType.CHAIN_LIGHTNING: {
                    this.abilities[abilityType] = new ChainLightningAbility();
                    return true;
                }
                case AbilityType.MOVE_SPEED: {
                    this.abilities[abilityType] = new moveSpeedAbility(player);
                    return true;
                }
                case AbilityType.SWORD: {
                    this.abilities[abilityType] = new SwordAbility(player.GetNode<Sword>("SwordPivot"));
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
                    this.abilities[abilityType].OnUpgrade(1);
                    return true;
                }
                break;
            }
            case 2: {
                if (this.abilities[abilityType].level2 < this.abilities[abilityType].level2Max) {
                    this.abilities[abilityType].level2 += 1;
                    this.abilities[abilityType].OnUpgrade(2);
                    return true;
                }
                break;
            }
            case 3: {
                if (this.abilities[abilityType].level3 < this.abilities[abilityType].level3Max) {
                    this.abilities[abilityType].level3 += 1;
                    this.abilities[abilityType].OnUpgrade(3);
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

    public int GetAbilityLevel(AbilityType abilityType) {
        if (this.abilities.ContainsKey(abilityType)) {
            return this.abilities[abilityType].GetTotalLevel();
        } else {
            return 0;
        }
    }
}
