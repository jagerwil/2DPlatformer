using UnityEngine;


public class Chest : MonoBehaviour
{
    private bool isOpened = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsOpened", false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();
        Debug.Log("IsEntered!");

        if (character && !isOpened)
        {
            Debug.Log("IsOpened!");
            animator.SetBool("IsOpened", true);
            isOpened = true;
        }
    }
}
