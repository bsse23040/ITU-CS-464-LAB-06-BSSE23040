using System.Collections;
using UnityEngine;

/// <summary>
/// Lecture 06's spawner, grown up: enemies arrive in waves that get bigger (2, 3, 4, ...),
/// with a rest between them. A wave is over when its enemies are gone.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int firstWave = 2;
    [SerializeField] int extraPerWave = 1;
    [SerializeField] float spawnGap = 1.2f;
    [SerializeField] float restBetweenWaves = 3f;

    int wave;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(restBetweenWaves);    // a calm start
        while (true)
        {
            wave++;
            int count = firstWave + (wave - 1) * extraPerWave;
            for (int i = 0; i < count; i++)
            {
                Transform at = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefab, at.position, at.rotation, transform);   // children, so childCount counts them
                yield return new WaitForSeconds(spawnGap);
            }
            yield return new WaitUntil(() => transform.childCount == 0);
            yield return new WaitForSeconds(restBetweenWaves);
        }
    }
}
