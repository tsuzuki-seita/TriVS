using UnityEngine;

public class AttributeSwitcher : MonoBehaviour
{
    public enum Attribute { Fire, Water, Grass }
    public Attribute currentAttribute;
    public int switchCost = 10; // 切り替えに必要なゲージ量
    public int currentGauge = 0;
    public int maxGauge = 100;

    public GameObject fireObject;  // Fire属性のオブジェクト
    public GameObject waterObject; // Water属性のオブジェクト
    public GameObject grassObject; // Grass属性のオブジェクト

    private void Start()
    {
        currentAttribute = Attribute.Fire; // 初期属性
        UpdateAttributeVisibility();
    }

    // 属性を切り替える
    public void SwitchAttribute()
    {
        if (currentGauge >= switchCost)
        {
            currentGauge -= switchCost;
            switch (currentAttribute)
            {
                case Attribute.Fire:
                    currentAttribute = Attribute.Water;
                    break;
                case Attribute.Water:
                    currentAttribute = Attribute.Grass;
                    break;
                case Attribute.Grass:
                    currentAttribute = Attribute.Fire;
                    break;
            }
            Debug.Log("Attribute switched to " + currentAttribute.ToString());
            UpdateAttributeVisibility(); // 属性切り替え時に表示を更新
        }
        else
        {
            Debug.Log("Not enough gauge to switch attribute.");
        }
    }

    // ゲージを追加する
    public void AddGauge(int amount)
    {
        currentGauge = Mathf.Min(currentGauge + amount, maxGauge);
        Debug.Log("Gauge: " + currentGauge);
    }

    // 各属性に応じてオブジェクトの表示を更新
    private void UpdateAttributeVisibility()
    {
        // Fire, Water, Grassのオブジェクトを切り替え
        fireObject.SetActive(currentAttribute == Attribute.Fire);
        waterObject.SetActive(currentAttribute == Attribute.Water);
        grassObject.SetActive(currentAttribute == Attribute.Grass);
    }
}
