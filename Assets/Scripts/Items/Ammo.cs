using UnityEngine;


public class Ammo : MonoBehaviour
{
    [SerializeField]
    private int ammoCount = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();

        if (character)
        {
            character.AmmoCount += ammoCount;
            Destroy(gameObject);
        }
    }
}
