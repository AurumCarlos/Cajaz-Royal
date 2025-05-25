using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{ 
    [SerializeField]private GameObject settingsWindow;
    [SerializeField]private GameObject ControlsWindow;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsWindow();
        }
    }
    
   public void StartMulti()
   {
       SceneManager.LoadSceneAsync("SampleScene");
   }   
   
   public void StartSolo()
   {
       SceneManager.LoadSceneAsync("Solo");
   }
   
   public void SettingsButton()
   {
         settingsWindow.SetActive(true);
   }

   public void CloseSettingsWindow()
   {
       settingsWindow.SetActive(false);
   }

   public void Close()
   {
       if (settingsWindow != null)
       {
           settingsWindow.SetActive(false);
       }
       else
       {
           Debug.LogWarning("Aucun menu n'est assigné à CloseMenu !");
       }
   }
   public void QuitGame()
   {
         Application.Quit();
   }
   private void ToggleSettingsWindow()
   {
       settingsWindow.SetActive(!settingsWindow.activeSelf);
   }

   public void Con()
   {
       ControlsWindow.SetActive(true);
   }

   public void Coff()
   {
       ControlsWindow.SetActive(false);
   }
}
