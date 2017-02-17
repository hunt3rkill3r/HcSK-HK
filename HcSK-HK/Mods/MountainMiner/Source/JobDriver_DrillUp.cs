using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;

namespace MountainMiner
{
    class JobDriver_DrillUp : JobDriver
    {
        const int ticks = GenDate.TicksPerDay;
        Building_MountainDrill comp => (Building_MountainDrill)TargetA.Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => {
                if (comp.CanDrillNow()) return false;
                return true;
            });
            yield return Toils_Reserve.Reserve(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);

            Toil mine = new Toil();
            mine.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
            mine.WithProgressBar(TargetIndex.A, () => comp.Progress);
            mine.tickAction = delegate
            {
                var pawn = mine.actor;
                comp.Drill(pawn.GetStatValue(StatDefOf.MiningSpeed) / ticks);
                pawn.skills.Learn(SkillDefOf.Mining, 0.125f);
                if (comp.Progress>=1)
                {
                    comp.DrillWorkDone(pawn);
                    EndJobWith(JobCondition.Succeeded);
                    pawn.records.Increment(RecordDefOf.CellsMined);
                }
            };
            mine.WithEffect(TargetThingA.def.repairEffect, TargetIndex.A);
            mine.defaultCompleteMode = ToilCompleteMode.Never;
            yield return mine;
        }
    }
}
