using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip sfx_landing; // Mino地面着地の音
    [SerializeField] AudioClip sfx_rotate; // Mino回転の音
    [SerializeField] AudioClip sfx_delete; // Mino消去の音

    public void PlaySfx(int index)
    {
        AudioClip sfx = null;
        switch (index)
        {
            case 0:
                sfx = sfx_landing;
                break;
            case 1:
                sfx = sfx_rotate;
                break;
            case 2:
                sfx = sfx_delete;
                break;
        }
        audioSource.PlayOneShot(sfx);
    }
}