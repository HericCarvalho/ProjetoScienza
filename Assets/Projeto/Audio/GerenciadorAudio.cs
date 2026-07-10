using UnityEngine;
using UnityEngine.Audio;

public class GerenciadorAudio : MonoBehaviour
{
    public static GerenciadorAudio Instancia { get; private set; }

    [Header("Configuração do Mixer")]
    [SerializeField] private AudioMixer mixerGlobal;

    [Header("Fontes de Áudio Fixas")]
    [SerializeField] private AudioSource fonteMusica;
    [SerializeField] private AudioSource fonteAmbiente;
    [SerializeField] private AudioSource fonteSFX2D;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TocarMusica(AudioClip musica, bool emLoop = true)
    {
        if (fonteMusica == null || musica == null) return;

        if (fonteMusica.clip == musica && fonteMusica.isPlaying) return;

        fonteMusica.clip = musica;
        fonteMusica.loop = emLoop;
        fonteMusica.Play();
    }

    public void TocarAmbiente(AudioClip somAmbiente, bool emLoop = true)
    {
        if (fonteAmbiente == null || somAmbiente == null) return;

        if (fonteAmbiente.clip == somAmbiente && fonteAmbiente.isPlaying) return;

        fonteAmbiente.clip = somAmbiente;
        fonteAmbiente.loop = emLoop;
        fonteAmbiente.Play();
    }

    public void TocarSFXGlobal(AudioClip sfx)
    {
        if (fonteSFX2D == null || sfx == null) return;
        fonteSFX2D.PlayOneShot(sfx);
    }
    public void TocarSFXNoObjeto(AudioClip sfx, Vector3 posicao, float volume = 1f)
    {
        if (sfx == null) return;
        AudioSource.PlayClipAtPoint(sfx, posicao, volume);
    }

    public void DefinirVolumeMusica(float valorLinear)
    {
        mixerGlobal.SetFloat("VolumeMusica", ConverterParaDecibel(valorLinear));
    }

    public void DefinirVolumeSFX(float valorLinear)
    {
        mixerGlobal.SetFloat("VolumeSFX", ConverterParaDecibel(valorLinear));
    }

    public void DefinirVolumeAmbiente(float valorLinear)
    {
        mixerGlobal.SetFloat("VolumeAmbiente", ConverterParaDecibel(valorLinear));
    }

    private float ConverterParaDecibel(float valorLinear)
    {
        valorLinear = Mathf.Clamp(valorLinear, 0.0001f, 1f);
        return Mathf.Log10(valorLinear) * 20;
    }
}