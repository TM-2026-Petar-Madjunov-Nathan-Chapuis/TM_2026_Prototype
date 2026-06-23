using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThermometerDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    [SerializeField] private CorporalTemperatureCalculation corporalTemperatureCalculation;
    [SerializeField] private Label temperatureLabel;
    [SerializeField] private VisualElement thermometerFill;
    [SerializeField] private float thermometerFillPercent;
    [SerializeField] private float localPlayerCorporalTemperature;
    private byte thermometerFillColorRed;
    private byte thermometerFillColorGreen = 0;
    private byte thermometerFillColorBlue;
    private byte thermometerFillColorOpacity = 255;
    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;


    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
    }
    void Start()
    {
        var root = uIDocument.rootVisualElement;
        temperatureLabel = root.Q<Label>("temperatureLabel");
        thermometerFill = root.Q<VisualElement>("thermometerTube").Q<VisualElement>("thermometerFill");
    }

    // Update is called once per frame
    void Update()
    {
        thermometerFillPercent = playerInfo.playerCorporalTemperature*2;
        
        DefineTemperatureLabelText();
        DefineThermometerFillHeight();
        DefineThermometerFilllColor();
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
}
