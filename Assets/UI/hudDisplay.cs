using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TM.Player;

public class HUDDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    [SerializeField] private CorporalTemperatureCalculation corporalTemperatureCalculation;
    [SerializeField] private Label temperatureLabel;
    [SerializeField] private VisualElement thermometerFill;
    [SerializeField] private float thermometerFillPercent;
    [SerializeField] private float localPlayerCorporalTemperature;
    [SerializeField] private VisualElement healthBarFill;
    [SerializeField] private CircularBarProgression circularFoodBar;
    [SerializeField] private Label foodBarLabel;
    [SerializeField] private VisualElement foodFill;
    private byte thermometerFillColorRed;
    private byte thermometerFillColorGreen = 0;
    private byte thermometerFillColorBlue;
    private byte thermometerFillColorOpacity = 255;
    
    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;
    private HungerAndThirstCalculation hungerAndThirstCalculation;

    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
        hungerAndThirstCalculation = player.GetComponent<HungerAndThirstCalculation>();
    }
    void Start()
    {
        var root = uIDocument.rootVisualElement;
        temperatureLabel = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill").Q<Label>("temperatureLabel");
        thermometerFill = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill");

        healthBarFill = root.Q<VisualElement>("heathBarDisplay").Q<VisualElement>("healthBarFill");

        foodBarLabel = root.Q<VisualElement>("foodDisplay").Q<VisualElement>("foodFill").Q<Label>("foodBarLabel");
        foodFill = root.Q<VisualElement>("foodDisplay").Q<VisualElement>("foodFill");
        circularFoodBar = root.Q<VisualElement>("foodDisplay").Q<CircularBarProgression>("circularFoodBar");
        circularFoodBar.baseLineWidht = 20f;
        circularFoodBar.color = new Color32(255, 127, 39, 255);
        circularFoodBar.background = true;
        circularFoodBar.backgroundColor = new Color32(255, 210, 174, 255);
        circularFoodBar.border = true;
        circularFoodBar.segments = true;

    }

    void Update()
    {
        thermometerFillPercent = playerInfo.playerCorporalTemperature*2;
        
        DefineTemperatureLabelText();
        //DefineThermometerFillHeight();
        DefineThermometerFilllColor();
        DefineHealthBarFill();
        DefineFoodBar();
        DefineFoodBarLabelText();
    }

    void DefineThermometerFilllColor()
    {

        thermometerFillColorRed = (byte)Mathf.RoundToInt(thermometerFillPercent*255f/90f);
        //Debug.Log((byte)Mathf.RoundToInt(thermometerFillPercent*255f/90f));
        thermometerFillColorBlue = (byte)Mathf.RoundToInt(255f - (thermometerFillPercent*255f/90f));
        //Debug.Log((byte)Mathf.RoundToInt(255f-(thermometerFillPercent*255f/90f)));

        Color thermometerFillColor = (Color)new Color32(thermometerFillColorRed, thermometerFillColorGreen, thermometerFillColorBlue, thermometerFillColorOpacity);
        thermometerFill.style.backgroundColor = thermometerFillColor;
    }

    void DefineThermometerFillHeight()
    {
        thermometerFill.style.height = Length.Percent(thermometerFillPercent);
    }
    void DefineTemperatureLabelText()
    {
        temperatureLabel.text = $"{playerInfo.playerCorporalTemperature}";
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

}
