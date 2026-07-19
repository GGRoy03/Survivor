using System;
using System.Collections.Generic;

using UnityEngine;

namespace Survivor.Event
{
    public class EventManager : MonoBehaviour
    {
        // ================================================
        // [SECTION] Unity Callbacks
        // ================================================

        private static EventManager m_Instance;
        public static EventManager Instance => m_Instance;

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ================================================
        // [SECTION] Event Listening
        // ================================================

        private Dictionary<int, Delegate> m_Listeners = new();

        public void AddListener<T>(Action<T> listener)
        {
            int type = typeof(T).GetHashCode();
            if (m_Listeners.TryGetValue(type, out var existing))
            {
                m_Listeners[type] = Delegate.Combine(existing, listener);
            }
            else
            {
                m_Listeners[type] = listener;
            }
        }

        public void RemoveListener<T>(Action<T> listener)
        {
            int type = typeof(T).GetHashCode();
            if (m_Listeners.TryGetValue(type, out var existing))
            {
                var result = Delegate.Remove(existing, listener);
                if (result == null)
                {
                    m_Listeners.Remove(type);
                }
                else
                {
                    m_Listeners[type] = result;
                }
            }
        }

        // ================================================
        // [SECTION] Event Pushing
        // ================================================

        public void PushEvent<T>(T payload)
        {
            int type = typeof(T).GetHashCode();
            if (m_Listeners.TryGetValue(type, out var listeners))
            {
                var invocationList = listeners.GetInvocationList();
                foreach(var invocation in invocationList)
                {
                    var action = invocation as Action<T>;
                    if(action != null)
                    {
                        action.Invoke(payload);
                    }
                }
            }
        }
    }
}