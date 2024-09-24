using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceDetection : MonoBehaviour
{
    public AttributeSwitcher attributeSwitcher;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EProjectile")) // 衝突したオブジェクトが攻撃（Projectile）の場合
        {
            // 攻撃元の属性情報を取得
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                Destroy(other.gameObject); // 衝突したプロジェクタイルを破壊
            }

            switch (attributeSwitcher.currentAttribute)
            {
                case AttributeSwitcher.Attribute.Fire:
                    if (projectile.sourceAttribute == AttributeSwitcher.Attribute.Water)
                    {
                        playerController.animator.SetBool("IsStun", true);
                        playerController.isStun = true;
                        Invoke("RemoveStun", 1);
                    }
                    break;

                case AttributeSwitcher.Attribute.Water:
                    if (projectile.sourceAttribute == AttributeSwitcher.Attribute.Lightning)
                    {
                        playerController.animator.SetBool("IsStun", true);
                        playerController.isStun = true;
                        Invoke("RemoveStun", 1);
                    }
                    break;

                case AttributeSwitcher.Attribute.Lightning:
                    if (projectile.sourceAttribute == AttributeSwitcher.Attribute.Fire)
                    {
                        playerController.animator.SetBool("IsStun", true);
                        playerController.isStun = true;
                        Invoke("RemoveStun", 1);
                    }
                    break;
            }
        }
    }

    void RemoveStun()
    {
        playerController.animator.SetBool("IsStun", false);
        playerController.isStun = false;
    }
}
