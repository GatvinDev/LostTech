using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LostTech
{
    [HarmonyPatch(typeof(Player), "ProcessDamage")]
    public static class PlayerArmorDamagePatch {
        public static void Prefix(Player __instance, DamageHitInfo hitInfo, float visualDelay, out List<int> __state)
        {
            //var visCreaturesCount = AccessTools.FieldRefAccess<Player, CreatureMovementState>("_movementStateOnSkipTurn")(__instance);
            //Debug.Log("Private member access for creature movement state: " + visCreaturesCount);
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
            var durabilityList = new List<int>();
            foreach(ItemStorage slot in __instance.CreatureData.Inventory.Slots){
                if (slot.Source == ItemStorageSource.WeaponSlot || slot.Empty) {
                    durabilityList.Add(0);
                    continue;
                }
                BreakableItemComponent comp = slot.First.Comp<BreakableItemComponent>();
                if (comp != null) {
                    durabilityList.Add(slot.First.Comp<BreakableItemComponent>().Durability);
                } else { durabilityList.Add(0); }
            }
            __state = durabilityList;
        }

        static void Postfix(Player __instance, List<int> __state) {
            var slotIndex = 0;
            foreach(ItemStorage slot in __instance.CreatureData.Inventory.Slots){
                if (slot.Source == ItemStorageSource.WeaponSlot || slot.Empty) {
                    slotIndex++;
                    continue; 
                }
                BreakableItemComponent comp = slot.First.Comp<BreakableItemComponent>();
                if (comp != null && comp.Durability < __state[slotIndex]) {
                    var durDiff = __state[slotIndex] - comp.Durability;
                    comp.Restore(durDiff, 0);
                    break;
                }
                slotIndex++;
            }
        }
    }
}
