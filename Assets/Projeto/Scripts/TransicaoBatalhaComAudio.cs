using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransicaoBatalhaComAudio : MonoBehaviour
{
    [Header("Componentes de UI")]
    [SerializeField] private Image imagemPreta;

    [Header("Configuração de Tempo Visual")]
    [SerializeField] private float tempoTotalEmPreto = 4.0f;
    [SerializeField] private float duracaoFadeVisual = 1.5f;

    [Header("Configuração de Tempo do Áudio")]
    [SerializeField] private float delayParaAudioComecar = 1.0f;
    [SerializeField] private float duracaoFadeAudio = 2.5f;

    [Header("Clipes de Áudio da Batalha")]
    [SerializeField] private AudioClip musicaBatalha;
    [SerializeField] private AudioClip ambienteBatalha;

    void Start()
    {
        if (imagemPreta == null) imagemPreta = GetComponent<Image>();

        if (imagemPreta != null)
        {
            Color c = imagemPreta.color;
            c.a = 1f;
            imagemPreta.color = c;
            imagemPreta.gameObject.SetActive(true);

            if (GerenciadorAudio.Instancia != null)
            {
                GerenciadorAudio.Instancia.DefinirVolumeMusica(0.0001f);
                GerenciadorAudio.Instancia.DefinirVolumeAmbiente(0.0001f);
            }

            StartCoroutine(SequenciaDiretor());
        }
    }

    private IEnumerator SequenciaDiretor()
    {
        // Silêncio e breu inicial
        yield return new WaitForSeconds(delayParaAudioComecar);

        // Dá o play nos clipes com o Mixer ainda totalmente mudo
        if (GerenciadorAudio.Instancia != null)
        {
            if (musicaBatalha != null) GerenciadorAudio.Instancia.TocarMusica(musicaBatalha);
            if (ambienteBatalha != null) GerenciadorAudio.Instancia.TocarAmbiente(ambienteBatalha);
        }

        float tempoAudio = 0f;
        while (tempoAudio < duracaoFadeAudio)
        {
            tempoAudio += Time.deltaTime;
            float progressoAudio = tempoAudio / duracaoFadeAudio;

            // Suaviza o progresso usando SmoothStep para as pontas do fade não virem secas
            float volumeSuavizado = Mathf.SmoothStep(0.0001f, 1f, progressoAudio);

            if (GerenciadorAudio.Instancia != null)
            {
                GerenciadorAudio.Instancia.DefinirVolumeMusica(volumeSuavizado);
                GerenciadorAudio.Instancia.DefinirVolumeAmbiente(volumeSuavizado);
            }
            yield return null;
        }

        // Garante volume máximo ao fim do fade
        if (GerenciadorAudio.Instancia != null)
        {
            GerenciadorAudio.Instancia.DefinirVolumeMusica(1f);
            GerenciadorAudio.Instancia.DefinirVolumeAmbiente(1f);
        }

        // Tempo restante no escuro com o som já estabelecido
        float tempoRestanteEmPreto = tempoTotalEmPreto - (delayParaAudioComecar + duracaoFadeAudio);
        if (tempoRestanteEmPreto > 0)
        {
            yield return new WaitForSeconds(tempoRestanteEmPreto);
        }

        // Fade out da tela preta
        float tempoVisual = 0f;
        Color corAtual = imagemPreta.color;

        while (tempoVisual < duracaoFadeVisual)
        {
            tempoVisual += Time.deltaTime;
            float progressoVisual = tempoVisual / duracaoFadeVisual;

            corAtual.a = Mathf.Lerp(1f, 0f, progressoVisual);
            imagemPreta.color = corAtual;

            yield return null;
        }

        corAtual.a = 0f;
        imagemPreta.color = corAtual;
        imagemPreta.gameObject.SetActive(false);
    }
}