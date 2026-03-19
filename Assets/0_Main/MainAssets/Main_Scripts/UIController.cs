using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("各種スライダー")]
    public Slider lifeSlider;
    public Slider player2Slider;
    public Slider player3Slider;

    [Header("ゲームオーバーパネル")]
    public GameObject gameoverPanel;

    PlayerChanger playerChanger;

    void Start()
    {
        lifeSlider.maxValue = 5;
        playerChanger = GameObject.FindGameObjectWithTag("PlayerFollower").GetComponent<PlayerChanger>();
        player2Slider.maxValue = playerChanger.player2TimeMax;
        player3Slider.maxValue = playerChanger.player3TimeMax;

        gameoverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        lifeSlider.value = GameManager.playerLife;
        player2Slider.value = playerChanger.Player2CurrentTime;
        player3Slider.value = playerChanger.Player3CurrentTime;

        if(GameManager.gameState == GameState.gameover)
        {
            gameoverPanel.SetActive(true);
        }
    }
}
