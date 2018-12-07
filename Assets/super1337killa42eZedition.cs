using UnityEngine;

public class super1337killa42eZedition : MonoBehaviour
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
    private int[,] textactiv = {
        { 7, 5, 3, 3 },
        { 6, 7, 6, 2 },
        { 1, 4, 1, 7 },
        { 9, 5, 9, 0 },
        { 2, 2, 3, 7 }
    };
    private int[,] coloractiv= {
        { 2, 8, 9, 7 },
        { 0, 4, 5, 6 },
        { 1, 5, 2, 1 },
        { 3, 4, 4, 7 },
        { 3, 3, 5, 9 }
 };


    bool isActive = false;
    bool _colorBlind = false;

    int lastDigitSolution;
    int activationCount = 0;
    int randomColor;
    int randomText;
    int solutionNumber;
    int _moduleId = 0;

    void Awake()
    {
        _moduleId++;
        GetComponent<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        GetComponent<KMNeedyModule>().OnNeedyDeactivation += OnNeedyDeactivation;
        Plunger.OnInteract += PlungerPress;
        GetComponent<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
        if (ColorBlind.ColorblindModeActive == true)
        {
            _colorBlind = true;
            cbColor.gameObject.SetActive(true);
        }
        else
            cbColor.gameObject.SetActive(false);
    }

   

    protected void OnNeedyActivation()
    {
        randomColor = Random.Range(0, 4);
        randomText = Random.Range(0, 4);
        PlungerRenderer.material = MaterialOptions[randomColor];
        buttonText.text = texts[randomText];
        if (activationCount >= 5)
            activationCount = 1;
        else
            activationCount++;
        isActive = true;
        if (randomColor == 1)
            buttonText.color = Color.white;
        else
            buttonText.color = Color.black;
        if (_colorBlind == true)
            cbColor.text = colors[randomColor];
    }

    protected void OnNeedyDeactivation()
    {
        isActive = false;
    }

    protected bool PlungerPress()
    {
        if (isActive == true)
        {
            //Calculates the Solution
            int textNumber;
            int colorNumber;
            textNumber = textactiv[activationCount, randomText];
            colorNumber = coloractiv[activationCount, randomColor];
            solutionNumber = (textNumber + colorNumber) % 10;
        }
            int bombTimeCurrent = (int)Bomb.GetTime() % 10;
            if (solutionNumber == bombTimeCurrent)
            {
                buttonPress.SetTrigger("PlungerTrigger");
                Plunger.AddInteractionPunch();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Plunger.transform);
                GetComponent<KMNeedyModule>().OnPass();
                isActive = false;
                return false;
            }
            else
            {
                GetComponent<KMNeedyModule>().OnStrike();
                isActive = false;
                LogMessage("You pressed the button at the wrong time, strike issued.");
                buttonPress.SetTrigger("PlungerTrigger");
                Plunger.AddInteractionPunch();
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Plunger.transform);
                return false;
            }
        }
        else
        {
            buttonPress.SetTrigger("PlungerTrigger");
                return false;
        }
    }

    void LogMessage(string message, params object[] parameters)
    {
        Debug.LogFormat("[The Plunger #{0}] {1}", _moduleId, string.Format(message, parameters));
    }

    protected void OnTimerExpired()
    {
        GetComponent<KMNeedyModule>().OnStrike();
        isActive = false;
    }

}