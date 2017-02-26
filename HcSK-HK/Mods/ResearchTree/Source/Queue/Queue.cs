﻿// Karel Kroeze
// Queue.cs
// 2016-12-28

using HugsLib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static FluffyResearchTree.Assets;

namespace FluffyResearchTree
{
    public class Queue : UtilityWorldObject
    {
        #region Fields

        private static readonly List<ResearchNode> _queue = new List<ResearchNode>();
        private static List<ResearchProjectDef> _saveableQueue;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Removes and returns the first node in the queue.
        /// </summary>
        /// <returns></returns>
        public static ResearchNode Pop
        {
            get
            {
                if ( _queue != null && _queue.Count > 0 )
                {
                    ResearchNode node = _queue[0];
                    _queue.RemoveAt( 0 );
                    return node;
                }

                return null;
            }
        }

        #endregion Properties

        #region Methods

        public static void Dequeue( ResearchNode node )
        {
            _queue.Remove( node );
            List<ResearchNode> followUps = _queue.Where( n => n.GetMissingRequiredRecursive().Contains( node ) ).ToList();
            foreach ( ResearchNode followUp in followUps )
                _queue.Remove( followUp );
        }

        public static void DrawLabels()
        {
            var i = 1;
            foreach ( Node node in _queue )
            {
                // draw coloured tag
                GUI.color = Color.green;
                GUI.DrawTexture( node.QueueRect, CircleFill );

                // if this is not first in line, grey out centre of tag
                if ( i > 1 )
                {
                    GUI.color = Color.grey;
                    GUI.DrawTexture( node.QueueRect.ContractedBy( 2f ), CircleFill );
                }

                // draw queue number
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label( node.QueueRect, i++.ToString() );
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public static void Enqueue( ResearchNode node, bool add )
        {
            // if we're not adding, clear the current queue and current research project
            if ( !add )
            {
                _queue.Clear();
                Find.ResearchManager.currentProj = null;
            }

            // add to the queue if not already in it
            if ( !_queue.Contains( node ) )
                _queue.Add( node );

            // try set the first research in the queue to be the current project.
            ResearchNode next = _queue.First();
            Find.ResearchManager.currentProj = next?.Research; // null if next is null.
        }

        public static void EnqueueRange( IEnumerable<ResearchNode> nodes, bool add )
        {
            // clear current Queue if not adding
            if ( !add )
            {
                _queue.Clear();
                Find.ResearchManager.currentProj = null;
            }

            // sorting by depth ensures prereqs are met - cost is just a bonus thingy.
            foreach ( ResearchNode node in nodes.OrderBy( node => node.Y ).ThenBy( node => node.Research.CostApparent ) )
                Enqueue( node, true );
        }

        public static bool IsQueued( ResearchNode node )
        {
            return _queue.Contains( node );
        }

        public static bool TryStartNext( out ResearchNode next )
        {
            if ( _queue.Count > 0 )
            {
                next = _queue.First();
                Find.ResearchManager.currentProj = next.Research;
                return true;
            }

            next = null;
            Find.ResearchManager.currentProj = null;
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            // store research defs as these are the defining elements
            if ( Scribe.mode == LoadSaveMode.Saving )
                _saveableQueue = _queue.Select( node => node.Research ).ToList();

            Scribe_Collections.LookList( ref _saveableQueue, "Queue", LookMode.Def );

            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                // initialize the tree if not initialized
                if ( !Tree.Initialized )
                    Tree.Initialize();

                // initialize the queue
                foreach ( ResearchProjectDef research in _saveableQueue )
                {
                    // find a node that matches the research - or null if none found
                    ResearchNode node = research.Node();

                    // enqueue the node
                    if ( node != null )
                    {
                        Enqueue( node, true );
                    }
                }
            }
        }

        #endregion Methods
    }
}
