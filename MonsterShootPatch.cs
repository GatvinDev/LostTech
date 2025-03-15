using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostTech
{
    [HarmonyPatch(typeof(Monster), "Shoot")]
    public static class MonsterShootPatch {
        public static void Prefix(Monster __instance, int x, int y, bool instant) {
            BasePickupItem currentWeapon = __instance.CreatureData.Inventory.CurrentWeapon;
            WeaponRecord weaponRecord = currentWeapon?.Record<WeaponRecord>();
            if (weaponRecord.RampUpValue > 0) {
                __instance.CreatureData.EffectsController.Add(new RampUpShotEffect(weaponRecord.RampUpValue, 2));
            }
        }
    }
}
