using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float attackRange = 2f;
    public int maxHP = 100;
    public int currentHP;
    public GameObject attackPrefab; // 攻撃のプレハブ
    public Transform attackSpawnPoint;
    public AttributeSwitcher attributeSwitcher; // 属性スイッチャー
    public float attackCooldown = 1f;
    private float lastAttackTime = 0;

    public int maxGauge = 50;
    public int currentGauge = 50;
    public int gaugeCost = 10; // 属性切り替えと攻撃に消費するゲージ量
    public float gaugeRegenerationRate = 3f; // 秒間のゲージ増加量

    private Animator animator;
    private bool isAttacking = false;
    private int comboStep = 0; // 0: Attack01, 1: Attack02, 2: Attack03

    private void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        StartCoroutine(RegenerateGauge());
    }

    private void Update()
    {
        if (!isAttacking && currentGauge >= gaugeCost) // 攻撃中は移動できない
        {
            Move();
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackCooldown && currentGauge >= gaugeCost)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentGauge >= gaugeCost)
        {
            attributeSwitcher.SwitchAttribute();
            currentGauge -= gaugeCost; // 属性切り替え時にゲージを消費
            UpdateGaugeUI();
        }
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        //進む方向に滑らかに向く。
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        // 移動アニメーションの制御
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;
        currentGauge -= gaugeCost; // 攻撃時にゲージを消費
        UpdateGaugeUI();

        if (comboStep == 0)
        {
            animator.SetTrigger("Attack01");
        }
        else if (comboStep == 1)
        {
            animator.SetTrigger("Attack02");
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("Attack03");
        }

        comboStep = (comboStep + 1) % 3; // 次のコンボステップへ
        Invoke(nameof(SpawnProjectile), 0.5f); // プロジェクタイルを生成
    }

    private void SpawnProjectile()
    {
        GameObject projectile = Instantiate(attackPrefab, attackSpawnPoint.position, Quaternion.identity);

        // Projectile に属性情報をセット
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.sourceAttribute = attributeSwitcher.currentAttribute; // 現在の属性をセット
            projectileScript.damage = 10; // ダメージ値（ここはお好みで調整）
        }

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            // ゲームオーバー処理
            Debug.Log("Player is dead!");
        }
    }

    public void CollectOrb()
    {
        currentGauge = Mathf.Min(currentGauge + 20, maxGauge); // オーブを取るとゲージが20増加
        UpdateGaugeUI();
    }

    private IEnumerator RegenerateGauge()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1秒ごとにゲージを増加
            currentGauge = Mathf.Min(currentGauge + (int)gaugeRegenerationRate, maxGauge);
            UpdateGaugeUI();
        }
    }

    private void UpdateGaugeUI()
    {
        // ゲージのUI更新処理（ここはあなたのプロジェクトに応じて実装）
        Debug.Log("Gauge: " + currentGauge);
    }
}
