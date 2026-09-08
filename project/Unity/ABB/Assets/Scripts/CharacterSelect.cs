using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{

    public BaseBotNetwork[] bots;
    public TMP_Dropdown selection;
    public BaseBotNetwork selectedBot, chosenBot;
    public GameObject switchCharButton, switchCharScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switchBot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Function that sets the selected character based on the dropdown menu value
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    virtual public void switchBot()
    {
        selectedBot = bots[selection.value];

    }

    /// <summary>
    /// Function that sets the chosen character and removes the selection screen
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    virtual public void confirmSelection()
    {
        chosenBot = selectedBot;
        switchCharScreen.SetActive(false);
        switchCharButton.SetActive(true);
    }

    /// <summary>
    /// Function that shows the character selection screen
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    virtual public void showScreen()
    {
        switchCharScreen.SetActive(true);
        switchCharButton.SetActive(false);
    }
}