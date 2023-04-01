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
        { 'ス', "---.-" },
        { 'セ', ".---." },
        { 'ソ', "---." },
        { 'タ', "-." },
        { 'チ', "..-." },
        { 'ツ', ".--." },
        { 'テ', ".-.--" },
        { 'ト', "..-.." },
        { 'ナ', ".-." },
        { 'ニ', "-.-." },
        { 'ヌ', "...." },
        { 'ネ', "--.-" },
        { 'ノ', "..--" },
        { 'ハ', "-..." },
        { 'ヒ', "--..-" },
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
        { 'ー', ".--.-" },
        { 'ヲ', ".---" },
        { 'ボ', "-.. .." },
        { 'ゾ', "---. .." },
        { 'ヅ', ".--. .." },
        { 'ビ', "--..- .." },
        { 'ゴ', "----- .." },
        { 'パ', "-... ..--." },
        { 'ペ', ". ..--." }
    };

    public static string generateSequence(char ch)
    {
        string output = string.Empty;

        foreach (char unit in morseTranslation[ch])
        {
            output += unit == ' ' ? ".." : unit == '-' ? "xxx" : "x";
            output += ".";
        }

        return output + "..";
    }

    public static string generateSequenceInput(char ch)
    {
        string output = string.Empty;

        foreach (char unit in morseTranslation[ch])
        {
            output += unit == '-' ? "-" : ".";
            output += " ";
        }

        return output;
    }

}
