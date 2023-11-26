using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float jumpCooldown = 0.5f;
    private float timeSinceLastJump = 0;
    private bool grounded;

    Vector2 vecGravity;

    [Header("Layers And Speed")]
    [SerializeField]private LayerMask floorLayer;
    [SerializeField]private LayerMask edgeLayer;
    [SerializeField]private float speed;


    [Header("Jumping System")]
    [SerializeField]private float jumpTime;
    [SerializeField]private float jumpMultiplier;
    [SerializeField]private float jumpStrength;
    [SerializeField]private float fallMultiplier;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    private void Update()
    {
        timeSinceLastJump += Time.deltaTime;
        float horizontalDirection = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        if(horizontalDirection < -0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalDirection > 0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if(Input.GetKey(KeyCode.Space) && timeSinceLastJump > jumpCooldown)
        {
            jump();
            timeSinceLastJump = 0;
        }


        if(onWall() == true)
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }

        if(body.velocity.y < 0)
        {
            body.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }
            anim.SetBool("isRunning", horizontalDirection != 0 && isGrounded() == true && onWall() == false);
            anim.SetBool("grounded", grounded);
        
    }


    private void jump()
    {
        if(isGrounded() == true)
        {
            body.velocity = new Vector2(body.velocity.x, jumpStrength);
            grounded = false;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0 , Vector2.down, 0.1f, floorLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0 ,new Vector2(transform.localScale.x, 0), 0.1f, edgeLayer);
        return raycastHit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}