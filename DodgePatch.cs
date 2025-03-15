using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostTech
{
    [HarmonyPatch(typeof(CreatureData), "GetDodge")]
    public static class DodgePatch
    {
        public static void Postfix(CreatureData __instance,ref float __result)
        {
            if (__instance.MovementState == CreatureMovementState.Slow)
            {
                __result += (float)0.25;
            }
        }
    }
}
