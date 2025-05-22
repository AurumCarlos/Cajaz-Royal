using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{ 
    public string levelToLoad;
    public GameObject settingsWindow;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsWindow();
        }
    }
    
   public void StartGame()
   {
       SceneManager.LoadSceneAsync(levelToLoad);
   }
   public void SettingsButton()
   {
         settingsWindow.SetActive(true);
   }

   public void CloseSettingsWindow()
   {
       settingsWindow.SetActive(false);
   }
      
   public void QuitGame()
   {
         Application.Quit();
   }
   private void ToggleSettingsWindow()
   {
       settingsWindow.SetActive(!settingsWindow.activeSelf);
   }
}
