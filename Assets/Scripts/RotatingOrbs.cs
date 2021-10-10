using System.Collections;
using System.Collections.Generic;
using PathCreation;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RotatingOrbs : MonoBehaviour
{
    public GameObject orbPrebaf;
    public PathCreator path;

    [Range(0, 10)]
    public int initialNumberOfOrbs = 0;

    [Range(0f, 5f)]
    public float rotationSpeed = 0.2f;

    [Range(0, 1)]
    public float aPoint, bPoint;

    private Vector3 posA, posB;

    private float offset = 1f;

    private List<GameObject> orbs;

    private void Start() {
        orbs = new List<GameObject>();
        AddOrbs(initialNumberOfOrbs);
    }

    private void Update() {
        offset += rotationSpeed * Time.deltaTime;
        offset %= 1f;
        for (int i = 0; i < orbs.Count; i++)
        {
            float currentTime = (float)i / (float)orbs.Count + offset;
            currentTime %= 1f;
            orbs[i].transform.position = path.path.GetPointAtTime(currentTime);
            if (currentTime >= aPoint && currentTime <= bPoint)
            {
                orbs[i].GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
            else
            {
                orbs[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
    }

    public void AddOrb() {
        orbs.Add(Instantiate(orbPrebaf, path.path.GetPointAtTime(0), Quaternion.identity));

    }

    public void AddOrbs(int numberOfOrbsToAdd) {
        for (int i = 0; i < numberOfOrbsToAdd; i++)
        {
            AddOrb();
        }
    }

    public void RemoveOrb() {
        if (orbs.Count > 0)
        {
            GameObject orb = orbs[0];
            orbs.RemoveAt(0);
            Destroy(orb);
        }
    }
    public void RemoveOrbs(int numberOfOrbsToRemove) {
        for (int i = 0; i < numberOfOrbsToRemove; i++)
        {
            RemoveOrb();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(posA, 0.1f);
        Gizmos.DrawSphere(posB, 0.1f);
    }

    private void OnValidate() {
        posA = path.path.GetPointAtTime(aPoint);
        posB = path.path.GetPointAtTime(bPoint);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RotatingOrbs))]
public class RotatingOrbsInpector : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        RotatingOrbs myScript = (RotatingOrbs)target;
        if (GUILayout.Button("Add orb"))
        {
            myScript.AddOrb();
        }
        if (GUILayout.Button("Remove orb"))
        {
            myScript.RemoveOrb();
        }
    }
}
#endif