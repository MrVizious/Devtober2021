using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public bool isAiming = false;
    public int maxNumberOfOrbs = 0;
    private void Awake() {
        isAiming = false;
    }
}
