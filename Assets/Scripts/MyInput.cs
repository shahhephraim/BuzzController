using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyInput : MonoBehaviour
{
    [SerializeField]
    private InputAction _muteInputAction, _unmuteInputAction;

    private static InputAction[] inputActions;

    [SerializeField]
    private InputAction[] _inputActions;

    [SerializeField]
    private TextMeshProUGUI _indicator;

    private static TextMeshProUGUI indicator;

    private static bool isMuted = false, isUserMuted = true;

    private void Awake()
    {
        using (var stream = new FileStream(Messenger.GetParentFolder() + "press" + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("none");
            }
        }


        indicator = _indicator;
        inputActions = _inputActions;


        foreach (var action in inputActions)
        {
            action.Enable();
        }
        _muteInputAction.Enable();
        _unmuteInputAction.Enable();
    }

    public static void Mute() => isMuted = true;

    public static void Unmute() => isMuted = false;

    private void Update()
    {
        if (_muteInputAction.IsPressed())
        {
            isUserMuted = true;
            indicator.text = "MUTED";
        }
        if (_unmuteInputAction.IsPressed())
        {
            isUserMuted = false;
            indicator.text = "UNMUTED";
        }

        if (isUserMuted)
        {
            return;
        }

        if (isMuted)
        {
            return;
        }

        for (int i = 0; i < inputActions.Length; i++)
        {
            if (inputActions[i].IsPressed())
            {
                using (var stream = new FileStream(Messenger.GetParentFolder() + "press" + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(i.ToString());
                    }
                }
            }
        }
    }
}
