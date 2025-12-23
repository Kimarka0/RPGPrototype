using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    private PlayerControls playerControls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Controls.OpenMenu.started += OnMenuButtonPressed;
    }

    private void Start()
    {
        if(menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
