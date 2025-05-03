using UnityEngine;
using UnityEngine.Audio;

using System.Collections;
using System.Collections.Generic;

namespace HorrorEngine
{
    public class MusicManager : SingletonBehaviour<MusicManager>
    {
        private struct MusicEntry
        {
            public AudioClip Clip;
            public float Volume;
        }

        [SerializeField] private AudioSource m_MainChannel;

        private Coroutine m_FadeRoutine;

        private List<MusicEntry> m_Music = new List<MusicEntry>();


        // --------------------------------------------------------------------

        public void Push(AudioClip clip, float volume, float duration)
        {
            m_Music.Add(new MusicEntry()
            {
                Clip = clip,
                Volume = volume
            });


            if (isActiveAndEnabled && (m_MainChannel.clip != clip || volume != m_MainChannel.volume))
                m_FadeRoutine = StartCoroutine(MusicTransition(clip, volume, duration));
        }

        // --------------------------------------------------------------------

        public void Pop(AudioClip clip, float duration)
        {
            if (m_Music.Count == 0 || m_Music[m_Music.Count - 1].Clip != clip)
            {
                Debug.LogWarning("MusicManager: Clip did not match last entry. It couldn't be popped");
            }

            m_Music.RemoveAt(m_Music.Count - 1);

            MusicEntry nextClip = m_Music.Count > 0 ? m_Music[m_Music.Count - 1] : new MusicEntry();

            if (isActiveAndEnabled && (m_MainChannel.clip != clip || m_MainChannel.volume > 0))
                m_FadeRoutine = StartCoroutine(MusicTransition(nextClip.Clip, nextClip.Volume, duration));
        }

        // --------------------------------------------------------------------

        private IEnumerator MusicTransition(AudioClip toClip, float volume, float duration)
        {            
            if (m_MainChannel.isPlaying)
                yield return Fade(m_MainChannel.volume, 0f, duration * 0.5f);

            if (toClip)
            {
                m_MainChannel.clip = toClip;
                m_MainChannel.Play();

                yield return Fade(m_MainChannel.volume, volume, duration * 0.5f);
            }
        }

        // --------------------------------------------------------------------

        public void Play(AudioClip clip, float volume)
        {
            if (m_FadeRoutine != null)
                StopCoroutine(m_FadeRoutine);

            m_Music.Clear();
            if (!m_MainChannel.isPlaying)
                m_MainChannel.volume = 0;

            Push(clip, volume, 0f);
        }

        // --------------------------------------------------------------------

        public void Stop()
        {
            m_MainChannel.Stop();
            m_MainChannel.clip = null;
            m_MainChannel.volume = 0;
            m_Music.Clear();
        }

        // --------------------------------------------------------------------

        public void FadeOut(float duration = 1f)
        {
            FadeOutTo(0f, duration);
        }

        // --------------------------------------------------------------------

        public void FadeOutTo(float to, float duration = 1f)
        {
            float volume = m_MainChannel.volume;
            m_FadeRoutine = StartCoroutine(Fade(volume, to, duration));
        }

        // --------------------------------------------------------------------

        public void FadeIn(float toVolume, float duration = 1f)
        {
            float volume = m_MainChannel.volume;
            FadeInFrom(volume, toVolume, duration);
        }

        // ----------------------------- ---------------------------------------

        public void FadeInFrom(float from, float volume, float duration = 1f)
        {
            m_FadeRoutine = StartCoroutine(Fade(from, volume, duration));
        }

        // --------------------------------------------------------------------

        public IEnumerator Fade(float from, float to, float duration)
        {
            float t = 0;
            while (t < duration)
            {
                t = t + Time.deltaTime;
                yield return Yielders.EndOfFrame;

                m_MainChannel.volume = t > 0 ? Mathf.Lerp(from, to, t / duration) : from;
            }

            m_MainChannel.volume = to;
        }
    }
}