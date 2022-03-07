using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabletManager : MonoBehaviour
{
    [SerializeField] Color activeTabColor;
    [SerializeField] Color inActiveTabColor;

    [SerializeField] List<Tabs> tabs = new List<Tabs>();

    int tabIndex = 1;

    public void OpenTab(int _tabIndex)
    {
        // Old Tab
        tabs[tabIndex].tabText.color = inActiveTabColor;

        tabs[tabIndex].tabHUD.SetActive(false);


        // New Tab
        tabs[_tabIndex].tabText.color = activeTabColor;

        tabs[_tabIndex].tabHUD.SetActive(true);
        
        tabIndex = _tabIndex;
    }
}

[System.Serializable]
public class Tabs
{
    public string tabName;

    public TextMeshProUGUI tabText;

    public GameObject tabHUD;
}
