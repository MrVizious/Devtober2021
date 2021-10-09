using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAiming : MonoBehaviour
{
    [Range(-75, 75)]
    public float aimAngle = 0f;
    [Range(1, 30)]
    public float force = 1f;
    [Range(2, 150)]
    public int numberOfPoints;
    [Range(0.5f, 10f)]
    public float previewTime = 4f;

    public LayerMask obstacleLayers;

    private void OnValidate() {
        transform.right = new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
        Vector3[] newPoints = GetParabolaPoints();
        GetComponent<LineRenderer>().positionCount = newPoints.Length;
        GetComponent<LineRenderer>().SetPositions(newPoints);
    }

    private Vector3[] GetParabolaPoints() {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= numberOfPoints; i++)
        {
            float instant = i * previewTime / numberOfPoints;
            float y = transform.position.y + Mathf.Sin(aimAngle * Mathf.Deg2Rad) * force * instant + (Physics2D.gravity.y * instant * instant) / 2;
            float x = transform.position.x + Mathf.Cos(aimAngle * Mathf.Deg2Rad) * force * instant;
            Vector3 newPoint = new Vector3(x, y, 0);
            points.Add(newPoint);
            Vector2 angle = new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
            Vector2 currentVelocity = force * angle + Vector2.up * Physics2D.gravity.y * instant;


            RaycastHit2D hit = Physics2D.Raycast((Vector2)newPoint, currentVelocity, 0.01f, obstacleLayers);
            if (hit.collider != null)
            {
                break;
            }
        }
        return points.ToArray();
    }

}
