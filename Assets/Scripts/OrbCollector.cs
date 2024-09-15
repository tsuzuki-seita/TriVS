using UnityEngine;

public class OrbCollector : MonoBehaviour
{
    public int orbValue = 20; // オーブ1つ当たりのゲージ増加量
    public AttributeSwitcher attributeSwitcher;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            // ゲージを増やす
            attributeSwitcher.AddGauge(orbValue);

            // オーブを消去
            Destroy(other.gameObject);
        }
    }
}
