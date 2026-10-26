# 3D Game Programming — 團隊規格與 Mini Project 分工

> 遊戲：百貨公司救援（護送老人、引導小孩到廁所），Unity 3D，三人小組。
> Claude Code 每次啟動會讀這份檔案；寫程式時請遵守以下規格。

## 0. 角色分工（來自 proposal 的 Workload Distribution）
- **徐畹茜｜Scene & Level**：多樓層百貨場景（店面、攤位、手扶梯、電梯）、環境線索（告示、腳印、排隊人龍）、樓層轉換與關卡/線索流程。
- **盧語沁｜NPC & Player**：玩家移動 + Cinemachine 第三人稱、NPC 護送（慢速老人、亂跑小孩）、廁所抵達觸發與任務完成。
- **鍾君逸｜Game Logic & UI**：開場故事、勝敗條件、計時器與 UI（樓層地圖、提示、救援清單）、音效與敘事文字、定義共用介面（GameEvents、IRescueTarget、IClueTrigger）。

每人負責一關（場景與「怪物/阻礙」需完全不同）：
| 樓層 | 負責 | 主題 / 阻礙 |
|---|---|---|
| B1 | 徐畹茜 | 美食街 / 排隊人潮 |
| 1F | 盧語沁 | 精品化妝品區 / 店員攔人、護送慢速老人 |
| 2F | 鍾君逸 | 玩具遊樂區 / 亂跑小孩、限時 |

## 1. Mini Project 規則重點（3D Game Programming Project Exercises_202609.pdf）
- 個人作業，每人至少 4 題，一次 demo 一題。
- 不可與 assignment 重疊；重用內容不計分，demo 時需說明哪些是新寫的。
- 用 Unity sample 須修改並加 ≥3 新功能，並以迴圈產生 ≥20 個物件。
- 自訂題：demo 前至少一週寄給助教；格式：名稱(≥2字)、描述 50–120 字、需有畫面、有動畫物件、≥1 技術、≥50 物件。

## 2. Mini Project 分配與順序
| 輪次 | 徐畹茜（場景） | 盧語沁（角色/NPC） | 鍾君逸（邏輯/UI） |
|---|---|---|---|
| 1 | 4.A.3 Game portals（手扶梯/電梯傳送） | 4.A.2 Character navigation | 4.E.2 Function selection（主選單） |
| 2 | 4.D.1 Environment effects（濕地板變慢/步道加速/彈跳床） | 4.B.1 A* path finding | 4.D.3 Character particle & sound effects |
| 3 | 4.A.1 Hierarchical model（展示架/娃娃機） | 4.C.2 Group steering（小孩、人潮） | 4.E.3 Game UI（小地圖、清單、計時條） |
| 4 | 4.C.3 Deformable surfaces（布旗/門簾） | 4.B.2 AI characters → 自訂「Escort Follower AI」 | 4.E.1 Save & Load |

- 備用題：4.C.4 Character interaction（推倒購物車/玩具）、4.C.1 Mass-spring（掛飾）。
- 不選：4.D.2 Combat（與正向主題衝突）。
- B.2 原題要求「攻擊」→ 改自訂題（拉衣角/要求等待），盧語沁 於第 2 輪 demo 前寄助教。
- 第 1 輪元件完成後盡快 merge 進共用 repo，供他人第 2 輪使用。

## 3. 開發環境
- Unity 6.3 LTS（6000.3.x，三人同小版號；課程有指定則從課程）、URP、新版 Input System、Cinemachine 3.x、TextMeshPro。
- NPC 尋路不用 NavMesh，統一用 盧語沁 的 A*。
- Editor：Asset Serialization = Force Text。
- Git：Unity `.gitignore`；LFS 追蹤 `*.fbx *.png *.jpg *.psd *.wav *.mp3`；`main` 受保護；分支 `feature/<名字>-<題目>`；PR 需一位隊友 review。

## 4. 共用介面（鍾君逸 第一週推上 repo）
```csharp
public static class GameEvents {
    public static event Action<IRescueTarget> RescueStarted, RescueCompleted;
    public static event Action<IClueTrigger> ClueFound;
    public static event Action<int, int> FloorChanged;   // from, to
    public static event Action GameWon, GameLost;
    public static void RaiseRescueStarted(IRescueTarget t)   => RescueStarted?.Invoke(t);
    public static void RaiseRescueCompleted(IRescueTarget t) => RescueCompleted?.Invoke(t);
    public static void RaiseClueFound(IClueTrigger c)        => ClueFound?.Invoke(c);
    public static void RaiseFloorChanged(int from, int to)   => FloorChanged?.Invoke(from, to);
    public static void RaiseGameWon()  => GameWon?.Invoke();
    public static void RaiseGameLost() => GameLost?.Invoke();
}

public enum RescueState { Waiting, Following, Wandering, Arrived }

public interface IRescueTarget {   // 盧語沁 實作
    string Id { get; }
    string DisplayName { get; }
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

## 5. 場景規格
- 1 unit = 1 m；身高：玩家 1.7、老人 1.6、小孩 1.1 m。
- 樓層高度：B1 y=-5、1F y=0、2F y=5；每層約 60×40 m。
- A* 網格每格 0.5 m，只把 Obstacle layer 當障礙。
- Layers：6 Player、7 NPC、8 Obstacle、9 Floor、10 Trigger、11 Interactable。
- Tags：Player、Restroom、Portal、Clue。
- 場景：`Main.unity`（鍾君逸：GameManager、UI、玩家、攝影機）；三樓層各一 scene，additive 載入，僅負責人可改；個人 `Scenes/Sandbox/<名字>.unity` 開發 mini project。
- 資料夾：`Assets/_Project/Scripts/{Core(鍾君逸),Level(徐畹茜),Characters(盧語沁),UI(鍾君逸)}`、`Prefabs/ Scenes/ Art/ Audio/`。
- 命名：類別 PascalCase、private `_camelCase`、namespace `Game.Core` / `Game.Level` …
- 美術：low-poly；Kenney、Quaternius（CC0）；Mixamo 動畫（Humanoid）；來源記入 References。

## 6. 作業規則
- 一律用自己的 GitHub 帳號 commit；訊息前綴題號，例：`[mini-4.A.3] add portal particle`。
- 每個 mini project 附 `README.md`，分「新寫的部分 / 重用的部分」。
- 流程：Sandbox 完成並 demo → 再整合進樓層 scene。

## 待辦 / 未定
- demo 實際日期（尚未提供，拿到後對應四輪）。
- 第一次會議確認本規格。
