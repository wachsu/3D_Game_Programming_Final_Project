using UnityEditor;
using UnityEngine;

namespace Game.Characters.EditorTools
{
    /// <summary>
    /// 開發輔助工具（非 mini project 評分內容）：量測選取物件的實際高度，
    /// 並依目標身高建議模型匯入的 Scale Factor。
    /// </summary>
    public static class ModelMeasureTool
    {
        const float TargetHeight = 1.7f; // MiniProject.md 第 6 節：玩家身高 1.7 m

        [MenuItem("Tools/Characters/Measure Selected Model")]
        static void MeasureSelected()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                Debug.LogWarning("[ModelMeasure] 請先在 Hierarchy 選取要量測的角色。");
                return;
            }

            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"[ModelMeasure] {go.name} 底下沒有 Renderer，無法量測。");
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            float height = bounds.size.y;
            string message = $"[ModelMeasure] {go.name}：實際高度 {height:F3} m（寬 {bounds.size.x:F3} m、深 {bounds.size.z:F3} m）";

            var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
            string assetPath = source != null ? AssetDatabase.GetAssetPath(source) : null;
            if (!string.IsNullOrEmpty(assetPath) && AssetImporter.GetAtPath(assetPath) is ModelImporter importer && height > 0f)
            {
                float lossy = go.transform.lossyScale.y;
                float suggested = importer.globalScale * TargetHeight / (height / lossy);
                message += $"\n來源：{assetPath}，目前 Scale Factor = {importer.globalScale}，Transform Scale Y = {lossy}" +
                           $"\n若要身高 {TargetHeight} m，建議 Scale Factor = {suggested:G4}";
            }

            Debug.Log(message);
        }

        [MenuItem("Tools/Characters/Measure Selected Model", true)]
        static bool CanMeasure() => Selection.activeGameObject != null;
    }
}
