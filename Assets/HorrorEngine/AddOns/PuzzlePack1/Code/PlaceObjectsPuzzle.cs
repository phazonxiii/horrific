using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    public class PlaceObjectsPuzzle : PuzzleBase
    {
        [Serializable]
        public class PlaceObjectPuzzleEntry
        {
            public PlaceableObject Object;
            public PlaceableObjectSlot Slot;
        }

        [SerializeField] private PlaceObjectPuzzleEntry[] m_Solution;

        private PlaceableObject m_Placeable;
        private Dictionary<PlaceableObject, PlaceableObjectSlot> m_HashedSolution = new Dictionary<PlaceableObject, PlaceableObjectSlot>();
        private Dictionary<PlaceableObject, PlaceableObjectSlot> m_Placed = new Dictionary<PlaceableObject, PlaceableObjectSlot>();

        // --------------------------------------------------------------------

        private void Awake()
        {
            foreach(var solutionEntry in m_Solution)
            {
                Debug.Assert(!m_HashedSolution.ContainsKey(solutionEntry.Object), "The same object can't appear twices in the puzzle solution");
                m_HashedSolution.Add(solutionEntry.Object, solutionEntry.Slot);
            }
        }

        // --------------------------------------------------------------------

        public void SelectPlaceable(PlaceableObject placeable)
        {
            Debug.Assert(placeable, "Selected Placeable can't be null");
            m_Placeable = placeable;    
        }

        // --------------------------------------------------------------------

        public void SetSelectedAtSlot(PlaceableObjectSlot slot)
        {
            m_Placeable.Place(slot);

            if (!m_Placed.ContainsKey(m_Placeable))
                m_Placed.Add(m_Placeable, slot);
            else
                m_Placed[m_Placeable] = slot;

            CheckSolution();
        }

        // --------------------------------------------------------------------

        private void CheckSolution()
        {
            foreach(var solutionEntry in m_HashedSolution)
            {
                if (m_Placed.TryGetValue(solutionEntry.Key, out PlaceableObjectSlot slot))
                {
                    if (solutionEntry.Value != slot)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            Solve();
        }
       

    }
}
