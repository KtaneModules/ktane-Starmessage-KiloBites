using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MorseData
{
    public static readonly Dictionary<char, string> morseTranslation = new Dictionary<char, string>()
    {
        { 'ア', "--.--" },
        { 'イ', ".-" },
        { 'ウ', "..-" },
        { 'エ', "-.---" },
        { 'オ', ".-..." },
        { 'カ', ".-.." },
        { 'キ', "-.-.." },
        { 'ク', "...-" },
        { 'ケ', "-.--" },
        { 'コ', "-----" },
        { 'サ', "-.-.-" },
        { 'シ', "--.-." },
        { 'セ', ".---." },
        { 'ソ', "---." },
        { 'タ', "-." },
        { 'チ', "..-." },
        { 'ツ', ".--." },
        { 'テ', ".-.--" },
        { 'ト', "..-.." },
        { 'ナ', ".-." },
        { '二', "-.-." },
        { 'ヌ', "...." },
        { 'ネ', "--.-" },
        { 'ノ', "..--" },
        { 'ハ', "-..." },
        { 'ヒ', "--.--" },
        { 'フ', "--.." },
        { 'へ', "." },
        { 'ホ', "-.." },
        { 'マ', "-..-" },
        { 'ミ', "..-.-" },
        { 'ム', "-" },
        { 'メ', "-...-" },
        { 'モ', "-..-." },
        { 'ヤ', ".--" },
        { 'ユ', "-..--" },
        { 'ヨ', "--" },
        { 'ラ', "..." },
        { 'リ', "--." },
        { 'ル', "-.--." },
        { 'レ', "---" },
        { 'ロ', ".-.-" },
        { 'ワ', "-.-" },
        { 'ン', ".-.-." },
        { 'ヲ', ".---" },
    };

    public static readonly Dictionary<char, string> morseTranslationSpecial = new Dictionary<char, string>()
    {
        { 'ボ', "-...." },
        { 'ゾ', "---..." },
        { 'ヅ', ".--..." },
        { 'ビ', "--.--.." },
        { 'ゴ', "-----.." },
        { 'パ', "-.....--." },
        { 'ペ', "...--." }
    };

    public static string generateSequence(char ch)
    {
        string output = string.Empty;

        foreach (char unit in morseTranslation[ch])
        {
            output += unit == '-' ? "xxx" : "x";
            output += ".";
        }

        return output + "..";
    }

}
