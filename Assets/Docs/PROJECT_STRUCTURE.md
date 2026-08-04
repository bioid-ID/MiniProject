# Portal Dungeon — Project Structure

## 씬 이름 변경 (Unity에서)

메뉴: **Tools → Portal Dungeon → Rename Scenes To Final Names**

| 기존 | 최종 | 역할 |
|------|------|------|
| SampleScene | **Lobby** | 로비/허브 |
| PortalDungeon | **MainDungeon** | 일반 던전 (성장 유지) |
| PortalDungeon 1 | **RoguelikeDungeon** | 초기 스탯 런, **골드만** 유지 |

## 수치 어디에 넣나 (중요)

| 종류 | 에셋 | 예 |
|------|------|----|
| 몬스터마다 다른 값 | **EnemyData** | HP, AI 거리, 쿨타임, 골드/경험치, 드랍, 넉백저항, 보스 연타 |
| 던전마다 스폰 | **DungeonSpawnProfile** | 어떤 몬스터, 각자 스폰간격, 초기 수, 보스 등장조건 |
| 플레이어/시스템 공통 | **GameBalance** | 포탈 거리, 재생식, 플레이어 기본 넉백감 |
| 종족/스킨 (예정) | **RaceData** | 초기 STR/DEX… (아직 미연결) |

### Unity에서 한 번에 만들기

1. **Tools → Portal Dungeon → Create Default Spawn Profiles**  
   → `Enemy_Basic/Elite/Boss` + `Resources/SpawnProfiles/MainDungeon|RoguelikeDungeon`
2. **Tools → Portal Dungeon → Create Or Select GameBalance**  
   → 플레이어/시스템만
3. 각 EnemyData / SpawnProfile Inspector에서 숫자 조절

새 몬스터 추가: EnemyData 복제 → SpawnProfile의 enemies 리스트에 추가 (간격·가중치 따로).

## 추천 Assets 구조

```
Assets/
  Art/Characters|Enemies|Items|Tiles|UI
  Audio/BGM|SFX
  Prefabs/Enemies|Items|Props|Portals
  Resources/
    Enemies/            # 스프라이트 (선택)
    SpawnProfiles/      # MainDungeon, RoguelikeDungeon
    GameBalance.asset
  ScriptableObjects/Enemies|Items|Portals|Races
  Scenes/Lobby|MainDungeon|RoguelikeDungeon
  Scripts/
  Docs/PROJECT_STRUCTURE.md
```

## Unity에서 할 일 (콘텐츠)

1. 씬 이름 변경
2. Tilemap + Props
3. Enemy 프리팹/스프라이트
4. ItemData.icon
5. EnemyData별로 드랍 테이블 채우기
6. (나중) DOTween으로 피격/드랍/UI 연출
