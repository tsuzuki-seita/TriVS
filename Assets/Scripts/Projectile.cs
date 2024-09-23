using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage; // このプロジェクタイルが与えるダメージ
    public AttributeSwitcher.Attribute sourceAttribute; // 攻撃を発射したキャラクターの属性
}
