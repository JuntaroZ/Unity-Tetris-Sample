using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] ParticleSystem myParticleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play(Vector3 position)
    {
        var ps = Instantiate(myParticleSystem, position, Quaternion.identity);
        ps.gameObject.SetActive(true);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);       
    }
}
