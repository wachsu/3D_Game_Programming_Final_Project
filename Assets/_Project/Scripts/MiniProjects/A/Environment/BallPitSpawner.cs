using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPitSpawner : MonoBehaviour
{
    [Header("Ball")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField, Min(1)] private int ballCount = 300;
    [SerializeField, Min(0.01f)] private float ballDiameter = 0.2f;

    [Header("Spawn Area")]
    [SerializeField]
    private Vector3 areaSize =
        new Vector3(5f, 2f, 4f);

    //[SerializeField, Min(0f)] private float spawnGap = 0.01f;
    [SerializeField, Min(1)] private int maxAttemptsPerBall = 200;

    [Header("Physics")]
    [SerializeField] private LayerMask obstacleLayers = ~0;

    [Header("Colors")]
    [SerializeField]
    private Color[] ballColors =
    {
        Color.red,
        Color.yellow,
        Color.blue,
        Color.green,
        Color.magenta
    };

    [Header("Spawn Timing")]
    [SerializeField] private int ballsPerBatch = 20;
    [SerializeField] private float batchInterval = 0.5f;

    //private readonly List<Vector3> spawnPositions = new();

    private IEnumerator Start()
    {
        yield return SpawnBalls();
    }

    private IEnumerator SpawnBalls()
    {
        if (ballPrefab == null || ballDiameter <= 0f)
            yield break;

        float radius = ballDiameter * 0.5f;
        int spawned = 0;

        while (spawned < ballCount)
        {
            int batchTarget = Mathf.Min(
                ballsPerBatch,
                ballCount - spawned
            );

            int batchSpawned = 0;

            for (int i = 0; i < batchTarget; i++)
            {
                bool found = false;
                Vector3 spawnPosition = Vector3.zero;

                for (int attempt = 0; attempt < maxAttemptsPerBall; attempt++)
                {
                    Vector3 localPosition = new Vector3(
                        Random.Range(
                            -areaSize.x / 2f + radius,
                             areaSize.x / 2f - radius
                        ),
                        Random.Range(
                            radius,
                            areaSize.y - radius
                        ),
                        Random.Range(
                            -areaSize.z / 2f + radius,
                             areaSize.z / 2f - radius
                        )
                    );

                    Vector3 candidate = transform.TransformPoint(localPosition);

                    bool blocked = Physics.CheckSphere(
                        candidate,
                        radius * 0.98f,
                        obstacleLayers,
                        QueryTriggerInteraction.Ignore
                    );

                    if (blocked)
                        continue;

                    spawnPosition = candidate;
                    found = true;
                    break;
                }

                if (!found)
                {
                    Debug.LogWarning(
                        $"Cannot find space for ball {spawned + 1}. " +
                        "Increase spawn height or decrease batch size."
                    );
                    yield break;
                }

                GameObject ball = Instantiate(
                    ballPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                ball.transform.localScale =
                    Vector3.one * ballDiameter;

                Renderer ballRenderer = ball.GetComponent<Renderer>();

                if (ballRenderer != null && ballColors.Length > 0)
                {
                    ballRenderer.material.color =
                        ballColors[Random.Range(0, ballColors.Length)];
                }

                Rigidbody rb = ball.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.WakeUp();
                }

                spawned++;
                batchSpawned++;
            }

            yield return new WaitForSeconds(batchInterval);
        }

        Debug.Log($"Finished spawning {spawned} balls.");
    }

    private readonly List<GameObject> spawnedBalls = new();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            new Vector3(0f, areaSize.y * 0.5f, 0f),
            areaSize
        );
    }
}