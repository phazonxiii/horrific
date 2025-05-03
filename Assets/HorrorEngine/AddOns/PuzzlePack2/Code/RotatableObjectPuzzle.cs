using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HorrorEngine
{
    public class RotatableObjectPuzzle : PuzzleBase
    {
        [Serializable]
        public class DirectionalObjectPuzzleEntry
        {
            public RotatableObject Object;
            public string TransformRefID;
        }

        [SerializeField] DirectionalObjectPuzzleEntry[] m_Solution;
        [SerializeField] public bool m_AnimateRotation;
        [SerializeField] public bool m_CheckSolutionAfterRotation = true;

        private RotatableObject m_DirectionalObject;
        private Dictionary<RotatableObject, string> m_HashedSolution = new Dictionary<RotatableObject, string>();
        private UnityAction m_OnRotationEnd;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_OnRotationEnd = OnRotationEnd;

            foreach (var solutionEntry in m_Solution)
            {
                m_HashedSolution.Add(solutionEntry.Object, solutionEntry.TransformRefID);
            }
        }

        // --------------------------------------------------------------------

        public void SelectDirectional(RotatableObject directionalObject)
        {
            Debug.Assert(directionalObject, "Selected Directional Object can't be null");

            if (m_AnimateRotation && m_DirectionalObject && m_CheckSolutionAfterRotation)
            {
                m_DirectionalObject.OnRotationEnd.RemoveListener(m_OnRotationEnd);
            }

            m_DirectionalObject = directionalObject;

            if (m_AnimateRotation && m_CheckSolutionAfterRotation)
            {
                m_DirectionalObject.OnRotationEnd.AddListener(m_OnRotationEnd);
            }
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            if (m_AnimateRotation && m_DirectionalObject)
            {
                m_DirectionalObject.OnRotationEnd.RemoveListener(m_OnRotationEnd);
            }
        }

        // --------------------------------------------------------------------

        public void SetSelectedToDirection(string directionID)
        {
            if (m_AnimateRotation)
            {
                m_DirectionalObject.RotateTo(directionID);
            }
            else
            {
                m_DirectionalObject.SnapTo(directionID);
                if (m_CheckSolutionAfterRotation)
                    CheckSolution();
            }

            
        }

        // --------------------------------------------------------------------

        private void OnRotationEnd()
        {
            CheckSolution();
        }

        // --------------------------------------------------------------------

        public void CheckSolution()
        {
            foreach (var solutionEntry in m_HashedSolution)
            {
                if (m_HashedSolution.TryGetValue(solutionEntry.Key, out string directionID))
                {
                    if (solutionEntry.Key.TransformRefID != directionID)
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

