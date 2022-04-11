using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace InvincibleBunkerTile
{
    [HarmonyPatch]
    internal class ModPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BunkerTileConfig), "CreateBuildingDef")]
        private static BuildingDef BunkerTileConfig_CreateBuildingDef_Postfix(BuildingDef def)
        {
            def.Invincible = true;
            return def;
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(WorldDamage), "ApplyDamage", typeof(int), typeof(float), typeof(int), typeof(string), typeof(string))]
        private static IEnumerable<CodeInstruction> WorldDamage_ApplyDamage_Two_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Debug.Log("Transpiler In");
            bool foundUninvincible = false;
            int startIndex = -1;
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_1 && codes[i + 1].opcode == OpCodes.Ldsfld)
                {
                    foundUninvincible = true;
                    startIndex = i+1;
                    break;
                }
            }
            var insert = new List<CodeInstruction>() 
            { 
                new CodeInstruction(OpCodes.Ldloc_0), 
                new CodeInstruction(OpCodes.Ret), 
            };
            codes.InsertRange(startIndex, insert);
            return codes.AsEnumerable();
        }
    }
}
