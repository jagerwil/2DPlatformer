using UnityEngine;
using System.Collections;


public class Character : Unit
{
    #region Events, properties and fields

    [SerializeField]
    private bool isPcInput = false;
    [SerializeField]
    [Range(0, 5)]
    private int lives = 5;
    [SerializeField]
    [Range(0, 5)]
    private int maxLives = 5;
    [SerializeField]
    private int ammoCount = 10;
    [SerializeField]
    private int coinsCount = 0;
    [SerializeField]
    private float projectileSpeed = 8.0f;
    [SerializeField]
    private float projectileLifetime = 1.4f;
    [SerializeField]
    private float gravity = -25.0f;
    [SerializeField]
    private float runSpeed = 3.0f;
    [SerializeField]
    private float jumpHeight = 2.5f;
    [SerializeField]
    private float crushJumpHeight = 2.0f;
    [SerializeField]
    private float inAirDamping = 1.0f;
    [SerializeField]
    private float blockMovingTime = 0.2f;
    [SerializeField]
    private float invincibleAfterDamageTime = 3.0f;
    [SerializeField]
    private float disappearBeforeDieTime = 0.3f;
    
    private bool isDied = false;
    private bool isMovingBlocked = false;
    private bool isInvincible = false;
    private bool isMoveButtonPressed = false;
    private float moveAxis = 0.0f;
    private Vector2 velocity = new Vector2(0.0f, 0.0f);

    private Canvas canvas;

    private Projectile projectileResource;
    private Animator animator;
    private SpriteRenderer sprite;
    new private Rigidbody2D rigidbody;
    private Transform spawnProjectilePoint;
    private CharacterController controller;
    private CharacterTimer timer;

    #endregion


    #region Getters and setters

    public int Lives
    {
        get { return lives; }
        set
        {
            lives = Mathf.Min(value, maxLives);
            lives = Mathf.Max(lives, 0);
            canvas.livesBar.Refresh(lives);
        }
    }

    public int MaxLives
    {
        get { return maxLives; }
    }

    public int AmmoCount
    {
        set
        {
            ammoCount = value;
            canvas.ammoBar.Refresh(ammoCount);
        }
        get { return ammoCount; }
    }

    public int CoinsCount
    {
        set
        {
            coinsCount = value;
            canvas.coinsBar.Refresh(coinsCount);
        }
        get { return coinsCount; }
    }

    public float BlockMovingTime
    {
        get { return blockMovingTime; }
    }

    public float InvincibleAfterDamageTime
    {
        get { return invincibleAfterDamageTime; }
    }

    public float DisappearBeforeDieTime
    {
        get { return disappearBeforeDieTime; }
    }

    public bool IsMovingBlocked
    {
        get { return isMovingBlocked; }
        set { isMovingBlocked = value; }
    }

    public bool IsInvincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }

    public bool IsDied
    {
        get { return isDied; }
        set { isDied = value; }
    }

    public SpriteRenderer Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }
    
    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    #endregion


    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();
        canvas = FindObjectOfType<Canvas>();

        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        spawnProjectilePoint = transform.Find("SpawnProjectilePoint");
        controller = GetComponent<CharacterController>();
        timer = GetComponent<CharacterTimer>();
        
        projectileResource = Resources.Load<Projectile>("Projectiles/Fireball");
    }

    private void Start()
    {
        Lives = maxLives;


        canvas.livesBar.Refresh(lives);
        canvas.ammoBar.Refresh(ammoCount);
        canvas.coinsBar.Refresh(coinsCount);

        MoveButtonClicked(1.0f);
        Move();
        MoveButtonUnclicked(1.0f);
    }

    private void Update()
    {
        if (!isDied)
        {
            if (isPcInput)
            {
                if (Input.GetButton("Horizontal"))
                {
                    isMoveButtonPressed = true;
                    moveAxis = Input.GetAxis("Horizontal");
                }
                else
                {
                    isMoveButtonPressed = false;
                    moveAxis = 0.0f;
                }

                if (Input.GetButtonDown("Fire1")) Shoot();
                if (Input.GetButtonDown("Jump")) Jump();
                if (Input.GetButtonDown("StopScene")) StopScene();
            }
            Move();
        }
    }

    private void StopScene()
    {
        if (Time.timeScale < 0.01f) Time.timeScale = 1.0f;
        else Time.timeScale = 0.0f;
    }

    #endregion


    #region Public methods

    public void MoveButtonClicked(float axis)
    {
        isMoveButtonPressed = true;
        moveAxis = axis;
    }

    public void MoveButtonUnclicked(float axis)
    {
        if (axis == moveAxis)
        {
            isMoveButtonPressed = false;
            moveAxis = 0.0f;
        }
    }

    public void Jump()
    {
        if (controller.IsGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            State = CharState.Jump;
        }
    }

    public void CrushJump()
    {
        velocity.y = Mathf.Sqrt(2f * crushJumpHeight * -gravity);
    }

    public void Shoot()
    {
        if (!isInvincible && ammoCount > 0)
        {
            --AmmoCount;
            Projectile projectile = Instantiate(projectileResource, spawnProjectilePoint.position, 
                                                projectileResource.transform.rotation) as Projectile;

            projectile.gameObject.layer = 9;
            projectile.Speed = projectileSpeed;
            projectile.Parent = this;
            projectile.Direction = projectile.transform.right * (sprite.flipX ? -1.0f : 1.0f);

            projectile.Activate(projectileLifetime);
        }
    }

    public bool ReceiveDamage(Direction direction)
    {
        if (isInvincible) return false;

        Debug.Log("Direction: " + direction.ToString());
        if (direction == Direction.Down)
        {
            velocity.y = 7.0f;
        }
        else if (direction == Direction.Left)
        {
            timer.StartTimer("BlockMoving");

            velocity.y = 5.0f;
            velocity.x = 6.0f;
        }
        else if (direction == Direction.Right)
        {
            timer.StartTimer("BlockMoving");

            velocity.y = 5.0f;
            velocity.x = -6.0f;
        }
        else //if (direction == Direction.Up)
        {
            velocity.y = -2.0f;
        }
        return ReceiveDamage();
    }

    public override bool ReceiveDamage()
    {
        //isInvictible already checked
        Lives--;
        Debug.Log("Receive Damage: " + lives);

        if (Lives <= 0)
        {
            StartDying();
        }
        else
        {
            timer.StartTimer("Invincibility");
        }

        return true;
    }

    public Direction getHitDirection(Vector3 position, float radius)
    {
        return getHitDirection(position, new Vector2(radius * 2, radius * 2));
    }

    public Direction getHitDirection(Vector3 position, Vector2 size)
    {
        Direction direction = Direction.Down; //Set Down side by default
        float angle = Vector2.SignedAngle(transform.right, (position - transform.position));
        float cornerAngle = Vector2.SignedAngle(transform.right, size);

        if (Mathf.Abs(angle) > 180 - cornerAngle) //Left side
        {
            direction = Direction.Left;
        }
        else if (angle <= 180 - cornerAngle && angle >= cornerAngle) //Up side
        {
            direction = Direction.Up;
        }
        else if (Mathf.Abs(angle) < cornerAngle) //Right side
        {
            direction = Direction.Right;
        }

        return direction;
    }
    
    #endregion


    #region Private methods

    private void Move()
    {
        float axis = 0.0f;

        if (isMoveButtonPressed && !isMovingBlocked)
        {
            axis = moveAxis;

            if (controller.IsGrounded)
            {
                State = CharState.Run;
                velocity.x = axis * runSpeed;
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, axis * runSpeed, Time.deltaTime * inAirDamping);
            }
        }
        else
        {
            if (controller.IsGrounded)
            {
                velocity.x = 0;
                State = CharState.Idle;
            }
        }

        if (!controller.IsGrounded)
        {
            State = (velocity.y >= 0.0f ? CharState.Jump : CharState.Fall);
        }

        //Apply gravity before moving
        velocity.y += gravity * Time.deltaTime;

        controller.move(velocity * Time.deltaTime);

        //Grab our current velocity to use as a base for all calculations
        velocity = controller.velocity;

        if (axis != 0.0f) sprite.flipX = axis < 0.0f;
    }

    public override void StartDying()
    {
        isDied = true;

        timer.StartTimer("DisappearBeforeDie");
        canvas.HideWidgets();
        canvas.dieMessage.ShowMessage();

        Debug.Log("Character is died ;(");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Projectile projectile = collider.gameObject.GetComponent<Projectile>();
        
        if (projectile && this != projectile.Parent)
        {
            Direction direction = projectile.Direction.x > 0.0f ? Direction.Left : Direction.Right;
            if (ReceiveDamage(direction))
            {
                projectile.HitSuccessful();
            }
        }
    }

    #endregion
}


public enum CharState
{
    Idle,
    Run,
    Jump,
    Fall
}

public enum Direction
{
    Up = 11,
    Down = -11,
    Left = -1,
    Right = 1
}
