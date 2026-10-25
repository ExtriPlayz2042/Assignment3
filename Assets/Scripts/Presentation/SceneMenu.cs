using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneMenu : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("RecreatedLevel");
    }
}