using UnityEngine;


public class Coin : MonoBehaviour
{
    [SerializeField]
    private int coinsCount = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();

        if (character)
        {
            character.CoinsCount += coinsCount;
            Destroy(gameObject);
        }
    }
}
