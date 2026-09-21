using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unSelectedSprite;
    public bool selected {private set; get; } = false;
    public bool visible {private set; get; } = true;
    public bool firstPerson {private set; get; } = true;
    public void Select()
    {
        this.selected = true;
        UpdateCrosshair();
    }
    public void UnSelect()
    {
        this.selected = false;
        UpdateCrosshair();
    }

    void Start()
    {
        UpdateCrosshair();
    }
    public void Hide()
    {
        this.visible = false;
        UpdateCrosshair();
    }
    public void Show()
    {
        this.visible = true;
        UpdateCrosshair();
    }
    public void FirstP()
    {
        this.firstPerson = true;
        UpdateCrosshair();
    }
    public void ThirdP()
    {
        this.firstPerson = false;
        UpdateCrosshair();
    }
    private void UpdateCrosshair()
    {
        if (visible && firstPerson)
        {
            crosshair.enabled = true;
            crosshair.sprite = selected ? selectedSprite : unSelectedSprite;
        }
        else 
        {
            crosshair.enabled = false;
        }
    }
}
