using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MGSC.Localization;

namespace LostTech
{
    [HarmonyPatch(typeof(DamageSystem), "CalculateHitInfo")]
    public static class HitInfoCalculationPatch {
        public static bool Prefix(int distance, float baseAccuracy, float baseDodge, float critChanceBonus, DmgInfo damage, float overallDmgMult, float woundBaseChance, int range = 1, bool autoCrit = false, bool autoHit = false, float critDamageBonus = 0f, ref DamageHitInfo __result) {
            float minHitChance = Data.Global.MinHitChance;
            float capAccuracy = Data.Global.CapAccuracy;
            float capDodge = Data.Global.CapDodge;
            float overcapCritModifier = Data.Global.OvercapCritModifier;
            float accuracy = Mathf.Clamp(baseAccuracy - baseDodge, minHitChance, capAccuracy);
            float num = Mathf.Max(0f, baseAccuracy - baseDodge - capAccuracy);
            float num2 = Mathf.Max(0f, baseDodge - baseAccuracy - capDodge);
            float value = damage.critChance + critChanceBonus + num * overcapCritModifier;
            value = Mathf.Clamp(value, 0f, 1f);
            float woundChance = Mathf.Max(0f, woundBaseChance - num2 * overcapCritModifier);
            __result = new DamageHitInfo(distance, overallDmgMult, damage, accuracy, value, range, autoCrit, autoHit, critDamageBonus)
            {
                woundChance = woundChance
            };
            return false;
        }
    }
}
