using BepInEx;
using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*Copyright(c) 2024 NBK_RedSpy

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//Borrowed with minor modifications from NBK_RedSpy's Simple Data Loader located at: 
//https://github.com/NBKRedSpy/QM-SimpleDataLoader

namespace LostTech
{

    [HarmonyPatch(typeof(ConfigLoader), "LoadSpecificFile")]
    public static class ReplaceFileLoad
    {

        /// <summary>
        /// Replaces the game's load with the custom load.
        /// </summary>
        /// <remarks>
        /// This is a really bad way to do this, but it will be replaced with official loading next week.
        /// </remarks>
        /// <param name="__instance"></param>
        /// <param name="___OnDescriptorsLoaded"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool Prefix(ConfigLoader __instance, Action<string, DescriptorsCollection> ___OnDescriptorsLoaded, string path)
        {
            string importFileName = Path.Combine(Plugin.ImportDir, path + ".tsv");
            string loadConfig = Path.Combine(Plugin.ImportDir, "DisableLoading.cfg");
            if (File.Exists(loadConfig))
            {
                return true;
            }

            string resourceText;

            if (File.Exists(importFileName))
            {
                Debug.Log($"Importing config file: {importFileName}");
                resourceText = File.ReadAllText(importFileName);
            }
            else
            {
                TextAsset obj = Resources.Load(path) as TextAsset;
                if (obj == null)
                {
                    throw new NotImplementedException("Failed open " + path + " in Resources folder.");
                }

                resourceText = obj.text;
            }

            string[] array = resourceText.Split('\n');

            bool flag = false;
            string text = string.Empty;
            DescriptorsCollection descriptorsCollection = null;
            IConfigParser configParser = null;
            string[] array2 = array;
            foreach (string text2 in array2)
            {
                if (string.IsNullOrEmpty(text2) || (text2.Length >= 2 && text2[0] == '/' && text2[1] == '/'))
                {
                    continue;
                }
                if (flag)
                {
                    flag = false;
                    configParser.ParseHeaders(__instance.SplitLine(text2));
                }
                else if (text2.Contains("#end"))
                {
                    configParser = null;
                    descriptorsCollection = null;
                }
                else if (text2[0] == '#')
                {
                    text = text2.Trim('\t', '\r', '\n', '#');
                    foreach (IConfigParser parser in __instance._parsers)
                    {
                        if (parser.ValidTableKey(text))
                        {
                            flag = true;
                            configParser = parser;
                        }
                    }
                    descriptorsCollection = Resources.Load<DescriptorsCollection>("DescriptorsCollections/" + text + "_descriptors");
                    if (descriptorsCollection != null)
                    {
                        ___OnDescriptorsLoaded.Invoke(text, descriptorsCollection);
                    }
                }
                else
                {
                    configParser?.ParseLine(__instance.SplitLine(text2), text, descriptorsCollection);
                }
            }

            return false;
        }
    }
}
