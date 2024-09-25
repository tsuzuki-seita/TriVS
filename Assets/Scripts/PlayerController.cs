using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public float attackRange = 2f;
    public Transform attackSpawnPoint;
    public AttributeSwitcher attributeSwitcher; // 属性スイッチャー
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;

    public int maxGauge = 50;
    int currentGauge;
    public int gaugeCost = 10; // 属性切り替えと攻撃に消費するゲージ量

    public Animator animator;
    private bool isAttacking = false;
    private bool isDefence = false;
    public bool isStun = false;
    private int comboStep = 0; // 0: Attack01, 1: Attack02, 2: Attack03
    // 攻撃ごとに異なるプレハブ
    public GameObject attackPrefab01; // Attack01 用のプレハブ
    public GameObject attackPrefab02; // Attack02 用のプレハブ

    public GameObject Defence;
    float _input_x;
    float _input_z;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Defence.SetActive(false);
    }

    private void Update()
    {
        if (!isAttacking && !isDefence && !isStun) // 攻撃中は移動できない
        {
            Move();
        }
        else
        {
            _input_x = 0;
            _input_z = 0;
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackCooldown && attributeSwitcher.currentGauge >= gaugeCost)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Space) && attributeSwitcher.currentGauge >= gaugeCost)
        {
            attributeSwitcher.SwitchAttribute();
            attributeSwitcher.currentGauge -= gaugeCost; // 属性切り替え時にゲージを消費
            attributeSwitcher.UpdateGaugeUI();
        }

        if (Input.GetMouseButtonDown(1))
        {
            isDefence = true;
            animator.SetBool("IsDefence", true);
            Defence.SetActive(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDefence = false;
            animator.SetBool("IsDefence", false);
            Defence.SetActive(false);
        }

    }

    private void Move()
    {
        //x軸方向、z軸方向の入力を取得
        //Horizontal、水平、横方向のイメージ
        _input_x = Input.GetAxis("Horizontal");
        //Vertical、垂直、縦方向のイメージ
        _input_z = Input.GetAxis("Vertical");

        //移動の向きなど座標関連はVector3で扱う
        Vector3 velocity = new Vector3(_input_x, 0, _input_z);
        //ベクトルの向きを取得
        Vector3 direction = velocity.normalized;

        //移動距離を計算
        float distance = moveSpeed * Time.deltaTime;
        //移動先を計算
        Vector3 destination = transform.position + direction * distance;

        //移動先に向けて回転
        transform.LookAt(destination);
        //移動先の座標を設定
        transform.position = destination;

        // 移動アニメーションの制御
        if (velocity != Vector3.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void Attack()
    {
        _input_x = 0;
        _input_z = 0;
        lastAttackTime = Time.time;
        isAttacking = true;
        attributeSwitcher.currentGauge -= gaugeCost; // 攻撃時にゲージを消費
        attributeSwitcher.UpdateGaugeUI();

        if (comboStep == 0)
        {
            animator.SetTrigger("Attack01");
            SpawnProjectile01();
        }
        else if (comboStep == 1)
        {
            animator.SetTrigger("Attack02");
            SpawnProjectile02();
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("Attack03");
            SpawnProjectile03();
        }

        comboStep = (comboStep + 1) % 3; // 次のコンボステップへ
        Invoke("RemoveAttacking", 0.5f);
    }

    void RemoveAttacking()
    {
        isAttacking = false;
    }

    private void SpawnProjectile01()
    {
        GameObject projectile = Instantiate(attackPrefab01, attackSpawnPoint.position, Quaternion.identity);
        SetProjectileAttribute(projectile, 10);
        
    }

    private void SpawnProjectile02()
    {
        GameObject projectile = Instantiate(attackPrefab02, attackSpawnPoint.position, Quaternion.identity);
        SetProjectileAttribute(projectile, 20);
        
    }

    private void SpawnProjectile03()
    {
        GameObject projectile = Instantiate(attackPrefab01, attackSpawnPoint.position, Quaternion.identity);
        SetProjectileAttribute(projectile, 30);
        
    }

    private void SetProjectileAttribute(GameObject projectile ,int damage)
    {
        // Projectile に属性情報をセット
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.sourceAttribute = attributeSwitcher.currentAttribute; // 現在の属性をセット
            projectileScript.damage = damage; // ダメージ値（ここはお好みで調整）
        }
    }
}
