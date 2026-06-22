using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using TM.Misc;

public class InventoryPlayerViewManager : MonoBehaviour
{
    [SerializeField] private float cameraDistance;
    [SerializeField] private float cameraDownAngle;
    [SerializeField] private float moveSensitivity;
    [SerializeField] private float playerYoffset;
    [SerializeField] private Camera playerViewCamera;
    [SerializeField] private GameObject dollPlayer;
    [SerializeField] private float angleX = 0;
    [SerializeField] private RenderTexture playerViewRT; 
    private VisualElement playerView;
    public void Enable(VisualElement playerView)
    {
        this.gameObject.SetActive(true);
        this.playerView = playerView;
        PlayerViewManipulator playerViewManipulator = new(this.playerView);
        playerViewManipulator.OnMoveAction += OnMoveAction;
        this.playerView.AddManipulator(playerViewManipulator);
        this.playerView.style.backgroundImage = Background.FromRenderTexture(playerViewRT);
        OnMoveAction(new Vector2(0,0));
    }
    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
    public void OnMoveAction(Vector2 delta)
    {
        this.angleX += delta.x * moveSensitivity;
        Vector3 pos = new Vector3(dollPlayer.transform.position.x, dollPlayer.transform.position.y + playerYoffset, dollPlayer.transform.position.z);
        Vector3 camPos = PlayerViewCameraMovement.CameraPosByAngle(pos, angleX, cameraDownAngle, cameraDistance);
        this.playerViewCamera.transform.position = camPos;
        this.playerViewCamera.transform.LookAt(pos);
    }
}