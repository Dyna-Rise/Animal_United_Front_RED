using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [Header("切り替え先シーン名")]
    public string sceneName;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(NextSceneLoad());
        }
    }

    //ちょっと待ってからシーン切り替え
    IEnumerator NextSceneLoad()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneName);
    }
}
