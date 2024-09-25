using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ECollisionDetection: MonoBehaviour
{
    public int orbValue = 20; // オーブ1つ当たりのゲージ増加量
    public AttributeSwitcher attributeSwitcher;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            attributeSwitcher.currentGauge = Mathf.Min(attributeSwitcher.currentGauge + 20, attributeSwitcher.maxGauge); // オーブを取るとゲージが20増加

            // オーブを消去
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Projectile")) // 衝突したオブジェクトが攻撃（Projectile）の場合
        {
            playerController.animator.SetTrigger("Damage");
            // 攻撃元の属性情報を取得
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                // ダメージ処理を呼び出す
                attributeSwitcher.TakeDamage(projectile.damage, projectile.sourceAttribute);
                Destroy(other.gameObject); // 衝突したプロジェクタイルを破壊
            }
        }

    }
}