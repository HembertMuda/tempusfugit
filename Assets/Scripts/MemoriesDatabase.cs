using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MemoriesDatabase01", menuName = "Create MemoriesDatabase")]
public class MemoriesDatabase : ScriptableObject
{
    [SerializeField, BoxGroup("Memories")]
    public Dictionary<string, List<string>> memories = new Dictionary<string, List<string>>();
}
