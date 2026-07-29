using Survivor.Player;
using UnityEngine;

public class BirbBullet : MonoBehaviour
{
    //
    // Unity Hooks
    //

    public bool IsConsumed { get; private set;}

    void Update()
    {
        transform.Translate(m_Speed * Time.deltaTime * m_Direction, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerController player))
        {
            player.Health -= m_Damage;
            IsConsumed     = true;
        }
    }

    //
    //
    //

    private float   m_Speed;
    private float   m_Damage;
    private Vector3 m_Direction;

    public void OnSpawn(Vector3 direction, Vector3 position, float speed, float damage)
    {
        m_Speed     = speed;
        m_Damage    = damage;
        m_Direction = direction;

        transform.position = position;
    }
}
