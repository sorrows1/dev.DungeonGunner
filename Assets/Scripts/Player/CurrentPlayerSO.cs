using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player")]
public class CurrentPlayerSO : ScriptableObject
{
    [field: SerializeField] public PlayerDetailsSO PlayerDetails {get; set;}
    [field: SerializeField] public string PlayerName {get; set;}
}
