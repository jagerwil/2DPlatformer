using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicObject : MonoBehaviour
{
    protected BoxCollider2D boxCollider;

    public BoxCollider2D BoxCollider
    {
        get { return boxCollider; }
    }

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
