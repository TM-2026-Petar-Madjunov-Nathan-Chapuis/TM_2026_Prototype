using System;
using System.Collections;
using TM.Input;
using TM.Misc;
using TM.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    private VisualElement MainMenu;
    private Button SettingsButton;
    private Button PlayButton;
    private Button QuitGameButton;

    private VisualElement SettingsMenu;
    private Button BackFromSettingsToPause;

    private VisualElement LoadSaveMenu;
    private VisualElement LoadFileList;
    private VisualElement FileSaveTemplate;
    private Button BackFromLoadingToPause;
    private Button CreateNewSaveButton;

    private bool loadingMenuIsOpen;
    private bool settingsMenuIsOpen;

    private VisualElement root;
    void Start()
    {
        root = uIDocument.rootVisualElement;

        MainMenu = root.Q<VisualElement>("MenuBackground");
        SettingsButton = MainMenu.Q<Button>("SettingsButton");
        PlayButton = MainMenu.Q<Button>("PlayButton"); //temporary button
        QuitGameButton = MainMenu.Q<Button>("QuitGameButton");

        SettingsMenu = root.Q<VisualElement>("SettingsMenu");
        BackFromSettingsToPause = root.Q<VisualElement>("SettingsMenu").Q<Button>("BackFromSettingsToPause");

        LoadSaveMenu = root.Q<VisualElement>("LoadSaveMenu");
        LoadFileList = LoadSaveMenu.Q<VisualElement>("FileSelection");
        FileSaveTemplate = LoadFileList.Q<VisualElement>("SaveFileTemplate");
        FileSaveTemplate.style.display = DisplayStyle.None;
        BackFromLoadingToPause = LoadSaveMenu.Q<Button>("BackFromLoadingToPause");
        CreateNewSaveButton = LoadSaveMenu.Q<Button>("CreateNewSave");

        settingsMenuIsOpen = true;
        OpenSettingsAction(new ClickEvent()); // ferme le menu au début

        loadingMenuIsOpen = true;
        OpenLoadMenuAction(new ClickEvent()); // ferme le menu au début

        EnableCallbacks();
    }
    private void EnableCallbacks()
    {
        SettingsButton.RegisterCallback<ClickEvent>(OpenSettingsAction);
        PlayButton.RegisterCallback<ClickEvent>(OpenLoadMenuAction);
        QuitGameButton.RegisterCallback<ClickEvent>(QuitGameAction);
    }
    private void DisableCallbacks()
    {
        SettingsButton.UnregisterCallback<ClickEvent>(OpenSettingsAction);
        PlayButton.UnregisterCallback<ClickEvent>(OpenLoadMenuAction);
        QuitGameButton.UnregisterCallback<ClickEvent>(QuitGameAction);
    }

    void ShowAndHideSettingsMenu()
    {
        if (settingsMenuIsOpen == true)
        {
            SettingsMenu.style.display = DisplayStyle.None;
            MainMenu.style.display = DisplayStyle.Flex;

            BackFromSettingsToPause.UnregisterCallback<ClickEvent>(OpenSettingsAction);
            settingsMenuIsOpen = false;
        }
        else // settingsMenuIsOpen == false
        {
            SettingsMenu.style.display = DisplayStyle.Flex;
            MainMenu.style.display = DisplayStyle.None;

            BackFromSettingsToPause.RegisterCallback<ClickEvent>(OpenSettingsAction);
            settingsMenuIsOpen = true;
        }
    }
    void ShowAndHideLoadingMenu()
    {
        if (loadingMenuIsOpen == true)
        {
            LoadSaveMenu.style.display = DisplayStyle.None;
            MainMenu.style.display = DisplayStyle.Flex;

            BackFromLoadingToPause.UnregisterCallback<ClickEvent>(OpenLoadMenuAction);
            BackFromLoadingToPause.UnregisterCallback<ClickEvent>(CreateNewSaveAction);
            loadingMenuIsOpen = false;
        }
        else
        {
            LoadSaveMenu.style.display = DisplayStyle.Flex;
            MainMenu.style.display = DisplayStyle.None;

            BackFromLoadingToPause.RegisterCallback<ClickEvent>(OpenLoadMenuAction);
            BackFromLoadingToPause.RegisterCallback<ClickEvent>(CreateNewSaveAction);
            loadingMenuIsOpen = true;

            LoadFileList.Clear();

            String[] filenames = SaveSlotManager.Instance.GetAllSaveFiles();
            foreach (string file in filenames)
            {
                Button saveFile = new Button();
                saveFile.AddToClassList("SaveFileButton");
                saveFile.AddToClassList("SaveFileTemplate");
                
                Label saveFileText = new Label();
                saveFileText.AddToClassList("SaveFileName");
                saveFileText.text = file;
                saveFile.Add(saveFileText);

                VisualElement bottomHolder = new VisualElement();
                bottomHolder.AddToClassList("BottomHolder");
                
                Button renameButton = new Button();
                renameButton.AddToClassList("PauseMenuButtonsUSS");
                renameButton.AddToClassList("RenameButton");
                renameButton.RegisterCallback<ClickEvent>(evt => RenameSavefile(file));

                Button deleteButton = new Button();
                deleteButton.AddToClassList("PauseMenuButtonsUSS");
                deleteButton.AddToClassList("DeleteButton");
                deleteButton.RegisterCallback<ClickEvent>(evt => DeleteSavefile(file));

                bottomHolder.Add(renameButton);
                bottomHolder.Add(deleteButton);

                saveFile.Add(bottomHolder);

                LoadFileList.Add(saveFile);
                saveFile.RegisterCallback<ClickEvent>(e => SelectSaveFile(file));
                saveFile.RegisterCallback<PointerMoveEvent>(evt => SaveFilePointerMove(evt, saveFile, renameButton, deleteButton));
                saveFile.RegisterCallback<PointerEnterEvent>(evt => saveFile.AddToClassList("SaveFileButtonHover"));
                saveFile.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    saveFile.RemoveFromClassList("SaveFileButtonHover");
                    saveFile.AddToClassList("SaveFileButton");
                });
            }
        }
    }
    private void SaveFilePointerMove(PointerMoveEvent evt, Button saveFile, Button renameButton, Button deleteButton)
    {
        if (renameButton.worldBound.Contains(evt.position) || deleteButton.worldBound.Contains(evt.position))
        {
            saveFile.RemoveFromClassList("SaveFileButtonHover");
            saveFile.AddToClassList("SaveFileButton");
        }
        else
        {
            saveFile.RemoveFromClassList("SaveFileButton");
            saveFile.AddToClassList("SaveFileButtonHover");
        }
    }
    private void SelectSaveFile(string fileName)
    {
        SaveSlotManager.Instance.SelectSaveFile(fileName);
    }
    private void DeleteSavefile(string fileName)
    {
        SaveSlotManager.Instance.DeleteSaveFile(fileName);
    }
    private void RenameSavefile(string filename)
    {
        
    }
    private void CreateNewSave()
    {
        Debug.Log("fds");
    }
    void OpenSettingsAction(ClickEvent clickEvent)
    {
        ShowAndHideSettingsMenu();
    }
    void OpenLoadMenuAction(ClickEvent clickEvent)
    {
        ShowAndHideLoadingMenu();
    }
    void QuitGameAction(ClickEvent clickEvent)
    {
        Application.Quit();
    }
    void CreateNewSaveAction(ClickEvent clickEvent)
    {
        CreateNewSave();
    }
}