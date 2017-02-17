using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MountainMiner
{
    public class WorkGiver_UpDrill : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDef.Named("ManualMountainMiner"));
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ManualMountainMiner")).Cast<Thing>();
        }

        public override bool ShouldSkip(Pawn pawn)
        {
            List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                if (allBuildingsColonist[i].def == ThingDef.Named("ManualMountainMiner"))
                {
                    CompPowerTrader comp = allBuildingsColonist[i].GetComp<CompPowerTrader>();
                    if (comp == null || comp.PowerOn)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t)
        {
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            Building building = t as Building;
            if (building == null)
            {
                return false;
            }
            if (building.IsForbidden(pawn))
            {
                return false;
            }
            if (!pawn.CanReserve(building, 1))
            {
                return false;
            }
            Building_MountainDrill mountainDrill = (Building_MountainDrill) building;
            return mountainDrill.CanDrillNow() && !building.IsBurning();
        }

        public override Job JobOnThing(Pawn pawn, Thing t)
        {
            return new Job(JobDefOf_MM.OperateHighDrill, t, 1500, true);
        }
    }
}
