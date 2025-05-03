using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HorrorEngine
{
    [Serializable]
    public class AnimatorEventCallback
    {
        public string Event;
        public UnityEvent OnEvent;
    }

    public class AnimatorEventHandler : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<AnimationEvent> OnEvent;

        public void TriggerEvent(AnimationEvent e)
        {
            OnEvent?.Invoke(e);
        }
    }
}