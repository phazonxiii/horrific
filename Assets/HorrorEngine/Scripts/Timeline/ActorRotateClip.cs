using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace HorrorEngine
{
    [System.Serializable]
    public class ActorRotateClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] ExposedReference<Transform> m_Target;
        [SerializeField] AnimatorStateHandle m_AnimState = null;
        [SerializeField] float m_AnimFadeTime = 0.25f;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ActorRotateClipBehaviour>.Create(graph);

            ActorRotateClipBehaviour playableBehaviour = playable.GetBehaviour();
            playableBehaviour.Target = m_Target.Resolve(graph.GetResolver());
            playableBehaviour.AnimState = m_AnimState;
            playableBehaviour.AnimFadeTime = m_AnimFadeTime;
            return playable;
        }
    }

    [System.Serializable]
    public class ActorRotateClipBehaviour : ActorAnimatedClipBehaviour
    {
        public Transform Target;

        private float m_Duration;
        private float m_TimeElapsed;
        private Quaternion m_InitialRotation;

        // --------------------------------------------------------------------

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);

            if (!Application.isPlaying)
            {
                return;
            }

            m_Duration = (float)playable.GetDuration();
            m_TimeElapsed = 0f;
            m_InitialRotation = m_Actor.transform.rotation;


        }

        // --------------------------------------------------------------------

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            if (!Application.isPlaying)
            {
                return;
            }

            if (m_TimeElapsed < m_Duration)
            {
                m_TimeElapsed += Time.deltaTime;
                float t = m_TimeElapsed / m_Duration;
                m_Actor.transform.rotation = Quaternion.Slerp(m_InitialRotation, Target.rotation, t);
            }
            else
            {
                m_Actor.transform.rotation = Target.rotation;
            }
        }

        // --------------------------------------------------------------------

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;

            base.OnBehaviourPause(playable, info);

            if (this.HasFinished(playable, info))
            {
                m_Actor.transform.rotation = Target.rotation;
            }
        }
    }
}