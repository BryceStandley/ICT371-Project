// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""80b73e15-9285-4baf-947e-38bb3bd2d985"",
            ""actions"": [
                {
                    ""name"": ""MovementX"",
                    ""type"": ""Value"",
                    ""id"": ""90a26d6c-ea89-4e3c-8081-8f9704750eb3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""Value"",
                    ""id"": ""6ba2bebb-e94b-4785-9031-5fa51c76bc8e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MovementY"",
                    ""type"": ""Button"",
                    ""id"": ""edd92a93-80fa-4d85-ad84-107e8b438a2e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""34c8f4af-0f91-42c8-8733-71ee67deb151"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MovementX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""AD"",
                    ""id"": ""18d11985-7633-4351-a1a8-0cce9386c9e0"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementX"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""05cf7569-cb09-4dbd-817b-da2c6aefdc70"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ee4ca7c4-b6bd-4d40-934b-e6b3e89dd7ba"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bf33fb6f-68d0-4860-a258-50a7b7309f3d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58a3ce06-bdba-45c0-ae74-d123ca96d4fb"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WS"",
                    ""id"": ""c8cdefd2-469a-407c-a827-3d76d48cd908"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementY"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""bdab6f50-d551-4327-8367-d996f8519304"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MovementY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7c953df3-0efc-4dac-9894-ec9662e9cf20"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MovementY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""474d6412-ae20-49eb-8808-fbee7ffcde36"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""TestKeys"",
            ""id"": ""9afc5e94-f880-4a83-9578-c94b9d3c7aa2"",
            ""actions"": [
                {
                    ""name"": ""DialogueTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""dafa99db-dcdf-4363-a729-6d5ff89db533"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a890d7d1-3454-4ff1-8400-5305fe09c5eb"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DialogueTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MovementX = m_Player.FindAction("MovementX", throwIfNotFound: true);
        m_Player_Camera = m_Player.FindAction("Camera", throwIfNotFound: true);
        m_Player_MovementY = m_Player.FindAction("MovementY", throwIfNotFound: true);
        // TestKeys
        m_TestKeys = asset.FindActionMap("TestKeys", throwIfNotFound: true);
        m_TestKeys_DialogueTrigger = m_TestKeys.FindAction("DialogueTrigger", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MovementX;
    private readonly InputAction m_Player_Camera;
    private readonly InputAction m_Player_MovementY;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @MovementX => m_Wrapper.m_Player_MovementX;
        public InputAction @Camera => m_Wrapper.m_Player_Camera;
        public InputAction @MovementY => m_Wrapper.m_Player_MovementY;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MovementX.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementX;
                @MovementX.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementX;
                @MovementX.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementX;
                @Camera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCamera;
                @MovementY.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementY;
                @MovementY.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementY;
                @MovementY.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovementY;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MovementX.started += instance.OnMovementX;
                @MovementX.performed += instance.OnMovementX;
                @MovementX.canceled += instance.OnMovementX;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @MovementY.started += instance.OnMovementY;
                @MovementY.performed += instance.OnMovementY;
                @MovementY.canceled += instance.OnMovementY;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // TestKeys
    private readonly InputActionMap m_TestKeys;
    private ITestKeysActions m_TestKeysActionsCallbackInterface;
    private readonly InputAction m_TestKeys_DialogueTrigger;
    public struct TestKeysActions
    {
        private @InputMaster m_Wrapper;
        public TestKeysActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @DialogueTrigger => m_Wrapper.m_TestKeys_DialogueTrigger;
        public InputActionMap Get() { return m_Wrapper.m_TestKeys; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TestKeysActions set) { return set.Get(); }
        public void SetCallbacks(ITestKeysActions instance)
        {
            if (m_Wrapper.m_TestKeysActionsCallbackInterface != null)
            {
                @DialogueTrigger.started -= m_Wrapper.m_TestKeysActionsCallbackInterface.OnDialogueTrigger;
                @DialogueTrigger.performed -= m_Wrapper.m_TestKeysActionsCallbackInterface.OnDialogueTrigger;
                @DialogueTrigger.canceled -= m_Wrapper.m_TestKeysActionsCallbackInterface.OnDialogueTrigger;
            }
            m_Wrapper.m_TestKeysActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DialogueTrigger.started += instance.OnDialogueTrigger;
                @DialogueTrigger.performed += instance.OnDialogueTrigger;
                @DialogueTrigger.canceled += instance.OnDialogueTrigger;
            }
        }
    }
    public TestKeysActions @TestKeys => new TestKeysActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovementX(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnMovementY(InputAction.CallbackContext context);
    }
    public interface ITestKeysActions
    {
        void OnDialogueTrigger(InputAction.CallbackContext context);
    }
}
