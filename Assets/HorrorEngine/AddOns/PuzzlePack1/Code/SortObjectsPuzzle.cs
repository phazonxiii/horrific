using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    public class SortObjectsPuzzle : PuzzleBase
    {
        [FormerlySerializedAs("Slots")]
        [SerializeField] private PlaceableObjectSlot[] m_Slots;

        [FormerlySerializedAs("Solution")]
        [SerializeField] private PlaceableObject[] m_Solution;

        [SerializeField] private float m_DelayAfterSelection = 1f;
        [SerializeField] private float m_DelayAfterSwap = 1f;

        [SerializeField] private UnityEvent OnSwapStart;
        [SerializeField] private UnityEvent OnSwapEnd;

        private PlaceableObjectSlot m_FirstSelection;

        // --------------------------------------------------------------------

        private void Awake()
        {
            Debug.Assert(m_Slots.Length == m_Solution.Length, "Slots and solution list should have the same number of items");
        }

        // --------------------------------------------------------------------

        public void SelectSlot(PlaceableObjectSlot slot)
        {
            if (m_FirstSelection == null)
            {
                m_FirstSelection = slot;
                m_FirstSelection.OnSelected?.Invoke();
            }
            else
            {
                slot.OnSelected?.Invoke();

                StartCoroutine(SwapSortable(m_FirstSelection, slot));       
            }
        }

        // --------------------------------------------------------------------

        private IEnumerator SwapSortable(PlaceableObjectSlot firstSlot, PlaceableObjectSlot secondSlot)
        {
            OnSwapStart?.Invoke();

            yield return Yielders.UnscaledTime(m_DelayAfterSelection);

            var placeable1 = firstSlot.GetComponentInChildren<PlaceableObject>();
            var placeable2 = secondSlot.GetComponentInChildren<PlaceableObject>();

            placeable1.Place(secondSlot);
            placeable2.Place(firstSlot);
            
            m_FirstSelection = null;

            yield return Yielders.UnscaledTime(m_DelayAfterSwap);

            OnSwapEnd?.Invoke();

            CheckSolution();
        }

        // --------------------------------------------------------------------

        private void CheckSolution()
        {
            bool solved = true;
            for (int i =0; i < m_Slots.Length; ++i)
            {
                var sortable = m_Slots[i].GetComponentInChildren<PlaceableObject>();
                if (sortable != m_Solution[i])
                    solved = false;
            }

            if (solved)
                Solve();
        }
    }
}