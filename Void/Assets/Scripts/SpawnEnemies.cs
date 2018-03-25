using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyObj;
    public int numberOfEnemies;

    public float min;
    public float max;

    public int grid;
    private int quadrant;

    private VoidControl voidControl;
    public LevelManager levelManager;

    protected void Awake()
    {
        quadrant = 0;
        voidControl = GameObject.FindGameObjectWithTag("Void").GetComponent<VoidControl>();
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.readyToSpawnEnemies, ReadyToSpawnEnemies);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.readyToSpawnEnemies, ReadyToSpawnEnemies);
    }

    void Start()
    {

    }

    void ReadyToSpawnEnemies()
    {
        Spawn(numberOfEnemies);
    }

    protected void Spawn(int num)
    {
        Vector3 pos = Vector3.zero;

        while (num > 0)
        {
            //spawn an enemy
            do
            {
                if (quadrant == 0)
                    pos = new Vector3(Random.Range((int)min, (int)max) * grid, 0, Random.Range((int)min, (int)max) * grid);
                else if (quadrant == 1)
                    pos = new Vector3(-Random.Range((int)min, (int)max) * grid, 0, Random.Range((int)min, (int)max) * grid);
                else if (quadrant == 2)
                    pos = new Vector3(Random.Range((int)min, (int)max) * grid, 0, -Random.Range((int)min, (int)max) * grid);
                else if (quadrant == 3)
                    pos = new Vector3(-Random.Range((int)min, (int)max) * grid, 0, -Random.Range((int)min, (int)max) * grid);
            } while (!voidControl.IsPositionUnique(pos));

            voidControl.usedPositions.Add(pos);
            GameObject clone = (GameObject)Instantiate(enemyObj, pos, Quaternion.identity);
            levelManager.enemies.Add(clone);
            num--;
            quadrant = (quadrant + 1) % 4;
        }

        EventManager.TriggerEvent(messages.readyToSpawnKeys);
    }
}
