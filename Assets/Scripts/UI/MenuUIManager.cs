using System;
using TM.Input;
using TM.Inventory.UI;
using TM.Misc;
using UnityEngine;
using UnityEngine.UIElements;

namespace TM.UI
{
    public class MenuUIManager : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset inventoryTemplate;
        [SerializeField] private UIDocument uIDocument;
        [SerializeField] private BlurManager blurManager;
        [SerializeField] private Shader flipShader;
        [SerializeField] private CrosshairManager crosshairManager;
        [SerializeField] private HUDDisplay hUDDisplay;
        private Material flipMaterial;
        private InventoryManager inventoryUIManager;
        private VisualElement root;
        private VisualElement templateHolder;

        private RenderTexture captureRT;
        private RenderTexture flippedRt;
        private RenderTexture blurredRT;

        private bool isOpen = false;

        void Awake()
        {
            root = uIDocument.rootVisualElement;
            templateHolder = root.Q<VisualElement>("MenuTemplateHolder");

            var instance = inventoryTemplate.CloneTree();
            instance.style.flexGrow = 1;
            instance.style.flexShrink = 1;
            instance.style.alignSelf = Align.Stretch;
            instance.style.width = Length.Percent(100);
            instance.style.height = Length.Percent(100);

            templateHolder.Add(instance);

            inventoryUIManager = new();
            inventoryUIManager.SetRoot(templateHolder);
            inventoryUIManager.OnDisable();

            flipMaterial = new Material(flipShader);

            root.style.display = DisplayStyle.None;
            hUDDisplay.Show();
            crosshairManager.Show();
            isOpen = false;
        }

        private void OnEnable()
        {
            InputManager.Instance.RegisterListener("OpenInventory", ToggleHideShow, InputValueType.Button, true);
        }

        private void OnDisable()
        {
            InputManager.Instance.UnRegisterListener("OpenInventory", ToggleHideShow);
        }

        private void ToggleHideShow(InputValues inputValues)
        {
            isOpen = !isOpen;

            root.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;

            if (isOpen)
            {
                CursorManager.Instance.ShowCursor();
                CaptureAndBlurBackground();
                this.inventoryUIManager.OnEnable();
                GameStateManager.Instance.FreezeTime();
                hUDDisplay.Hide();
                crosshairManager.Hide();
            }
            else
            {
                GameStateManager.Instance.UnFreezeTime();
                this.inventoryUIManager.OnDisable();
                CursorManager.Instance.HideCursor();
                hUDDisplay.Show();
                crosshairManager.Show();
            }
        }

        private void CaptureAndBlurBackground()
        {
            UpdateRenderTexture();

            ScreenCapture.CaptureScreenshotIntoRenderTexture(captureRT); //capture the game view

            Graphics.Blit(captureRT, flippedRt, flipMaterial); //flip the image cause somehow its flipped.
            blurredRT = blurManager.Blur(flippedRt); //blur the image
            root.style.backgroundImage = Background.FromRenderTexture(blurredRT);//apply to background
        }

        private void UpdateRenderTexture()
        {
            if (captureRT == null || captureRT.width != Screen.width || captureRT.height != Screen.height)
            {
                if (captureRT != null)
                {
                    captureRT.Release();
                }
                captureRT = new RenderTexture(Screen.width, Screen.height, 0);
                captureRT.Create();
            }
            if (flippedRt == null || flippedRt.width != Screen.width || flippedRt.height != Screen.height)
            {
                if (flippedRt != null)
                {
                    flippedRt.Release();
                }
                flippedRt = new RenderTexture(Screen.width, Screen.height, 0);
                flippedRt.Create();
            }
        }
    }
}