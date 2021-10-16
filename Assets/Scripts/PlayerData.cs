using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public int maxNumberOfOrbs = 0, currentNumberOfOrbs;
    public UnityEvent onOrbRemoved, onOrbAdded;
    public bool facingRight = true;


    public bool isAiming = true;



    private void OnEnable() {
        currentNumberOfOrbs = maxNumberOfOrbs;
    }

    public bool RemoveOrb() {
        return RemoveOrbs(1);
    }
    public bool RemoveOrbs(int numberToRemove) {
        if (currentNumberOfOrbs >= numberToRemove)
        {
            currentNumberOfOrbs -= numberToRemove;
            onOrbRemoved.Invoke();
            return true;
        }
        return false;
    }

    public bool AddOrb() {
        return AddOrbs(1);
    }
    public bool AddOrbs(int numberToAdd) {
        if (currentNumberOfOrbs + numberToAdd <= maxNumberOfOrbs)
        {
            currentNumberOfOrbs += numberToAdd;
            onOrbAdded.Invoke();
            return true;
        }
        return false;
    }
}