using UnityEngine;
using System.Collections;


public class Monster : Unit
{
    [SerializeField]
    protected bool haveCrushResist;
    [SerializeField]
    protected bool isBulletProof;
    [SerializeField]
    protected int lives = 1;

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();
        
        if (character && !character.IsInvincible)
        {
            Direction direction = character.getHitDirection(transform.position, boxCollider.size);

            if (!haveCrushResist)
            {
                if (direction == Direction.Down)
                {
                    ReceiveDamage();
                    character.CrushJump();
                }
                else
                {
                    character.ReceiveDamage(direction);
                }
            }
            else
            {
                character.ReceiveDamage(direction);
            }
            
        }

        if (!isBulletProof)
        {
            Projectile projectile = collider.GetComponent<Projectile>();

            if (projectile && projectile.Parent is Character)
            {
                ReceiveDamage();
                projectile.HitSuccessful();
            }
        }
    }

    public override bool ReceiveDamage()
    {
        if (--lives <= 0) StartDying();
        return true;
    }
}
