using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCena : MonoBehaviour
{
    [SerializeField] private string nomeCena;

    public void TrocarCena(string nomeDaCena)
    {
        if (string.IsNullOrWhiteSpace(nomeDaCena))
        {
            Debug.LogWarning("Nome da cena vazio.");
            return;
        }

        SceneManager.LoadScene(nomeDaCena);
    }

    public void TrocarCenaNoInspector()
    {
        TrocarCena(nomeCena);
    }
}
