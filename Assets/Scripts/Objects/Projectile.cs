using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 8.0f;
    
    private BasicObject parent;
    new private CircleCollider2D collider;
    private Vector3 direction;
    private SpriteRenderer sprite;

    public BasicObject Parent
    {
        set { parent = value; }
        get { return parent; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public Vector3 Direction
    {
        set
        {
            direction = value;
            sprite.flipX = direction.x < 0.0f;
        }
        get
        {
            return direction;
        }
    }

    public CircleCollider2D Collider
    {
        get { return collider; }
    }

    public Color Color
    {
        set { sprite.color = value; }
    }

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
    }

    public void Activate(float lifeTime)
    {
        Destroy(gameObject, lifeTime);
    }
    
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    public void HitSuccessful()
    {
        Destroy(gameObject, 0.01f);
    }
}
