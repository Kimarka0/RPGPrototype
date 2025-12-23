using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Image[] tabImages;
    [SerializeField] private GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNum)
    {
        for(int i = 0; i< pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNum].SetActive(true);
        tabImages[tabNum].color = Color.white;
    }
}
