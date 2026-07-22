using Survivor.Core;

using System;
using System.Collections;
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
            // Pump The Spatial Audio Queue
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
                        audioSource.PlayOneShot(audioClip);
                    }

                    ReleaseAudioSource(audioSource);
                }
            }
        }

        //
        // Audio Source Pool
        //

        private Queue<AudioSource> m_AudioSourcePool;

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
            Test = 0,
            AnotherOne = 1,
            AndYetAnotherOne = 2,
        }

        public struct SpatialAudio
        {
            public GameAudioKey Key;
            public Vector3      Position;
        }

        private Queue<SpatialAudio> m_SpatialAudioQueue;

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

        //
        // I'd like to expose this nicely in the editor as well. Somehow.
        // With some safety checks and whatnot... Okay. So... and I mean.
        // Uhm. Still need the editor side of things. This is basically the only thing
        // that's missing. It's for sure custom editor territory, though I am widly unsure
        // on how to actually implement it... Perhaps... we just read this data from a scriptable
        // object or something.
        //

        [SerializeField]
        private EnumTable<GameAudioKey, AudioClip> m_ClipTable;

    }
}