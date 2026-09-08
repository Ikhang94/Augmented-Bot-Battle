using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public BaseBot player;
    public BaseBotNetwork playerNetwork;
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        showHp();
    }

    public void setPlayer(BaseBot bot)
    {
        if (bot != null)
        {
            player = bot;
            slider.maxValue = bot.GetHp();
        }
    }

    public void setPlayer(BaseBotNetwork bot)
    {
        if (bot != null)
        {
            playerNetwork = bot;
            slider.maxValue = bot.GetHp();
        }
    }

    void showHp()
    {
        if (playerNetwork != null)
        {
            slider.value = playerNetwork.GetHp();
        }
        if (player != null)
        {
            slider.value = player.GetHp();
        }
    }
}
