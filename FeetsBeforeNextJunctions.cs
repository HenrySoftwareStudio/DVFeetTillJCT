using HarmonyLib;
using UnityModManagerNet;
using TMPro;
using UnityEngine;
using DV.Signs;
namespace DVMods.FeetTillJunction
{
    [EnableReloading]
    public class FeetsBeforeNextJunctions
    {
        public static UnityModManager.ModEntry mod;
        public static bool Main(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            modEntry.OnToggle = OnToggle;
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            try
            {
                var harmony = new Harmony(modEntry.Info.Id);
                if (value)
                {
                    harmony.PatchAll();
                }
                else
                {
                    harmony.UnpatchAll(modEntry.Info.Id);
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e} has happened");
                return false;
            }
        }
    }

    public class Patching
    {
        public static readonly float KMtoFeet = 3280.84f;

        public class PatchSign : TextMeshPro
        {
            [HarmonyPatch(typeof(TextMeshPro), nameof(PatchSign.Awake))]
            public static class TMPAwakePatch
            {
                public static void Postfix(TextMeshPro __instance)
                {
                    var tmp = __instance;
                    Debug.Log("Getting instance");
                    if (tmp.transform.GetComponentInParent<SignDebug>() == null)
                        return;
                    string text = tmp.text;
                    if (text.Length > 0 && (!(text.Contains("+") | text.Contains("-")) & text.Contains("0.")))
                    {
                        Debug.Log("Patching for" + __instance.GetHashCode());
                        if (float.TryParse(text, out var kmTillJCT))
                        {
                            var mph = Mathf.RoundToInt(kmTillJCT * Patching.KMtoFeet);
                            tmp.text = mph.ToString() + "ft";
                        }
                    }
                }
            }
        }
    }
}
