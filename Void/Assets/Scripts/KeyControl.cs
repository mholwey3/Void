using UnityEngine;
using System.Collections;

public class KeyControl : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            EventManager.TriggerEvent(messages.pickUpKey);
            gameObject.SetActive(false);
        }
    }
}