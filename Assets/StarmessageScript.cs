using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class StarmessageScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;

	public KMSelectable[] buttons;

	public GameObject[] stars;
	public GameObject timeBar;
	public GameObject[] morseObj;

	public Material unlit, lit;
	public MeshRenderer[] starLEDS;
	public MeshRenderer timerBarLED;
	public MeshRenderer moon;
	public MeshRenderer[] buttonObj;
	public TextMesh[] buttonText;
	public Material solveGreen;

	private float[] offsets = new float[4];
	private bool[] rotation = new bool[4];
	float[] starSpeed = { 1, 1, 1, 1 };

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private static readonly Vector3 origTimer = new Vector3(0.095f, 1, 0.04f);

	private static readonly Vector3[] dotCoords = { new Vector3(0.035f, 0.01984f, -0.065f), new Vector3(0.065f, 0.01984f, -0.035f) };
	private static readonly Vector3[] dashCoords = { new Vector3(0.035f, 0.02f, -0.065f), new Vector3(0.065f, 0.02f, -0.035f) };
	private bool flipped;

	

	private static readonly int[,] digitTable =
	{
		{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
		{ 1, 0, 1, 2, 3, 4, 5, 6, 7, 8 },
		{ 2, 1, 0, 1, 2, 3, 4, 5, 6, 7 },
		{ 3, 2, 1, 0, 1, 2, 3, 4, 5, 6 },
		{ 4, 3, 2, 1, 0, 1, 2, 3, 4, 5 },
		{ 5, 4, 3, 2, 1, 0, 1, 2, 3, 4 },
		{ 6, 5, 4, 3, 2, 1, 0, 1, 2, 3 },
		{ 7, 6, 5, 4, 3, 2, 1, 0, 1, 2 },
		{ 8, 7, 6, 5, 4, 3, 2, 1, 0, 1 },
		{ 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
	};

	private static readonly string[] wordList = { "ホシボシ", "シンゾウ", "クラヤミ", "シンパク", "カンケイ", "クチヅケ", "スペース", "セイウン", "テンゴク", "コイビト" };

	private int selectedValue;
	private string selectedWord;

	private Coroutine[] flashes = new Coroutine[4];
	private string[] morseTranslation = new string[4];
	private int[] pointers = new int[4];
	private float flashSpeed = 0.25f;
	private Coroutine timer, striked;

	private int stage = 0;

	private List<string> numbersToAdd = new List<string>();

	private string input = "";
    char[] japanese = new char[3];

    string morseToBinary(string input)
	{
		string output = string.Empty;

		for (int i = 0; i < input.Length; i++)
		{
			output += input[i] == '-' ? "1" : input[i] == '.' ? "0" : "";
		}
		
		return output;
	}

	string binaryToMorse(string input)
	{
		string output = string.Empty;

		for (int i = 0; i < input.Length; i++)
		{
			output += input[i] == '1' ? "-" : ".";
		}

		return output;
	}

	char japCharSetup(int ix, int alt)
	{
		switch (alt)
		{
			case 0:
				switch (ix)
				{
					case 0:
						return 'テ';
					case 1:
						return 'ム';
					case 2:
						return 'ロ';
					case 3:
						return 'レ';
					case 4:
						return 'メ';
					case 5:
						return 'チ';
					case 6:
						return 'タ';
					case 7:
						return 'ル';
					case 8:
						return 'ツ';
					case 9:
						return 'ソ';

                }
				break;
			case 1:
				switch (ix)
				{
					case 0:
						return 'ア';
					case 1:
						return 'ン';
					case 2:
						return 'ク';
					case 3:
						return 'シ';
					case 4:
						return 'ラ';
					case 5:
						return 'ケ';
					case 6:
						return 'コ';
					case 7:
						return 'セ';
					case 8:
						return 'ト';
					case 9:
						return 'カ';

                }
				break;
		}

		return '?';
	}






    void Awake()
    {

		moduleId = moduleIdCounter++;


		foreach (KMSelectable button in buttons)
		{
			button.OnInteract += delegate () { buttonPress(button); return false; };
		}

    }

	
	void Start()
    {
		for (int i = 0; i < 4; i++)
		{
			rotation[i] = rnd.Range(0, 2) == 1;
		}
		calculatingStuff();
		StartCoroutine(flicker());
    }

	void calculatingStuff()
	{
		int ix = rnd.Range(0, wordList.Count());
		selectedWord = wordList[ix];
		selectedValue = digitTable[ix, Bomb.GetSerialNumberNumbers().Last()];

		Debug.LogFormat("[Starmessage #{0}] The selected word is: {1}", moduleId, selectedWord);
		Debug.LogFormat("[Starmessage #{0}] Therefore, row {1}, column {2} is: {3}", moduleId, ix, Bomb.GetSerialNumberNumbers().Last(), selectedValue);

		string modified = string.Empty;

		var shuffleThing = new List<int>() { 0, 1, 2, 3 };

		shuffleThing.Shuffle();

		for (int i = 0; i < 4; i++)
		{
			modified += selectedWord[shuffleThing[i]];
		}

		Debug.LogFormat("[Starmessage #{0}] After scrambling the characters: {1}", moduleId, modified);


		for (int i = 0; i < 4; i++)
		{
			morseTranslation[i] = MorseData.generateSequence(modified[i]);
			pointers[i] = 0;
			flashes[i] = StartCoroutine(morseFlash(i));
		}

		flipped = rnd.Range(0, 2) == 1;

		morseObj[0].transform.localPosition = flipped ? dotCoords[1] : dotCoords[0];
		morseObj[1].transform.localPosition = flipped ? dashCoords[0] : dashCoords[1];

		Debug.LogFormat("[Starmessage #{0}] The - button is on the {1}. Therefore, start on the {1} column of the cell on index {2}.", moduleId, flipped ? "left" : "right", selectedValue);

		int currentValue = selectedValue;
		bool inv = flipped;
		

		for (int i = 0; i < 3; i++)
		{
			japanese[i] = japCharSetup(currentValue, inv ? 0 : 1);
			currentValue++;
			currentValue %= 10;
			inv = !inv;
			numbersToAdd.Add(morseToBinary(MorseData.generateSequenceInput(japanese[i])));
		}

		Debug.LogFormat("[Starmessage #{0}] The following characters to input in morse are: {1}", moduleId, japanese.Join(", "));
    }

	void strikeLog(string reason)
	{
        StopCoroutine(timer);
        Module.HandleStrike();
		input = string.Empty;
		stage = 0;
		timeBar.transform.localScale = origTimer;
		timerBarLED.material = unlit;
		timer = null;
		Debug.LogFormat("[Starmessage #{0}] {1} Strike!", moduleId, reason);
		striked = StartCoroutine(strikeCooldown());
	}

	IEnumerator morseFlash(int ix)
	{
		yield return null;

		while (true)
		{
			starLEDS[ix].material = morseTranslation[ix][pointers[ix]] == 'x' ? lit : unlit;
			yield return new WaitForSeconds(flashSpeed);
			pointers[ix]++;
			pointers[ix] %= morseTranslation[ix].Length;
		}
	}

	IEnumerator flicker()
	{
		yield return null;

		float flickerRange;

		while (true)
		{
			flickerRange = rnd.Range(0.3f, 0.8f);
			moon.material.color = new Color(flickerRange, flickerRange, flickerRange, 1);
			yield return new WaitForSeconds(rnd.Range(0.08f, 0.15f));
		}
	}

	IEnumerator strikeCooldown()
	{
		var duration = 10f;
		var elapsed = 0f;

		while (elapsed <= duration)
		{
			yield return null;
			elapsed += Time.deltaTime;
		}

		striked = null;
	}

	void buttonPress(KMSelectable button)
	{
		button.AddInteractionPunch(0.4f);

		if (moduleSolved || striked != null || stage == 3)
		{
			if (moduleSolved || striked != null)
			{
				Audio.PlaySoundAtTransform("NoPress", transform);
			}
			else
			{
				return;
			}
			return;
		}

		for (int i = 0; i < 2; i++)
		{
			if (button == buttons[i])
			{
                if (timer == null)
                {
                    timer = StartCoroutine(startTimer());
                }

                switch (i)
				{
					case 0:
						Audio.PlaySoundAtTransform(flipped ? "Dash" : "Dot", transform);
						if (input.Length != numbersToAdd[stage].Length)
						{
							input += flipped ? "1" : "0";
						}
						break;
					case 1:
						Audio.PlaySoundAtTransform(flipped ? "Dot" : "Dash", transform);
						if (input.Length != numbersToAdd[stage].Length)
						{
							input += flipped ? "0" : "1";
						}
						break;
				}


				if (input.Length == numbersToAdd[stage].Length)
				{
                    StopCoroutine(timer);

                    if (input.Equals(numbersToAdd[stage]))
					{
                        Audio.PlaySoundAtTransform("Click", transform);
                        if (stage < 2)
						{
							stage++;
							timer = StartCoroutine(startTimer());
						}
						else
						{
							StartCoroutine(solveAnimation());
							Debug.LogFormat("[Starmessage #{0}] All inputs are correct! Solved!", moduleId);
						}
					}
                    else
                    {
                        string expected = binaryToMorse(morseToBinary(MorseData.generateSequenceInput(japanese[stage])));
                        string inputted = binaryToMorse(input);
                        stage = 0;

                        strikeLog(string.Format("Expected input is: {0}, but inputted {1} instead!", expected, inputted));
                    }

                    input = string.Empty;
				}
			}
		}
	}

	IEnumerator startTimer()
	{

		if (timeBar.transform.localScale != origTimer)
		{
			timeBar.transform.localScale = origTimer;
		}

		float current = 0, max = 4;

		timerBarLED.material = lit;

		while (current <= max)
		{
			yield return null;
			var invert = Mathf.Max(0, Mathf.Min(max - current, max)) / max;

			timeBar.transform.localScale = new Vector3(origTimer.x * invert, timeBar.transform.localScale.y, timeBar.transform.localScale.z);
			current += Time.deltaTime;
		}
        strikeLog("The time has ran out!");

    }

	IEnumerator solveAnimation()
	{
		yield return null;

		timeBar.transform.localScale = origTimer;
		timerBarLED.material = unlit;

		var snareCount = 0;

		Audio.PlaySoundAtTransform("Solve", transform);
		for (int i = 0; i < 4; i++)
		{
            StopCoroutine(flashes[i]);
            starLEDS[i].material = unlit;
		}
		for (int i = 0; i < 3; i++)
		{
			flashes[i] = StartCoroutine(slowDown(i));
			starLEDS[i].material = lit;
			yield return new WaitForSeconds(0.5f);
		}
		flashes[3] = StartCoroutine(slowDown(3));

		while (snareCount != 4)
		{
			for (int i = 0; i < 4; i++)
			{
				starLEDS[i].material = lit;
			}
			yield return new WaitForSeconds(0.0625f);
			for (int i = 0; i < 4; i++)
			{
				starLEDS[i].material = unlit;
			}
			yield return new WaitForSeconds(0.0625f);
			snareCount++;
		}
		for (int i = 0; i < 4; i++)
		{
			starLEDS[i].material = lit;
		}
		yield return new WaitForSeconds(0.125f);
		for (int i = 0; i < 4; i++)
		{
			starLEDS[i].material = unlit;
		}
		yield return new WaitForSeconds(0.375f);

		int firstIx = 0;

		while (firstIx != 1)
		{
            for (int i = 0; i < 4; i++)
            {
                starLEDS[i].material = i == 0 || i == 3 ? lit : unlit;
            }
			yield return new WaitForSeconds(0.25f);
            for (int i = 0; i < 4; i++)
            {
                starLEDS[i].material = unlit;
            }
            yield return new WaitForSeconds(0.25f);
            firstIx++;
        }
        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = i == 0 || i == 3 ? lit : unlit;
        }
		yield return new WaitForSeconds(0.250f);
		for (int i = 0; i < 4; i++)
		{
			starLEDS[i].material = i == 1 | i == 2 ? lit : unlit;
		}
		yield return new WaitForSeconds(0.250f);
		for (int i = 0; i < 4; i++)
		{
			starLEDS[i].material = unlit;
		}
        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = lit;
			yield return new WaitForSeconds(0.0625f);
            starLEDS[i].material = unlit;
            yield return new WaitForSeconds(0.0625f);
        }
        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = lit;
        }

        int finalIx = 0;

		while (finalIx != 2)
		{
			for (int i = 3; i > 0; i--)
			{
				starLEDS[i].material = unlit;
				yield return new WaitForSeconds(0.0625f);
				starLEDS[i].material = lit;
                yield return new WaitForSeconds(0.0625f);
            }
			finalIx++;
		}

        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = unlit;
        }
		yield return new WaitForSeconds(0.250f);
		char gg = 'G';

        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = i == 1 || i == 2 ? solveGreen : unlit;
        }
		morseObj[flipped ? 1 : 0].SetActive(false);
		buttonObj[0].material = solveGreen;
		buttonText[0].text = gg.ToString();
		yield return new WaitForSeconds(0.250f);
		for (int i = 0; i < 4; i++)
		{
			starLEDS[i].material = solveGreen;
		}
		morseObj[flipped ? 0 : 1].SetActive(false);
		buttonObj[1].material = solveGreen;
		buttonText[1].text = gg.ToString();
		yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            starLEDS[i].material = unlit;
        }
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
		moduleSolved = true;
		Module.HandlePass();

    }

	IEnumerator slowDown(int ix)
	{
		while (starSpeed[ix] > 0)
		{
			yield return null;
			starSpeed[ix] -= 0.33f * Time.deltaTime;
		}
		starSpeed[ix] = 0;
	}
	
	
	void Update()
    {

		if (moduleSolved)
		{
			return;
		}

		for (int i = 0; i < 4; i++)
		{
			offsets[i] = Time.deltaTime * 35 * starSpeed[i];

			if (!rotation[i])
			{
				offsets[i] *= -1;
			}

			stars[i].transform.localEulerAngles += offsets[i] * Vector3.up;
		}
    }

	// Twitch Plays


#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} .- inputs the morse code.";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand (string command)
    {
		yield return null;

		if (striked != null)
		{
			yield return "sendtochaterror This module is on cooldown! Please wait after 10 seconds to reinput!";
			yield break;
		}

		if (!command.Any(x => ".-".Contains(x)))
		{
			yield return "sendtochaterror Please reinput your morse string!";
			yield break;
		}
		else if (command.Length > 5)
		{
			yield return "sendtochatterror Morse length cannot go over 5!";
			yield break;
		}

		int[] numsToPress = command.Select(x => flipped ? "-.".IndexOf(x) : ".-".IndexOf(x)).ToArray();

		for (int i = 0; i < numsToPress.Length; i++)
		{
			if (striked != null)
			{
				yield return "strike";
				yield break;
			}
            buttons[numsToPress[i]].OnInteract();
			yield return new WaitForSeconds(0.1f);
        }
    }

	IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;

		while (striked != null)
		{
			yield return true;
		}

		var ix = stage;

		if (input.Length != 0)
		{
			input = string.Empty;
			StopCoroutine(timer);
			timer = StartCoroutine(startTimer());
		}

		while (ix != 3)
		{
			var answer = numbersToAdd[ix].Select(x => flipped ? "10".IndexOf(x) : "01".IndexOf(x)).ToArray();
			for (int i = 0; i < answer.Length; i++)
			{
				buttons[answer[i]].OnInteract();
				yield return new WaitForSeconds(0.1f);
			}
			ix++;
		}

		while (!moduleSolved)
		{
			yield return true;
		}

    }


}





