using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    public float moveSpeed;
    public float rotateSpeed;
    public float attackRange = 2f;
    public Transform attackSpawnPoint;
    public AttributeSwitcher attributeSwitcher;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;

    public int maxGauge = 50;
    public int maxHP = 100; // エージェントの最大HP
    private int currentHP; // 現在のHP
    public int gaugeCost = 10;

    public Animator animator;
    private bool isAttacking = false;
    private bool isDefence = false;
    public bool isStun = false;
    private int comboStep = 0;

    public GameObject attackPrefab01;
    public GameObject attackPrefab02;
    public GameObject Defence;
    float _input_x;
    float _input_z;

    // 他のエージェントへの参照
    public PlayerAgent opponentAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Defence.SetActive(false);
        ResetAgent();
    }

    // エージェントの初期化時に呼ばれる関数
    public override void Initialize()
    {
        base.Initialize();
    }

    // エージェントの観察を記録する関数
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position); // 自分の位置を観察
        sensor.AddObservation(attributeSwitcher.currentGauge); // ゲージの状態を観察
        sensor.AddObservation(currentHP); // 現在のHPを観察
        sensor.AddObservation(isAttacking); // 攻撃状態かどうかを観察
        sensor.AddObservation(isDefence); // 防御状態かどうかを観察
    }

    // エージェントの行動を実行する関数
    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        var discreteActions = actions.DiscreteActions;

        // 1. 移動 (Horizontal, Verticalの軸による入力)
        _input_x = continuousActions[0];
        _input_z = continuousActions[1];

        Vector3 moveDirection = new Vector3(_input_x, 0, _input_z).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 2. 属性切り替え (スペースキーに対応)
        if (discreteActions[0] == 1 && attributeSwitcher.currentGauge >= gaugeCost)
        {
            attributeSwitcher.SwitchAttribute();
            attributeSwitcher.currentGauge -= gaugeCost;
            attributeSwitcher.UpdateGaugeUI();
        }

        // 3. 攻撃 (マウス左クリックに対応)
        if (discreteActions[1] == 1 && Time.time - lastAttackTime > attackCooldown && attributeSwitcher.currentGauge >= gaugeCost)
        {
            Attack();
        }

        // 4. 防御 (マウス右クリックに対応)
        if (discreteActions[2] == 1)
        {
            isDefence = true;
            animator.SetBool("IsDefence", true);
            Defence.SetActive(true);
        }
        else if (discreteActions[2] == 0)
        {
            isDefence = false;
            animator.SetBool("IsDefence", false);
            Defence.SetActive(false);
        }

        // エージェントがフィールド外に出た場合やHPが0になった場合にリセットする
        CheckAgentState();
    }

    // エージェントがヒューマンプレイヤーのように操作する場合の定義
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // 横方向の移動入力
        continuousActions[1] = Input.GetAxis("Vertical");   // 縦方向の移動入力

        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0; // スペースキーの入力 (属性切替)
        discreteActions[1] = Input.GetMouseButtonDown(0) ? 1 : 0;     // マウス左クリック (攻撃)
        discreteActions[2] = Input.GetMouseButton(1) ? 1 : 0;         // マウス右クリック (防御)
    }

    private void Attack()
    {
        _input_x = 0;
        _input_z = 0;
        lastAttackTime = Time.time;
        isAttacking = true;
        attributeSwitcher.currentGauge -= gaugeCost;
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

        comboStep = (comboStep + 1) % 3;
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

    private void SetProjectileAttribute(GameObject projectile, int damage)
    {
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.sourceAttribute = attributeSwitcher.currentAttribute;
            projectileScript.damage = damage;
        }
    }

    // エージェントの状態をチェックしてリセット条件を確認する
    private void CheckAgentState()
    {
        // HPが0以下になった場合
        if (currentHP <= 0)
        {
            // HPが0になったエージェントに-1の報酬
            SetReward(-1.0f);

            // 勝利した相手のエージェントに+1の報酬
            if (opponentAgent != null)
            {
                opponentAgent.SetReward(1.0f);
                opponentAgent.EndEpisode();
            }

            EndEpisode(); // 自エージェントのエピソードも終了
        }

        // y軸が0未満になった場合（フィールド外に出た場合）
        if (transform.position.y < 0)
        {
            SetReward(-1.0f);

            // 相手エージェントに勝利報酬
            if (opponentAgent != null)
            {
                opponentAgent.SetReward(1.0f);
                opponentAgent.EndEpisode();
            }

            EndEpisode(); // 自エージェントのエピソードも終了
        }
    }

    // エージェントのHPにダメージを与える関数
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            CheckAgentState();
        }
    }

    // エージェントのリセット処理
    public override void OnEpisodeBegin()
    {
        ResetAgent();
    }

    // エージェントの状態を初期化する関数
    private void ResetAgent()
    {
        currentHP = maxHP; // HPを初期化
        attributeSwitcher.currentGauge = maxGauge; // ゲージを初期化
        transform.position = new Vector3(0, 1, 0); // 初期位置にリセット
        isAttacking = false;
        isDefence = false;
        comboStep = 0;
        Defence.SetActive(false);
        animator.SetBool("IsDefence", false);
    }
}