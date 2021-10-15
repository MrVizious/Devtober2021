using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ArmAiming : MonoBehaviour
{
    public float minAngle = -75f, maxAngle = 75f;
    [Range(-75, 75)]
    public float aimAngle = 0f;
    public float minForce = 1f, maxForce = 30f;
    [Range(-30, 30)]
    public float force = 1f;
    private Vector2 deltaInput = Vector2.zero;



    [Range(2, 500)]
    public int numberOfPoints;
    [Range(0.5f, 10f)]
    public float previewTime = 4f;

    public LayerMask obstacleLayers;

    public GameObject bullet;
    public LineRenderer line;

    public float angleChange = 5f, forceChange = 10f;

    private void Update() {
        Cursor.lockState = CursorLockMode.Locked;
        AdjustAngle();
        DrawLine();
    }

    private void OnValidate() {
        DrawLine();
    }

    private void DrawLine() {
        transform.right = new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
        Vector3[] newPoints = GetParabolaPoints();
        line.positionCount = newPoints.Length;
        line.SetPositions(newPoints);
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


            RaycastHit2D hit = Physics2D.Raycast((Vector2)newPoint, currentVelocity.normalized, 0.01f, obstacleLayers);
            if (hit.collider != null)
            {
                break;
            }
        }
        return points.ToArray();
    }

    public void InputThrow(InputAction.CallbackContext context) {
        if (context.started)
        {
            Throw();
        }
    }
    public void Throw() {
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        Vector2 angle = new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
        newBullet.GetComponent<Rigidbody2D>().velocity = angle * force;
    }

    public void InputAngle(InputAction.CallbackContext context) {
        deltaInput = context.ReadValue<Vector2>().normalized;
    }

    private void AdjustAngle() {
        force = force + deltaInput.x * forceChange * Time.deltaTime;
        if (force < minForce && force > -minForce)
        {
            Debug.Log("Angle change");
            transform.localPosition = new Vector2(Mathf.Abs(transform.localPosition.x) * Mathf.Sign(-force), transform.localPosition.y);
            GetComponent<SpriteRenderer>().flipX = force > 0f;
            force = -minForce * Mathf.Sign(force);
            aimAngle = -aimAngle;
        }
        force = Mathf.Clamp(force, -maxForce, maxForce);
        aimAngle = aimAngle + deltaInput.y * angleChange * Time.deltaTime * force;
        //aimAngle = Mathf.Clamp(aimAngle + deltaInput.y * angleChange * Time.deltaTime * force, minAngle, maxAngle);
        aimAngle %= 360;
        aimAngle = Mathf.Clamp(aimAngle, minAngle, maxAngle);
    }

}


#if UNITY_EDITOR
[CustomEditor(typeof(ArmAiming))]
public class ArmAimingInpector : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ArmAiming myScript = (ArmAiming)target;
        if (GUILayout.Button("Thow"))
        {
            myScript.Throw();
        }
    }
}
#endif