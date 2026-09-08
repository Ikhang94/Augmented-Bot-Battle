using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SceneSwitcher : MonoBehaviour
{
    public int sceneIndex;
    public Image transition;
    public Vector4 colorSwitching;
    public bool startSwitch = false;
    public ARSession ar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(transition!=null)
        {
        colorSwitching = transition.color;
        }
    }
    private void Awake()
    {
        getArSession();
    }

    /// <summary>
    /// Function that gets the ar session so it can properly be reset on scene switch
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void getArSession()
    {
        ar = GameObject.FindAnyObjectByType<ARSession>();
    }
    // Update is called once per frame
    void Update()
    {
        if (startSwitch)
        {
            switchingScenes();
        }
    }


    /// <summary>
    /// Function that switches scenes
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void switchingScenes()
    {
        Time.timeScale = 1.0f;
        if (transition != null)
        {
            transition.gameObject.SetActive(true);
            if(transition.color.a < 1)
            {
                colorSwitching += new Vector4(0, 0, 0, 5 * Time.deltaTime);
                transition.color = colorSwitching;
            }
            else if (sceneIndex != null)
            {
                if (ar != null)
                {
                    LoaderUtility.Deinitialize();
                    LoaderUtility.Initialize();
                    ar.Reset();
                }
                SceneManager.LoadScene(sceneIndex);
            }
        }

        else if (sceneIndex != null)
        {
            if (ar != null)
                {
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
                    ar.Reset(); }
            SceneManager.LoadScene(sceneIndex,LoadSceneMode.Single);
        }
    }
    public void switchScenes()
    {
        startSwitch = true;
    }
}
