using UnityEngine;

namespace Survivor.Core
{
    public struct PhysicsQueryIterator
    {
        private Collider[] m_Colliders;
        private int        m_CurrentIndex;

        public PhysicsQueryIterator(string mask, float range, Vector3 position)
        {
            int layerMask = LayerMask.GetMask(mask);

            m_CurrentIndex = 0;
            m_Colliders    = Physics.OverlapSphere(position, range, layerMask);
        }

        //public PhysicsQueryIterator(string[] masks, float range, Vector3 position)
        //{
        //    int layerMask = 0;
        //    foreach(var mask in masks)
        //    {
        //        layerMask |= LayerMask.GetMask(mask);
        //    }

        //    m_CurrentIndex = 0;
        //    m_Colliders    = Physics.OverlapSphere(position, range, layerMask);
        //}

        public bool FindNext<T>(out T result) where T : Component
        {
            T foundItem = null;
            while(m_CurrentIndex < m_Colliders.Length)
            {
                var collider = m_Colliders[m_CurrentIndex++];
                if(collider.gameObject.TryGetComponent(out T component))
                {
                    foundItem = component;
                    break;    
                }
            }

            result = foundItem;
            return result != null;
        }
    }
}