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
    [SerializeField] private Label healthBarLabel;
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
    private HealthCalculation healthCalculation;

    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
        hungerAndThirstCalculation = player.GetComponent<HungerAndThirstCalculation>();
        corporalTemperatureCalculation = player.GetComponent<CorporalTemperatureCalculation>();
        healthCalculation = player.GetComponent<HealthCalculation>();
    }
    void Start()
    {
        var root = uIDocument.rootVisualElement;
        temperatureLabel = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill").Q<Label>("temperatureLabel");
        thermometerFill = root.Q<VisualElement>("temperatureDisplay").Q<VisualElement>("temperatureFill");

        healthBarFill = root.Q<VisualElement>("heathBarDisplay").Q<VisualElement>("healthBarFill");
        healthBarLabel = root.Q<VisualElement>("heathBarDisplay").Q<Label>("healthBarLabel");

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
        circularThirstBar.color = new Color32(40, 190, 235, 255);
        circularThirstBar.background = true;
        circularThirstBar.backgroundColor = new Color32(164, 235, 255, 255);
        circularThirstBar.border = true;
        circularThirstBar.segments = true;
    }

    void Update()
    {
        
        
        DefineTemperatureLabelText();
        //DefineThermometerFillHeight();
        DefineThermometerFilllColor();

        DefineHealthBarFill();
        DefineHealthBarLabelText();
        
        DefineFoodBar();
        DefineFoodBarLabelText();
        
        DefineThirstBar();
        DefineThirstBarLabelText();
    }

    void DefineThermometerFilllColor()
    {   
        if(playerInfo.playerCorporalTemperature >= temperatureWhenColorIsFullRed)
        {
            thermometerFillColorRed = 255;
            thermometerFillColorBlue = 0;
        }
        else if(playerInfo.playerCorporalTemperature <= temperatureWhenColorIsFullBlue)
        {
            thermometerFillColorRed = 0;
            thermometerFillColorBlue = 255;
        }
        else
        {
            thermometerFillColorRed = (byte)Mathf.RoundToInt((playerInfo.playerCorporalTemperature-temperatureWhenColorIsFullBlue)*255f/(temperatureWhenColorIsFullRed-temperatureWhenColorIsFullBlue));
            //Debug.Log((byte)Mathf.RoundToInt(thermometerFillPercent*255f/90f));
            thermometerFillColorBlue = (byte)Mathf.RoundToInt(255f - ((playerInfo.playerCorporalTemperature-temperatureWhenColorIsFullBlue)*255f/(temperatureWhenColorIsFullRed-temperatureWhenColorIsFullBlue)));
            //Debug.Log((byte)Mathf.RoundToInt(255f-(thermometerFillPercent*255f/90f)));
        }

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

    void DefineHealthBarLabelText()
    {   
//   if(playerInfo.playerHealth >= 15)
//       {   
//           healthBarLabel.style.unityTextAlign = TextAnchor.MiddleRight;
//           healthBarLabel.style.right = new Length(1, LengthUnit.Pixel);
//       }
//    else
//       {
//           healthBarLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
//           healthBarLabel.style.left = new Length(1, LengthUnit.Pixel);
//       }
        healthBarLabel.text = $"{healthCalculation.roundedHealth}";
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
