using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSolo : MonoBehaviour
{
    public BaseBot[] bots;
    public TMP_Dropdown selection1, selection2;
    public BaseBot selectedBot1, chosenBot1, selectedBot2, chosenBot2;
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

    virtual public void switchBot()
    {
        selectedBot1 = bots[selection1.value];
        selectedBot2 = bots[selection2.value];
    }

    virtual public void confirmSelection()
    {
        chosenBot1 = selectedBot1;
        chosenBot2 = selectedBot2;
        switchCharScreen.SetActive(false);
        switchCharButton.SetActive(true);
    }

    virtual public void showScreen()
    {
        switchCharScreen.SetActive(true);
        switchCharButton.SetActive(false);
    }
}
