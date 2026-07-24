public interface IInteractable
{
    public void Interact();
    public InteractionType GetInteractionType();
}

public enum InteractionType
{
    Use, 
    PickUp,
    Open,
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
            _ => ""
        };
    }
}