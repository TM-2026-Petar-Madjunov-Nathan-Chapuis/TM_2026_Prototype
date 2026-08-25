using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TM.Player;

public class HUDDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    [SerializeField] private Label temperatureLabel;
    [SerializeField] private VisualElement thermometerFill;
    [SerializeField] private VisualElement healthBarFill;
    [SerializeField] private CircularBarProgression circularFoodBar;
    [SerializeField] private Label foodBarLabel;
    [SerializeField] private CircularBarProgression circularThirstBar;
    [SerializeField] private Label thirstBarLabel;
    private byte thermometerFillColorRed;
    private byte thermometerFillColorGreen = 30;
    private byte thermometerFillColorBlue;
    private byte thermometerFillColorOpacity = 255;
    private float temperatureWhenColorIsFullRed = 37.5f;
    private float temperatureWhenColorIsFullBlue =34f;

    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;
    private HungerAndThirstCalculation hungerAndThirstCalculation;
    private CorporalTemperatureCalculation corporalTemperatureCalculation;

    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
        hungerAndThirstCalculation = player.GetComponent<HungerAndThirstCalculation>();
        corporalTemperatureCalculation = player.GetComponent<CorporalTemperatureCalculation>();
    }
    void Start()
    {
        var root = uIDocument.rootVisualElement;
        temperatureLabel = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill").Q<Label>("temperatureLabel");
        thermometerFill = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill");

        healthBarFill = root.Q<VisualElement>("heathBarDisplay").Q<VisualElement>("healthBarFill");

        foodBarLabel = root.Q<VisualElement>("foodDisplay").Q<VisualElement>("foodFill").Q<Label>("foodBarLabel");
        circularFoodBar = root.Q<VisualElement>("foodDisplay").Q<CircularBarProgression>("circularFoodBar");
        circularFoodBar.baseLineWidht = 20f;
        circularFoodBar.color = new Color32(255, 127, 39, 255);
        circularFoodBar.background = true;
        circularFoodBar.backgroundColor = new Color32(255, 210, 174, 255);
        circularFoodBar.border = true;
        circularFoodBar.segments = true;

        thirstBarLabel = root.Q<VisualElement>("thirstDisplay").Q<VisualElement>("thirstFill").Q<Label>("thirstBarLabel");
        circularThirstBar = root.Q<VisualElement>("thirstDisplay").Q<CircularBarProgression>("circularThirstBar");
        circularThirstBar.baseLineWidht = 20f;
        circularThirstBar.color = new Color32(135, 213, 232, 255);
        circularThirstBar.background = true;
        circularThirstBar.backgroundColor = new Color32(166, 217, 227, 255);
        circularThirstBar.border = true;
        circularThirstBar.segments = true;
    }

    void Update()
    {
        
        
        DefineTemperatureLabelText();
        //DefineThermometerFillHeight();
        DefineThermometerFilllColor();

        DefineHealthBarFill();
        
        DefineFoodBar();
        DefineFoodBarLabelText();
        
        DefineThirstBar();
        DefineThirstBarLabelText();
    }

    void DefineThermometerFilllColor()
    {

        thermometerFillColorRed = (byte)Mathf.RoundToInt((playerInfo.playerCorporalTemperature-temperatureWhenColorIsFullBlue)*255f/(temperatureWhenColorIsFullRed-temperatureWhenColorIsFullBlue));
        //Debug.Log((byte)Mathf.RoundToInt(thermometerFillPercent*255f/90f));
        thermometerFillColorBlue = (byte)Mathf.RoundToInt(255f - ((playerInfo.playerCorporalTemperature-temperatureWhenColorIsFullBlue)*255f/(temperatureWhenColorIsFullRed-temperatureWhenColorIsFullBlue)));
        //Debug.Log((byte)Mathf.RoundToInt(255f-(thermometerFillPercent*255f/90f)));

        Color thermometerFillColor = (Color)new Color32(thermometerFillColorRed, thermometerFillColorGreen, thermometerFillColorBlue, thermometerFillColorOpacity);
        thermometerFill.style.backgroundColor = thermometerFillColor;
    }

    void DefineThermometerFillHeight()
    {
        thermometerFill.style.height = Length.Percent(playerInfo.playerCorporalTemperature*2);
    }
    void DefineTemperatureLabelText()
    {
        temperatureLabel.text = $"{corporalTemperatureCalculation.roundedCorpralTemperature}"+"°C";
    }
    void DefineHealthBarFill()
    {
        healthBarFill.style.width = Length.Percent(playerInfo.playerHealth);
    }
    void DefineFoodBar()
    {
        circularFoodBar.progress = playerInfo.playerHunger;
    }
    void DefineFoodBarLabelText()
    {   
        foodBarLabel.text = $"{hungerAndThirstCalculation.roundedPlayerHunger}";
    }
    void DefineThirstBar()
    {
        circularThirstBar.progress = playerInfo.playerThirst;
    }
    void DefineThirstBarLabelText()
    {   
        thirstBarLabel.text = $"{hungerAndThirstCalculation.roundedplayerThirst}";
    }

}
