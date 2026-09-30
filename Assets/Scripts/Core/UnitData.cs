using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string unitName;
    [TextArea(3, 5)] public string description; // Textbox mit Zeilenumbruch
    public string stats; // z.B. "HP: 5 | Dmg: 1"
}