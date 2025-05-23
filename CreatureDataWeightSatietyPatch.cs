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
    [HarmonyPatch(typeof(CreatureData), "GetItemsWeightSatietyDrain")]
    public static class CreatureDataWeightSatietyPatch {
        public static void Postfix(ref float __result) {
            __result = Mathf.RoundToInt(__result / 2f);
        }
    }
}
