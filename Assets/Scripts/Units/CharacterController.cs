using System;
using System.Collections.Generic;
using UnityEngine;


public class CharacterController : MonoBehaviour
{
    #region Internal types

    struct CharacterRaycastOrigins
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;
        public Vector3 bottomLeft;
    }

    public class CharacterCollisionState2D
    {
        public bool right;
        public bool left;
        public bool above;
        public bool below;
        public bool becameGroundedThisFrame;
        public bool wasGroundedLastFrame;
        public bool movingDownSlope;
        
        public bool hasCollision()
        {
            return below || right || left || above;
        }
        
        public void reset()
        {
            right = left = above = below = becameGroundedThisFrame = movingDownSlope = false;
        }
    }

    public class CharacterTriggerState2D
    {
        public bool right;
        public bool left;
        public bool above;
        public bool below;

        public void reset()
        {
            right = left = above = below = false;
        }
    }

    #endregion


    #region Events, properties and fields

    public event Action<RaycastHit2D> onControllerCollidedEvent;
    
    [SerializeField]
    [Range(0.001f, 0.3f)]
    float skinWidth = 0.02f;
    
    public float SkinWidth
    {
        get { return skinWidth; }
        set
        {
            skinWidth = value;
            recalculateDistanceBetweenRays();
        }
    }
    
    public LayerMask platformMask = 0;
    public LayerMask triggerMask = 0;
    public float jumpingThreshold = 0.07f;

    [Range(2, 20)]
    public int totalHorizontalRays = 8;
    [Range(2, 20)]
    public int totalVerticalRays = 4;
    
    [HideInInspector]
    public new Transform transform;

    [HideInInspector]
    public BoxCollider2D boxCollider;

    [HideInInspector]
    public Rigidbody2D rigidBody2D;

    [HideInInspector]
    public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();

    [HideInInspector]
    public CharacterTriggerState2D triggerState = new CharacterTriggerState2D();

    [HideInInspector]
    public Vector3 velocity;

    public bool IsGrounded { get { return collisionState.below; } }
    
    public CharacterTriggerState2D TriggerState
    {
        get { return triggerState; }
    }

    const float kSkinWidthFloatFudgeFactor = 0.001f;

    #endregion
    

    CharacterRaycastOrigins raycastOrigins;
    RaycastHit2D raycastHit;
    List<RaycastHit2D> raycastHitsThisFrame = new List<RaycastHit2D>(2);
    RaycastHit2D triggerRaycast;
    
    float verticalDistanceBetweenRays;
    float horizontalDistanceBetweenRays;
    

    #region MonoBehaviour

    void Awake()
    {
        transform = GetComponent<Transform>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        
        SkinWidth = skinWidth;
        
        for (var i = 0; i < 32; i++)
        {
            if ((triggerMask.value & 1 << i) == 0)
                Physics2D.IgnoreLayerCollision(gameObject.layer, i);
        }
    }
    #endregion
    

    #region Public

    public void move(Vector3 deltaMovement)
    {
        //Save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
        collisionState.wasGroundedLastFrame = collisionState.below;

        //Clear our state
        collisionState.reset();
        raycastHitsThisFrame.Clear();

        primeRaycastOrigins();

        //Now we check movement in the horizontal dir
        if (deltaMovement.x != 0f)
            moveHorizontally(ref deltaMovement);

        //Next, check movement in the vertical dir
        if (deltaMovement.y != 0f)
            moveVertically(ref deltaMovement);

        //Move then update our state
        deltaMovement.z = 0;
        transform.Translate(deltaMovement, Space.World);

        //Only calculate velocity if we have a non-zero deltaTime
        if (Time.deltaTime > 0f)
            velocity = deltaMovement / Time.deltaTime;

        //Set our becameGrounded state based on the previous and current collision state
        if (!collisionState.wasGroundedLastFrame && collisionState.below)
            collisionState.becameGroundedThisFrame = true;

        //Send off the collision events if we have a listener
        if (onControllerCollidedEvent != null)
        {
            for (var i = 0; i < raycastHitsThisFrame.Count; i++)
                onControllerCollidedEvent(raycastHitsThisFrame[i]);
        }
    }
    
    public void warpToGrounded()
    {
        do
        {
            move(new Vector3(0, -1f, 0));
        } while (!IsGrounded);
    }
    
    public void recalculateDistanceBetweenRays()
    {
        //Figure out the distance between our rays in both directions
        //horizontal
        var colliderUseableHeight = boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2f * skinWidth);
        verticalDistanceBetweenRays = colliderUseableHeight / (totalHorizontalRays - 1);

        //vertical
        var colliderUseableWidth = boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2f * skinWidth);
        horizontalDistanceBetweenRays = colliderUseableWidth / (totalVerticalRays - 1);
    }

    #endregion
    

    #region Movement methods
    
    void primeRaycastOrigins()
    {
        //Our raycasts need to be fired from the bounds inset by the skinWidth
        var modifiedBounds = boxCollider.bounds;
        modifiedBounds.Expand(-2f * skinWidth);

        raycastOrigins.topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
        raycastOrigins.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
        raycastOrigins.bottomLeft = modifiedBounds.min;
    }
    
    void moveHorizontally(ref Vector3 deltaMovement)
    {
        bool isGoingRight = deltaMovement.x > 0;
        float rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
        Vector2 rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        Vector3 initialRayOrigin = isGoingRight ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        for (var i = 0; i < totalHorizontalRays; i++)
        {
            var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * verticalDistanceBetweenRays);
            
            raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask);

            if (raycastHit)
            {
                //The bottom ray can hit a slope but no other ray can so we have special handling for these cases
                if (i == 0 && Mathf.RoundToInt(Vector2.Angle(raycastHit.normal, Vector2.up)) != 90)
                {
                    raycastHitsThisFrame.Add(raycastHit);
                    
                    if (!collisionState.wasGroundedLastFrame)
                    {
                        float flushDistance = Mathf.Sign(deltaMovement.x) * (raycastHit.distance - skinWidth);
                        transform.Translate(new Vector2(flushDistance, 0));
                    }
                    break;
                }

                //Set our new deltaMovement and recalculate the rayDistance taking it into account
                deltaMovement.x = raycastHit.point.x - ray.x;
                rayDistance = Mathf.Abs(deltaMovement.x);

                //Remember to remove the skinWidth from our deltaMovement
                if (isGoingRight)
                {
                    deltaMovement.x -= skinWidth;
                    collisionState.right = true;
                }
                else
                {
                    deltaMovement.x += skinWidth;
                    collisionState.left = true;
                }

                raycastHitsThisFrame.Add(raycastHit);
                
                if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
                    break;
            }
        }
    }
    
    void moveVertically(ref Vector3 deltaMovement)
    {
        var isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
        var initialRayOrigin = isGoingUp ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;

        //Apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
        initialRayOrigin.x += deltaMovement.x;
        
        var mask = platformMask;

        for (var i = 0; i < totalVerticalRays; i++)
        {
            var ray = new Vector2(initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y);

            raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
            if (raycastHit)
            {
                //Set our new deltaMovement and recalculate the rayDistance taking it into account
                deltaMovement.y = raycastHit.point.y - ray.y;
                rayDistance = Mathf.Abs(deltaMovement.y);

                //Remember to remove the skinWidth from our deltaMovement
                if (isGoingUp)
                {
                    deltaMovement.y -= skinWidth;
                    collisionState.above = true;
                }
                else
                {
                    deltaMovement.y += skinWidth;
                    collisionState.below = true;
                }

                raycastHitsThisFrame.Add(raycastHit);
                
                if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
                    break;
            }
        }

        
    }

    #endregion
}
