
using UnityEngine;
using System;

[Serializable]
public class ShopFoodConfig
{
    public string shopName;
    public GameObject[] foodPrefabs;
}

public class FoodCourtGenerator : MonoBehaviour
{
    [Header("Dining Area")]
    public int rows = 3;
    public int columns = 4;
    public float tableSpacingX = 5f;
    public float tableSpacingZ = 5f;

    [Header("Shops")]
    public int shopCount = 5;

    [Header("Portal Clearance")]
    public Transform[] portals;
    public float portalClearance = 3f;

    [Header("Materials")]
    public Material tableMaterial;
    public Material chairMaterial;
    public Material shopMaterial;
    public Material plantMaterial;
    public Material signMaterial;

    [Header("Shop Design")]
    public string[] shopNames =
{
    "BURGER",
    "PIZZA",
    "BREAD",
    "COFFEE",
    "DESSERT"
};

    public Color[] shopColors =
    {
    new Color(0.90f, 0.30f, 0.20f),
    new Color(0.95f, 0.65f, 0.15f),
    new Color(0.25f, 0.65f, 0.55f),
    new Color(0.45f, 0.30f, 0.20f),
    new Color(0.75f, 0.45f, 0.75f)
};

    [Header("Food Models")]
    public ShopFoodConfig[] shopFoods;
    public float foodScale = 0.5f;
    private Transform generatedRoot;

    void Start()
    {
        GenerateEnvironment();
    }

    void GenerateEnvironment()
    {
        GameObject container = new GameObject("GeneratedEnvironment");
        container.transform.SetParent(transform, false);
        generatedRoot = container.transform;

        GenerateTables();
        GenerateShops();
        //GeneratePlants();
    }


    void GenerateTables()
    {
        float startX = -(columns - 1) * tableSpacingX / 2f;
        float startZ = -(rows - 1) * tableSpacingZ / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = transform.position +
                    new Vector3(
                        startX + col * tableSpacingX,
                        0f,
                        startZ + row * tableSpacingZ
                    );

                if (IsNearPortal(position))
                    continue;

                GameObject diningSet =
                    new GameObject($"DiningSet_{row}_{col}");

                diningSet.transform.SetParent(generatedRoot);
                diningSet.transform.position = position;

                // Tabletop
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "Tabletop",
                    diningSet.transform,
                    new Vector3(0f, 0.75f, 0f),
                    new Vector3(1.8f, 0.15f, 1.2f),
                    tableMaterial
                );

                // Four table legs
                for (int x = -1; x <= 1; x += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        CreatePrimitive(
                            PrimitiveType.Cube,
                            "TableLeg",
                            diningSet.transform,
                            new Vector3(x * 0.7f, 0.35f, z * 0.42f),
                            new Vector3(0.12f, 0.7f, 0.12f),
                            tableMaterial
                        );
                    }
                }

                // Two chairs
                for (int side = -1; side <= 1; side += 2)
                {
                    CreateChair(diningSet.transform, side);
                }
            }
        }
    }

    void CreateChair(Transform parent, int side)
    {
        GameObject chair = new GameObject(
            side < 0 ? "Chair_Left" : "Chair_Right"
        );

        chair.transform.SetParent(parent, false);
        chair.transform.localPosition =
            new Vector3(side * 1.45f, 0f, 0f);

        // Seat
        CreatePrimitive(
            PrimitiveType.Cube,
            "Seat",
            chair.transform,
            new Vector3(0f, 0.45f, 0f),
            new Vector3(0.7f, 0.12f, 0.7f),
            chairMaterial
        );

        // Backrest (on the outer side)
        CreatePrimitive(
            PrimitiveType.Cube,
            "Backrest",
            chair.transform,
            new Vector3(side * 0.3f, 0.85f, 0f),
            new Vector3(0.12f, 0.8f, 0.7f),
            chairMaterial
        );

        // Four chair legs
        for (int x = -1; x <= 1; x += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "ChairLeg",
                    chair.transform,
                    new Vector3(x * 0.25f, 0.2f, z * 0.25f),
                    new Vector3(0.1f, 0.4f, 0.1f),
                    chairMaterial
                );
            }
        }
    }

    void GenerateShops()
    {
        for (int i = 0; i < shopCount; i++)
        {
            float x = (i - (shopCount - 1) / 2f) * 6f;

            GameObject shop = new GameObject($"FoodShop_{i + 1}");
            shop.transform.SetParent(generatedRoot, false);
            shop.transform.localPosition = new Vector3(x, 0f, 12.8f);

            Color shopColor = shopColors[i % shopColors.Length];

            Material coloredMaterial = new Material(
                Shader.Find("Universal Render Pipeline/Lit")
            );
            coloredMaterial.color = shopColor;

            // Back wall
            CreatePrimitive(
                PrimitiveType.Cube, "BackWall", shop.transform,
                new Vector3(0f, 1.75f, 1.1f),
                new Vector3(5f, 3.5f, 0.25f),
                shopMaterial
            );

            // Left and right supporting columns
            for (int side = -1; side <= 1; side += 2)
            {
                CreatePrimitive(
                    PrimitiveType.Cube, "SideColumn", shop.transform,
                    new Vector3(side * 2.45f, 1.75f, 0f),
                    new Vector3(0.2f, 3.5f, 2.4f),
                    shopMaterial
                );
            }

            // Service counter
            CreatePrimitive(
                PrimitiveType.Cube, "ServiceCounter", shop.transform,
                new Vector3(0f, 0.55f, -0.65f),
                new Vector3(4.4f, 1.1f, 0.9f),
                shopMaterial
            );

            // Colored signboard
            CreatePrimitive(
                PrimitiveType.Cube, "Signboard", shop.transform,
                new Vector3(0f, 3.15f, 0f),
                new Vector3(4.8f, 0.65f, 0.3f),
                coloredMaterial
            );

            // Canopy
            CreatePrimitive(
                PrimitiveType.Cube, "Canopy", shop.transform,
                new Vector3(0f, 2.7f, -0.7f),
                new Vector3(5f, 0.12f, 2f),
                coloredMaterial
            );


            // Display food

            ShopFoodConfig config =
                (shopFoods != null && i < shopFoods.Length)
                    ? shopFoods[i]
                    : null;

            if (config != null && config.foodPrefabs != null)
            {
                int foodCount = config.foodPrefabs.Length;

                for (int j = 0; j < foodCount; j++)
                {
                    GameObject prefab = config.foodPrefabs[j];

                    if (prefab == null)
                        continue;

                    // Arrange foods evenly across the counter
                    float spacing = 1.0f;
                    float k = (j - (foodCount - 1) / 2f) * spacing;

                    GameObject food = Instantiate(
                        prefab,
                        shop.transform
                    );

                    food.name = $"DisplayFood_{j + 1}";

                    // Counter height = 1.1
                    food.transform.localPosition =
                        new Vector3(k, 1.1f, -0.65f);

                    food.transform.localRotation = Quaternion.identity;

                    // Preserve the prefab's original scale.
                    // Do not overwrite localScale.
                }
            }



            // Shop name
            GameObject nameObject = new GameObject("ShopName");
            nameObject.transform.SetParent(shop.transform, false);
            nameObject.transform.localPosition =
                new Vector3(0f, 3.15f, -0.19f);

            nameObject.transform.localRotation =
                Quaternion.Euler(0f, 0f, 0f);

            TextMesh nameText = nameObject.AddComponent<TextMesh>();
            nameText.text = shopNames[i % shopNames.Length];
            nameText.fontSize = 64;
            nameText.characterSize = 0.08f;
            nameText.anchor = TextAnchor.MiddleCenter;
            nameText.alignment = TextAlignment.Center;
            nameText.color = Color.white;
        }
    }



    //void GeneratePlants()
    //{
    //    Vector3[] plantPositions =
    //    {
    //        new Vector3(-12f, 0f, -10f),
    //        new Vector3( 12f, 0f, -10f),
    //        new Vector3(-12f, 0f,  10f),
    //        new Vector3( 12f, 0f,  10f),
    //        new Vector3(-12f, 0f,   0f),
    //        new Vector3( 12f, 0f,   0f)
    //    };

    //    for (int i = 0; i < plantPositions.Length; i++)
    //    {
    //        Vector3 position = plantPositions[i];

    //        if (IsNearPortal(transform.position + position))
    //            continue;

    //        CreatePrimitive(
    //            PrimitiveType.Cylinder,
    //            $"Plant_{i + 1}",
    //            generatedRoot,
    //            position + Vector3.up * 0.9f,
    //            new Vector3(0.6f, 0.9f, 0.6f),
    //            plantMaterial
    //        );
    //    }
    //}

    bool IsNearPortal(Vector3 position)
    {
        foreach (Transform portal in portals)
        {
            if (portal == null)
                continue;

            Vector2 a = new Vector2(position.x, position.z);
            Vector2 b = new Vector2(
                portal.position.x,
                portal.position.z
            );

            if (Vector2.Distance(a, b) < portalClearance)
                return true;
        }

        return false;
    }

    GameObject CreatePrimitive(
        PrimitiveType type,
        string objectName,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material
    )
    {
        GameObject obj = GameObject.CreatePrimitive(type);

        obj.name = objectName;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;

        if (material != null)
        {
            obj.GetComponent<Renderer>().material = material;
        }

        return obj;
    }
}
