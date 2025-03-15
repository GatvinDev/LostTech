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
    [HarmonyPatch(typeof(DamageSystem), "CalculateDamage")]
    public static class ArmorCalculationPatch {
        public static bool Prefix(int incomeDamage, int resistance, float dmgModifier, float resModifier, float weaponArmorPen, ref int __result) {
            float modRes = (float)resistance * (resModifier - weaponArmorPen);
            float totalDamage = (float)incomeDamage - modRes;
            __result = Mathf.RoundToInt(Math.Max(1f, totalDamage * dmgModifier));
            Debug.Log("Incoming Damage: " + incomeDamage);
            Debug.Log("Incoming Resistance: " + resistance);
            Debug.Log("final damage total: " + __result);
            return false;
        }
    }
}
