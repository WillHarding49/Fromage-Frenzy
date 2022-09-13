using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
/**************************************************************************************
* Type: (Class)
* 
* Name: GameManager.cs
* 
* 
*
* Author: Joseph
*
* Description: Used manage the whole game such as changing scenes 
*
* Change Log:
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 24/06/2021    JG          1.00        -Initial Created
* 01/08/2021    WH          1.01        -Added exit
* 02/08/2021    WH          1.02        -Added volume slider
* 05/08/2021    WH          1.03        -Added new sliders for each sound mixer
* 16/08/2021    JG          1.04        -Added scene transitions
* 17/08/2021    WH          1.05        -Added playerprefs delete button
* 17/08/2021    JG          1.06        -added a main menu hot key
* 18/08/2021    WH          1.07        -Added tooltips
**************************************************************************************/
public class GameManager : MonoBehaviour
{
    #region vars
    [Tooltip("Audio Mixer for the Sound Effects")]
    public AudioMixer m_audioMixerSoundEffects;

    [Tooltip("Audio Mixer for the Background Music")]
    public AudioMixer m_audioMixerMusic;
    
    [Tooltip("Audio Mixer for the Racer sound effects")]
    public AudioMixer m_audioMixerRacer;
    
    [Tooltip("Animation for the transition")]
    public Animator m_transition;

    private float m_transitionTime = 1f;
    #endregion
   
   
    void Update()
    {
        //when you press escape goes back to main menu
        if (Input.GetKey(KeyCode.Escape))
        {
            StartCoroutine(LoadScene(0));
        }
    }

    #region UI Button Code
    /**************************************************************************************
   * Type: Function
   * 
   * Name: ChangeSceneButton
   * Parameters: int p_sceneNumber
   *
   * Author: Will Harding
   *
   * Description: Changes scene when pressed 
   * 
   *
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 02/08/2021    WH          1.00        -Initial Created
   * 16/08/2021    JG          1.01         -added screen transition coroutine
   **************************************************************************************/
    public void ChangeSceneButton(int p_sceneNumber)
    {
        StartCoroutine(LoadScene(p_sceneNumber));
    }
    /**************************************************************************************
   * Type: Function
   * 
   * Name: LoadScene
   * Parameters: int p_sceneNumber
   *
   * Author: Joseph Gilmore
   *
   * Description: Changes scene when pressed 
   * 
   *
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 16/08/2021    JG         1.00        -Initial Created
   **************************************************************************************/
    IEnumerator LoadScene(int p_sceneNumber)
    {
        //starts transition animation
        m_transition.SetTrigger("Start");
        yield return new WaitForSeconds(m_transitionTime);
        //Gets the scene build number from Unity engine and changes the scene
        SceneManager.LoadScene(p_sceneNumber);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: SetVolumeSoundEffects
    * Parameters: float p_volume
    *
    * Author: Will Harding
    *
    * Description: Changes audio mixer volume for sound effects
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 02/08/2021    WH          1.00        -Initial Created
    * 05/08/2021    Wh          1.01        -Made so it only changes sound effects
    **************************************************************************************/
    public void SetVolumeSoundEffects(float p_volume)
    {
        //Volume float is logorithmic as the volume mixer scale is also. It is *20 so it scales between 0 and -40 on the mixer, being full and mute volumes
        m_audioMixerSoundEffects.SetFloat("Volume", Mathf.Log10(p_volume) * 20);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: SetVolumeMusic
    * Parameters: float p_volume
    *
    * Author: Will Harding
    *
    * Description: Changes audio mixer volume for BG music
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 05/08/2021    WH          1.00        -Initial Created 
    **************************************************************************************/
    public void SetVolumeMusic(float p_volume)
    {
        //Volume float is logorithmic as the volume mixer scale is also. It is *20 so it scales between 0 and -40 on the mixer, being full and mute volumes
        m_audioMixerMusic.SetFloat("Volume", Mathf.Log10(p_volume) * 20);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: SetVolumeRacer
    * Parameters: float p_volume
    *
    * Author: Will Harding
    *
    * Description: Changes audio mixer volume for Racer sounds
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/08/2021    WH          1.00        -Initial Created 
    **************************************************************************************/
        public void SetVolumeRacer(float p_volume)
        {
            //Volume float is logorithmic as the volume mixer scale is also. It is *20 so it scales between 0 and -40 on the mixer, being full and mute volumes
            m_audioMixerRacer.SetFloat("Volume", Mathf.Log10(p_volume) * 20);
    }   

    public void ExitGame()
    {
        Application.Quit();

    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DeletePlayerPrefs
    *
    * Author: Will Harding
    *
    * Description: Deletes all playerprefs
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DeleteGrandPrixData
    *
    * Author: Will Harding
    *
    * Description: Deletes the ints of the playerprefs that save the score across the races
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/08/2021    WH          1.00        -Initial Created
    * 18/08/2021    WH          1.01        -Moved from PlayerPrefsManager to Gamemanager
    **************************************************************************************/
    public void DeleteGrandPrixData()
    {
        //Get highscore, delete playerprefs, and reset highscore
        int highscore = PlayerPrefs.GetInt("Highscore");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Highscore", highscore);
        PlayerPrefs.Save();
    }

    #endregion
}
