using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Nandonalt_Limbs
{
    class JobGiver_AnimalHunt : ThinkNode_JobGiver
    {
        private const float MinDistFromEnemy = 25f;
        public static List<string> ignoredPreys = new List<string>(new string[] { "Boomrat", "Boomalope" });
        private float radius = 30f;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_AnimalHunt jobGiver_RescueNearby = (JobGiver_AnimalHunt)base.DeepCopy(resolve);
            return jobGiver_RescueNearby;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            List<Pawn> AdditionalPredators = new List<Pawn>();
            if (pawn.GetComp<Comp_Hunting>().stateBool)
            {
                                
                    List<Pawn> allPawnsSpawned = pawn.Map.mapPawns.AllPawnsSpawned;
                    foreach (Pawn p in allPawnsSpawned)
                    {
                    if (AdditionalPredators.Count < 4)
                    {
                        if (p.Faction == pawn.Faction && p.training != null && p.training.IsCompleted(DefDatabase<TrainableDef>.GetNamed("HuntingTraining")))
                        {
                            float distance = (pawn.Position - p.Position).LengthHorizontal;
                            if (p.kindDef == pawn.kindDef && !p.Downed && !p.Dead && pawn.GetComp<Comp_Hunting>().stateBool && distance <= 30f)
                            {
                                if (pawn.jobs.curDriver != null && pawn.jobs.curDriver.asleep)
                                {
                                }
                                else
                                {
                                    AdditionalPredators.Add(p);
                                }
                            }
                        }
                    }
                    }

                bool huntAlone;
                Pawn prey = BestPawnToHuntForPredator(pawn, AdditionalPredators, out huntAlone);


                if (prey != null)
                {
                    if (!huntAlone)
                    {
                        foreach (Pawn p in AdditionalPredators)
                        {
                            Job job = new Job(DefDatabase<JobDef>.GetNamed("HuntTrainedAssist"), prey, pawn);
                            p.jobs.TryTakeOrderedJob(job);
                        }
                    }
                    return new Job(DefDatabase<JobDef>.GetNamed("HuntTrained"), prey)
                    {
                        killIncappedTarget = true
                    };
                }
            }
            return null;
        }


        private static Pawn BestPawnToHuntForPredator(Pawn predator, List<Pawn> add, out bool huntAlone)
        {
            huntAlone = true;
            if (predator.meleeVerbs.TryGetMeleeVerb() == null)
            {
                return null;
            }
            bool flag = false;
            float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.25f)
            {
                flag = true;
            }
            List<Pawn> allPawnsSpawned = predator.Map.mapPawns.AllPawnsSpawned;
            Pawn pawn = null;
            float num = 0f;
            bool tutorialMode = TutorSystem.TutorialMode;
          
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn2 = allPawnsSpawned[i];
                if (predator.GetRoom() == pawn2.GetRoom() && ForbidUtility.InAllowedArea(pawn2.Position, predator))
                {
                    if (predator != pawn2)
                    {
                        if (!flag || pawn2.Downed)
                        {
                            bool huntAlonetemp = true;
                            if (!ignoredPreys.Contains(pawn2.kindDef.defName) &&  IsAcceptablePreyFor(predator, pawn2, add, out huntAlonetemp))
                            {
                             /*   bool canEatHuman = false;
                                if (pawn.playerSettings.master != null && pawn.playerSettings.master.story.traits.HasTrait(TraitDefOf.Cannibal))
                                {
                                    canEatHuman = true;
                                }*/
                                    if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                                    {
                                    if (!pawn2.IsForbidden(predator))
                                    {
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            if (!pawn2.RaceProps.Humanlike) { 
                                                float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
                                            if (preyScoreFor > num || pawn == null)
                                            {
                                                    huntAlone = huntAlonetemp;
                                                num = preyScoreFor;
                                                pawn = pawn2;
                                            }
                                        }
                                    }
                                        }
                                    }
                                
                            }
                        }
                    }
                }
            }
            return pawn;
        }

        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey, List<Pawn> add, out bool f)
        {
            f = false;
           /*if (!prey.RaceProps.canBePredatorPrey)
            {
                return false;
            }*/
            if (!prey.RaceProps.IsFlesh)
            {
                return false;
            }
     
            float bodySizeFinal = predator.RaceProps.maxPreyBodySize;
            foreach (Pawn p in add)
            {

                bodySizeFinal += p.RaceProps.maxPreyBodySize;
            }

            if (prey.BodySize > bodySizeFinal)
            {
                return false;
            }
            if (!prey.Downed)
            {
                if (prey.kindDef.combatPower > 2f * predator.kindDef.combatPower)
                {
                    return false;
                }
                float combatPowerOffset = 0f;
                foreach(Pawn p in add)
                {
                    combatPowerOffset += p.kindDef.combatPower * p.health.summaryHealth.SummaryHealthPercent * p.ageTracker.CurLifeStage.bodySizeFactor;
                }
                float num = prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent * prey.ageTracker.CurLifeStage.bodySizeFactor;
             
                float num2 = (predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent * predator.ageTracker.CurLifeStage.bodySizeFactor) + combatPowerOffset;
                if (num > 0.85f * num2)
                {
                    return false;
                }

                if (num < 0.85f * (num2 - combatPowerOffset))
                {
                    f = true;
             
                }
            }
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (!predator.RaceProps.herdAnimal || predator.def != prey.def);
        }

    }


}
