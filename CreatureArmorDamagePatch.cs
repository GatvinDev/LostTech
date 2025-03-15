using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostTech
{
    [HarmonyPatch(typeof(Creature), "ProcessDamage")]
    public static class CreatureArmorDamagePatch {
        public static void Prefix(Creature __instance, DamageHitInfo hitInfo, float visualDelay) {
            var possibleSlots = 0;
            List<BreakableItemComponent> hitList = new List<BreakableItemComponent>();

            foreach(ItemStorage slot in __instance.CreatureData.Inventory.Slots){
                if (slot.Source == ItemStorageSource.WeaponSlot) {
                    continue;
                }
                possibleSlots++;
                if (!slot.Empty) {
                    BreakableItemComponent comp = slot.First.Comp<BreakableItemComponent>();
                    if (comp != null) {
                        hitList.Add(comp);
                    }
                }
            }
            
            var rolledSlot = UnityEngine.Random.Range(0, possibleSlots);
            
            if (rolledSlot < hitList.Count) {
                var damageResist = DamageSystem.CalculateMinimalResist(__instance,Data.DamageTypes.GetRecord(hitInfo.info.damage));
                var equipDamage = Math.Min(hitInfo.finalDmg, damageResist);
                hitList[rolledSlot].Break(equipDamage);
            }
        }
    }
}
