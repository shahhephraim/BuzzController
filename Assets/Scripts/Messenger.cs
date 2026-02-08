using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class Messenger : MonoBehaviour
{
    [SerializeField]
    private GameObject _menu, _assessButtons, _startButton;

    private string starterPath => GetParentFolder() + "starter" + ".txt";
    private string assessPath => GetParentFolder() + "assess" + ".txt";

    private void Awake()
    {
        using(var fileStream = new FileStream(starterPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine("none");
            }
        }
        using (var fileStream = new FileStream(assessPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine("none");
            }
        }

        using (var stream = new FileStream(switchPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("none");
            }
        }
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_startButton);
    }

    public void StartCombat()
    {
        using (FileStream fileStream = new FileStream(starterPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("start");
            }
        }
        _startButton.SetActive(false);
        StartCoroutine(WaitingOnEnd());
    }

    public void Assess(bool assessment)
    {
        var text = assessment ? "right" : "wrong";

        using(var stream = new FileStream(assessPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using(var writer =  new StreamWriter(stream))
            {
                writer.WriteLine(text);
            }
        }

        _menu.SetActive(true);
        _assessButtons.SetActive(false);

        MyInput.Unmute();

        StartCoroutine(WaitingOnEnd());
    }

    private IEnumerator WaitingOnEnd()
    {
        while (true)
        {
            using (var fileStream = new FileStream(starterPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    if (reader.ReadLine() == "ended")
                    {
                        _startButton.SetActive(true);
                        StopAllCoroutines();
                    }
                }
            }

            using (var stream = new FileStream(assessPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    if (reader.ReadLine() == "asking")
                    {
                        _menu.SetActive(false);
                        _assessButtons.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(_assessButtons.transform.GetChild(0).gameObject);
                        MyInput.Mute();
                        StopAllCoroutines();
                    }
                }
            }

            yield return null;
        }
    }

    public void Switch()
    {
        using (var stream = new FileStream(switchPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("yes");
            }
        }
    }

    private string switchPath => GetParentFolder() + "switch" + ".txt";

    public static string GetParentFolder()
    {
        var path = Application.dataPath;
        var firstReached = false;

        for (int i = path.Length - 2; ; i--)
        {
            if (path[i] == '/')
            {
                if (!firstReached)
                {
                    firstReached = true;
                    continue;
                }
                return path.Remove(++i);
            }
        }
    }
}
