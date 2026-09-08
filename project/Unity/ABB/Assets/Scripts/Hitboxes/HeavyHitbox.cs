using UnityEngine;

public class HeavyHitbox : HitBox
{
    public float startHitStun ;//Used if this attack is used at the start of a combo
    public float comboHitStun ;//Used if this attack is used during a combo
    public bool isCombo = false; //will be used to detect if this was used as a combo starter
    

    override public void hit()
    {
        foreach (BaseBot bot in getBothit())
        {
            if (!bot.invincible)
            {
                owner.addSp(spGain);
                if (hitFx != null)
                {
                    hitFx.time = 0;
                    hitFx.Play();
                }
                if (bot.state == "hurt")
                {
                    //isCombo = true;
                    hitStun = comboHitStun;
                }
                else
                {
                    isCombo = false;
                    hitStun = startHitStun;
                }

                bot.Hurt(damage, hitStun, transform.TransformDirection(direction * owner.transform.lossyScale.y));
                if (scaling < bot.scaling)
                    bot.scaling -= scaling;
                else bot.scaling = 0.1f;
            }
        }
    }
}

