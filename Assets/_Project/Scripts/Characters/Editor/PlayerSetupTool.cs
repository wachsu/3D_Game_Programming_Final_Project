using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Game.Characters.EditorTools
{
    /// <summary>
    /// 開發輔助工具（非 mini project 評分內容）：建立玩家用的 Animator Controller 與 Player prefab。
    /// </summary>
    public static class PlayerSetupTool
    {
        const string ModelPath      = "Assets/_Project/Art/Characters/Smooth_Male_Casual.fbx";
        const string AnimFolder     = "Assets/_Project/Art/Characters/Animations";
        const string ControllerPath = AnimFolder + "/PlayerLocomotion.controller";
        const string PrefabFolder   = "Assets/_Project/Prefabs/Characters";
        const string PrefabPath     = PrefabFolder + "/Player.prefab";

        // Blend Tree 門檻（m/s），之後可依 PlayerMotor 的實際速度調整
        const float IdleThreshold = 0f;
        const float WalkThreshold = 2f;
        const float RunThreshold  = 5f;

        [MenuItem("Tools/Characters/Create Player Prefab")]
        static void CreatePlayerPrefab()
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (model == null)
            {
                Debug.LogError($"[PlayerSetup] 找不到模型：{ModelPath}");
                return;
            }

            var controller = LoadOrCreateController();
            if (controller == null)
                return;

            EnsureFolder("Assets/_Project/Prefabs", "Characters");

            var root = new GameObject("Player") { tag = "Player" };
            int playerLayer = LayerMask.NameToLayer("Player");

            var cc = root.AddComponent<CharacterController>();
            cc.height = 1.7f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.85f, 0f);
            cc.stepOffset = 0.3f;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, root.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            var animator = instance.GetComponent<Animator>();
            if (animator == null)
                animator = instance.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;

            if (playerLayer >= 0)
                SetLayerRecursively(root, playerLayer);

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);
            Object.DestroyImmediate(root);

            if (!success)
            {
                Debug.LogError($"[PlayerSetup] 無法儲存 prefab：{PrefabPath}");
                return;
            }

            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log($"[PlayerSetup] 已建立 {PrefabPath}（Animator：{ControllerPath}）");
        }

        static AnimatorController LoadOrCreateController()
        {
            var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (existing != null)
            {
                Debug.Log($"[PlayerSetup] 沿用既有的 {ControllerPath}");
                return existing;
            }

            var clips = AssetDatabase.LoadAllAssetsAtPath(ModelPath)
                .OfType<AnimationClip>()
                .Where(c => !c.name.StartsWith("__preview__"))
                .ToArray();

            var idle = clips.FirstOrDefault(c => c.name.EndsWith("Man_Idle"));
            var walk = clips.FirstOrDefault(c => c.name.EndsWith("Man_Walk"));
            var run  = clips.FirstOrDefault(c => c.name.EndsWith("Man_Run"));
            if (idle == null || walk == null || run == null)
            {
                Debug.LogError("[PlayerSetup] 模型裡找不到 Man_Idle / Man_Walk / Man_Run 動畫。");
                return null;
            }

            EnsureFolder("Assets/_Project/Art/Characters", "Animations");

            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

            controller.CreateBlendTreeInController("Locomotion", out BlendTree tree, 0);
            tree.blendType = BlendTreeType.Simple1D;
            tree.blendParameter = "Speed";
            tree.useAutomaticThresholds = false;
            tree.AddChild(idle, IdleThreshold);
            tree.AddChild(walk, WalkThreshold);
            tree.AddChild(run, RunThreshold);

            AssetDatabase.SaveAssets();
            return controller;
        }

        static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
                AssetDatabase.CreateFolder(parent, name);
        }

        static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}
