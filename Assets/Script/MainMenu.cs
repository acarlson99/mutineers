using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mmenu;
    public GameObject levelMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayLevel(int levelnum)
    {
        // Play Now Button has been pressed, here you can initialize your game (For example Load a Scene called GameLevel etc.)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene/l" + levelnum.ToString("00"));
    }

    public void LevelMenu()
    {
        // Show Level Menu
        mmenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        mmenu.SetActive(true);
        levelMenu.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void DocRedir()
    {
        Application.OpenURL("./TODO.html");
    }
}
