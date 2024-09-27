using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // タイムテキスト、パネル、スコアテキスト（Inspectorでアタッチ）
    public Text timeText;            // 00:00 形式で表示するタイムテキスト
    public GameObject winPanel;      // 勝利時に表示するパネル
    public GameObject losePanel;     // 敗北時に表示するパネル
    public Text scoreText;           // スコアを表示するテキスト（WinPanel内）

    private float startTime;         // ゲーム開始からの経過時間
    private bool isGameActive = true; // ゲームが進行中かどうか

    // スタート時に初期化
    private void Start()
    {
        startTime = Time.time;
        winPanel.SetActive(false);  // 初期状態で非表示
        losePanel.SetActive(false); // 初期状態で非表示
        StartCoroutine(UpdateTimeText());
    }

    // 毎秒ごとにタイムテキストを更新
    private IEnumerator UpdateTimeText()
    {
        while (isGameActive)
        {
            float timeElapsed = Time.time - startTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);

            // タイムテキストを更新
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1f); // 1秒ごとに更新
        }
    }

    // プレイヤーが負けた時に呼び出すメソッド
    public void PlayerLose()
    {
        isGameActive = false;  // ゲーム進行を停止
        losePanel.SetActive(true);  // 敗北パネルを表示
        timeText.text = "";
    }

    // プレイヤーが勝った時に呼び出すメソッド
    public void PlayerWin()
    {
        isGameActive = false;  // ゲーム進行を停止
        winPanel.SetActive(true);  // 勝利パネルを表示

        // 勝利までにかかった時間の逆数に100をかけたスコアを計算
        float timeElapsed = Time.time - startTime;
        float score = Mathf.Max(1, 100f / timeElapsed);  // スコアの最低値を1にする

        // スコアをテキストに反映
        scoreText.text = "Score：" + Mathf.RoundToInt(score).ToString();
        timeText.text = "";
    }

    public void OnTittleButton()
    {
        SceneManager.LoadScene("Tittle");
    }
}