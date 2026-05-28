using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst { get; private set; }

    [SerializeField] private AudioSource AudioSource_BGM;
    [SerializeField] private AudioSource AudioSource_SFX;

    [SerializeField] private AudioClip _buttonClickSFX;
    [SerializeField] private AudioClip _mainBGM;

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        if (AudioSource_SFX == null)
        {
            return;
        }

        AudioSource_SFX.PlayOneShot(audioClip);
    }

    public void PlayBGM(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        if (AudioSource_BGM == null)
        {
            return;
        }

        AudioSource_BGM.clip = audioClip;
        AudioSource_BGM.Play();
    }

    public void PlayMainBGM()
    {
        PlayBGM(_mainBGM);
    }

    public void PlayButtonClickSFX()
    {
        PlaySFX(_buttonClickSFX);
    }
}
