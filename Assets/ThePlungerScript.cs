using UnityEngine;

public class ThePlungerScript : MonoBehaviour
{
    /// <summary>
    /// On the Subject of The Plunger
    /// Created by Livio
    /// </summary>
   
    public KMAudio Audio;
    public KMSelectable Plunger;
    public KMNeedyModule Module;
    public KMBombInfo Bomb;
    public Renderer PlungerRenderer;
    public Material[] MaterialOptions;
    public Animator buttonPress;
    public TextMesh buttonText;
    public KMColorblindMode ColorBlind;
    public TextMesh cbColor;
    private string[] colors = { "Red", "Blue", "Green",  "Yellow" };
    private string[] texts = { "Foe", "Though", "Neat", "Need" };
    private readonly int[,] textactiv = 
    {
        { 7, 5, 3, 3 },
        { 6, 7, 6, 2 },
        { 1, 4, 1, 7 },
        { 9, 5, 9, 0 },
        { 2, 2, 3, 7 }
    };
    private readonly int[,] coloractiv= 
    {
        { 3, 3, 5, 9 },
        { 2, 8, 9, 7 },
        { 0, 4, 5, 6 },
        { 1, 5, 2, 1 },
        { 3, 4, 4, 7 }
    };

    bool _colorBlind = false;
    private bool isActive = false;

    private int lastDigitSolution;
    private int activationCount = 0;
    private int randomColor;
    private int randomText;
    private int solutionNumber;
    private static int _moduleIdCounter = 1;
    private int _moduleId;

    void Awake()
    {
        _moduleId = _moduleIdCounter++;
        GetComponent<KMNeedyModule>().OnNeedyActivation += delegate
        {
            OnNeedyActivation();
        };
        GetComponent<KMNeedyModule>().OnNeedyDeactivation += delegate
        {
            OnNeedyDeactivation();
        };

        Plunger.OnInteract += delegate
        {
            PlungerPress();
            return false;
        };

        GetComponent<KMNeedyModule>().OnTimerExpired += delegate
        {
            OnTimerExpired();
        };

        if(ColorBlind.ColorblindModeActive == true)
        {
            _colorBlind = true;
            cbColor.gameObject.SetActive(true);
        }
        else
        {
            cbColor.gameObject.SetActive(false);
        }

    }

    private void OnNeedyActivation()
    {
        randomColor = Random.Range(0, 4);
        randomText = Random.Range(0, 4);
        PlungerRenderer.material = MaterialOptions[randomColor];
        buttonText.text = texts[randomText];
        if (activationCount >= 5)
        {
            activationCount = 1;
        }
        else
        {
            activationCount++;
        }

        isActive = true;

        if(randomColor == 1)
        {
            buttonText.color = Color.white;
        }
        else
        {
            buttonText.color = Color.black;
        }

        if(ColorBlind == true)
        {
            cbColor.text = colors[randomColor];
        }

        int textNumber;
        int colorNumber;

        textNumber = textactiv[(int)activationCount - 1, (int)randomText];
        colorNumber = coloractiv[(int)activationCount - 1, (int)randomColor];

        solutionNumber = (textNumber + colorNumber) % 10;
        LogMessage("The plungers color is: {0}. And text is {1}.", colors[randomColor], texts[randomText]);
        LogMessage("Module activated. The module has activated {0} time(s). The correct number is: {1}", activationCount, solutionNumber);
    }

    private void OnNeedyDeactivation()
    {
        isActive = false;
    }

    private void OnTimerExpired()
    {
        GetComponent<KMNeedyModule>().HandleStrike();
        isActive = false;
    }
    
    private void PlungerPress()
    {
        if(isActive == true)
        {
            int bombTimeCurrent = (int)Bomb.GetTime() % 10;
            if(solutionNumber == bombTimeCurrent)
            {
                buttonPress.SetTrigger("PlungerTrigger");
                Plunger.AddInteractionPunch();
                GetComponent<KMNeedyModule>().HandlePass();
                LogMessage("You pressed the button at the correct time, module deactivated {0}.", Bomb.GetTime());
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Plunger.transform);
                isActive = false;
            }
            else
            {
                GetComponent<KMNeedyModule>().HandleStrike();
                isActive = false;
                GetComponent<KMNeedyModule>().HandlePass();
                LogMessage("You pressed the button at the wrong time, strike issued {0}.", Bomb.GetTime());
                buttonPress.SetTrigger("PlungerTrigger");
                Plunger.AddInteractionPunch();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Plunger.transform);
            }
        }
        else
        {
            buttonPress.SetTrigger("PlungerTrigger");
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Plunger.transform);
            Plunger.AddInteractionPunch();
        }

    }

    void LogMessage(string message, params object[] parameters)
    {
        Debug.LogFormat("[The Plunger #{0}] {1}", _moduleId, string.Format(message, parameters));
    }
}