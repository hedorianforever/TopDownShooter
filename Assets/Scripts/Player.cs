using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxHealth = 50f;
    [SerializeField] float dashSpeed = 30f;
    [SerializeField] float dashCooldown = 5f;
    [SerializeField] float dashLength = .15f;

    [SerializeField] Ghost ghostPrefab = default;
    [SerializeField] Transform weaponSlot = default;

    [SerializeField] AudioClip hurtSound = default;

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

    private bool isInvulnerable = false;

    //needed so mouse scroll wheel input is not read multiple times at once
    private bool isChangingWeapon = false;

    [HideInInspector] public bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //needed so it won't collide with itself on physics2d.overlapcircle calls
        Physics2D.queriesStartInColliders = false;

        //being handled by the game manager
        //currentHealth = maxHealth;

        //if (uiManager != null)
        //{
        //    uiManager.UpdatePlayerHealthUI(currentHealth, maxHealth);
        //}
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        ChangeDirectionFaced();
        SetPlayerAnimation();

        Debug.Log("CAN DASH? " + canDash);
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
            isInvulnerable = true;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        //currentDashTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentDashTargetPos = transform.position + new Vector3(horizontal, vertical, 0) * 10;

        float t = 0;

        while (t < dashLength)
        {
            t += Time.deltaTime;
            if (isDead)
            {
                yield break;
            }
            yield return null;
        }

        isDashing = false;
        ghostPrefab.shouldMakeGhost = false;
        isInvulnerable = false;

        yield return StartCoroutine(UIManager.Instance.DashCooldownRoutine(dashCooldown));


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

        UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);
        StartCoroutine(UIManager.Instance.PlayerHurtRoutine());
        StartCoroutine(TakeDamageRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            AudioManager.Instance.PlayClip(hurtSound);
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
        animator.SetTrigger("dieTrigger");
        isDead = true;
        Destroy(weaponSlot.gameObject);
        Destroy(rb, .1f);
        Destroy(gameObject, 1f);
        GameObject.FindGameObjectWithTag("SceneTransitions").GetComponent<SceneTransitions>().LoadLoseScene();
        Destroy(this);
    }

    IEnumerator ChangeWeaponRoutine()
    {
        WeaponManager.Instance.ChangeWeapon(Input.GetAxis("Mouse ScrollWheel"));
        yield return new WaitForSeconds(.3f);
        isChangingWeapon = false;
    }

    public void Heal(int healAmount)
    {
        Debug.Log("HEALING!");
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, healAmount, maxHealth);

        UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);

    }

    IEnumerator TakeDamageRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1f);
        isInvulnerable = false;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;

        UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);
    }

    public bool IsFullHealth()
    {
        return currentHealth == maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool GetIsInvulnerable()
    {
        return isInvulnerable;
    }

    private void LateUpdate()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);

    }

}
