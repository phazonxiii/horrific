using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace HorrorEngine
{
   
    [System.Serializable]
    public class ActorMoveClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] ExposedReference<Shape> m_Path;
        [SerializeField] bool m_TeleportToStart;
        [SerializeField] float m_RotationRate = 500;
        [SerializeField] AnimatorStateHandle m_AnimState = null;
        [SerializeField] float m_AnimFadeTime = 0.25f;
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ActorMoveClipBehaviour>.Create(graph);

            ActorMoveClipBehaviour playableBehaviour = playable.GetBehaviour();
            playableBehaviour.Path = m_Path.Resolve(graph.GetResolver());
            playableBehaviour.TeleportToStart = m_TeleportToStart;
            playableBehaviour.RotationRate = m_RotationRate;
            playableBehaviour.AnimState = m_AnimState;
            playableBehaviour.AnimFadeTime = m_AnimFadeTime;
            return playable;
        }
    }

    [System.Serializable]
    public class ActorMoveClipBehaviour : ActorAnimatedClipBehaviour
    {
        public Shape Path;
        public bool TeleportToStart;
        public float RotationRate;

        private bool m_ReachedEnd;
        private double m_Duration;
        private float m_CurrenDuration;
        private Transform m_ActorTransform;

        // --------------------------------------------------------------------

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            base.OnBehaviourPlay(playable, info);

            m_Duration = playable.GetDuration();
            m_ReachedEnd = false;
            m_ActorTransform = m_Actor.transform;
            
            if (TeleportToStart)
            {
                Vector3 initPoint = Path.GetWorldPoint(0);
                Vector3 tangent = Path.GetPointTangent(0);
                m_Actor.PlaceAt(initPoint, Quaternion.LookRotation(tangent));
            }
        }

        // --------------------------------------------------------------------

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            base.ProcessFrame(playable, info, playerData);

            if (m_ReachedEnd)
            {
                return;
            }

            m_CurrenDuration += Time.deltaTime;

            float t = Mathf.Clamp01(m_CurrenDuration / (float)m_Duration);
            Vector3 targetPos = Path.GetWorldPointAtT(t);
            Vector3 targetDir = (targetPos - m_ActorTransform.position).normalized;
            Quaternion targetRot = Quaternion.RotateTowards(m_ActorTransform.rotation, Quaternion.LookRotation(targetDir, Vector3.up), RotationRate * Time.deltaTime);

            if (t >= 1.0f)
            {
                targetPos = Path.GetWorldPoint(Path.Points.Count - 1);
                m_ReachedEnd = true;
            }

            m_ActorTransform.SetPositionAndRotation(targetPos, targetRot);
        }

        // --------------------------------------------------------------------

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);

            if (!Application.isPlaying)
                return;

        }
    }
}