using UnityEngine;

public class GuardController : MonoBehaviour
{
    public bool guarded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        guarded = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("PlayerAttack"))
        {
            //Debug.Log(tag);
            Destroy(other);

            guarded = true;
        }
    }
}
