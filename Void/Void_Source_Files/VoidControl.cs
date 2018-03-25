using UnityEngine;
using System.Collections;

public class VoidControl : MonoBehaviour
{
    [SerializeField]
    private AudioClip unlockClip;
    [SerializeField]
    private AudioClip warpClip;
    [SerializeField]
    private AudioClip endWarpClip;
    private bool locked = true;

    void OnEnable()
    {
        EventManager.StartListening(messages.unlockVoid, UnlockVoid);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.unlockVoid, UnlockVoid);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals("Player") && !locked)
        {
            StartCoroutine(GameComplete());
        }
        else
        {
            AudioManager.instance.PlayRandomEffect(false, warpClip);
            col.transform.position = Vector3.zero;
        }
    }

    IEnumerator GameComplete()
    {
        EventManager.TriggerEvent(messages.beatTheGame);
        AudioManager.instance.PlayRandomEffect(false, endWarpClip);
        yield return new WaitForSeconds(endWarpClip.length);
        Application.LoadLevel(2);
    }

    void UnlockVoid()
    {
        locked = false;
        AudioManager.instance.PlayRandomEffect(false, unlockClip);
    }
}
