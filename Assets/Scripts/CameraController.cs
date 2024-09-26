using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    // プレイヤーオブジェクト（Inspectorでアタッチ可能にする）
    public Transform playerTransform;

    // カメラとプレイヤーの相対的な位置
    private Vector3 offset;
    private Quaternion defolt;

    // カメラがプレイヤーと常に一定の距離を保つように初期設定
    private void Start()
    {
        // カメラとプレイヤーの相対的な位置を計算
        offset = transform.position - playerTransform.position;
        defolt = this.transform.rotation;
    }

    // 毎フレームごとにカメラの位置を更新
    private void LateUpdate()
    {
        // プレイヤーの現在位置に対して相対的な位置を維持
        transform.position = playerTransform.position + offset;

        // プレイヤーが回転してもカメラは回転しないようにする
        // カメラの回転は固定されており、プレイヤーの回転に影響されない
        transform.rotation = defolt; // またはデフォルトの回転に固定
    }
}