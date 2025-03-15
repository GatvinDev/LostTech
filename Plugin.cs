using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace LostTech
{
    
    public class Plugin
    {
        [Hook(ModHookType.BeforeBootstrap)]
        public static void BeforeBootstrap(IModContext context)
        {
            Debug.Log("Executing Mod Code");
            new Harmony("Gatvin" + Assembly.GetExecutingAssembly().GetName().Name).PatchAll();
        }

    }
}
