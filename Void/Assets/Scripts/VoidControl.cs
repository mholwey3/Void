using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoidControl : MonoBehaviour
{
    [SerializeField]
    private AudioClip warpClip;
    [SerializeField]
    private AudioClip enterVoidClip;
    [SerializeField]
    private AudioClip endWarpClip;
    private bool locked = true;

    public List<Vector3> usedPositions;

    void Awake()
    {
        usedPositions = new List<Vector3>();
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.beatTheGame, BeatTheGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.beatTheGame, BeatTheGame);
    }

    void Start()
    {
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            AudioManager.instance.PlaySingle(false, 1.0f, warpClip);
            EventManager.TriggerEvent(messages.warpToStart);
        }
    }

    void BeatTheGame()
    {
        StartCoroutine(BeatTheGameCo());
    }

    IEnumerator BeatTheGameCo()
    {
        AudioManager.instance.PlayRandomEffect(false, endWarpClip);
        yield return new WaitForSeconds(endWarpClip.length);
        Application.LoadLevel(1);
    }

    public bool IsPositionUnique(Vector3 position)
    {
        for (int i = 0; i < usedPositions.Count; i++)
        {
            if (position == usedPositions[i])
                return false;
        }

        return true;
    }
}
