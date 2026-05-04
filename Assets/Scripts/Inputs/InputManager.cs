

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TM.Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; } //avoid external functions messing around with the instance

        Dictionary<DictKey, InputListener> inputListeners = new Dictionary<DictKey, InputListener>(); //need for this dict key because multiple listeners might refer to same input (COULD BE OPTIMIZED BY STORING NAME AS KEY AND ARRAY OF ACTIONS AS VALUE, but not needed for now)
        public InputActionAsset inputActions;
        private InputActionMap playerActionMap;
        void Awake() //best "singleton" class for unity | MUST BE SET IN THE SCRIPT ORDERING AT FIRST IN UNITY SETTINGS BECAUSE THIS IS BEING CALLED IN SOME ONENABLE() SCRIPTS
        {
            if (Instance != null && Instance != this) //avoid multiple instances
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            playerActionMap = inputActions.FindActionMap("Player");
        }
        void Update()
        {
            foreach (InputListener listener in inputListeners.Values)
            {
                switch (listener.inputValueType)
                {
                    case InputValueType.Button:
                        if(listener.inputAction.WasPressedThisFrame())
                        {
                            listener.action(new InputValues());
                        };
                        break;
                    case InputValueType.Float:
                        if (listener.useTriggeredFrames)
                        {
                            if (listener.inputAction.triggered)
                            {
                                float floatValue = listener.inputAction.ReadValue<float>();
                                listener.action(new InputValues{floatValue = floatValue});                        
                            }
                            else
                            {
                                float floatValue = listener.inputAction.ReadValue<float>();
                                listener.action(new InputValues{floatValue = floatValue});                                
                            }
                        }

                        break;
                    case InputValueType.Vector2:
                        if (listener.useTriggeredFrames)
                        {
                            Vector2 vector2Value = listener.inputAction.ReadValue<Vector2>();
                            listener.action(new InputValues{vector2Value = vector2Value});                      
                        }
                        else
                        {
                            Vector2 vector2Value = listener.inputAction.ReadValue<Vector2>();
                            listener.action(new InputValues{vector2Value = vector2Value});                
                        }
                        break;
                }
            }
        }
        public void RegisterListener(string InputActionName, Action<InputValues> action, InputValueType inputValueType, bool useTriggeredFrames)
        {
            InputAction inputAction = playerActionMap.FindAction(InputActionName, true);
            this.inputListeners.Add(new DictKey{name=InputActionName, action=action},new InputListener
            {
                inputAction = inputAction,
                action = action,
                inputValueType = inputValueType,
                useTriggeredFrames = useTriggeredFrames,
            });
        }
        public void UnRegisterListener(string InputActionName, Action<InputValues> action)
        {
            DictKey key = new DictKey{name=InputActionName, action=action};
            if(!inputListeners.TryGetValue(key, out var listener))
                return;
            inputListeners.Remove(key);
        }
    }
    public class DictKey //refer to line 14 to get why this is needed
    {
        public string name;
        public Action<InputValues> action;
    }
    public class InputListener
    {
        public Action<InputValues> action; //allows Action function to take any number and types of arguments if for example its a vector2 value or just a button
        public InputAction inputAction;
        public InputValueType inputValueType;
        public bool useTriggeredFrames;
    }
    public enum InputValueType{ //to easily differenciate each input type in the input listening switch statement
        Button,
        Float,
        Vector2
    }
    public class InputValues //holds all possible input values, exandable ofc
    {
        public bool BoolValue;
        public float floatValue;
        public Vector2 vector2Value;
    }
}