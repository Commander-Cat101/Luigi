using MelonLoader;
using BTD_Mod_Helper;
using Luigi;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using UnityEngine;
using HarmonyLib;
using System;
using System.Reflection;
using BTD_Mod_Helper.Api;
using Random = System.Random;
using BTD_Mod_Helper.Api.Display;
using Il2CppSystem;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;

[assembly: MelonInfo(typeof(Luigi.LuigiTower), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Luigi;

public class FireBallDisplay : ModDisplay
{
    public override string BaseDisplay => Generic2dDisplay;      

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        Set2DTexture(node, "FireBallDisplay");
    }   
}
public class LuigiTower : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<LuigiTower>("Luigi loaded!");
    }
}
public class Luigi : ModTower
{
    public override TowerSet TowerSet => TowerSet.Magic;
    public override string BaseTower => TowerType.DartMonkey;
    public override int Cost => 650;
    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 5;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Luigi is here in BTD6";

    public override bool Use2DModel => true;
    public override string Icon => "LuigiIcon";

    public override string Portrait => "LuigiIcon";

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile = Game.instance.model.GetTower(TowerType.MonkeySub).GetAttackModel().weapons[0].projectile.Duplicate();
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = 0;
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<FireBallDisplay>();
        
    }

    public override string Get2DTexture(int[] tiers)
    {
        return "LuigiDisplay";
    }
}
public class FlameBlast : ModUpgrade<Luigi>
{
    // public override string Portrait => "Don't need to override this, using the default of Pair-Portrait.png";
    // public override string Icon => "Don't need to override this, using the default of Pair-Icon.png";
    public override string Portrait => "LuigiIcon";
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 400;

    // public override string DisplayName => "Don't need to override this, the default turns it into 'Pair'"

    public override string Description => "Fireballs now light bloons on fire upon landing";

    public override void ApplyUpgrade(TowerModel towerModel)
    {

        var Fire = Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 2).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>();
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Fire);
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0 };
    }
}
public class LighterFireballs : ModUpgrade<Luigi>
{
    // public override string Portrait => "Don't need to override this, using the default of Pair-Portrait.png";
    // public override string Icon => "Don't need to override this, using the default of Pair-Icon.png";
    public override string Portrait => "LuigiIcon";
    public override int Path => MIDDLE;
    public override int Tier => 2;
    public override int Cost => 1000;

    // public override string DisplayName => "Don't need to override this, the default turns it into 'Pair'"

    public override string Description => "Lighter fireballs allow much faster firing";

    public override void ApplyUpgrade(TowerModel towerModel)
    {

        towerModel.GetAttackModel().weapons[0].Rate *= .4f;
    }
}
public class SuperThump : ModUpgrade<Luigi>
{
    // public override string Portrait => "Don't need to override this, using the default of Pair-Portrait.png";
    // public override string Icon => "Don't need to override this, using the default of Pair-Icon.png";
    public override string Portrait => "LuigiIcon";
    public override int Path => MIDDLE;
    public override int Tier => 3;
    public override int Cost => 0;//3500

    // public override string DisplayName => "Don't need to override this, the default turns it into 'Pair'"

    public override string Description => "Luigi gains a ground pound attack";

    public override void ApplyUpgrade(TowerModel towerModel)
    {


        var projectile = Game.instance.model.GetTower(TowerType.BombShooter, 4, 0, 0).GetAttackModel().weapons[0].projectile.Duplicate();
        projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage *= 25;
        projectile.display = new() { guidRef = "" };
        var weapon = towerModel.GetAttackModel().Duplicate();
        weapon.weapons[0].projectile = projectile;
        weapon.weapons[0].Rate = 8;
        weapon.name = "GroundPound";
        towerModel.AddBehavior(weapon);
    }
}
public class Shockwaves : ModUpgrade<Luigi>
{
    // public override string Portrait => "Don't need to override this, using the default of Pair-Portrait.png";
    // public override string Icon => "Don't need to override this, using the default of Pair-Icon.png";
    public override string Portrait => "LuigiIcon";
    public override int Path => MIDDLE;
    public override int Tier => 4;
    public override int Cost => 7000;

    // public override string DisplayName => "Don't need to override this, the default turns it into 'Pair'"

    public override string Description => "Luigis ground pound now shockwaves knocking back bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {


        foreach (var attacks in towerModel.GetAttackModels())
        {
            if (attacks.name.Contains("Ground"))
            {
                
                attacks.weapons[0].Rate *= .4f;
                var wind = Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 2, 0).GetAttackModel().weapons[0].projectile.GetBehavior<WindModel>();
                wind.distanceMax = 20;
                wind.distanceMin = 2;   
                attacks.weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.AddBehavior(wind);
            }
            else
            {
                attacks.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().Interval = .2f;
                attacks.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage = 3f;
                attacks.weapons[0].Rate *= .5f;
            }
        }
    }
}
public class Superuigi : ModUpgrade<Luigi>
{
    // public override string Portrait => "Don't need to override this, using the default of Pair-Portrait.png";
    // public override string Icon => "Don't need to override this, using the default of Pair-Icon.png";
    public override string Portrait => "LuigiIcon";
    public override int Path => MIDDLE;
    public override int Tier => 5;
    public override int Cost => 8500000;

    // public override string DisplayName => "Don't need to override this, the default turns it into 'Pair'"

    public override string Description => "Luigi has grown to unstoppable powers, his fireball can do thousands of damage per tick and his stomps now PERMANANTLY stun bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {


        foreach (var attacks in towerModel.GetAttackModels())
        {
            if (attacks.name.Contains("Ground"))
            {
                attacks.weapons[0].Rate *= .001f;
                attacks.weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce = 9000000;
                var wind = attacks.weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<WindModel>();
                wind.affectMoab = true;
                wind.speedMultiplier = 0;
            }
            else
            {
                attacks.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().Interval = 0.001f;
                attacks.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage = 200000f;
                attacks.weapons[0].Rate *= .015f;
                attacks.weapons[0].projectile.pierce *= 30;
                attacks.weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed *= 2;
                attacks.weapons[0].projectile.GetDamageModel().damage = 200000;
            }
            attacks.range = 150;
        }
        towerModel.range = 150;
        towerModel.AddBehavior(new OverrideCamoDetectionModel("camooo", true));
    }
}