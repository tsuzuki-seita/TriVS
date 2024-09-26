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

    // Orbにつけるタグ名
    public string orbTag = "Orb"; // Orbに設定するタグを指定

    // Stageの範囲を計算するための変数
    private Bounds stageBounds;

    // スタート時にOrbをランダムに配置
    private void Start()
    {
        // Stageオブジェクトの大きさと位置を取得
        stageBounds = stage.GetComponent<Renderer>().bounds;

        // 指定された数のOrbを生成
        SpawnOrbs();
    }

    // Orbをランダムに配置するメソッド
    public void SpawnOrbs()
    {
        // 既存のOrbをすべて削除
        DeleteExistingOrbs();

        for (int i = 0; i < numberOfOrbs; i++)
        {
            // ランダムなXとZ座標を生成（Stageの中心を基準に、その範囲内でランダムに配置）
            float randomX = Random.Range(stageBounds.min.x -1 , stageBounds.max.x + 1);
            float randomZ = Random.Range(stageBounds.min.z - 1, stageBounds.max.z + 1);

            // OrbのY座標はStageの上に固定する（少し上に浮かせて配置する）
            float randomY = stageBounds.center.y + 1f;

            // ランダムな位置を生成
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // Orbをランダムな位置に生成
            GameObject orb = Instantiate(orbPrefab, randomPosition, Quaternion.identity);
        }
    }

    // 既存のOrbをすべて削除するメソッド
    private void DeleteExistingOrbs()
    {
        // タグがorbTagであるすべてのオブジェクトを取得
        GameObject[] existingOrbs = GameObject.FindGameObjectsWithTag(orbTag);

        // すべてのオブジェクトを削除
        foreach (GameObject orb in existingOrbs)
        {
            Destroy(orb);
        }
    }
}