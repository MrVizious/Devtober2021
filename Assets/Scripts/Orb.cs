using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public bool thrown = false;
    [Range(0, 5)]
    public float floatingHeight = 1f;
    [Range(0, 1000)]
    public float buoyancy;
    public float scale;
    private Rigidbody2D rb;
    private ParticleSystem particles;

    private void Start() {
        particles = GetComponentInChildren<ParticleSystem>();
        if (thrown)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate() {
        if (thrown && Mathf.Abs(rb.velocity.x) <= Mathf.Epsilon)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, floatingHeight, ~(1 << LayerMask.NameToLayer("Orb")));

            if (hit.collider != null && hit.distance < floatingHeight)
            {
                Debug.Log(hit.collider.gameObject.name);
                float proportionalHeight = (floatingHeight - hit.distance) / floatingHeight;
                Vector2 appliedHoverForce = Vector3.up * (proportionalHeight * buoyancy - Mathf.PerlinNoise(0f, Time.time + transform.position.x));
                rb.drag = Mathf.PerlinNoise(0f, Time.time + transform.position.x) * 0.4f;
                rb.AddForce(appliedHoverForce, ForceMode2D.Force);
            }
            else
            {
                rb.drag = 0f;
            }
        }
    }

    private void OnDestroy() {
        particles.Play();
        GameObject newParticles = Instantiate(particles.gameObject, transform.position, Quaternion.identity, null);
        Destroy(newParticles, 1);
    }

}