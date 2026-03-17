using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(player != null)
        {
            transform.position = player.transform.position;
        }
    }

    public void TargetReset()
    {
        //対象を取りなおし
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
