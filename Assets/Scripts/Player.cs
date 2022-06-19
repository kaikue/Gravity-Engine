using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum AnimState
    {
        Stand,
        Walk,
        Jump,
        Fall,
        Push
    }

    private const float runAcceleration = 30;
    public const float maxRunSpeed = 7;
    private const float jumpForce = 8;
    private const float gravityForce = 15;
    private const float maxFallSpeed = 20;

    public Sprite standSprite;
    public Sprite[] walkSprites;
    public Sprite jumpSprite;
    public Sprite fallSprite;
    public Sprite[] pushSprites;
    public SpriteRenderer lightSR;
    public Color lightOn;
    public Color lightOff;

    private Rigidbody2D rb;
    private EdgeCollider2D ec;
    private GameManager gm;

    private const float runFrameTime = 0.1f;
    private SpriteRenderer sr;
    private AnimState animState = AnimState.Stand;
    private int animFrame = 0;
    private float frameTime; //max time of frame
    private float frameTimer; //goes from frameTime down to 0
    public bool facingLeft = false; //for animation (images face right)

    private bool jumpQueued = false;
    private bool canJump = false;
    private bool canGravChange = false;
    private GameManager.Direction? gravChangeQueued = null;
    private bool wasOnGround = false;
    private Coroutine crtCancelQueuedJump;
    private const float jumpBufferTime = 0.1f; //time before hitting ground a jump will still be queued
    private const float jumpGraceTime = 0.1f; //time after leaving ground player can still jump (coyote time)
    private Vector2 gravDirection = Vector2.down;

    private const float pitchVariation = 0.15f;
    private AudioSource audioSrc;
    public AudioClip jumpSound;
    public AudioClip landSound;

    private bool finishedLevel = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        ec = gameObject.GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        standSprite = sr.sprite;
        audioSrc = GetComponent<AudioSource>();
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            TryStopCoroutine(crtCancelQueuedJump);
            jumpQueued = true;
            crtCancelQueuedJump = StartCoroutine(CancelQueuedJump());
        }

        if (Input.GetButtonDown("GravChangeUp"))
		{
            gravChangeQueued = GameManager.Direction.Up;
		}
        if (Input.GetButtonDown("GravChangeDown"))
        {
            gravChangeQueued = GameManager.Direction.Down;
        }
        if (Input.GetButtonDown("GravChangeLeft"))
        {
            gravChangeQueued = GameManager.Direction.Left;
        }
        if (Input.GetButtonDown("GravChangeRight"))
        {
            gravChangeQueued = GameManager.Direction.Right;
        }

        //TODO if W/A/S/D/right stick direction pressed and it wasn't before:
        /*if (Input.GetButtonDown("Jump"))
        {
            TryStopCoroutine(crtCancelQueuedGravChange);
            gravChangeQueued = true;
            crtCancelQueuedGravChange = StartCoroutine(CancelQueuedGravChange());
        }*/

        /*if (Input.GetKeyDown(KeyCode.N))
		{
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        }*/

        sr.flipX = facingLeft;
        lightSR.flipX = facingLeft;
        AdvanceAnim();
        sr.sprite = GetAnimSprite();
        lightSR.color = canGravChange ? lightOn : lightOff;
    }

    private Collider2D RaycastTiles(Vector2 startPoint, Vector2 endPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPoint, endPoint - startPoint, Vector2.Distance(startPoint, endPoint), LayerMask.GetMask("Tiles", "Crates"));
        return hit.collider;
    }

    private bool CheckSide(int point0, int point1, Vector2 direction)
    {
        Vector2 p0 = transform.TransformPoint(ec.points[point0]);
        Vector2 p1 = transform.TransformPoint(ec.points[point1]);
        Vector2 startPoint = p0 + direction * 0.02f;
        Vector2 endPoint = p1 + direction * 0.02f;
        Collider2D collider = RaycastTiles(startPoint, endPoint);
        return collider != null;
    }

    private RaycastHit2D[] CheckHeadCrates()
	{
        Vector2 p0 = transform.TransformPoint(ec.points[1]);
        Vector2 p1 = transform.TransformPoint(ec.points[2]);
        Vector2 startPoint = p0 - gravDirection * 0.02f;
        Vector2 endPoint = p1 - gravDirection * 0.02f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, endPoint - startPoint, Vector2.Distance(startPoint, endPoint), LayerMask.GetMask("Crates"));
        return hits;
    }

    private void FixedUpdate()
    {
        if (gravChangeQueued != null)
		{
            if (canGravChange)
            {
                Vector2 gravVec = GameManager.GetGravityVector(gravChangeQueued.Value);
                if (gravVec != gravDirection)
                {
                    rb.velocity = Vector2.zero;
                    gravDirection = gravVec;
                    gm.ChangeGravity(gravChangeQueued.Value);
                    switch (gravChangeQueued) //TODO make this one line?
                    {
                        case GameManager.Direction.Up:
                            transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
                            break;
                        case GameManager.Direction.Down:
                            transform.rotation = Quaternion.identity;
                            break;
                        case GameManager.Direction.Left:
                            transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
                            break;
                        case GameManager.Direction.Right:
                            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                            break;
                    }
                }
            }
            gravChangeQueued = null;
		}

        float xVel = 0;
        float yVel = 0;
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        bool onGround;
        bool onCeiling;

        if (gravDirection == Vector2.up || gravDirection == Vector2.down)
		{
            float prevXVel = rb.velocity.x;
            float dx = runAcceleration * Time.fixedDeltaTime * xInput;
            if (prevXVel != 0 && Mathf.Sign(xInput) != Mathf.Sign(prevXVel))
            {
                xVel = 0;
            }
            else
            {
                xVel = prevXVel + dx;
                float speedCap = Mathf.Abs(xInput * maxRunSpeed);
                xVel = Mathf.Clamp(xVel, -speedCap, speedCap);
            }

            if (gravDirection == Vector2.down)
			{
                if (xInput != 0)
                {
                    facingLeft = xInput < 0;
                }
            }
            else
			{
                if (xInput != 0)
                {
                    facingLeft = xInput > 0;
                }
            }
        }
        else
		{
            float prevYVel = rb.velocity.y;
            float dy = runAcceleration * Time.fixedDeltaTime * yInput;
            if (prevYVel != 0 && Mathf.Sign(yInput) != Mathf.Sign(prevYVel))
            {
                yVel = 0;
            }
            else
            {
                yVel = prevYVel + dy;
                float speedCap = Mathf.Abs(yInput * maxRunSpeed);
                yVel = Mathf.Clamp(yVel, -speedCap, speedCap);
            }

            if (gravDirection == Vector2.left)
            {
                if (yInput != 0)
                {
                    facingLeft = yInput > 0;
                }
            }
            else
            {
                if (yInput != 0)
                {
                    facingLeft = yInput < 0;
                }
            }
        }

        onGround = CheckSide(4, 3, gravDirection);
        onCeiling = CheckSide(1, 2, -gravDirection);
        if (onGround)
        {
            canJump = true;
            canGravChange = true;

            if (gravDirection == Vector2.up || gravDirection == Vector2.down)
            {
                yVel = 0;
                animState = xVel == 0 ? AnimState.Stand : AnimState.Walk;
            }
            else
			{
                xVel = 0;
                animState = yVel == 0 ? AnimState.Stand : AnimState.Walk;
            }
        }
        else
        {
            if (gravDirection == Vector2.up || gravDirection == Vector2.down)
            {
                yVel = Mathf.Clamp(rb.velocity.y + gravityForce * gravDirection.y * Time.fixedDeltaTime, -maxFallSpeed, maxFallSpeed);
                if (Mathf.Sign(yVel) == Mathf.Sign(gravDirection.y))
                {
                    animState = AnimState.Fall;
                }
            }
            else
			{
                xVel = Mathf.Clamp(rb.velocity.x + gravityForce * gravDirection.x * Time.fixedDeltaTime, -maxFallSpeed, maxFallSpeed);
                if (Mathf.Sign(xVel) == Mathf.Sign(gravDirection.x))
                {
                    animState = AnimState.Fall;
                }
            }

            if (wasOnGround)
            {
                StartCoroutine(LeaveGround());
            }
        }
        wasOnGround = onGround;

        if (onCeiling)
        {
            if (gravDirection == Vector2.up)
			{
                yVel = Mathf.Max(yVel, 0);
			}
            else if (gravDirection == Vector2.down)
            {
                yVel = Mathf.Min(yVel, 0);
            }
            if (gravDirection == Vector2.left)
            {
                xVel = Mathf.Min(xVel, 0);
            }
            else if (gravDirection == Vector2.right)
            {
                xVel = Mathf.Max(xVel, 0);
            }
            PlaySound(landSound);
        }

        Vector2 vel = new Vector2(xVel, yVel);

        RaycastHit2D[] headCrates = CheckHeadCrates();
        if (headCrates.Length > 0)
        {
            canJump = false;
            foreach (RaycastHit2D headCrate in headCrates)
            {
                Rigidbody2D crateRB = headCrate.collider.attachedRigidbody;
                crateRB.velocity = vel;
                crateRB.MovePosition(crateRB.position + vel * Time.fixedDeltaTime);
            }
        }

        if (jumpQueued)
        {
            if (canJump)
            {
                StopCancelQueuedJump();
                jumpQueued = false;
                canJump = false;
                canGravChange = false;
                vel += jumpForce * -gravDirection;
                PlaySound(jumpSound);
                animState = AnimState.Jump;
            }
        }

        rb.velocity = vel;
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf || finishedLevel) return;

        GameObject collider = collision.collider.gameObject;

        if (collider.layer == LayerMask.NameToLayer("Tiles"))
        {
            if (collision.GetContact(0).normal.x != 0)
            {
                //against wall, not ceiling
                PlaySound(landSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        GameObject collider = collision.gameObject;
        if (collider.CompareTag("NextLevel"))
		{
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        GameObject collider = collision.gameObject;
    }

    private Sprite GetAnimSprite()
    {
        switch (animState)
        {
            case AnimState.Stand:
                return standSprite;
            case AnimState.Walk:
                return walkSprites[animFrame];
            case AnimState.Jump:
                return jumpSprite;
            case AnimState.Fall:
                return fallSprite;
            case AnimState.Push:
                return pushSprites[animFrame];
        }
        return standSprite;
    }

    private void TryStopCoroutine(Coroutine crt)
    {
        if (crt != null)
        {
            StopCoroutine(crt);
        }
    }

    private void StopCancelQueuedJump()
    {
        TryStopCoroutine(crtCancelQueuedJump);
    }

    private IEnumerator CancelQueuedJump()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpQueued = false;
    }

    private IEnumerator LeaveGround()
    {
        yield return new WaitForSeconds(jumpGraceTime);
        canJump = false;
        canGravChange = false;
    }

    private void AdvanceAnim()
    {
        if (animState == AnimState.Walk || animState == AnimState.Push)
        {
            frameTime = runFrameTime;
            AdvanceFrame(walkSprites.Length);
        }
        else
        {
            animFrame = 0;
            frameTimer = frameTime;
        }
    }

    private void AdvanceFrame(int numFrames)
    {
        if (animFrame >= numFrames)
        {
            animFrame = 0;
        }

        frameTimer -= Time.deltaTime;
        if (frameTimer <= 0)
        {
            frameTimer = frameTime;
            animFrame = (animFrame + 1) % numFrames;
        }
    }

    public void PlaySound(AudioClip sound, bool randomizePitch = false)
    {
        if (randomizePitch)
        {
            audioSrc.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        }
        else
        {
            audioSrc.pitch = 1;
        }
        audioSrc.PlayOneShot(sound);
    }
}
