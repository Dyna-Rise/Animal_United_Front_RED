using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;

    [Header("最小・最大座標")]
    public float minX = 0.0f;
    public float maxX = 10.0f;
    public float minY = 0.0f;
    public float maxY = 10.0f;

    [Header("追随スピード")]
    public float speed = 5.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollower");
    }

    void LateUpdate()
    {
        float x = player.transform.position.x;
        if(x < minX) x = minX;
        else if (x > maxX) x = maxX;

        float y = player.transform.position.y + 0.5f;
        if (y < minY) y = minY;
        else if (y > maxX) y = maxY;

        Vector3 targetPosition = new Vector3(x, y, -10);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }
}
