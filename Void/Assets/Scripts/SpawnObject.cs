using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnObject : MonoBehaviour
{
    [SerializeField]
    protected GameObject objectToClone;

    [SerializeField]
    protected int min;
    [SerializeField]
    protected int max;

    protected int grid;

    protected int quadrant;

    protected VoidControl voidControl;

    protected void Awake()
    {
        quadrant = 0;
        voidControl = GameObject.FindGameObjectWithTag("Void").GetComponent<VoidControl>();
    }

    protected virtual void Start()
    {
        AudioManager.instance.StopMusic();
    }

    protected virtual void Spawn(int num)
    {
        Vector3 pos = Vector3.zero;

        while (num > 0)
        {
            //spawn an object
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
            } while (!voidControl.IsPositionUnique(pos));

            voidControl.usedPositions.Add(pos);
            GameObject clone = (GameObject)Instantiate(objectToClone, pos, Quaternion.identity);
            num--;
            quadrant = (quadrant + 1) % 4;
        }
    }
}
