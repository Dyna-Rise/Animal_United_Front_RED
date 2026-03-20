using UnityEngine;

public class Enemy3Generator : MonoBehaviour
{
    [Header("生成プレハブ")]
    public GameObject enemy3Prefab;

    [Header("索敵距離")]
    public float distance = 5.0f;

    [Header("生成間隔・生成回数")]
    public float interval = 5.0f;
    public int maxCount = 1;

    float timer = 0; //時間カウント
    int count = 0; //回数カウント


    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerFollower");

        //1回目はすぐに出る
        timer = interval;
    }

    void Update()
    {
        //距離確認
        float d = Vector3.Distance(player.transform.position, transform.position);

        //索敵距離内
        if(d < distance)
        {
            timer += Time.deltaTime;
            //時間が来る＆回数が残っていれば
            if(timer >= interval && maxCount > count)
            {
                Instantiate(
                    enemy3Prefab,
                    transform.position,
                    Quaternion.identity);

                timer = 0;
                count++;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
