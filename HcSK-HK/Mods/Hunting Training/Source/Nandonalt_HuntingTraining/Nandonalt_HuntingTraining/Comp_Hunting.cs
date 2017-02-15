using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Nandonalt_Limbs
{
    public class Comp_Hunting : ThingComp
    {
        public int state = 0;
        public bool stateBool = true;
        public override void PostExposeData()
        {
            Scribe_Values.LookValue<int>(ref state, "huntingState", 0);
            Scribe_Values.LookValue<bool>(ref stateBool, "huntingStateB", true);
        }
   }

    public class CompProps_Hunting : CompProperties
    {
        public CompProps_Hunting()
        {
            this.compClass = typeof(Comp_Hunting);
        }
    }
}

