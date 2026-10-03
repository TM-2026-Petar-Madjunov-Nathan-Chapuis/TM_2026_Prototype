using System;
using TM.Input;
using TM.Misc;
using TM.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenuManager : MonoBehaviour, ISaveable
{
    [SerializeField] private UIDocument uIDocument;
    private VisualElement PauseMenu;
    private Button ReturnToGameButton;
    private Button SettingsButton;
    private Button SaveButton;
    private Button LoadButton; //temporary button
    private Button SaveAndQuitToMainMenuButton;
    private Button QuitGameButton;
    private VisualElement SettingsMenu;
    private Button BackFromSettingsToPause;
    private bool menuIsOpen;
    private bool settingsMenuIsOpen;
    private VisualElement root;

    [SerializeField] private BlurManager blurManager;
    private Material flipMaterial;
    [SerializeField] private Shader flipShader;
    private RenderTexture captureRT;
    private RenderTexture flippedRt;
    private RenderTexture blurredRT;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private SavingManager savingManager;


    public string UID => "PauseMenuManager";

    void Awake()
    {
        flipMaterial = new Material(flipShader);
    }
    void Start()
    {
        root = uIDocument.rootVisualElement;

        PauseMenu = root.Q<VisualElement>("MenuBackground");
        ReturnToGameButton = root.Q<VisualElement>("MenuBackground").Q<Button>("ReturnToGameButton");
        SettingsButton = root.Q<VisualElement>("MenuBackground").Q<Button>("SettingsButton");
        SaveButton = root.Q<VisualElement>("MenuBackground").Q<Button>("SaveButton");
        LoadButton = root.Q<VisualElement>("MenuBackground").Q<Button>("LoadButton"); //temporary button
        SaveAndQuitToMainMenuButton = root.Q<VisualElement>("MenuBackground").Q<Button>("SaveAndQuitToMainMenuButton");
        QuitGameButton = root.Q<VisualElement>("MenuBackground").Q<Button>("QuitGameButton");

        SettingsMenu = root.Q<VisualElement>("SettingsMenu");
        BackFromSettingsToPause = root.Q<VisualElement>("SettingsMenu").Q<Button>("BackFromSettingsToPause");

        menuIsOpen = true;
        ShowAndHidePauseMenu(new InputValues()); // ferme le menu au début

        settingsMenuIsOpen = true;
        OpenSettingsAction(new ClickEvent()); // ferme le menu au début
    }

    void OnEnable()
    {
        InputManager.Instance.RegisterListener("OpenPauseMenu", ShowAndHidePauseMenu, InputValueType.Button, true); //[ESCAPE]
    }
    void OnDisable()
    {
        InputManager.Instance.UnRegisterListener("OpenPauseMenu", ShowAndHidePauseMenu);
    }

    private void ShowPauseMenu()
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    private void HidePauseMenu()
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
    void ShowAndHidePauseMenu(InputValues input)
    {
        if (menuIsOpen == true)
        {
            HidePauseMenu();
            GameStateManager.Instance.UnFreezeTime();
            inputActions.FindActionMap("Player").Enable();

            CursorManager.Instance.HideCursor();

            ReturnToGameButton.UnregisterCallback<ClickEvent>(ReturnToGameAction);
            SettingsButton.UnregisterCallback<ClickEvent>(OpenSettingsAction);
            SaveButton.UnregisterCallback<ClickEvent>(SaveAction);
            LoadButton.UnregisterCallback<ClickEvent>(LoadAction); //temporary button
            SaveAndQuitToMainMenuButton.UnregisterCallback<ClickEvent>(SaveAndQuitToMainMenuAction);
            QuitGameButton.UnregisterCallback<ClickEvent>(QuitGameAction);

            menuIsOpen = false;

            if (settingsMenuIsOpen == true)
            {
                ShowAndHideSettingsMenu();
            }

        }
        else // menuIsOpen == false
        {
            ShowPauseMenu();
            GameStateManager.Instance.FreezeTime();
            inputActions.FindActionMap("Player").Disable();
            inputActions.FindAction("Player/OpenPauseMenu").Enable();
            CaptureAndBlurBackground();


            CursorManager.Instance.ShowCursor();

            ReturnToGameButton.RegisterCallback<ClickEvent>(ReturnToGameAction);
            SettingsButton.RegisterCallback<ClickEvent>(OpenSettingsAction);
            SaveButton.RegisterCallback<ClickEvent>(SaveAction);
            LoadButton.RegisterCallback<ClickEvent>(LoadAction); //temporary button

            menuIsOpen = true;
        }
    }

    void ShowAndHideSettingsMenu()
    {
        if (settingsMenuIsOpen == true)
        {
            SettingsMenu.style.display = DisplayStyle.None;
            PauseMenu.style.display = DisplayStyle.Flex;

            BackFromSettingsToPause.UnregisterCallback<ClickEvent>(OpenSettingsAction);

            settingsMenuIsOpen = false;
        }
        else // settingsMenuIsOpen == false
        {
            SettingsMenu.style.display = DisplayStyle.Flex;
            PauseMenu.style.display = DisplayStyle.None;

            BackFromSettingsToPause.RegisterCallback<ClickEvent>(OpenSettingsAction);

            settingsMenuIsOpen = true;
        }
    }


    void ReturnToGameAction(ClickEvent clickEvent)
    {
        ShowAndHidePauseMenu(new InputValues());
    }
    void OpenSettingsAction(ClickEvent clickEvent)
    {
        ShowAndHideSettingsMenu();
    }
    void SaveAction(ClickEvent clickEvent)
    {
        savingManager.Save();
        Debug.Log("saved succesfully");
    }
    void LoadAction(ClickEvent clickEvent) //temporary button
    {
        savingManager.Load();
        Debug.Log("loaded succesfully");
    }
    void SaveAndQuitToMainMenuAction(ClickEvent clickEvent)
    {
        savingManager.Save();
        Application.Quit(); // temporary
    }
    void QuitGameAction(ClickEvent clickEvent)
    {
        Application.Quit();
    }

    //copied from MenuUIManager

    private void CaptureAndBlurBackground()
    {
        UpdateRenderTexture();

        ScreenCapture.CaptureScreenshotIntoRenderTexture(captureRT); //capture the game view

        Graphics.Blit(captureRT, flippedRt, flipMaterial); //flip the image cause somehow its flipped.
        blurredRT = blurManager.Blur(flippedRt); //blur the image
        root.style.backgroundImage = Background.FromRenderTexture(blurredRT);//apply to background
    }

    private void UpdateRenderTexture()
    {
        if (captureRT == null || captureRT.width != Screen.width || captureRT.height != Screen.height)
        {
            if (captureRT != null)
            {
                captureRT.Release();
            }
            captureRT = new RenderTexture(Screen.width, Screen.height, 0);
            captureRT.Create();
        }
        if (flippedRt == null || flippedRt.width != Screen.width || flippedRt.height != Screen.height)
        {
            if (flippedRt != null)
            {
                flippedRt.Release();
            }
            flippedRt = new RenderTexture(Screen.width, Screen.height, 0);
            flippedRt.Create();
        }
    }

    public object SaveData()
    {
        return null;
    }

    public void LoadData(string data)
    {
        menuIsOpen = true;
        ShowAndHidePauseMenu(new InputValues());
    }

    //until here

}