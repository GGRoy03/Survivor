using Survivor.Core;

using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Survivor.Audio
{

    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance;

        //
        // Unity Hooks
        //

        private readonly List<AudioSource> m_ActiveAudioSources = new();

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //
        // NOTE:
        // This must be ran after every single system has already enqueued their
        // audio commands.
        //

        private void Update()
        {
            //
            // Push back into the pool audio sources that are done playing their sound.
            //

            for(int audioSourceIdx = 0; audioSourceIdx < m_ActiveAudioSources.Count;)
            {
                //
                // We only increment the index in cases where we don't swap-back
                // erase the active audio source. We use a swap-back approach to avoid copying
                // the buffer multiple times and to simplify the iteration.
                //

                var audioSource = m_ActiveAudioSources[audioSourceIdx];
                if(audioSource != null && !audioSource.isPlaying)
                {
                    m_ActiveAudioSources.RemoveAtSwapBack(audioSourceIdx);

                    ReleaseAudioSource(audioSource);
                }
                else
                {
                    ++audioSourceIdx;
                }
            }

            //
            // Pump the spatial audio queue.
            //

            if(m_SpatialAudioQueue != null && m_ClipTable != null)
            {
                while(m_SpatialAudioQueue.TryDequeue(out SpatialAudio payload))
                {
                    var audioSource = AcquireAudioSource();
                    var audioClip   = m_ClipTable.GetFromKey(payload.Key);

                    if (audioSource != null && audioClip != null)
                    {
                        audioSource.transform.position = payload.Position;
                        audioSource.clip               = audioClip;

                        audioSource.Play();
                    }

                    m_ActiveAudioSources.Add(audioSource);
                }
            }
        }

        //
        // Audio Source Pool
        //

        private readonly Queue<AudioSource> m_AudioSourcePool = new();

        private AudioSource AcquireAudioSource()
        {
            Debug.Assert(m_AudioSourcePool != null);

            AudioSource result = null;

            if (m_AudioSourcePool.Count > 0)
            {
                result = m_AudioSourcePool.Dequeue();
            }
            else
            {
                var audioSourceObject = new GameObject("Audio Source Instance", typeof(AudioSource));
                if (audioSourceObject != null)
                {
                    audioSourceObject.transform.parent = transform;
                    result = audioSourceObject.GetComponent<AudioSource>();
                }
            }

            return result;
        }

        private void ReleaseAudioSource(AudioSource audioSource)
        {
            Debug.Assert(m_AudioSourcePool != null);

            if (audioSource != null)
            {
                m_AudioSourcePool.Enqueue(audioSource);
            }
        }


        //
        // Audio Commands
        //

        [System.Serializable]
        public enum GameAudioKey
        {
            PlayerAttack = 0,
        }

        public struct SpatialAudio
        {
            public GameAudioKey Key;
            public Vector3      Position;
        }

        private readonly Queue<SpatialAudio> m_SpatialAudioQueue = new();

        public void PushAudioCommand(GameAudioKey key, Vector3 position)
        {
            if (m_SpatialAudioQueue != null)
            {
                m_SpatialAudioQueue.Enqueue(new SpatialAudio()
                {
                    Key      = key,
                    Position = position
                });
            }
        }

        [SerializeField]
        private EnumTable<GameAudioKey, AudioClip> m_ClipTable;
    }
}