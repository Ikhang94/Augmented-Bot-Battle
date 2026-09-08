using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class HitBoxNetwork : NetworkBehaviour
{
    public Vector3 direction;
    public int damage;
    public int spGain;
    public float hitStun;
    public BaseBotNetwork owner;
    public Collider hitbox;

    public List<BaseBotNetwork> bots, bothit = new List<BaseBotNetwork>();
    public float scaling = 0.1f;
    public ParticleSystem hitFx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        owner = this.GetComponentInParent<BaseBotNetwork>();
        hitbox = this.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Fonction qui s�ex�cute lorsque le collider d�un robot entre en contact /// avec notre robot. Elle permet stocker le robot en contact dans une liste
    /// </summary>
    /// <param name="other">Param�tre de type collider qui repr�sente le collider du robot entr� en contact avec notre robot</param>
    /// <returns>Fonction de type Void</returns>
    /// <exception cref="ExceptionType">Conditions d'exception</exception>
    private void OnTriggerEnter(Collider other)
    {
        BaseBotNetwork otherComponent = other.GetComponent<BaseBotNetwork>();
        if (otherComponent != null)
        {
            bots.Add(otherComponent);
        }
    }

    /// <summary>
    /// Fonction qui s�ex�cute lorsque le collider d�un robot sort du collider de notre robot. Une fois cela faite, le robot qui est sorti est supprim� de la liste
    /// </summary>
    /// <param name="other">Param�tre de type collider qui repr�sente le collider du robot entr� en contact avec notre robot</param>
    /// <returns>Fonction de type Void</returns>
    /// <exception cref="ExceptionType">Conditions d'exception</exception>
    private void OnTriggerExit(Collider other)
    {
        BaseBotNetwork otherComponent = other.GetComponent<BaseBotNetwork>();
        if (otherComponent != null)
        {
            bots.Remove(otherComponent);
        }
    }

    /// <summary>
    /// Fonction qui permet de vider la liste des robots qui vont se prendre des d�g�ts
    /// </summary>
    /// <returns>Fonction de type Void</returns>
    /// <exception cref="ExceptionType">Conditions d'exception</exception>
    [Rpc(SendTo.ClientsAndHost)]
    public void clearHitboxServerRpc()
    {
        bothit.Clear();
    }

    [Rpc(SendTo.ClientsAndHost)]
    /// <summary>
    /// Fonction qui permet d�ex�cuter la m�thode hurt des robots qui sont dans la liste
    /// </summary>
    /// <returns>Fonction de type Void</returns>
    /// <exception cref="ExceptionType">Conditions d'exception</exception>
    virtual public void hitServerRpc()
    {
        foreach (BaseBotNetwork bot in getBothit())
        {
            if (!IsOwner) return;
            if (!bot.invincible.Value)
            {
                Debug.Log("collide hitbox");
                bot.HurtRPC(damage, hitStun, transform.TransformDirection(direction * owner.transform.lossyScale.y),scaling);
                //owner.addSp(spGain);
                if (hitFx != null)
                {
                    hitFx.time = 0;
                    hitFx.Play();
                }
                //if (scaling < bot.scaling)
                //    bot.scaling -= scaling;
                //else bot.scaling = 0.1f;
            }
        }
    }
    virtual public void hit()
    {
        hitServerRpc();
    }

    /// <summary>
    /// Fonction qui permet de r�cuperer les robots dans la liste
    /// </summary>
    /// <returns>Fonction de type Void</returns>
    /// <exception cref="ExceptionType">Conditions d'exception</e
    public List<BaseBotNetwork> getBothit()
    {
        //Collider detectedColliders = Physics.OverlapBox(colHitbox);

        List<BaseBotNetwork> botList = new List<BaseBotNetwork>();

        foreach (BaseBotNetwork bot in bots)
        {
            if (bot != null && !bothit.Contains(bot))
            {
                botList.Add(bot);
                bothit.Add(bot);
            }
        }

        return botList;
    }
}
