using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MountainMiner
{
    class Building_MountainDrill : Building
    {
        private CompPowerTrader powerComp;

        private float progress;

        public float Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
            }
        }

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            powerComp = GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue(ref progress, "MountainProgress", 0f);
        }

        public void Drill(float miningPoints)
        {
            progress += miningPoints;
            if (UnityEngine.Random.Range(0, 1000) == 0)
            {
                ProduceLump();
            }
        }

        public void DrillWorkDone(Pawn driller)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(Map))
                {
                    Map.roofGrid.SetRoof(intVec, RoofDefOf.RoofRockThin);
                }
            }
            Progress = 0f;
        }

        private void ProduceLump()
        {
            ThingDef def;
            IntVec3 c;
            if (TryGetNextResource(out def, out c))
            {
                Thing thing = ThingMaker.MakeThing(def, null);
                GenPlace.TryPlaceThing(thing, InteractionCell, Map, ThingPlaceMode.Near);
            }
            return;
        }

        public bool TryGetNextResource(out ThingDef resDef, out IntVec3 cell)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(Map))
                {
                    ThingDef thingDef = DefDatabase<ThingDef>.GetNamed("Chunk" + Map.terrainGrid.TerrainAt(intVec).defName.Split('_')[0], false);
                    //GenStep_RocksFromGrid.RockDefAt(intVec);
                    if (thingDef == null)
                    {
                        if (!Find.World.NaturalRockTypesIn(Map.areaManager.Home.ID).TryRandomElement(out thingDef))
                            thingDef = ThingDef.Named("Sandstone");
                        thingDef = ThingDef.Named("Chunk" + thingDef);
                    }
                    //Log.Message(GenStep_RocksFromGrid.RockDefAt(intVec).defName);

                    resDef = thingDef;
                    cell = intVec;
                    return true;
                }
            }
            resDef = null;
            cell = IntVec3.Invalid;
            return false;
        }

        public bool CanDrillNow()
        {
            return (powerComp == null || powerComp.PowerOn) && RoofPresent();
        }

        public bool RoofPresent()
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(Map) && Map.roofGrid.RoofAt(intVec) != null && Map.roofGrid.RoofAt(intVec).isThickRoof)
                    return true;
            }
            return false;
        }

        public override string GetInspectString()
        {
            return string.Concat(new string[]
            {
                base.GetInspectString(),
                "Progress"/*.Translate()*/,
                ": ",
                Progress.ToStringPercent()
            });
        }
    }
}