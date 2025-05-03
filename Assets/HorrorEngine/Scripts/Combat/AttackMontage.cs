using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    public class AttackMontage : MonoBehaviour
    {
        public AttackBase Attack;

        public float CountConsumption = 1f;

        [Header("Timing")]
        public float Duration;
        public float MontageDelay;
        public float AttackActivationDelay;

        [Header("Animation")]
        public AnimatorStateHandle Animation;
        public float AnimationBlendTime = 0.2f;
        [SerializeField] private AnimatorEventCallback[] m_AnimEventCallbacks;

        [Header("Audio")]
        [SerializeField] public AudioSource m_AudioSource;
        [FormerlySerializedAs("AttackSound")]
        [SerializeField] private AudioClip m_PreDelaySoundWillStart; 
        [SerializeField] private AudioClip m_PreDelaySoundWillNotStart;
        [SerializeField] private AudioClip m_AttackSound;
        [SerializeField] private bool m_IsAttackSoundLoop;
        [SerializeField] private AudioClip m_AttackCompleteSound;
        [SerializeField] private AudioClip m_AttackNotStartedSound; // TODO - Link to a "reason" enum
        [SerializeField] private AudioClip m_AttackInterruptedSound;

        [Header("Events")]
        public UnityEvent OnPreDelayWillStart;
        public UnityEvent OnPreDelayWillNotStart;
        public UnityEvent OnPlay;
        public UnityEvent OnComplete;
        public UnityEvent OnInterrupt;

        private bool m_Interrupted;
        private Animator m_CurrentAnimator;

        private UnityAction<AnimationEvent> m_OnAnimatorEvent;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_OnAnimatorEvent = OnAnimatorEvent;

            if (!Attack)
            {
                Attack = GetComponent<AttackBase>();
            }
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            if (m_CurrentAnimator != null)
            {
                m_CurrentAnimator.GetComponent<AnimatorEventHandler>().OnEvent.RemoveListener(m_OnAnimatorEvent);
            }
        }

        // --------------------------------------------------------------------

        public void PreDelay(bool canStart)
        {
            if (m_AudioSource)
            {
                m_AudioSource.clip = null;
                if (canStart && m_PreDelaySoundWillStart)
                    m_AudioSource.PlayOneShot(m_PreDelaySoundWillStart);
                else if (!canStart && m_PreDelaySoundWillNotStart)
                    m_AudioSource.PlayOneShot(m_PreDelaySoundWillNotStart);
            }

            if (canStart)
                OnPreDelayWillStart?.Invoke();
            else
                OnPreDelayWillNotStart?.Invoke();
        }

        // --------------------------------------------------------------------

        public void Play(Animator animator)
        {
            m_Interrupted = false;

            if (m_AttackSound && m_AudioSource)
            {
                m_AudioSource.loop = m_IsAttackSoundLoop;
                if (m_IsAttackSoundLoop)
                {
                    if (m_AudioSource.clip != m_AttackSound)
                    {
                        m_AudioSource.clip = m_AttackSound;
                        m_AudioSource.Play();
                    }
                }
                else
                {
                    m_AudioSource.PlayOneShot(m_AttackSound);
                }
            }

            
            animator.CrossFadeInFixedTime(Animation.Hash, AnimationBlendTime);
            
            if (m_CurrentAnimator != animator)
            {
                if (m_CurrentAnimator != null)
                {
                    m_CurrentAnimator.GetComponent<AnimatorEventHandler>().OnEvent.RemoveListener(m_OnAnimatorEvent);
                }
                
                m_CurrentAnimator = animator;

                if (m_CurrentAnimator != null)
                {
                    m_CurrentAnimator.GetComponent<AnimatorEventHandler>().OnEvent.AddListener(m_OnAnimatorEvent);
                }
            }
            
            if (Attack)
                StartCoroutine(StartAttackDelayed(AttackActivationDelay));


            OnPlay?.Invoke();
        }

        // --------------------------------------------------------------------

        private void OnAnimatorEvent(AnimationEvent e)
        {
            foreach (var callback in m_AnimEventCallbacks)
            {
                if (callback.Event == e.stringParameter)
                {
                    callback.OnEvent?.Invoke();
                }
            }
        }
        
        // --------------------------------------------------------------------

        public void OnNotStarted()
        {
            if (m_AttackNotStartedSound && m_AudioSource)
            {
                m_AudioSource.PlayOneShot(m_AttackNotStartedSound);
            }
        }

        // --------------------------------------------------------------------

        IEnumerator StartAttackDelayed(float delay)
        {
            yield return Yielders.Time(delay);
            Attack.StartAttack();
        }

        // --------------------------------------------------------------------

        public void Complete()
        {
            if (m_AudioSource)
            {
                if (!m_Interrupted && m_AttackCompleteSound)
                {
                    m_AudioSource.Stop();
                    m_AudioSource.PlayOneShot(m_AttackCompleteSound);
                }
            }

            StopAllCoroutines();

            OnComplete?.Invoke();
        }


        // --------------------------------------------------------------------

        public void Interrupt()
        {
            
            if (m_AudioSource)
            {
                m_AudioSource.Stop();
                if (m_AttackInterruptedSound)
                    m_AudioSource.PlayOneShot(m_AttackInterruptedSound);
            }

            OnInterrupt?.Invoke();
            m_Interrupted = true;
        }
    }
}
