using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public enum messages
{
    readyToSpawnEnemies,
    readyToSpawnKeys,
	playerTakeDamage,
    killEnemy,
    pickUpKey,
    unlockVoid,
    warpToStart,
    playerDead,
    beatTheLevel,
    beatTheGame
};

public class EventManager : MonoBehaviour {

	public Dictionary <int, UnityEvent> eventDictionary;	
	private static EventManager eventManager;
	
	public static EventManager instance
	{
		get
		{
			if (!eventManager)
			{
				eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;
				
				if (!eventManager)
				{
					Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
				}
				else
				{
					eventManager.Init (); 
				}
			}
			
			return eventManager;
		}
	}
	
	void Init ()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<int, UnityEvent>();
		}
	}
	
	public static void StartListening (messages eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue ((int)eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add ((int)eventName, thisEvent);
		}
	}
	
	public static void StopListening (messages eventName, UnityAction listener)
	{
		if (eventManager == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue ((int)eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	public static void TriggerEvent (messages eventName)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue ((int)eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}
}