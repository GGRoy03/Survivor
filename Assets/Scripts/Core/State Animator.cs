using System.Collections.Generic;

using UnityEngine;

namespace Survivor.Core
{
    [RequireComponent(typeof(Animator))]
    public abstract class StateAnimator : MonoBehaviour
    {
        private Animator m_Animator;

        //
        // Unity Hooks
        //

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        //
        // Param Hooks
        //

        public void SetParam(float value, int animation)
        {
            if(m_Animator != null)
            {
                m_Animator.SetFloat(animation, value);
            }
        }

        public void SetParam(bool value, int animation)
        {
            if(m_Animator != null)
            {
                m_Animator.SetBool(animation, value);
            }
        }

        public void SetParam(int animation)
        {
            if(m_Animator != null)
            {
                m_Animator.SetTrigger(animation);
            }
        }

        //
        // Helpers
        //

        public bool IsClipPlaying(int animation = 0)
        {
            bool result = false;

            if(m_Animator != null)
            {
                var state = m_Animator.GetCurrentAnimatorStateInfo(0);
                if(animation != 0)
                {
                    result = state.normalizedTime < 1.0f && state.shortNameHash == animation;
                }
                else
                {
                    result = state.normalizedTime < 1.0f;
                }
            }

            return result;
        }
    }
}