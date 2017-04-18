﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainThreadDispatcher : MonoBehaviour {

	private static readonly Queue<Action> _executionQueue = new Queue<Action>();

	public void Update ()
	{
		lock (_executionQueue)
		{
			while (_executionQueue.Count > 0) {
				_executionQueue.Dequeue ().Invoke ();
			}
		}
	}

	public void Enqueue (IEnumerator action)
	{
		lock (_executionQueue)
		{
			_executionQueue.Enqueue (() => {
				StartCoroutine (action);
			});
		}
	}

	public void Enqueue (Action action)
	{
		Enqueue (ActionWrapper (action));
	}

	IEnumerator ActionWrapper (Action a)
	{
		a ();
		yield return null;
	}

	private static MainThreadDispatcher _instance = null;

	public static bool Exists () {
		return _instance != null;
	}

	public static MainThreadDispatcher Instance ()
	{
		if (!Exists ()) {
			throw new Exception ("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}
		return _instance;
	}


	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}

	void OnDestroy () {
		_instance = null;
	}
}