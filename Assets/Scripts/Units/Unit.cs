using UnityEngine;
using System.Collections;


public class Unit : BasicObject
{
    public virtual bool ReceiveDamage()
    {
        Die();
        return true;
    }

    public virtual void StartDying()
    {
        Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
