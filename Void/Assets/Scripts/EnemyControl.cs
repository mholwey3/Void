using UnityEngine;
using System.Collections;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] idleClips;
    [SerializeField]
    private AudioClip[] windUpAttackClips;
    [SerializeField]
    private AudioClip[] attackClips;
    [SerializeField]
    private AudioClip[] deadClips;

    public AudioSource idleSource;
    public AudioSource actionSource;

    private bool playerInRange = false;
    private bool canAttack = true;

    void OnEnable()
    {
        EventManager.StartListening(messages.killEnemy, KillEnemy);
        EventManager.StartListening(messages.playerDead, PlayerDead);
        EventManager.StartListening(messages.beatTheGame, BeatTheGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.killEnemy, KillEnemy);
        EventManager.StopListening(messages.playerDead, PlayerDead);
        EventManager.StopListening(messages.beatTheGame, BeatTheGame);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            playerInRange = true;
            StartCoroutine(Attack(col));
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator Attack(Collider col)
    {
        while (playerInRange && canAttack)
        {
            PlayRandomEffect(false, windUpAttackClips);
            yield return new WaitForSeconds(1.0f);
            if (playerInRange && canAttack)
            {
                PlayRandomEffect(false, attackClips);
                EventManager.TriggerEvent(messages.playerTakeDamage);
                yield return new WaitForSeconds(1.0f);
            }
        }
    }

    void KillEnemy()
    {
        if (playerInRange)
        {
            StartCoroutine(KillEnemyCo());
        }
    }

    IEnumerator KillEnemyCo()
    {
        canAttack = false;
        idleSource.Stop();
        PlayRandomEffect(false, deadClips);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
    }

    void PlayerDead()
    {
        canAttack = false;
    }

    void BeatTheGame()
    {
        GetComponent<EnemyControl>().enabled = false;
    }

    public void PlayRandomEffect(bool isLooped, params AudioClip[] clips)
    {
        float lowPitch = .95f;
        float highPitch = 1.05f;
        int randomClip = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitch, highPitch);

        actionSource.clip = clips[randomClip];
        actionSource.pitch = randomPitch;
        actionSource.loop = isLooped;
        actionSource.Play();
    }
}
