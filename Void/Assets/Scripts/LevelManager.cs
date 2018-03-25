using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour 
{
    [SerializeField]
    SpawnEnemies spawnEnemies;
    [SerializeField]
    SpawnKeys spawnKeys;
    [SerializeField]
    PlayerControl playerControl;
    [SerializeField]
    GameObject voidObj;
    [SerializeField]
    GameObject reverbZone;

    [SerializeField]
    AudioClip beatTheLevelClip;

    public List<GameObject> enemies;
    public List<GameObject> keys;

    int currentLevel;

    void Start()
    {
        currentLevel = 1;
        LoadNewLevel(currentLevel);
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.beatTheLevel, DetermineNextLevel);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.beatTheLevel, DetermineNextLevel);
    }

    void DetermineNextLevel()
    {
        StartCoroutine(DetermineNextLevelCo());
    }

    IEnumerator DetermineNextLevelCo()
    {
        playerControl.isLanded = false;
        if (currentLevel < 4)
        {
            currentLevel++;
            WipeOldLevel();
            AudioManager.instance.PlaySingle(false, 1.0f, beatTheLevelClip);
            yield return new WaitForSeconds(beatTheLevelClip.length);
            LoadNewLevel(currentLevel);
        }
        else if (currentLevel == 4)
        {
            EventManager.TriggerEvent(messages.beatTheGame);
        }
    }

    void WipeOldLevel()
    {
        foreach (GameObject enemy in enemies)
            Destroy(enemy);

        foreach (GameObject key in keys)
            Destroy(key);

        enemies.Clear();
        keys.Clear();
    }

    void LoadNewLevel(int lvlNum)
    {
        spawnEnemies.numberOfEnemies = lvlNum * 10;
        spawnEnemies.min = lvlNum * 10f * 0.2f;
        spawnEnemies.max = lvlNum * 10f * 0.8f;

        spawnKeys.numberOfKeys = lvlNum;
        spawnKeys.min = lvlNum * 10f * 0.2f;
        spawnKeys.max = lvlNum * 10f * 0.8f;

        playerControl.numKeys = 0;
        playerControl.currentLevel = lvlNum;

        voidObj.GetComponent<BoxCollider>().size = new Vector3(lvlNum * 100, 1, lvlNum * 100);
        reverbZone.GetComponent<AudioReverbZone>().maxDistance = (lvlNum * 100) + 100;

        EventManager.TriggerEvent(messages.readyToSpawnEnemies);
        EventManager.TriggerEvent(messages.warpToStart);
    }

    public void PlayEntityAudio(bool play)
    {
        foreach (GameObject key in keys)
        {
            key.GetComponent<AudioSource>().enabled = play;
        }

        foreach (GameObject enemy in enemies)
        {
            AudioSource[] sources = enemy.GetComponents<AudioSource>();
            sources[0].enabled = play;
        }
    }
}
