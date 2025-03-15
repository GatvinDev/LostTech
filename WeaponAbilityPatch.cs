using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostTech
{
    [HarmonyPatch(typeof(WeaponRecord), "get_ArmorPenetration")]
    public static class WeaponAbilityPatch
    {
        public static void Postfix(WeaponRecord __instance, ref float __result)
        {
            __result = 0f;
        }
    }
}
