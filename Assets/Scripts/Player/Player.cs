using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"경로 확인 Save Path: {Application.persistentDataPath}");
        // 진짜로 할때는 경로를 바꿔야?
    }
}
