using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHP = 100;
    public int currentHP;
    public GameObject attackPrefab; // 攻撃のプレハブ
    public Transform attackSpawnPoint;
    public AttributeSwitcher attributeSwitcher;
    public float attackCooldown = 1f;

    private float lastAttackTime = 0;
    private bool isAttacking = false;
    private int attackCombo = 0;
    private Animator animator;

    private void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>(); // Animatorの取得
    }

    private void Update()
    {
        if (!isAttacking)
        {
            Move();
            UpdateAnimation();
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackCooldown)
        {
            PerformAttack();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            attributeSwitcher.SwitchAttribute();
        }
    }

    // 移動処理
    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 移動の適用
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    // アニメーションの更新（移動中はWalk、停止中はIdle）
    private void UpdateAnimation()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isWalking = Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0;

        if (isWalking)
        {
            animator.SetBool("isWalking", true); // Walkアニメーション再生
        }
        else
        {
            animator.SetBool("isWalking", false); // Idleアニメーション再生
        }
    }

    // 攻撃処理
    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        attackCombo++;

        if (attackCombo == 1)
        {
            // 1回目の攻撃
            animator.SetTrigger("Attack01");
        }
        else if (attackCombo == 2)
        {
            // 2回目の攻撃
            animator.SetTrigger("Attack02");
        }
        else if (attackCombo == 3)
        {
            // 3回目の攻撃
            animator.SetTrigger("Attack03");
        }

        isAttacking = true;
        attackCombo = Mathf.Clamp(attackCombo, 0, 3); // 攻撃は3回まで
    }

    // 攻撃のアニメーション中に呼ばれる
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        attackCombo = 0; // 攻撃コンボをリセット
    }

    // ダメージ処理
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            // ゲームオーバー処理
            Debug.Log("Player is dead!");
        }
    }
}
