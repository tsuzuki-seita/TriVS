using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    // Orbのプレハブ（Inspectorでアタッチ可能にする）
    public GameObject orbPrefab;

    // Stageの範囲（Inspectorでアタッチ可能にする）
    public GameObject stage;

    // 配置するOrbの数
    public int numberOfOrbs = 10;

    // Stageの範囲を計算するための変数
    private Vector3 stageBounds;

    // スタート時にOrbをランダムに配置
    private void Start()
    {
        // Stageオブジェクトの大きさを取得
        stageBounds = stage.GetComponent<Renderer>().bounds.size;

        // 指定された数のOrbを生成
        SpawnOrbs();
    }

    // Orbをランダムに配置するメソッド
    private void SpawnOrbs()
    {
        for (int i = 0; i < numberOfOrbs; i++)
        {
            // ランダムなXとZ座標を生成（Stageの範囲内）
            float randomX = Random.Range(-stageBounds.x / 2, stageBounds.x / 2);
            float randomZ = Random.Range(-stageBounds.z / 2, stageBounds.z / 2);

            // OrbのY座標はStageの上に固定する（少し上に浮かせて配置する）
            float randomY = stage.transform.position.y + 1f;

            // ランダムな位置を生成
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // Orbをランダムな位置に生成
            Instantiate(orbPrefab, randomPosition, Quaternion.identity);
        }
    }
}