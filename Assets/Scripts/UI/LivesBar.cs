using UnityEngine;
using System.Collections;


public class LivesBar : MonoBehaviour
{
    private Transform[] hearts;
    private Character character;
    private int currentLives = 1;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        hearts = new Transform[character.MaxLives];

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = transform.GetChild(i);
        }
    }

    public void Refresh(int lives)
    {
        if (lives == currentLives) return;
        else if (lives < currentLives)
        {
            for (int i = lives; i < currentLives; ++i)
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = currentLives; i < lives; ++i)
            {
                hearts[i].gameObject.SetActive(true);
            }
        }
        
        currentLives = lives;
    }
}
