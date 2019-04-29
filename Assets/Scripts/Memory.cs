using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Memory
{
    [SerializeField] public string Name;

    [SerializeField, TextArea] public string ChoiceText;

    [SerializeField, TextArea] public List<string> Sentences = new List<string>();
}
