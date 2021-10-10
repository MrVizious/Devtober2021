using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private ParticleSystem particles;

    private void Start() {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnDestroy() {
        particles.Play();
        GameObject newParticles = Instantiate(particles.gameObject, transform.position, Quaternion.identity, null);
        Destroy(newParticles, 1);
    }
}
