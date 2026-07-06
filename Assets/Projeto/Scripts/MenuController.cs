using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void IniciarFase()
    {
        Debug.Log("Iniciando a fase...");
        SceneManager.LoadScene("Estação");
    }
}
