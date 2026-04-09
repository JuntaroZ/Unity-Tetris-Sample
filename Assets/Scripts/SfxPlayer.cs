using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public enum SfxType
    {
        Landing,
        Rotate,
        Delete,
        AddScore,
        AddScoreMax
    }

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip sfx_landing; // Mino地面着地の音
    [SerializeField] AudioClip sfx_rotate; // Mino回転の音
    [SerializeField] AudioClip sfx_delete; // Mino消去の音
    [SerializeField] AudioClip sfx_add_score; // スコア加算の音
    [SerializeField] AudioClip sfx_add_score_max; // スコア加算の音（最大値）
    [SerializeField] AudioClip sfx_game_over; // ゲームオーバーの音

    public void PlaySfx(SfxType sfxType)
    {
        AudioClip sfx = null;
        switch (sfxType)
        {
            case SfxType.Landing:
                sfx = sfx_landing;
                break;
            case SfxType.Rotate:
                sfx = sfx_rotate;
                break;
            case SfxType.Delete:
                sfx = sfx_delete;
                break;
            case SfxType.AddScore:
                sfx = sfx_add_score;
                break;
            case SfxType.AddScoreMax:
                sfx = sfx_add_score_max;
                break;
        }
        audioSource.PlayOneShot(sfx);
    }
}