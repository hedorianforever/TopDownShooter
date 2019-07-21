using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxHealth = 50f;
    [SerializeField] float dashSpeed = 30f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] float dashLength = .15f;
    
    [SerializeField] Ghost ghostPrefab = default;
    //[SerializeField] Weapon equippedWeapon = default;
    [SerializeField] Transform weaponSlot = default;

    [Header("UI related variables")]
    [SerializeField] Text healthText; 

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveAmount;

    private float currentHealth;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 currentDashTargetPos;

    //needed so mouse scroll wheel input is not read multiple times at once
    private bool isChangingWeapon = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Physics2D.queriesStartInColliders = false;

        currentHealth = maxHealth;

        ChangeHealthUI(currentHealth);
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        ChangeDirectionFaced();
        SetPlayerAnimation();

        Dash();

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isChangingWeapon)
        {
            isChangingWeapon = true;
            StartCoroutine(ChangeWeaponRoutine());
        }
    }

    private void Dash()
    {
        if (Input.GetMouseButtonDown(1) && canDash)
        {
            canDash = false;
            ghostPrefab.shouldMakeGhost = true;
            isDashing = true;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        currentDashTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float t = 0;

        while (t < dashLength)
        {
            t += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        ghostPrefab.shouldMakeGhost = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ChangeDirectionFaced()
    {
        //changes direction player is facing, but not when player is idle
        if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
            //transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
            //transform.localScale = new Vector3(1, 1, 1);

        }
    }

    private void SetPlayerAnimation()
    {
        if (horizontal != 0 || vertical != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void MovePlayer()
    {
        if (isDashing)
        {
            Vector3 nextMovePoint = Vector3.MoveTowards(transform.position, currentDashTargetPos, dashSpeed * Time.deltaTime);
            bool hasHitObstacle = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(nextMovePoint, .2f);
            if (colliders.Length == 0)
            {
                transform.position = nextMovePoint;
            }
            else
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Obstacle")
                    {
                        hasHitObstacle = true;
                        break;
                    }
                }

                if (!hasHitObstacle)
                {
                    transform.position = nextMovePoint;
                }
            }
        }
        else
        {
            if (horizontal != 0 && vertical != 0)
            {
                //limit movement when moving diagonally, so you don't move so fast
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }

            rb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        ChangeHealthUI(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ChangeWeapon(Weapon weapon)
    {
        //already using a weapon
        if (weaponSlot.childCount > 0)
        {
            Transform equippedWeapon = weaponSlot.GetChild(0);
            Destroy(equippedWeapon.gameObject);
        } 
 
        Weapon newWeapon = Instantiate(weapon, weaponSlot.position, Quaternion.identity);
        newWeapon.transform.parent = weaponSlot;
    }

    private void Die()
    {
        // TODO : death fx
        Destroy(gameObject);
    }

    private void ChangeHealthUI(float currentHealth)
    {
        healthText.text = Mathf.Clamp(currentHealth, 0, 1000).ToString() + "/" + maxHealth.ToString();
        //if (currentHealth <= maxHealth / 2)
        //{
        //    //change sprite to half a heart
        //    //TODO
        //}
    }

    IEnumerator ChangeWeaponRoutine()
    {
        WeaponManager.Instance.ChangeWeapon(Input.GetAxis("Mouse ScrollWheel"));
        yield return new WaitForSeconds(.5f);
        isChangingWeapon = false;
    }

}
