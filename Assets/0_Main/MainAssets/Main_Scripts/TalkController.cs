// TalkController.cs
using System.Collections; // Coroutineのために必要
using TMPro; // TextMeshProを使うために必要
using UnityEngine;
using UnityEngine.SceneManagement;

public class TalkController : MonoBehaviour
{
    [Header("対象テキスト")]
    public TextMeshProUGUI talkText;

    [Header("対象データTalk Data")]
    public TalkData talkData; 
    [Header("表示時間")]
    public float displayDuration = 4.0f;

    [Header("ステージシーン")]
    public string sceneName;


    int currentTalkIndex = 0; // 現在表示中のセリフのインデックス
    Coroutine talkCoroutine; // セリフ表示コルーチンへの参照

    void Start()
    {
        // 初期化: まずはテキストをクリアしておく
        if (talkText != null)
        {
            talkText.text = "";
        }

        // セリフ表示を開始
        StartTalkSequence();
    }

    //シーケンス開始
    public void StartTalkSequence()
    {
        // 既にコルーチンが実行中なら停止
        if (talkCoroutine != null)
        {
            StopCoroutine(talkCoroutine);
        }
        currentTalkIndex = 0; // インデックスをリセット
        talkCoroutine = StartCoroutine(DisplayTalksCoroutine());
    }

    /// セリフを設定秒ごとに順番に表示するコルーチン
    private IEnumerator DisplayTalksCoroutine()
    {
        //全部のセリフをいうまで繰り返し
        while (currentTalkIndex < talkData.talks.Length)
        {
            // 現在のセリフを取得
            Talk currentTalk = talkData.talks[currentTalkIndex];

            // UIにセリフを表示
            if (talkText != null)
            {
                talkText.text = currentTalk.message;
            }

            // 指定された時間待機
            yield return new WaitForSeconds(displayDuration);

            // 次のセリフへ進む
            currentTalkIndex++;
        }

        // すべてのセリフが表示された後の処理
        yield return new WaitForSeconds(3.0f);
        //変数に指定したシーンに飛ぶ
        SceneManager.LoadScene(sceneName);
    }

    // 必要に応じて、外部から次のセリフに進むなどのメソッドを追加することもできます
    // public void NextTalk() { /* ロジック */ }
}