using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Manager_Menu : MonoBehaviour
{

    #region Singleton
    public static Manager_Menu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(Manager_Menu)) as Manager_Menu;
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static Manager_Menu instance;
    #endregion

    private static bool hasSplashScreenloaded = false;
    private static float volume_SFX = 0.5f;
    private static float volume_Music = 0.5f;

    public Enums.MenuState menuState = Enums.MenuState.MENU_MAIN;
    private Coroutine lerpCoroutine = null;
    private Coroutine splashCoroutine = null;
    private int menuPanelIndex = 1;
    private bool areOptionsSelected = false;

    public static Action<float> event_onBarValueChanged = null;

    private Image countdownDigits = null, countdownFight = null;
    private List<Sprite> countdownSprites = new List<Sprite>();
    private Coroutine countdownCoroutine = null;

    private AudioSource audioSource = null;

    [SerializeField]
    private bool debug1Player = false;
    [SerializeField]
    private float lerpDuration = 0.0f;
    [SerializeField]
    private float splashScreenFadeTime = 0.0f;
    [SerializeField]
    private GameObject playButton = null;
    [SerializeField]
    private GameObject mainMenuContainer = null;
    [SerializeField]
    private Image splashBackground = null, splashLogo = null;
    [SerializeField]
    private List<Image> mainMenuImages = new List<Image>();
    [SerializeField]
    private List<Text> mainMenuTexts = new List<Text>();
    [SerializeField]
    private List<Text> splashTexts = new List<Text>();
    [SerializeField]
    private List<Transform> menuPanelPos = new List<Transform>();
    [SerializeField]
    private GameObject volumeSFXSlider = null;
    [SerializeField]
    private Renderer logoAnim = null;

    [SerializeField]
    private AudioSource mainMenuMusic = null;
    [SerializeField]
    private AudioClip navigationClick = null;
    [SerializeField]
    private AudioClip selectionClick = null;
    [SerializeField]
    private AudioClip playerSelect = null;
    [SerializeField]
    private AudioClip backClick = null;
    [SerializeField]
    private AudioClip gameStart = null;
    [SerializeField]
    private AudioClip gameMusic = null;


    private void Awake()
    {
        SceneManager.activeSceneChanged += LoadNewSceneMethod;
        Init();
        audioSource = this.GetComponent<AudioSource>();
    }


    private void Init()
    {
        if (menuState.Equals(Enums.MenuState.MENU_MAIN))
        {
            ConnexionMenu(0.0f, 0.0f);
            leftPlayerReadyQuestion.CrossFadeAlpha(0.0f, 0.0f, true);
            rightPlayerReadyQuestion.CrossFadeAlpha(0.0f, 0.0f, true);
            splashLogo.transform.parent.gameObject.SetActive(true);

            if (!hasSplashScreenloaded)
            {
                splashLogo.CrossFadeAlpha(0.0f, 0.0f, true);
                splashCoroutine = StartCoroutine(SplashCoroutine());
            }
            else
            {
                splashBackground.CrossFadeAlpha(0.0f, 1.0f, true);
                splashLogo.CrossFadeAlpha(0.0f, 0.0f, true);
                splashTexts.ForEach(st => st.CrossFadeAlpha(0.0f, 0.0f, true));
            }
        }
        else if (menuState.Equals(Enums.MenuState.MENU_GAME))
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);

            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }
    }

    private void LoadNewSceneMethod(Scene src, Scene dest)
    {
        DontDestroyOnLoad(this);

        /*if (menuState.Equals (Enums.MenuState.MENU_CONNECTION)) 
		{
			RefContainer refs = FindObjectOfType<RefContainer> ();
			leftAButton = refs.leftAButton;
			rightAButton = refs.rightAButton;
			leftReadyQuestion = refs.leftReadyQuestion;
			rightReadyQuestion = refs.rightReadyQuestion;
			leftReady = refs.leftReady;
			rightReady = refs.rightReady;

			leftReady.CrossFadeAlpha (0.0f, 0.0f, true);
			rightReady.CrossFadeAlpha (0.0f, 0.0f, true);

			if (debug1Player) 
			{
				isRightReady = true;
				rightReady.CrossFadeAlpha (1.0f, 0.15f, true);
				rightReadyQuestion.CrossFadeAlpha (0.0f, 0.15f, true);
				rightAButton.CrossFadeAlpha (0.0f, 0.15f, true);

			}
		}*/

        if (menuState.Equals(Enums.MenuState.MENU_GAME))
        {
            RefContainer refs = FindObjectOfType<RefContainer>();

            countdownDigits = refs.countdownDigits;
            countdownFight = refs.countdownFight;
            countdownSprites.Add(refs.countdown3);
            countdownSprites.Add(refs.countdown2);
            countdownSprites.Add(refs.countdown1);
            Invoke("Init", 0.1f);
        }
    }

    private bool IsInputAvailable()
    {
        return hasSplashScreenloaded;
    }

    private void Update()
    {
        #region MAIN MENU
        if (menuState.Equals(Enums.MenuState.MENU_MAIN))
        {
            if (Input.GetButtonDown("P1BButton"))
            {
                Back();
                audioSource.clip = selectionClick;
                audioSource.Play();
            }

            if (areOptionsSelected)
            {
                float horizontal = Input.GetAxis("P1DirectionX");
                if (horizontal > 0.5f || horizontal < -0.5f)
                {
                    event_onBarValueChanged(horizontal * 0.025f);
                    audioSource.clip = navigationClick;
                    audioSource.Play();
                }
            }
        }

        #endregion

        #region CONNECTION
        if (menuState.Equals(Enums.MenuState.MENU_CONNECTION))
        {
            if (Input.GetButtonDown("P1BButton"))
            {
                if (isLeftReady)
                {
                    isLeftReady = false;
                    leftPlayerReady.CrossFadeAlpha(0.0f, 0.15f, true);
                    leftPlayerReadyQuestion.CrossFadeAlpha(1.0f, 0.15f, true);
                }
                else
                {
                    menuState = Enums.MenuState.MENU_MAIN;
                    //SceneManager.LoadScene ("MainMenu");
                    ConnexionMenu(0.0f, 0.15f);
                    MainMenu(1.0f, 0.2f);
                    EventSystem.current.SetSelectedGameObject(playButton);
                }
                audioSource.clip = backClick;
                audioSource.Play();
            }
            if (Input.GetButtonDown("P2BButton"))
            {
                if (isRightReady)
                {
                    isRightReady = false;
                    rightPlayerReady.CrossFadeAlpha(0.0f, 0.15f, true);
                    rightPlayerReadyQuestion.CrossFadeAlpha(1.0f, 0.15f, true);
                }
                else
                {
                    menuState = Enums.MenuState.MENU_MAIN;
                    ConnexionMenu(0.0f, 0.15f);
                    MainMenu(1.0f, 0.2f);
                    EventSystem.current.SetSelectedGameObject(playButton);
                    //SceneManager.LoadScene ("MainMenu");
                }
                audioSource.Play();
            }
            if (Input.GetButtonDown("P1AButton"))
            {
                if (!isLeftReady)
                {
                    isLeftReady = true;
                    leftPlayerReady.CrossFadeAlpha(1.0f, 0.15f, true);
                    leftPlayerReadyQuestion.CrossFadeAlpha(0.0f, 0.15f, true);
                    audioSource.clip = playerSelect;
                    audioSource.Play();

                    if (isRightReady)
                    {
                        audioSource.clip = gameStart;
                        audioSource.Play();

                        mainMenuMusic.clip = gameMusic;
                        mainMenuMusic.Play();
                        menuState = Enums.MenuState.MENU_GAME;
                        Invoke("LoadGameScene", 0.25f);
                    }
                }
            }

            if (Input.GetButtonDown("P2AButton"))
            {
                if (!isRightReady)
                {
                    isRightReady = true;
                    rightPlayerReady.CrossFadeAlpha(1.0f, 0.15f, true);
                    rightPlayerReadyQuestion.CrossFadeAlpha(0.0f, 0.15f, true);
                    audioSource.clip = playerSelect;
                    audioSource.Play();

                    if (isLeftReady)
                    {
                        audioSource.clip = gameStart;
                        audioSource.Play();

                        mainMenuMusic.clip = gameMusic;
                        mainMenuMusic.Play();
                        menuState = Enums.MenuState.MENU_GAME;
                        Invoke("LoadGameScene", 0.25f);
                    }
                }
            }
        }
        #endregion
    }

    #region MAIN MENU

    public void Play()
    {
        if (!IsInputAvailable())
            return;

        //Invoke ("LoadConnectionScene", 0.25f);
        LoadConnectionScene();
    }

    private void LoadConnectionScene()
    {
        //SceneManager.LoadScene ("ConnexionScene");
        EventSystem.current.SetSelectedGameObject(null);
        ConnexionMenu(1.0f, 0.25f);
        MainMenu(0.0f, 0.1f);
        Invoke("TransitionToConnexion", 0.15f);
    }

    private void TransitionToConnexion()
    {
        menuState = Enums.MenuState.MENU_CONNECTION;
        if (debug1Player)
        {
            isRightReady = true;
            rightPlayerReady.CrossFadeAlpha(1.0f, 0.15f, true);
            rightPlayerReadyQuestion.CrossFadeAlpha(0.0f, 0.15f, true);
        }
    }
    public void Options()
    {
        if (!IsInputAvailable())
            return;

        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpPanelTo(-1));
        EventSystem.current.SetSelectedGameObject(volumeSFXSlider);
        areOptionsSelected = true;
    }

    public void Credits()
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpPanelTo(1));
        EventSystem.current.SetSelectedGameObject(null);
        areOptionsSelected = false;
    }

    public void Back()
    {

        if (!IsInputAvailable() || menuPanelIndex == 1)
            return;
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpPanelTo(menuPanelIndex > 1 ? -1 : 1));
        EventSystem.current.SetSelectedGameObject(playButton);
        areOptionsSelected = false;
    }

    public void Exit()
    {
        if (!IsInputAvailable())
            return;
        Application.Quit();
    }

    public void AdjustVolumeSFX(float value)
    {
        volume_SFX = value;
    }

    public void AdjustVolumeMusic(float value)
    {
        volume_Music = value;
        mainMenuMusic.volume = volume_Music;
    }

    public void AdjustGraphicsBrightness(float value)
    {
        Manager_Camera.Instance.brightness = value;
    }

    public void AdjustGraphicsContrast(float value)
    {
        Manager_Camera.Instance.contrast = value;
    }

    private IEnumerator LerpCameraTo(Vector3 targetPos)
    {
        float currentLerpTime = 0.0f;
        Vector3 startPos = Camera.main.transform.position;

        while (currentLerpTime < lerpDuration)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / lerpDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            Camera.main.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator LerpPanelTo(int mod)
    {
        menuPanelIndex += mod;
        float currentLerpTime = 0.0f;
        Vector3 startPos = mainMenuContainer.transform.position;
        Vector3 targetPos = menuPanelPos[menuPanelIndex].position;

        while (currentLerpTime < lerpDuration)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / lerpDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            mainMenuContainer.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator SplashCoroutine()
    {
        float currentLerpTime = 0.0f;

        float fadeInTimer = 1.5f;
        while (currentLerpTime < fadeInTimer)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / fadeInTimer;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            float targetAlpha = Mathf.Lerp(0.0f, 1.0f, t);
            splashLogo.canvasRenderer.SetAlpha(targetAlpha);
            splashTexts.ForEach(st => st.canvasRenderer.SetAlpha(targetAlpha));
            float targetAlpha2 = Mathf.Lerp(0.0f, 1.0f, t);
            logoAnim.material.color = new Color(logoAnim.material.color.r, logoAnim.material.color.g, logoAnim.material.color.b, targetAlpha2);
            logoAnim.gameObject.GetComponent<MenuMoviePlayer>().Play();

            yield return null;
        }

        currentLerpTime = 0.0f;
        yield return new WaitForSeconds(1.5f);

        while (currentLerpTime < splashScreenFadeTime)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / splashScreenFadeTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            float targetAlpha = Mathf.Lerp(1.0f, 0.0f, t);
            splashLogo.canvasRenderer.SetAlpha(targetAlpha);
            splashTexts.ForEach(st => st.canvasRenderer.SetAlpha(targetAlpha));
            splashBackground.canvasRenderer.SetAlpha(targetAlpha);
            logoAnim.material.color = new Color(logoAnim.material.color.r, logoAnim.material.color.g, logoAnim.material.color.b, targetAlpha);
            yield return null;
        }

        currentLerpTime = 0.0f;

        while (currentLerpTime < splashScreenFadeTime)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / splashScreenFadeTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            float targetAlpha = Mathf.Lerp(1.0f, 0.0f, t);

            yield return null;
        }

        hasSplashScreenloaded = true;
        yield return null;
    }

    #endregion

    #region CONNECTION MENU

    [SerializeField]
    private Image leftPlayerReady = null, leftPlayerReadyQuestion = null;
    [SerializeField]
    private Image rightPlayerReady = null, rightPlayerReadyQuestion = null;
    [SerializeField]
    private Image controllerMapping = null, connectionBackground = null;
    [SerializeField]
    private Text p1Text = null, p2Text = null;
    private bool isLeftReady = false, isRightReady = false;

    private void LoadGameScene()
    {
        menuState = Enums.MenuState.MENU_GAME;
        SceneManager.LoadScene("GameScene");
    }

    private void ConnexionMenu(float alpha, float duration)
    {
        if (connectionBackground)
            connectionBackground.CrossFadeAlpha(alpha, duration, true);
        controllerMapping.CrossFadeAlpha(alpha, duration, true);
        leftPlayerReadyQuestion.CrossFadeAlpha(alpha, duration, true);
        rightPlayerReadyQuestion.CrossFadeAlpha(alpha, duration, true);
        leftPlayerReady.CrossFadeAlpha(0.0f, duration, true);
        rightPlayerReady.CrossFadeAlpha(0.0f, duration, true);
        p1Text.CrossFadeAlpha(alpha, duration, true);
        p2Text.CrossFadeAlpha(alpha, duration, true);
    }

    private void MainMenu(float alpha, float duration)
    {
        mainMenuImages.ForEach(mmr => mmr.CrossFadeAlpha(alpha, duration, true));
        mainMenuTexts.ForEach(mmr => mmr.CrossFadeAlpha(alpha, duration, true));
    }

    #endregion

    #region GAME MENU

    private IEnumerator CountdownCoroutine()
    {
        Manager_Game.Instance.PauseGame();

        for (int i = 0; i < 3; ++i)
        {
            float currentLerpTime = 0.0f;
            Vector3 startingSize = new Vector3(2.5f, 2.5f, 2.5f);
            Vector3 targetSize = Vector3.one;
            float startingAlpha = 0.0f;
            float targetAlpha = 1.0f;

            countdownDigits.sprite = countdownSprites[i];
            countdownDigits.transform.localScale = startingSize;
            countdownDigits.color = new Color(countdownDigits.color.r, countdownDigits.color.g, countdownDigits.color.b, startingAlpha);
            countdownDigits.enabled = true;

            yield return StartCoroutine(Utils.WaitForRealSeconds(0.15f));

            while (currentLerpTime < 0.5f)
            {
                currentLerpTime += Time.unscaledDeltaTime;
                float t = currentLerpTime / 0.5f;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                Vector3 curSize = Vector3.Lerp(startingSize, targetSize, t);
                countdownDigits.transform.localScale = curSize;

                float curAlpha = Mathf.Lerp(startingAlpha, targetAlpha, t);
                countdownDigits.color = new Color(countdownDigits.color.r, countdownDigits.color.g, countdownDigits.color.b, curAlpha);

                yield return null;
            }

            countdownDigits.color = new Color(countdownDigits.color.r, countdownDigits.color.g, countdownDigits.color.b, targetAlpha);
            yield return StartCoroutine(Utils.WaitForRealSeconds(0.35f));
            countdownDigits.enabled = false;

            yield return null;
        }

        countdownFight.CrossFadeAlpha(0.0f, 0.0f, true);
        countdownFight.CrossFadeAlpha(1.0f, 0.1f, true);
        countdownFight.enabled = true;
        yield return StartCoroutine(Utils.WaitForRealSeconds(0.35f));
        countdownFight.CrossFadeAlpha(0.0f, 0.25f, true);

        Manager_Game.Instance.UnpauseGame();
        Manager_Game.Instance.StartGame();
        yield return null;
    }

    #endregion
}
