using System.IO;
using Game.UI.Demo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Cinemachine;
using Game.Core;

namespace Game.EditorTools
{
    /// <summary>一鍵建立 4.E.3 Game UI 示範場景（選單：Game/Build Game UI Demo Scene）。</summary>
    public static class GameUiDemoSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Sandbox/MiniProjects_E/E_GameUI_Demo.unity";
        private const string PlayerPrefabPath = "Assets/_Project/Prefabs/Characters/Player.prefab";

        [MenuItem("Game/Build Game UI Demo Scene")]
        public static void Build()
        {
            var player = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (player == null)
            {
                Debug.LogError("[GameUiDemo] 找不到 Player prefab：" + PlayerPrefabPath);
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 光與攝影機
            var light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<CinemachineBrain>();

            // 地板
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(60f, 1f, 60f);
            floor.transform.position = new Vector3(0f, -0.5f, 0f);
            floor.layer = LayerMask.NameToLayer("Floor");
            Tint(floor, new Color(0.35f, 0.38f, 0.42f));

            // 用迴圈產生 36 個障礙物（餐桌 / 立柱），讓小地圖有東西可看
            var obstacles = new GameObject("Obstacles").transform;
            for (int i = 0; i < 36; i++)
            {
                int row = i / 6, col = i % 6;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Table_" + i;
                cube.transform.SetParent(obstacles);
                float h = 0.8f + (i % 3) * 0.6f;
                cube.transform.localScale = new Vector3(2.5f, h, 2.5f);
                cube.transform.position = new Vector3(-22f + col * 8.8f, h * 0.5f, -22f + row * 8.8f);
                cube.layer = LayerMask.NameToLayer("Obstacle");
                Tint(cube, Color.HSVToRGB((i * 0.07f) % 1f, 0.35f, 0.85f));
            }

            // 玩家 + 第三人稱鏡頭
            var playerGo = (GameObject)PrefabUtility.InstantiatePrefab(player);
            playerGo.name = "Player";
            playerGo.transform.position = new Vector3(0f, 0.1f, -26f);
            Selection.activeGameObject = playerGo;
            EditorApplication.ExecuteMenuItem("Tools/Characters/Setup Third Person Camera");

            // 假 NPC（Adult / Child / Elderly）
            var npcs = new GameObject("DemoNpcs").transform;
            var defs = new (string id, string name, NpcType type, Vector3 pos, Color color)[]
            {
                ("npc_01", "Uncle Wang", NpcType.Adult,   new Vector3(-12f, 1f, -12f), new Color(0.3f, 0.8f, 1f)),
                ("npc_02", "Mei", NpcType.Child,          new Vector3(10f, 1f, -8f),   new Color(1f, 0.5f, 0.85f)),
                ("npc_03", "Grandpa Lin", NpcType.Elderly,new Vector3(-6f, 1f, 8f),    new Color(0.7f, 0.65f, 1f)),
                ("npc_04", "Ms. Chen", NpcType.Adult,     new Vector3(14f, 1f, 14f),   new Color(0.3f, 0.8f, 1f)),
                ("npc_05", "Tom", NpcType.Child,          new Vector3(-16f, 1f, 18f),  new Color(1f, 0.5f, 0.85f)),
            };
            foreach (var d in defs)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.name = d.name;
                go.transform.SetParent(npcs);
                go.transform.position = d.pos;
                go.layer = LayerMask.NameToLayer("NPC");
                Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());
                Tint(go, d.color);
                var npc = go.AddComponent<DemoRescueNpc>();
                var so = new SerializedObject(npc);
                so.FindProperty("_id").stringValue = d.id;
                so.FindProperty("_displayName").stringValue = d.name;
                so.FindProperty("_type").enumValueIndex = (int)d.type;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            new GameObject("GameUiDemoDriver").AddComponent<GameUiDemoDriver>();
            GameHudBuilder.Build();

            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("[GameUiDemo] 完成：" + ScenePath);
        }

        private static void Tint(GameObject go, Color c)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var mat = new Material(r.sharedMaterial) { color = c };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            r.sharedMaterial = mat;
        }
    }
}
