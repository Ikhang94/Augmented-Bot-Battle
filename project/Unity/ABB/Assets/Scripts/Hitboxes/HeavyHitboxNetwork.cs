using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class HeavyHitboxNetwork : HitBoxNetwork
{
    public float startHitStun;//Used if this attack is used at the start of a combo
    public float comboHitStun;//Used if this attack is used during a combo
    public bool isCombo = false; //will be used to detect if this was used as a combo starter

    [Rpc(SendTo.ClientsAndHost)]
    override public void hitServerRpc()
    {
        foreach (BaseBotNetwork bot in getBothit())
        {
            if (!IsOwner) return;
            if (!bot.invincible.Value)
            {
                //owner.addSp(spGain);
                if (hitFx != null)
                {
                    hitFx.time = 0;
                    hitFx.Play();
                }
                if (bot.state.Value == "hurt")
                {
                    //isCombo = true;
                    hitStun = comboHitStun;
                }
                else
                {
                    isCombo = false;
                    hitStun = startHitStun;
                }

                bot.HurtRPC(damage, hitStun, transform.TransformDirection(direction * owner.transform.lossyScale.y),scaling);
                //if (scaling < bot.scaling)
                //    bot.scaling -= scaling;
                //else bot.scaling = 0.1f;
            }
        }
    }
}

