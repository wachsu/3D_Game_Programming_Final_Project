using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Characters.EditorTools
{
    /// <summary>
    /// 開發輔助工具（非 mini project 評分內容）：在目前開啟的場景建立跟隨 Player 的第三人稱 Cinemachine 鏡頭。
    /// </summary>
    public static class CameraSetupTool
    {
        const string CameraName = "PlayerCamera";

        [MenuItem("Tools/Characters/Setup Third Person Camera")]
        static void SetupCamera()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[CameraSetup] 場景裡找不到 Tag 為 Player 的物件。");
                return;
            }

            var existing = GameObject.Find(CameraName);
            if (existing != null)
            {
                Debug.LogWarning($"[CameraSetup] 場景裡已經有 {CameraName}，沒有重複建立。");
                Selection.activeGameObject = existing;
                return;
            }

            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[CameraSetup] 場景裡找不到 Tag 為 MainCamera 的攝影機。");
                return;
            }
            if (mainCamera.GetComponent<CinemachineBrain>() == null)
                Undo.AddComponent<CinemachineBrain>(mainCamera.gameObject);

            var go = new GameObject(CameraName);
            Undo.RegisterCreatedObjectUndo(go, "Create Player Camera");

            var cam = go.AddComponent<CinemachineCamera>();
            cam.Follow = player.transform;

            var orbital = go.AddComponent<CinemachineOrbitalFollow>();
            orbital.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            orbital.Radius = 4f;
            orbital.TargetOffset = new Vector3(0f, 1.2f, 0f);
            orbital.TrackerSettings.BindingMode = BindingMode.WorldSpace;   // 鏡頭不跟著角色轉向
            orbital.VerticalAxis.Value = 15f;
            orbital.RadialAxis.Range = new Vector2(0.5f, 2f);                // 滾輪縮放：2–8 m

            var composer = go.AddComponent<CinemachineRotationComposer>();
            composer.TargetOffset = new Vector3(0f, 1.4f, 0f);              // 看向頭部附近

            var deoccluder = go.AddComponent<CinemachineDeoccluder>();
            deoccluder.CollideAgainst = 1 << LayerMask.NameToLayer("Obstacle");
            deoccluder.IgnoreTag = "Player";

            go.AddComponent<ThirdPersonCameraInput>();

            EditorSceneManager.MarkSceneDirty(go.scene);
            Selection.activeGameObject = go;
            Debug.Log($"[CameraSetup] 已建立 {CameraName}，跟隨 {player.name}。記得存檔。");
        }
    }
}
