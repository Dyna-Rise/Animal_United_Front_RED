using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("行先シーン")]
    public string sceneName;

    bool isStart;

    void Start()
    {
        StartCoroutine(IntervalStart());
    }

    void OnAttack(InputValue value)
    {
        if (value.isPressed && isStart)
        {
            SceneLoad();
        }
    }

    public void SceneLoad()
    {
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator IntervalStart()
    {
        yield return new WaitForSeconds(0.5f);
        isStart = true;
    }
}
