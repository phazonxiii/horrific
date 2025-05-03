using UnityEngine;
using UnityEngine.AI;

namespace HorrorEngine
{
    public class EnemyStateAttack : ActorStateWithDuration
    {
        [SerializeField] EnemyStateAlerted m_AlertedState;
        [SerializeField] float m_FacingSpeed = 1f;
        [SerializeField] AttackMontage m_AttackMontage;
        [SerializeField] bool m_RotateTowardsTarget = true;
        public float AttackDistance = 1f;
        public float Cooldown = 3;

        private NavMeshAgent m_NavMeshAgent;
        private EnemySensesController m_EnemySenses;
        private float m_LastAttackTime;
        private NavMeshPath m_NavPath;

        // --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            m_NavPath = new NavMeshPath();
            m_NavMeshAgent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
        }

        // --------------------------------------------------------------------

        public override void StateEnter(IActorState fromState)
        {
            if (m_AnimationState)
                Debug.LogWarning("AnimationState will be overwritten by the attack animation", gameObject);

            base.StateEnter(fromState);

            m_Duration = m_AttackMontage.Duration;

            m_AttackMontage.Play(Actor.MainAnimator);
        }

        // --------------------------------------------------------------------

        public override void StateUpdate()
        {
            base.StateUpdate();

            // Face target
            if (m_RotateTowardsTarget)
            {
                Vector3 lookPos = m_EnemySenses.LastKnownPosition - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                Actor.transform.rotation = Quaternion.Slerp(Actor.transform.rotation, rotation, Time.deltaTime * m_FacingSpeed);
            }

        }


        // --------------------------------------------------------------------

        public override void StateExit(IActorState intoState)
        {
            m_AnimationState = null;
            m_LastAttackTime = Time.time;

            base.StateExit(intoState);
        }

        // --------------------------------------------------------------------

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();

            if (m_EnemySenses.IsPlayerDetected)
                SetState(m_AlertedState);
        }

        // --------------------------------------------------------------------

        public virtual bool CanEnter()
        {
            m_NavPath.ClearCorners();
            m_NavMeshAgent.CalculatePath(m_EnemySenses.LastKnownPosition, m_NavPath);
            
            if ((m_NavPath.status != NavMeshPathStatus.PathInvalid) && (m_NavPath.corners.Length > 1))
            {
                float distToTarget = 0;
                for (int i = 1; i < m_NavPath.corners.Length; ++i)
                {
                    distToTarget += Vector3.Distance(m_NavPath.corners[i - 1], m_NavPath.corners[i]);
                    if (distToTarget > AttackDistance)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return (Time.time - m_LastAttackTime) > Cooldown;
        }
    }
}
