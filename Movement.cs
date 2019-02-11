using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MovementBase {

    public GameObject walkparticle;
    public AudioSource music;
    public AudioClip walking;
    Rigidbody2D rb;
    public bool walking;
    public bool canmove;
    public bool grounded;
    public LayerMask whatTohit;
    float Gravityphy;
    Dash dashs;
    float maxClimbAngle = 45f;
	public Animator anim;
    public BoxCollider2D box;
	Vector3 lastdist;
	float timer;
    bool m_ToggleChange;
    bool blocked;
    void Awake()
    {
		StartCoroutine(checkdistance());
        rb = GetComponent<Rigidbody2D>();
        dashs = gameObject.GetComponent<Dash>();
    }
	IEnumerator checkdistance()
	{
		while(true)
		{
			
			float distance = Vector3.Distance(lastdist,transform.position);
			if(distance<.01f)
				blocked = true;
			else
				blocked=false;
			lastdist = transform.position;
			yield return new WaitForSeconds(.1f);
		}
	}
    void GroundCheck()
    {
        RaycastHit2D left = Physics2D.Raycast(new Vector2(transform.position.x-.35f, transform.position.y), -Vector2.up, 1f, whatTohit);
        RaycastHit2D right = Physics2D.Raycast(new Vector2(transform.position.x+.35f, transform.position.y), -Vector2.up, 1f, whatTohit);
        Debug.DrawRay(new Vector2(transform.position.x - .35f, transform.position.y), -Vector2.up, Color.red);
        if (left.collider != null||right.collider != null)
            grounded = true;
        else
            grounded = false;
    }

    void Movement(float speed)
    {

        float h = Input.GetAxisRaw("Horizontal");

        RaycastHit2D down = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .5f), Vector2.right * h, .4f, whatTohit);
        RaycastHit2D up = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .5f), Vector2.right * h, .4f, whatTohit);
        RaycastHit2D middle = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right * h, .4f, whatTohit);
        //walkparticle.SetActive(true);
        if (down == true)
        {
            float slopeAngle = Vector2.Angle(down.normal, Vector2.up);
            if (slopeAngle <= maxClimbAngle)
            {
                 if (h == 0)
                 {
                     box.sharedMaterial.friction = 2;//friction.friction = 10;
                 }
                ClimbSlope(rb.velocity, slopeAngle);
            }
        }

        if (h != 0)
        {
            if (down == false && up == false && middle == false)
            {
                walking = true;
                if (dashs.dashe != true)
                {
                    rb.velocity += new Vector2(1, 0) * speed * h;
                }
                else
                {
                    rb.AddForce((Vector2.right * dashs.speed * Time.deltaTime * h), ForceMode2D.Impulse);
                }
            }
            if (down == false && rb.velocity.x != 0 && blocked)
            {
                rb.AddForce(Vector2.up, ForceMode2D.Impulse);
                blocked = false;
            }
        }
        else
            walking = false;
        if (walking && music.isPlaying == false)
            music.Play();
            
        if (walking == false)
        {
            music.Stop();
        }
        if (h!= 0&&grounded)
        {
            anim.SetFloat("Speed", 1);
           
        }
        else
            anim.SetFloat("Speed", 0);
       
    }
    void MaxSpeed(float maxspeed)
    {
        if (rb.velocity.x > maxspeed)
            rb.velocity = new Vector2(maxspeed,rb.velocity.y);
        if (rb.velocity.x < -maxspeed)
            rb.velocity = new Vector2(-maxspeed,rb.velocity.y);
    }
    void ClimbSlope(Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        rb.velocity += new Vector2(0, .75f / 3);
        velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
     
    }
    void Jumping(int strength)
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * strength;
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * 2 * Time.deltaTime * Gravityphy;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Time.deltaTime* Gravityphy;
        }
    }
    void Gravity(float power)
    {
        RaycastHit2D down = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),-Vector2.up, 1f, whatTohit);
        if(!grounded)
        rb.velocity -= new Vector2(0, power); 
        else
        rb.velocity -= new Vector2(0, power/3);
        if (rb.velocity.y < -9.81f)
            rb.velocity = new Vector2(rb.velocity.x ,Gravityphy);
        if (down)
            Gravityphy = -4;
        else
            Gravityphy = -9.81f;
    }
    protected override void Update()
    {              
        MaxSpeed(5);
        if(canmove == true)
        Movement(.5f);
    }
    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if(h==0)
            anim.GetComponent<Animator>().Play("player_idle");
        else
            canmove = true;
            if (grounded)
            anim.SetBool("Grounded",true);
        else
            anim.SetBool("Grounded", false);
        box.sharedMaterial.friction = 1;
        GroundCheck();
        Gravity(.2f);
        Jumping(7);
    }
}
