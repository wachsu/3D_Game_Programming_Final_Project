using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Column Settings")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 6;
    [SerializeField] private float spacing = 3f;

    [Header("Appearance")]
    [SerializeField] private Material columnMaterial;

    private void Start()
    {
        GenerateColumns();
    }

    private void GenerateColumns()
    {
        int columnsPerSide = 6;
        float boundary = 13f;
        float spacing = 4f;

        for (int i = 0; i < columnsPerSide; i++)
        {
            float position = -10f + i * spacing;

            // 上方
            CreateColumn(
                new Vector3(position, 1.5f, boundary)
            );

            // 下方
            CreateColumn(
                new Vector3(position, 1.5f, -boundary)
            );

            // 左方
            CreateColumn(
                new Vector3(-boundary, 1.5f, position)
            );

            // 右方
            CreateColumn(
                new Vector3(boundary, 1.5f, position)
            );
        }
    }
    private void CreateColumn(Vector3 position)
    {
        GameObject column = GameObject.CreatePrimitive(
            PrimitiveType.Cylinder
        );

        column.name = "Column";

        column.transform.SetParent(transform);

        column.transform.position = position;

        column.transform.localScale =
            new Vector3(0.5f, 1.5f, 0.5f);

        column.layer =
            LayerMask.NameToLayer("Obstacle");

        if (columnMaterial != null)
        {
            column.GetComponent<Renderer>().material =
                columnMaterial;
        }
    }
}