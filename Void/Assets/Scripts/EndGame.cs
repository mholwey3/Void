using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private AudioClip bgMusic;

    void Awake()
    {
        AudioManager.instance.PlayNewMusic(bgMusic);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Application.LoadLevel(0);
        }
    }
}
