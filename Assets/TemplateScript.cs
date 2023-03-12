using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class TemplateScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake()
    {

		moduleId = moduleIdCounter++;

		/*
		foreach (KMSelectable button in Buttons)
			button.OnInteract() += delegate () { ButtonPress(button); return false };
		*/

		//Button.OnInteract += delegate () { ButtonPress(); return false };

    }

	
	void Start()
    {

    }
	
	
	void Update()
    {

    }

	// Twitch Plays


#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"Use <!{0} foobar> to do something.";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand (string command)
    {
		command = command.Trim().ToUpperInvariant();
		List<string> parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		yield return null;
    }

	IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;
    }


}





