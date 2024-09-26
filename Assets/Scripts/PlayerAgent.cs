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
    public GameObject Orbs;
    public GameObject Stage;

    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;

    public int maxHP = 100; // エージェントの最大HP
    public int gaugeCost = 10;

    public Animator animator;
    private bool isAttacking = false;
    private bool isDefence = false;
    public bool isStun = false;
    private int comboStep = 0;

    public GameObject attackPrefab01;
    public GameObject attackPrefab02;
    public GameObject Defence;
    public GameObject PlayerSpawn;
    public GameObject EnemySpawn;

    float _input_x;
    float _input_z;

    // 他のエージェントへの参照
    //public PlayerAgent opponentAgent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Defence.SetActive(false);
        ResetAgent();
    }

    // エージェントの初期化時に呼ばれる関数
    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        Defence.SetActive(false);
        base.Initialize();
    }

    // エージェントの観察を記録する関数
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position); // 自分の位置を観察
        sensor.AddObservation(attributeSwitcher.currentGauge); // ゲージの状態を観察
        sensor.AddObservation(attributeSwitcher.currentHP); // 現在のHPを観察
        sensor.AddObservation(isAttacking); // 攻撃状態かどうかを観察
        sensor.AddObservation(isDefence); // 防御状態かどうかを観察
        //sensor.AddObservation(opponentAgent.transform.position); // 自分の位置を観察
        //sensor.AddObservation(opponentAgent.attributeSwitcher.currentGauge); // ゲージの状態を観察
        //sensor.AddObservation(opponentAgent.attributeSwitcher.currentHP); // 現在のHPを観察
        //sensor.AddObservation(opponentAgent.isAttacking); // 攻撃状態かどうかを観察
        //sensor.AddObservation(opponentAgent.isDefence); // 防御状態かどうかを観察
    }

    // エージェントの行動を実行する関数
    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        var discreteActions = actions.DiscreteActions;

        // 1. 移動 (Horizontal, Verticalの軸による入力)
        _input_x = continuousActions[0];
        _input_z = continuousActions[1];

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

        // 2. 属性切り替え (スペースキーに対応) 
        if (discreteActions[0] == 1 && attributeSwitcher.currentGauge >= gaugeCost && Time.time - lastAttackTime > attackCooldown)
        {
            attributeSwitcher.SwitchAttribute();
            attributeSwitcher.UpdateGaugeUI();
            lastAttackTime = Time.time;
        }

        // 3. 攻撃 (マウス左クリックに対応) 
        if (discreteActions[1] == 1 && attributeSwitcher.currentGauge >= gaugeCost && Time.time - lastAttackTime > attackCooldown)
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

    }

    // エージェントがヒューマンプレイヤーのように操作する場合の定義
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // 横方向の移動入力
        continuousActions[1] = Input.GetAxis("Vertical");   // 縦方向の移動入力

        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0; // スペースキーの入力 (属性切替)
        discreteActions[1] = Input.GetMouseButton(0) ? 1 : 0;     // マウス左クリック (攻撃)
        discreteActions[2] = Input.GetMouseButton(1) ? 1 : 0;         // マウス右クリック (防御)
    }

    private void Attack()
    {
        _input_x = 0;
        _input_z = 0;
        isAttacking = true;
        lastAttackTime = Time.time;
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
    public void CheckAgentState()
    {
        // 勝利した相手のエージェントに+1の報酬
        //if (opponentAgent != null)
        //{
        //    opponentAgent.SetReward(1.0f);
        //    opponentAgent.ResetAgent();
        //    opponentAgent.EndEpisode();
        //}
        //if(transform.position.y < 0)
        //{
        //    ResetAgent();
        //    opponentAgent.ResetAgent();
        //}

        Debug.Log("報酬獲得");
        ResetAgent();
        //opponentAgent.ResetAgent();
        EndEpisode(); // 自エージェントのエピソードも終了
    }

    // エージェントのリセット処理
    //public override void OnEpisodeBegin()
    //{
    //    ResetAgent();
    //    Debug.Log("リセット");
    //}

    // エージェントの状態を初期化する関数
    public void ResetAgent()
    {
        attributeSwitcher.currentHP = maxHP; // HPを初期化
        attributeSwitcher.currentGauge = 0; // ゲージを初期化
        attributeSwitcher.UpdateHpUI();
        attributeSwitcher.UpdateGaugeUI();
        attributeSwitcher.currentAttribute = AttributeSwitcher.Attribute.Fire;
        isAttacking = false;
        isDefence = false;
        comboStep = 0;
        Defence.SetActive(false);
        animator.SetBool("IsDefence", false);
        animator.SetBool("IsWalking", false);
        if (PlayerSpawn != null)
        {
            transform.position = PlayerSpawn.transform.position;
            transform.rotation = PlayerSpawn.transform.rotation;
        }
        if (EnemySpawn != null)
        {
            transform.position = EnemySpawn.transform.position;
            transform.rotation = EnemySpawn.transform.rotation;
        }
        // タグがorbTagであるすべてのオブジェクトを取得
        GameObject[] existingOrbs = GameObject.FindGameObjectsWithTag("Orbs");
        // すべてのオブジェクトを削除
        foreach (GameObject orb in existingOrbs)
        {
            Destroy(orb);
        }
        if (Orbs != null)
        {
            Instantiate(Orbs, Stage.transform);
        }
    }
}