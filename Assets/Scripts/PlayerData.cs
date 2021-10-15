using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public int maxNumberOfOrbs = 0;
    public bool facingRight = true;


    public bool isAiming = true;



    private void Awake() {
        isAiming = false;
    }
}
