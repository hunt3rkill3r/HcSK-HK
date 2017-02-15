using System;
using System.Reflection;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;
namespace Nandonalt_Limbs
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
          static HarmonyPatches()
        {
                 foreach(ThingDef d in DefDatabase<ThingDef>.AllDefs)
            {
                 if(d.race != null && d.race.Animal)
                {
                    d.comps.Add(new CompProps_Hunting());
                }
            }
          
            var harmony = HarmonyInstance.Create("rimworld.nandonalt.hunting");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
                    }

        public static FieldInfo MapFieldInfo;
    }
    [HarmonyPatch(typeof(TrainingCardUtility))]
    [HarmonyPatch("TryDrawTrainableRow")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(Pawn), typeof(TrainableDef) })]
    class TryDrawTrainableRow
    {
        static void Postfix(Rect rect, Pawn pawn, TrainableDef td)
        {
            if (td == DefDatabase<TrainableDef>.GetNamed("HuntingTraining"))
            {
                Rect rect2 = rect;
                rect2.x = rect2.width - 75f;
                                    
                if (pawn.training.IsCompleted(td))
                {
                    Widgets.Checkbox(rect2.position, ref pawn.GetComp<Comp_Hunting>().stateBool);
                }
            }
        }
    }


}
