using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneMenu : MonoBehaviour
{
    public bool startMenu;
    void OnGUI()
    {
        if (!startMenu) {
           return;
        }
        float scale = Mathf.Clamp(Screen.height / 900f, .5f, 2f);
        var title = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(44 * scale), alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        title.normal.textColor = new Color(.65f, .9f, .9f);
        GUI.Label(new Rect(0, Screen.height * .28f, Screen.width, 70 * scale), "PACSTUDENT", title);
        var subtitle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(16 * scale), alignment = TextAnchor.MiddleCenter };
        subtitle.normal.textColor = new Color(.7f, .76f, .85f);
        GUI.Label(new Rect(0, Screen.height * .39f, Screen.width, 40 * scale), "LEVEL 01  /  SPRITES IN MOTION", subtitle);
        if (GUI.Button(new Rect((Screen.width - 240 * scale) / 2, Screen.height * .52f, 240 * scale, 52 * scale), "Open level")){
            SceneManager.LoadScene("RecreatedLevel");
        }
    }
}
