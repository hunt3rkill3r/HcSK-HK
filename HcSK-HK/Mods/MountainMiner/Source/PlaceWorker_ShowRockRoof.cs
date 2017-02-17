using System.Linq;
using Verse;

namespace MountainMiner
{
    public class PlaceWorker_ShowRockRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
        {
            if (loc != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 intVec = loc + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(Map) && Map.roofGrid.RoofAt(intVec) != null && Map.roofGrid.RoofAt(intVec).isThickRoof)
                            return true;
                }
                return new AcceptanceReport("Must be placed under overhead Mountain");
            }
            return false;
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            foreach (IntVec3 current in from cur in Map.AllCells where Map.roofGrid.RoofAt(cur) != null && Map.roofGrid.RoofAt(cur).isThickRoof select cur)
                CellRenderer.RenderCell(current);
        }
    }
}