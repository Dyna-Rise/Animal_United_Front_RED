using UnityEngine;

public class EnemyTestDamage : MonoBehaviour
{
    public int life;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerAttack")
        {
            life--;

            if(life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
