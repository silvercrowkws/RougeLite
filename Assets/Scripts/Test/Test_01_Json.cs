using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_01_Json : TestBase
{
#if UNITY_EDITOR    

    /// <summary>
    /// 게임 매니저
    /// </summary>
    GameManager gameManager;

    /// <summary>
    /// 플레이어
    /// </summary>
    Player player;

    /// <summary>
    /// 세이브 매니저
    /// </summary>
    SaveManager saveManager;

    /// <summary>
    /// 로드 매니저
    /// </summary>
    LoadManager loadManager;

    /// <summary>
    /// 플레이어의 정보(저장용)
    /// </summary>
    PlayerData tempPlayerData;

    /// <summary>
    /// 플레이어의 정보(로드용)
    /// </summary>
    PlayerData loadPlayerData;

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;

        tempPlayerData = new PlayerData();
        saveManager = GameManager.Instance.SaveManager;
        loadManager = GameManager.Instance.LoadManager;
    }

    void PrintData(PlayerData loadPlayerData)
    {
        Debug.Log($"name : {loadPlayerData.name}");
        Debug.Log($"level : {loadPlayerData.level}");
        foreach (string item in loadPlayerData.items)
        {
            Debug.Log($"item : {item}");
        }
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        tempPlayerData.name = "Player_Test_1";
        tempPlayerData.level = 1;
        tempPlayerData.items = new string[] { "sword_1", "shield_1" };
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        tempPlayerData.name = "Player_Test_2";
        tempPlayerData.level = 2;
        tempPlayerData.items = new string[] { "sword_2", "shield_2" };
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        tempPlayerData.name = "Player_Test_3";
        tempPlayerData.level = 3;
        tempPlayerData.items = new string[] { "sword_3", "shield_3" };
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        
    }

    protected override void OnTest6(InputAction.CallbackContext context)
    {
        Debug.Log("데이터 저장");
        saveManager.SaveData(tempPlayerData);       // 데이터 저장
    }

    protected override void OnTest7(InputAction.CallbackContext context)
    {
        Debug.Log("데이터 로드");
        loadPlayerData = loadManager.LoadData();    // 데이터 로드
    }

    protected override void OnTest8(InputAction.CallbackContext context)
    {
        PrintData(loadPlayerData);                              // 불러온 데이터 출력
    }

#endif
}
