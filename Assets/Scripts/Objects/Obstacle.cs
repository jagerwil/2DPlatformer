using UnityEngine;
using System.Collections;


public class Obstacle : BasicObject
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit)
        {
            Character character = unit as Character;
            if (character != null)
            {
                character.ReceiveDamage(Direction.Down);
            }
            else unit.ReceiveDamage();
        }
    }
}
