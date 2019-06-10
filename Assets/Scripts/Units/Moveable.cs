using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Moveable : MonoBehaviour
{
    [SerializeField]
    protected float speed = 2.0f;
    [SerializeField]
    private MovingSide side = MovingSide.Left;
    [SerializeField]
    private float distanceToObstacle = 0.75f;
    [SerializeField]
    private bool checkMoveDistance = false;
    [SerializeField]
    private bool checkFloor = true;
    [SerializeField]
    private float moveDistance = 0.0f;

    private float currentMoveDistance = 0.0f;
    private Vector3 direction;
    private Vector2 size;
    private SpriteRenderer sprite;
    private Vector3 position;

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        size = GetComponent<BoxCollider2D>().size;
        direction = transform.right * (int)side;
        sprite.flipX = side > 0.0f;
    }
    
    private void Update()
    {
        position = transform.position;
        Move();
    }

    private void Move()
    {
        float distance = speed * Time.deltaTime;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position + direction * (distance + distanceToObstacle), 0.05f);
        
        position.y -= size.y / 2;
        Collider2D[] floorColliders = Physics2D.OverlapCircleAll(position + direction * (distance + distanceToObstacle), 0.05f);
        position.y += size.y / 2;

        int length = colliders.Length;
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Character>() != null) --length;
        }

        int floorLength = 0;
        if (checkFloor)
        {
            floorLength = floorColliders.Length;
            foreach (Collider2D collider in colliders)
            {
                if (collider.GetComponent<BasicObject>() != null) --length;
            }
        }

        if (length > 0 || (checkFloor && floorLength == 0) || (checkMoveDistance && currentMoveDistance > moveDistance))
        {
            Turn();
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, 
                                                 distance);
        
        if (checkMoveDistance) currentMoveDistance += distance;
    }

    private void Turn()
    {
        direction *= -1;
        sprite.flipX = direction.x > 0.0f;

        if (checkMoveDistance)
        {
            currentMoveDistance = moveDistance - currentMoveDistance;
        }
    }
}


public enum MovingSide
{
    Left = -1,
    Right = 1
}
