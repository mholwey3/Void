using UnityEngine;
using System.Collections;

public class SpawnKeys : MonoBehaviour
{
    [SerializeField]
    private GameObject keyObj;
    public int numberOfKeys;

    public float min;
    public float max;

    public int grid;

    private int quadrant;

    private VoidControl voidControl;
    public LevelManager levelManager;

    private float pitch = 1.0f;

    protected void Awake()
    {
        quadrant = 0;
        voidControl = GameObject.FindGameObjectWithTag("Void").GetComponent<VoidControl>();
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.readyToSpawnKeys, ReadyToSpawnKeys);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.readyToSpawnKeys, ReadyToSpawnKeys);
    }

    void Start()
    {
        AudioManager.instance.StopMusic();
    }

    void ReadyToSpawnKeys()
    {
        Spawn(numberOfKeys);
    }

    protected void Spawn(int num)
    {
        pitch = 1f;
        Vector3 pos = Vector3.zero;

        while (num > 0)
        {
            //spawn a key
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
            GameObject clone = (GameObject)Instantiate(keyObj, pos, Quaternion.identity);
            levelManager.keys.Add(clone);
            clone.GetComponent<AudioSource>().pitch = pitch;
            
            num--;
            quadrant = (quadrant + 1) % 4;
            pitch += 0.5f;
        }
    }
}
