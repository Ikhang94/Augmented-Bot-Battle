using UnityEngine;
using UnityEngine.UI;

public class HpBarNetwork : MonoBehaviour
{
    public BaseBotNetwork player;
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

    public void setPlayer(BaseBotNetwork bot)
    {
        if (bot != null)
        {
            player = bot;
            slider.maxValue = bot.GetHp();
        }
    }

    void showHp()
    {
        if (player != null)
        {
            slider.value = player.GetHp();
        }
    }
}
