using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MazeEnter : MonoBehaviour
{
    [SerializeField] private GameObject mazeMenuPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            mazeMenuPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        mazeMenuPanel.SetActive(false);
    }

    public void YesButtonClick()
    {
        SceneManager.LoadScene("Maze");
    }

    public void NoButtonClick()
    {
        mazeMenuPanel.SetActive(false);
        Destroy(gameObject);
    }
}

