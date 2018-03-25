using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private AudioClip keyShimmerClip;

    public int numberOfEnemies;
    public int grid;

    private List<Vector3> usedPositions;

    void Awake()
    {
        AudioManager.instance.StopMusic();
        usedPositions = new List<Vector3>();
        Spawn(numberOfEnemies);
    }

    void Spawn(int num)
    {
        int min = 5;
        int max = 50;
        int quadrant;
        int keyHolder = Random.Range(1, num + 1);
        Vector3 pos = Vector3.zero;

        while (num > 0)
        {
            //spawn an enemy
            quadrant = Random.Range(0, 4);
            do
            {
                if (quadrant == 0)
                    pos = new Vector3(Random.Range(min, max) * grid, 0, Random.Range(min, max) * grid);
                else if (quadrant == 1)
                    pos = new Vector3(-Random.Range(min, max) * grid, 0, Random.Range(min, max) * grid);
                else if (quadrant == 2)
                    pos = new Vector3(Random.Range(min, max) * grid, 0, -Random.Range(min, max) * grid);
                else if (quadrant == 3)
                    pos = new Vector3(-Random.Range(min, max) * grid, 0, -Random.Range(min, max) * grid);
            } while (!IsPositionUnique(pos));

            usedPositions.Add(pos);
            GameObject clone = (GameObject)Instantiate(enemy, pos, Quaternion.identity);
            if (keyHolder == num)
            {
                EnemyControl eControl = clone.GetComponent<EnemyControl>();
                eControl.hasKey = true;
                eControl.idleSource.clip = keyShimmerClip;
                eControl.idleSource.volume = 0.5f;
                eControl.idleSource.Play();
            }
            num--;
        }
    }

    bool IsPositionUnique(Vector3 position)
    {
        for (int i = 0; i < usedPositions.Count; i++)
        {
            if (position == usedPositions[i])
                return false;
        }

        return true;
    }
}
