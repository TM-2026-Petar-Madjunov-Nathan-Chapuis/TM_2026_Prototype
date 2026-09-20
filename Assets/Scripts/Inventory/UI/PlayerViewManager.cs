using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using TM.Misc;
using UnityEditor.Rendering;
using TM.Inventory;
using TM.UI;

namespace TM.Inventory.UI
{
    public class PlayerViewManager : MonoBehaviour
    {
        [SerializeField] private float cameraDistance;
        [SerializeField] private float cameraDownAngle;
        [SerializeField] private float moveSensitivity;
        [SerializeField] private float playerYoffset;
        [SerializeField] private Camera playerViewCamera;
        [SerializeField] private GameObject dollPlayer;
        [SerializeField] private float angleX = 0;
        [SerializeField] private float widthSizePercentage = 25;
        [SerializeField] private float aspectRatioheightperwidth = 3;
        [SerializeField, Min(1)] private int renderScale = 2;
        public RenderTexture playerViewRT;
        private VisualElement playerView;
        private float width;
        private float height;
        public void Enable(VisualElement playerView)
        {
            this.gameObject.SetActive(true);
            this.playerView = playerView;
            PlayerViewManipulator playerViewManipulator = new(this.playerView);
            playerViewManipulator.OnMoveAction += OnMoveAction;
            this.playerView.AddManipulator(playerViewManipulator);
            this.playerView.RegisterCallback<GeometryChangedEvent>(UpdatePlayerViewSize);
            this.playerView.style.backgroundImage = Background.FromRenderTexture(playerViewRT);
            this.width = this.playerView.resolvedStyle.width;
            this.height = this.playerView.resolvedStyle.height;
            UpdatePlayerViewSize(null);
            OnMoveAction(new Vector2(0, 0));
        }
        void Update()
        {
            UpdatePlayerViewSize(null);
        }
        public void Disable()
        {
            this.gameObject.SetActive(false);
            this.playerView?.UnregisterCallback<GeometryChangedEvent>(UpdatePlayerViewSize);

        }
        public void OnMoveAction(Vector2 delta)
        {
            this.angleX += delta.x * moveSensitivity;
            Vector3 pos = new Vector3(dollPlayer.transform.position.x, dollPlayer.transform.position.y + playerYoffset, dollPlayer.transform.position.z);
            Vector3 camPos = PlayerViewCameraMovement.CameraPosByAngle(pos, angleX, cameraDownAngle, cameraDistance);
            this.playerViewCamera.transform.position = camPos;
            this.playerViewCamera.transform.LookAt(pos);
        }
        private void UpdatePlayerViewSize(GeometryChangedEvent evt)
        {
            float maxWidth = this.playerView.parent.resolvedStyle.width;
            int width = (int)(maxWidth * this.widthSizePercentage / 100);
            int height = (int)(this.aspectRatioheightperwidth * width);
            if (this.width == width && this.height == height) return;
            if (this.playerViewRT != null)
            {
                this.playerViewCamera.targetTexture = null;
                this.playerViewRT.Release();
                DestroyImmediate(this.playerViewRT, true);
            }
            this.playerViewRT = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            this.playerViewRT.width = width * this.renderScale;
            this.playerViewRT.height = height * this.renderScale;
            this.playerViewRT.filterMode = FilterMode.Bilinear;
            this.playerViewRT.Create();
            this.playerViewCamera.targetTexture = this.playerViewRT;

            this.playerView.style.height = height;
            this.playerView.style.width = width;
            this.playerView.style.backgroundImage = Background.FromRenderTexture(this.playerViewRT);
        }
    }
}