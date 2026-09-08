using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HitBox : MonoBehaviour
{
    public Vector3 direction;
    public int damage;
    public int spGain;
    public float hitStun;
    public BaseBot owner;
    public Collider hitbox;
    public List<BaseBot> bots, bothit = new List<BaseBot>();
    public float scaling = 0.1f;
    public ParticleSystem hitFx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        owner = this.GetComponentInParent<BaseBot>();
        hitbox = this.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Function that's called when the robot collider collides with the robot, it stocks the robot in the list
    /// </summary>
    /// <param name="other">Collider parameter that represents the robot that collided with the collider</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    private void OnTriggerEnter(Collider other)
    {
        BaseBot otherComponent = other.GetComponent<BaseBot>();
        if (otherComponent != null)
        {
            bots.Add(otherComponent);
        }
    }

    /// <summary>
    /// Function that's called when the robot leaves the collider, it removes the robot from the list
    /// </summary>
    /// <param name="other">Collider parameter that represents the robot that left the collider</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    private void OnTriggerExit(Collider other)
    {
        BaseBot otherComponent = other.GetComponent<BaseBot>();
        if (otherComponent != null)
        {
            bots.Remove(otherComponent);
        }
    }

    /// <summary>
    /// Function that empties the list of robots that were hit
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void clearHitbox()
    {
        bothit.Clear();
    }

    /// <summary>
    /// Function that calls the hurt function of the robot, simulating a hit
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    virtual public void hit()
    {

        foreach (BaseBot bot in getBothit())
        {
            if (!bot.invincible)
            {
                bot.Hurt(damage, hitStun, transform.TransformDirection(direction * owner.transform.lossyScale.y));
                owner.addSp(spGain);
                if (hitFx!=null) 
                {
                    hitFx.time = 0;
                    hitFx.Play(); 
                }
                if (scaling < bot.scaling)
                    bot.scaling -= scaling;
                else bot.scaling = 0.1f;
            }
        }
    }

    /// <summary>
    /// Function that gets the robot in the list
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</e
    public List<BaseBot> getBothit()
    {
        //Collider detectedColliders = Physics.OverlapBox(colHitbox);

        List<BaseBot> botList = new List<BaseBot>();

        foreach (BaseBot bot in bots)
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
