using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
    private Button NewGameButton;
    private Button LoadGameButton; 
    private Button SettingsButton;
    private Button CreditsButton;
    
    private Button QuitGameButton;

    private VisualElement SettingsMenu;
    private Button BackFromSettingsToPause;

    private VisualElement LoadSaveMenu;
    private VisualElement LoadFileList;

    private Button BackFromLoadingToPause;
    private Button CreateNewSaveButton;

    private VisualElement SaveInfosMenu;
    private VisualElement SaveImage;
    private VisualElement SaveName;
    private Label FirstTimeDateValue;
    private Label LastTimeDateValue;
    private Label TimePlayedValue;
    private Button PlayButton;

    private VisualElement selectedSaveFile;


    private bool loadingMenuIsOpen;
    private bool settingsMenuIsOpen;

    private VisualElement root;
    void Start()
    {
        root = uIDocument.rootVisualElement;

        MainMenu = root.Q<VisualElement>("MenuBackground");
        NewGameButton = MainMenu.Q<Button>("NewGameButton");
        LoadGameButton = MainMenu.Q<Button>("LoadGameButton");
        SettingsButton = MainMenu.Q<Button>("SettingsButton");
        CreditsButton = MainMenu.Q<Button>("CreditsButton");
        QuitGameButton = MainMenu.Q<Button>("QuitGameButton");

        SettingsMenu = root.Q<VisualElement>("SettingsMenu");
        BackFromSettingsToPause = root.Q<VisualElement>("SettingsMenu").Q<Button>("BackFromSettingsToPause");

        LoadSaveMenu = root.Q<VisualElement>("LoadSaveMenu");
        LoadFileList = LoadSaveMenu.Q<VisualElement>("FileSelection");
        BackFromLoadingToPause = LoadSaveMenu.Q<Button>("BackFromLoadingToPause");
        CreateNewSaveButton = LoadSaveMenu.Q<Button>("CreateNewSaveButton");

        SaveInfosMenu = root.Q<VisualElement>("SaveInfosMenu");
        SaveImage = SaveInfosMenu.Q<VisualElement>("SaveImage");
        FirstTimeDateValue = SaveInfosMenu.Q<Label>("FirstTimeDateValue");
        LastTimeDateValue = SaveInfosMenu.Q<Label>("LastTimeDateValue");
        TimePlayedValue = SaveInfosMenu.Q<Label>("TimePlayedValue");
        PlayButton = SaveInfosMenu.Q<Button>("PlayButton");


        settingsMenuIsOpen = true;
        OpenSettingsAction(new ClickEvent()); // ferme le menu au début

        loadingMenuIsOpen = true;
        OpenLoadMenuAction(new ClickEvent()); // ferme le menu au début 

        MainMenu.style.display = DisplayStyle.Flex;
        CloseMenuBySwiping(MainMenu, false, 0.5f);
        
        CloseMenuBySwiping(SaveInfosMenu, true, 0f);

        EnableCallbacks();
    }
    private void EnableCallbacks()
    {
        SettingsButton.RegisterCallback<ClickEvent>(OpenSettingsAction);
        LoadGameButton.RegisterCallback<ClickEvent>(OpenLoadMenuAction);
        QuitGameButton.RegisterCallback<ClickEvent>(QuitGameAction);
        PlayButton.RegisterCallback<ClickEvent>(PlaySelectedAction);
    }

    void ShowAndHideSettingsMenu()
    {
        if (settingsMenuIsOpen == true)
        {
            SettingsMenu.style.display = DisplayStyle.None;
            //MainMenu.style.display = DisplayStyle.Flex;
            CloseMenuBySwiping(MainMenu, false, 0.5f);

            BackFromSettingsToPause.UnregisterCallback<ClickEvent>(OpenSettingsAction);
            settingsMenuIsOpen = false;
        }
        else // settingsMenuIsOpen == false
        {
            SettingsMenu.style.display = DisplayStyle.Flex;
            //MainMenu.style.display = DisplayStyle.None;
            CloseMenuBySwiping(MainMenu, true, 0.5f);

            BackFromSettingsToPause.RegisterCallback<ClickEvent>(OpenSettingsAction);
            settingsMenuIsOpen = true;
        }
    }
    void ShowAndHideLoadingMenu()
    {
        if (loadingMenuIsOpen == true)
        {
            LoadSaveMenu.style.display = DisplayStyle.None;
            //MainMenu.style.display = DisplayStyle.Flex;
            CloseMenuBySwiping(MainMenu, false, 0.5f);

            BackFromLoadingToPause.UnregisterCallback<ClickEvent>(OpenLoadMenuAction);
            BackFromLoadingToPause.UnregisterCallback<ClickEvent>(CreateNewSaveAction);
            loadingMenuIsOpen = false;
        }
        else
        {
            this.selectedSaveFile = null;
            ShowAndHideSelectedMenu();
            LoadSaveMenu.style.display = DisplayStyle.Flex;
            //MainMenu.style.display = DisplayStyle.None;
            CloseMenuBySwiping(MainMenu, true, 0.5f);

            BackFromLoadingToPause.RegisterCallback<ClickEvent>(OpenLoadMenuAction);
            BackFromLoadingToPause.RegisterCallback<ClickEvent>(CreateNewSaveAction);
            loadingMenuIsOpen = true;

            LoadFileList.Query<VisualElement>(className: "SaveFileTemplate") //all the save files
                        .ToList() //to a list
                        .ForEach(button => button.RemoveFromHierarchy()); //remove all of them.

            String[] filenames = SaveSlotManager.Instance.GetAllSaveFiles();
            foreach (string file in filenames)
            {
                SaveFile saveFileInfo = SaveSlotManager.Instance.GetSaveFileInfo(file);

                VisualElement saveFile = new VisualElement();
                saveFile.userData = file;
                saveFile.AddToClassList("SaveFileTemplate");

                //SAVE FILE IMAGE
                VisualElement saveFileImage = new VisualElement();
                saveFileImage.AddToClassList("SaveFileImage");

                VisualElement bottomBorder = new VisualElement();
                bottomBorder.AddToClassList("SaveImageBottomLeftBorder");

                VisualElement topborder = new VisualElement();
                topborder.AddToClassList("SaveImageTopRightBorder");

                saveFileImage.Add(bottomBorder);
                saveFileImage.Add(topborder);

                //SAVE FILE BACKGROUND (INFOS)
                VisualElement background = new VisualElement();
                background.AddToClassList("SaveFileBackground");

                VisualElement separator = new VisualElement();
                separator.AddToClassList("BackgroundSeparator");

                Label saveName = new Label();
                saveName.text = file;
                saveName.AddToClassList("SaveFileName");

                Button renameButton = new Button();
                renameButton.AddToClassList("RenameButton");
                renameButton.RegisterCallback<ClickEvent>(evt => RenameSavefile(file));

                Button deleteButton = new Button();
                deleteButton.AddToClassList("DeleteButton");
                deleteButton.RegisterCallback<ClickEvent>(evt => DeleteSavefile(file));

                Label date = new Label();
                date.text = saveFileInfo.lastSavedAtUtc.ToString();
                date.AddToClassList("SaveFileDate");

                //HIDDEN BUTTON 
                Button hiddenButton = new Button();
                hiddenButton.AddToClassList("SaveFileHiddenButton");
                hiddenButton.RegisterCallback<ClickEvent>(evt => SelectSaveFile(evt, saveFile, file));

                background.Add(separator);
                background.Add(saveName);
                background.Add(renameButton);
                background.Add(deleteButton);
                background.Add(date);

                saveFile.Add(saveFileImage);
                saveFile.Add(background);
                saveFile.Add(hiddenButton);

                LoadFileList.Add(saveFile);
            }
        }
    }
    private void ShowAndHideSelectedMenu()
    {
        if (this.selectedSaveFile != null)
        {
            SaveInfosMenu.style.display = DisplayStyle.Flex;
            CloseMenuBySwiping(SaveInfosMenu, false, 0.5f);
            SaveFile saveFileInfo = SaveSlotManager.Instance.GetSaveFileInfo(this.selectedSaveFile.userData as string);
            LastTimeDateValue.text = saveFileInfo.lastSavedAtUtc.ToString();
            FirstTimeDateValue.text = saveFileInfo.createdAtUtc.ToString();
            TimePlayedValue.text = FormatPlayTime(saveFileInfo.totalPlaySeconds);
        }
    }
    private string FormatPlayTime(double totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        return $"{(int)time.TotalHours:D2} hours, {time.Minutes:D2} minutes, {time.Seconds:D2} seconds"; //creates a string with hours:minutes. :D2 transforms 5 into 05 and 12 into 12 (keeps two digits)
    }
    private void CloseMenuBySwiping(VisualElement menu, bool menuIsOpen, float timeAnimation)
    {   

        if(menuIsOpen == true)
        {
            menu.style.translate = new StyleTranslate(new Translate(Length.Percent(0f),Length.Percent(100f)));
            //menu.style.display = DisplayStyle.None;
        }
        if(menuIsOpen == false)
        {
            //menu.style.display = DisplayStyle.Flex;
            menu.style.translate = new StyleTranslate(new Translate(Length.Percent(0f),Length.Percent(0f)));
        }
    }
    private void SelectSaveFile(ClickEvent evt, VisualElement saveFile, string fileName)
    {
        if (evt.clickCount == 1)
        {
            if (this.selectedSaveFile != null)
            {
                this.selectedSaveFile.RemoveFromClassList("selected");
            }
            saveFile.AddToClassList("selected");
            this.selectedSaveFile = saveFile;
        }
        else if (evt.clickCount == 2)
        {
            SaveSlotManager.Instance.SelectSaveFile(fileName);     
        }
        ShowAndHideSelectedMenu();
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
    private void PlaySelectedAction(ClickEvent clickEvent)
    {
        if (this.selectedSaveFile.userData  != null)
        {
            SaveSlotManager.Instance.SelectSaveFile(this.selectedSaveFile.userData as string);        
        }
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