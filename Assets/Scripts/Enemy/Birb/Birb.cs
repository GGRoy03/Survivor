using Survivor.Audio;
using Survivor.Player;

using System.Collections.Generic;

using Unity.Collections;
using UnityEngine;

namespace Survivor.Enemy
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class Birb : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject   m_BirbBulletPrefab;
        [SerializeField] private GameObject   m_BulletSpawnPoint;
        [SerializeField] private BirbBehavior m_Behavior;     

        //
        // Unity Hooks
        //

        private BirbState        m_CurrentState;
        private EnemyAnimator    m_Animator;
        private PlayerController m_Player;

        private void Awake()
        {
            //
            // Create the states
            //

            Idle   = gameObject.AddComponent<BirbIdle>();
            Attack = gameObject.AddComponent<BirbAttack>();
            Dead   = gameObject.AddComponent<BirbDead>();

            //
            // Bind the dependencies
            //

            m_Player   = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            m_Animator = GetComponent<EnemyAnimator>();

            //
            // Set the start state.
            //

            m_CurrentState = Idle;
        }

        private void Update()
        {
            if(m_CurrentState != null)
            {
                //
                // Update the current state
                //

                m_CurrentState.OnUpdate(this, m_Behavior, m_Player, m_Animator, AudioSystem.Instance);

                //
                // Try to push some bullets from the active set back to the pool.
                //

                if(m_ActiveBullets != null && m_BulletPool != null)
                {
                    UpdateActiveList();
                }
            }    
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player Weapon"))
            {
                if (m_CurrentState != null)
                {
                    ChangeState(Dead);
                }
            }
        }

        //
        // Bullet Pool
        //

        private readonly Queue<BirbBullet> m_BulletPool    = new();
        private readonly List<BirbBullet>  m_ActiveBullets = new();

        public BirbBullet AcquireBullet()
        {
            BirbBullet result = null;
     
            if(m_BulletPool != null)
            {
                if(!m_BulletPool.TryDequeue(out result))
                {
                    var bulletObject = Instantiate(m_BirbBulletPrefab);
                    if(bulletObject != null)
                    {
                        bulletObject.SetActive(true);

                        result = bulletObject.GetComponent<BirbBullet>();            
                    }
                }
            }

            return result;
        }

        public void ReleaseBullet(BirbBullet bullet)
        {
            if(m_BulletPool != null && bullet != null)
            {
                bullet.gameObject.SetActive(false);

                m_BulletPool.Enqueue(bullet);
            }
        }

        private void UpdateActiveList()
        {
            Debug.Assert(m_BulletPool    != null);
            Debug.Assert(m_ActiveBullets != null);

            float sqrBulletRange = m_Behavior.BulletRange * m_Behavior.BulletRange;
            for(int bulletIdx = 0; bulletIdx < m_ActiveBullets.Count;)
            {
                var bullet = m_ActiveBullets[bulletIdx];
                if(bullet != null)
                {
                    var sqrDistance = Math.SqrDistance(transform, bullet.transform);
                    if (sqrDistance > sqrBulletRange || bullet.IsConsumed)
                    {
                        m_ActiveBullets.RemoveAtSwapBack(bulletIdx);
                        ReleaseBullet(bullet);
                    }
                    else
                    {
                        ++bulletIdx;
                    }
                }
            }
        }

        //
        // State-Machine Interface
        //
        public BirbIdle   Idle   { get; private set; }
        public BirbAttack Attack { get; private set; }
        public BirbDead   Dead   { get; private set; }

        public void ChangeState(BirbState state)
        {
            m_CurrentState = state;
        }

        public Vector3 BulletSpawnPoint => m_BulletSpawnPoint.transform.position;
    }

    //
    // Inheriting from Birb is nice and easy, but it's almost certainly a mistake.
    //

    public abstract class BirbState : MonoBehaviour
    {
        public abstract void OnUpdate(Birb birb, BirbBehavior behavior, PlayerController player, EnemyAnimator animator, AudioSystem audio);
    }
}