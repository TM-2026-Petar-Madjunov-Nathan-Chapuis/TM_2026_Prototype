using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject player);
    public InteractionType GetInteractionType();
}

public enum InteractionType
{
    Use, 
    PickUp,
    Open,
    Fuel,
}
public static class InteractionText
{
    public static string GetTextFromType(InteractionType type)
    {
        return type switch
        {
            InteractionType.Use => "Use",
            InteractionType.PickUp => "Pick Up",
            InteractionType.Open => "Open",
            InteractionType.Fuel => "Fuel",
            _ => ""
        };
    }
}