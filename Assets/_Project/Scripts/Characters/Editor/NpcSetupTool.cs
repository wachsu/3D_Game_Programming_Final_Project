using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Characters.EditorTools
{
    /// <summary>
    /// 開發輔助工具（非 mini project 評分內容）：在目前開啟的場景放一個站著播 Idle 的 NPC。
    /// </summary>
    public static class NpcSetupTool
    {
        const string ModelPath      = "Assets/_Project/Art/Characters/Smooth_Male_Suit.fbx";
        const string ControllerPath = "Assets/_Project/Art/Characters/Animations/PlayerLocomotion.controller";
        static readonly Vector3 SpawnPosition = new Vector3(4f, 0f, -4f);

        [MenuItem("Tools/Characters/Add Idle NPC")]
        static void AddIdleNpc()
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ControllerPath);
            if (model == null || controller == null)
            {
                Debug.LogError($"[NpcSetup] 找不到 {ModelPath} 或 {ControllerPath}。");
                return;
            }

            var root = new GameObject("NPC_Suit");
            Undo.RegisterCreatedObjectUndo(root, "Add Idle NPC");
            root.transform.SetPositionAndRotation(SpawnPosition, Quaternion.Euler(0f, 200f, 0f));

            var capsule = root.AddComponent<CapsuleCollider>();
            capsule.height = 1.7f;
            capsule.radius = 0.3f;
            capsule.center = new Vector3(0f, 0.85f, 0f);
            root.AddComponent<NpcIdle>();

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, root.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            var animator = instance.GetComponent<Animator>();
            if (animator == null)
                animator = instance.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;   // 與玩家共用：同一包骨架，Speed 預設 0 → Idle
            animator.applyRootMotion = false;

            int npcLayer = LayerMask.NameToLayer("NPC");
            if (npcLayer >= 0)
                SetLayerRecursively(root, npcLayer);

            EditorSceneManager.MarkSceneDirty(root.scene);
            Selection.activeGameObject = root;
            Debug.Log($"[NpcSetup] 已在 {SpawnPosition} 放置 NPC_Suit。記得存檔。");
        }

        static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}
