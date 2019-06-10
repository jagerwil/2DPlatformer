using UnityEngine;


public class Heart : MonoBehaviour
{
    [SerializeField]
    private int lives = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();
        
        if (character && character.Lives < character.MaxLives)
        {
            character.Lives += lives;
            Destroy(gameObject);
        }
    }
}
