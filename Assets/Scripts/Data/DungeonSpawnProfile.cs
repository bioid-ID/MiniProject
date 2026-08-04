using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 던전별 스폰표. Resources/SpawnProfiles/MainDungeon|RoguelikeDungeon
/// </summary>
[CreateAssetMenu(fileName = "DungeonSpawnProfile", menuName = "ScriptableObjects/Dungeon Spawn Profile")]
public class DungeonSpawnProfile : ScriptableObject
{
    [Header("게임 시작 시 바로 깔 수")]
    public int initialSpawnCount = 2;

    [Header("화면 밖 스폰 여유")]
    public float viewportSpawnMargin = 1.2f;

    [Header("스폰 목록 (종류마다 간격·비중 따로)")]
    public List<EnemySpawnEntry> enemies = new();

    [Header("보스 웨이브")]
    public int bossKillsRequired = 12;
    public float bossSecondsRequired = 90f;
    public int bossStageLevel = 5;
    public EnemyData bossData;
    public string bossPrefabKey = EnemyPrefabCatalog.Boss;
}

[Serializable]
public class EnemySpawnEntry
{
    [Tooltip("Resources/Enemies 이미지 파일명 (확장자 제외). 예: Enemy_Basic\nEnemyData.bodySprite가 있으면 그쪽이 우선.")]
    public string prefabKey = EnemyPrefabCatalog.Basic;

    [Tooltip("이 줄이 쓸 EnemyData (스탯/드랍/이미지)")]
    public EnemyData enemyData;

    [Tooltip("뽑힐 비중. 예) Basic=3, Elite=1 → Basic이 약 75%, Elite 25%")]
    public float weight = 1f;

    [Tooltip("이 종류만의 스폰 주기(초). 3이면 이 줄은 3초마다 시도")]
    public float spawnInterval = 3f;

    [Tooltip("스폰될 때 Initialize에 넣는 레벨")]
    public int stageLevel = 1;

    [Tooltip("동시에 살아 있을 수 있는 최대 수. 0=제한 없음. 3이면 이 종류가 이미 3마리면 더 안 나옴")]
    public int maxAlive = 0;
}
