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
    }
}