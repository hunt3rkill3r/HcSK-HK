using RimWorld;
using System;
using System.IO;
using System.Reflection;
using Verse;

namespace PermaDeathNameFix
{
    [StaticConstructorOnStartup]
    public class Injector_NameFix
    {
        static Injector_NameFix()
        {
            Log.Message("PermadeathNameFix..er...fix.. no seriously, without this random line of code here, it doesn't work");
            MethodInfo method = typeof(NamePlayerFactionDialogUtility).GetMethod("IsValidName");
            MethodInfo method2 = typeof(ReplacementCode).GetMethod("_IsValidName");
            Detour.TryDetourFromTo(method, method2);
            Log.Message("PermadeathNameFix..er...fix.. no seriously, without this random line of code here, it doesn't work");
            MethodInfo method3 = typeof(PermadeathModeUtility).GetMethod("CheckUpdatePermadeathModeUniqueNameOnGameLoad");
            MethodInfo method4 = typeof(ReplacementCode).GetMethod("_CheckUpdatePermadeathModeUniqueNameOnGameLoad");
            Detour.TryDetourFromTo(method3, method4);
            Log.Message("PermadeathNameFix..er...fix.. no seriously, without this random line of code here, it doesn't work");
        }
    }
}