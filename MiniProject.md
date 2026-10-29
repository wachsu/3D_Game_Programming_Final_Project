# ⟪Duty Calls: Restroom Rescue⟫（屎命召喚）— 團隊規格與 Mini Project 分工

> 遊戲：美食街食安危機爆發，沒被波及的平凡主角接下樓層地圖，在百貨公司裡把急著上廁所的人一一帶到廁所。Unity 3D 第三人稱，三人小組。
> Repo：https://github.com/wachsu/3D_Game_Programming_Final_Project
> Claude Code 每次啟動會讀這份檔案；寫程式時請遵守以下規格。

## 0. 角色分工（來自 proposal 的 Workload Distribution）
- **徐畹茜｜Scene & Level**：多樓層百貨場景（店面、攤位、手扶梯、電梯作為空間分隔，而非迷宮牆）、環境線索（告示、腳印、排隊人龍）、樓層轉換與關卡/線索流程。
- **盧語沁｜NPC & Player**：玩家移動 + Cinemachine 第三人稱、NPC 護送（大人、慢速老人、亂跑小孩）、廁所抵達觸發與任務完成。
- **鍾君逸｜Game Logic & UI**：開場故事、勝敗條件、計時器與 UI（樓層地圖、提示、救援清單）、音效與敘事文字、定義共用介面（GameEvents、IRescueTarget、IClueTrigger）。

## 1. 關卡內容（依 proposal 第 7 節）
每人負責一關的場景搭建；三關場景與 NPC 類型完全不同。依序過關：救完該關所有 NPC 才解鎖下一關，每關有獨立計時。

| 關卡 | 負責 | 場景 | NPC | 阻礙 | 專屬機制 |
|---|---|---|---|---|---|
| Level 1｜B1 | 徐畹茜 | 美食街（食安危機發生地） | 大人：正常速度、穩定跟隨、不亂跑 | 餐桌、椅子、餐車；清潔車或臨時物品擋路，需找替代路線 | 教學關：互動、帶路、繞過障礙 |
| Level 2｜3F | 鍾君逸 | 玩具樓層 | 小孩：速度較快，太久沒注意會跑去玩具展示區 | 玩具架、積木堆、玩具箱、大型布偶（會擋視線） | 跳躍：引導小孩跳過低矮障礙（積木堆、玩具箱），可走與第 1 關不同的路線；跑掉要重新找回並互動 |
| Level 3｜7F | 盧語沁 | 電影院| 老人：速度慢，玩家走太快會落後 | 不規則座位、窄走道、階梯、散落飲料杯與爆米花桶 | 手電筒（昏暗環境）；推開可移動障礙（Rigidbody） |

英雄旅程對應：第 1 關 = The Inmost Cave；第 2 關 = The Supreme Ordeal → Seizing the Sword；第 3 關 = The Road Back → Resurrection。

### 操作（proposal 第 6 節）
| 按鍵 | 動作 |
|---|---|
| W/A/S/D | 移動 |
| 滑鼠 | 鏡頭方向 |
| E | 與 NPC 互動、開始帶路 |
| R | 跳躍；並指示小孩跳過低矮障礙 |
| F | 推開可移動障礙 |

### 特效清單（proposal 第 8 節，由鍾君逸整合）
- 想上廁所的 NPC：呻吟／抱怨聲。
- 剩餘時間不多：畫面震動、心跳聲、喘氣聲。
- 找到廁所：閃光粒子、開門音效。
- 跳躍：起跳、落地音效。
- 推障礙：物體移動／碰撞聲。
- 電影院：手電筒光照、昏暗環境光。

## 2. Mini Project 規則重點（3D Game Programming Project Exercises_202609.pdf）
- 個人作業，每人至少 4 題，一次 demo 一題。
- 不可與 assignment 重疊；重用內容不計分，demo 時需說明哪些是新寫的。
- 用 Unity sample 須修改並加 ≥3 新功能，並以迴圈產生 ≥20 個物件。
- 自訂題：demo 前至少一週寄給助教；格式：名稱(≥2字)、描述 50–120 字、需有畫面、有動畫物件、≥1 技術、≥50 物件。

## 3. Mini Project 分配與順序
| 輪次 | 徐畹茜（場景） | 盧語沁（角色/NPC） | 鍾君逸（邏輯/UI） |
|---|---|---|---|
| 1 | 4.A.3 Game portals（手扶梯/電梯在三關樓層間傳送） | 4.A.2 Character navigation | 4.E.2 Function selection（主選單） |
| 2 | 4.D.1 Environment effects（美食街：灑出的湯汁變慢、清潔後打蠟地板加速、跳上台階） | 4.B.1 A* path finding | 4.D.3 Character particle & sound effects（找到廁所閃光、跳躍/落地音效） |
| 3 | 4.A.1 Hierarchical model（餐車、旋轉招牌） | 4.C.2 Group steering（小孩亂跑、分心跑向玩具） | 4.E.3 Game UI（小地圖、救援清單、計時條） |
| 4 | 4.C.3 Deformable surfaces（美食街攤位布簾/布旗） | 4.B.2 AI characters → 自訂「Escort Follower AI」（老人跟隨） | 4.E.1 Save & Load |

- 備用題：4.C.4 Character interaction（正好對應第 3 關推障礙機制）、4.C.1 Mass-spring（掛飾）。
- 不選：4.D.2 Combat（與正向主題衝突）。
- B.2 原題要求「攻擊」→ 改自訂題（拉衣角/要求等待），盧語沁 於第 2 輪 demo 前寄助教。
- 第 1 輪元件完成後盡快 merge 進共用 repo，供他人第 2 輪使用。

## 4. 開發環境
- Unity 6.3 LTS（6000.3.x，三人同小版號；課程有指定則從課程）、URP、新版 Input System、Cinemachine 3.x、TextMeshPro。
- NPC 尋路：統一用 盧語沁 的 A*（mini project 4.B.1 要求自行實作）。註：proposal 的 Sprint Planning 寫了安裝 AI Navigation，若 A* 進度卡住超過三天，依風險管理改用 NavMesh 作為備案。
- 目標平台：Windows 10/11 64-bit，8 GB RAM，DX10+ GPU。
- Editor：Asset Serialization = Force Text。
- Git：Unity `.gitignore`；LFS 追蹤 `*.fbx *.png *.jpg *.psd *.wav *.mp3`；`main` 受保護；分支 `feature/<名字>-<題目>`；PR 需一位隊友 review；同一個 scene 同時只能一人編輯。

## 5. 共用介面（鍾君逸 第一週推上 repo）
```csharp
public static class GameEvents {
    public static event Action<IRescueTarget> RescueStarted, RescueCompleted;
    public static event Action<IClueTrigger> ClueFound;
    public static event Action<int, int> FloorChanged;   // from, to
    public static event Action<int> LevelCleared;        // levelIndex 1–3
    public static event Action GameWon, GameLost;
    public static void RaiseRescueStarted(IRescueTarget t)   => RescueStarted?.Invoke(t);
    public static void RaiseRescueCompleted(IRescueTarget t) => RescueCompleted?.Invoke(t);
    public static void RaiseClueFound(IClueTrigger c)        => ClueFound?.Invoke(c);
    public static void RaiseFloorChanged(int from, int to)   => FloorChanged?.Invoke(from, to);
    public static void RaiseLevelCleared(int level)          => LevelCleared?.Invoke(level);
    public static void RaiseGameWon()  => GameWon?.Invoke();
    public static void RaiseGameLost() => GameLost?.Invoke();
}

public enum NpcType { Adult, Child, Elderly }
public enum RescueState { Waiting, Following, Wandering, Arrived }

public interface IRescueTarget {   // 盧語沁 實作
    string Id { get; }
    string DisplayName { get; }
    NpcType Type { get; }
    RescueState State { get; }
    Transform Transform { get; }
}

public interface IClueTrigger {    // 徐畹茜 實作
    string ClueId { get; }
    int Floor { get; }
    void Reveal();
}

public interface IPlayerMotor {    // 盧語沁 實作，徐畹茜、鍾君逸 呼叫
    void SetSpeedMultiplier(float multiplier, float duration);
    void Jump(float height);
    void Teleport(Vector3 pos, Quaternion rot);
    bool IsMoving { get; }
}

public interface ISaveable {       // 需存檔元件實作，鍾君逸 的 E.1 收集
    string SaveId { get; }
    string CaptureState();          // JsonUtility.ToJson
    void RestoreState(string json);
}
```
- Teleport：關 CharacterController → SetPositionAndRotation → 開 → `Physics.SyncTransforms()`。
- 元件間只透過 GameEvents 與介面溝通，不直接 GetComponent 別人的類別。
- 存檔：JSON 於 `Application.persistentDataPath/save.json`。

## 6. 場景規格
- 1 unit = 1 m；身高：玩家 1.7、大人 1.7、老人 1.6、小孩 1.1 m。
- 樓層高度：B1 美食街 y=-5、3F 玩具樓層 y=10、7F 電影院 y=30；每層約 60×40 m。
- A* 網格每格 0.5 m，只把 Obstacle layer 當障礙；LowObstacle 對小孩（跳躍時）可通行。
- Layers：6 Player、7 NPC、8 Obstacle、9 Floor、10 Trigger、11 Interactable、12 LowObstacle。
- Tags：Player、Restroom、Portal、Clue、Pushable。
- 場景：`Main.unity`（鍾君逸：GameManager、UI、玩家、攝影機）；三關各一 scene（`Level1_FoodCourt`、`Level2_ToyFloor`、`Level3_Theater`），additive 載入，僅負責人可改；個人 `Scenes/Sandbox/<名字>.unity` 開發 mini project。
- 資料夾：`Assets/_Project/Scripts/{Core(鍾君逸),Level(徐畹茜),Characters(盧語沁),UI(鍾君逸)}`、`Prefabs/ Scenes/ Art/ Audio/`。
- 命名：類別 PascalCase、private `_camelCase`、namespace `Game.Core` / `Game.Level` …
- 美術：low-poly；Kenney、Poly Pizza、Quaternius；Mixamo 動畫（Humanoid）；音效 Pixabay、Kenney Impact Sounds；來源記入 References。

## 7. 作業規則
- 一律用自己的 GitHub 帳號 commit；訊息前綴題號，例：`[mini-4.A.3] add portal particle`。
- 每個 mini project 附 `README.md`，分「新寫的部分 / 重用的部分」。
- 流程：Sandbox 完成並 demo → 再整合進關卡 scene。
- 卡關超過三天 → 立即改用簡化或備案做法（proposal 風險管理）。
