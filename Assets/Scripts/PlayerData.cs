using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public int maxNumberOfOrbs = 0, currentNumberOfOrbs;
    public UnityEvent onOrbRemoved, onOrbAdded;
    public float playerBrightness = 1f;
    public float brightnessThreshold = 0.61f;
    public UnityEvent onBrightnessDown, onBrightnessUp;


    private void OnEnable() {
        currentNumberOfOrbs = maxNumberOfOrbs;
        playerBrightness = 1f;
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

    public void SetPlayerBrightness(float newLevel) {
        if (playerBrightness < brightnessThreshold && newLevel >= brightnessThreshold)
        {
            onBrightnessUp.Invoke();
            Debug.Log("Brightness up!");
        }
        else if (playerBrightness >= brightnessThreshold && newLevel < brightnessThreshold)
        {
            onBrightnessDown.Invoke();
            Debug.Log("Brightness down!");
        }
        playerBrightness = newLevel;
    }
}