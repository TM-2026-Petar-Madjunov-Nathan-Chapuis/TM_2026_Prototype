using UnityEngine;
using UnityEngine.UIElements;

public class DropAreaManager : MonoBehaviour
{
    private VisualElement dropItemIcon;
    private VisualElement weightBarHolder;
    private VisualElement weightBar;
    public bool hightlighted = false;
    public void Enable(VisualElement weightbar, VisualElement weightbarholder, VisualElement icon)
    {
        this.weightBar = weightbar;
        this.weightBarHolder = weightbarholder;
        this.dropItemIcon = icon;
    }
    public void Disable()
    {
        this.hightlighted = false;
        if (this.weightBar != null) SwitchToWeightBar();
        
    }
    public void SwitchToWeightBar()
    {
        this.hightlighted = false;
        weightBarHolder.style.height = 100;
        dropItemIcon.SetEnabled(false);
        dropItemIcon.style.display = DisplayStyle.None;
        weightBar.SetEnabled(true);
        weightBar.style.display = DisplayStyle.Flex;
        weightBarHolder.style.backgroundColor = new Color(0, 0, 0, 0);
    }
    public void SwitchToDropArea()
    {
        dropItemIcon.SetEnabled(true);
        dropItemIcon.style.display = DisplayStyle.Flex;
        weightBar.SetEnabled(false);
        weightBar.style.display = DisplayStyle.None;
        weightBarHolder.style.backgroundColor = new Color(0, 0, 0, 0.4f);
    }
    public void Hightlight(Vector2 pos, Vector2Int gridSize, float cellSize)
    {
        if(0 < pos.x && pos.x < gridSize.x * cellSize && pos.y > gridSize.y * cellSize)
        {
            this.weightBarHolder.style.height = 115;
            this.hightlighted = true;
        } else if (this.hightlighted)
        {
            this.hightlighted = false;
            this.weightBarHolder.style.height = 100;
        }
    }
}