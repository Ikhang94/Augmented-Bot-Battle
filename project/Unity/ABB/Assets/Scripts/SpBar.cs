using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    public BaseBot player;
    public Slider slider;
    //public HpBar HpBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        //player = HpBar.player;
    }

    // Update is called once per frame
    void Update()
    {
        showSp();
    }

    public void setPlayer(BaseBot bot)
    {
        if (bot != null)
        {
            player = bot;
            slider.maxValue = bot.maxSp;
        }
    }

    void showSp()
    {
        if (player != null)
        {
            slider.value = player.sp;
            slider.maxValue = player.maxSp;
        }
    }
}
