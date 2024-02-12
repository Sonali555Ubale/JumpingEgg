using GooglePlayGames;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{

	public static GameManager instance;

	private int score = 0;
	private int coins = 0;
	private int highscore = 0;
	private float highscoreTimer = 0f;
	public int counter { get; private set; }


	[SerializeField]
	private TMP_Text scoreText;
	[SerializeField]
	private TMP_Text highScoreText;
	[SerializeField]
	private TMP_Text coinsText;
	[SerializeField]
	private TMP_Text timerText;
	[SerializeField]
	private GameObject OverlayImage = null;

	public Text achText;
	public Text xTimesText;

	private int xTimes = 0;
	private int timesDead = 0;
	public string uiTag;
	public Animator achAnimator;
	public GameObject pauseMenuPanel;
	public Image pauseBtn;
	public Image restartBtn;
	public Image playtBtn;
	public Cinemachine.CinemachineVirtualCamera PlayerCamera;
	[SerializeField] CameraTimeMode cameraTimeMode;
	[SerializeField] Animator animatorCannotSeeOverlay;
	[SerializeField] Animator animatorSceneTransition;

	public Image timeBar;

	public Image imgComment;
	[SerializeField]
	private TMP_Text textComment;

	private float maxTime = 3f;
	private float timeLeft = 0f;
	private float totalAnimationTime = 6f;

	[SerializeField] private BowlController bowlController;
	[SerializeField] BranchSpawner branchSpawner;
	[SerializeField] GameObject wallBottom;


	/// ////////////////////////// PRogressin logic here //////////////////


	[SerializeField]
	private static float RateAtWhichCountDecreases = 2;
	private readonly int maxBowlsPerSpawn = 5;
	[SerializeField]
	private static int MinSpawn = 2;

	/// ///////////////////////////////

	// Player details
	Rigidbody2D player;

	// Bowl details
	public bool lastBowlWasStationary = false;
	public float lastBowlYPosition = -Mathf.Infinity;

	private string strAchievement;
	float timer;


	// TIME ATTACK mode variables
	public GameObject bgTimeAttack;


	// Declaring PlayerPrefs Constants
	private const string PLAYER_DEAD = "player_dead";
	private const string PLAYER_HIGHSCORE = "player_highscore";
	private const string PLAYER_HIGHSCORE_TIMER = "player_highscore_timer";
	private const string PLAYER_COINS = "player_coins";
	private const string IS_TUTORIAL_COMPLETE = "is_tutorial_complete";

	private bool isTutorialComplete = false;

	// Scene Management
	GameMode currentScene;

	float lastX = 0f;
	float wallPositionY = -1f;

	public float WallPositionY { get => wallPositionY; set => wallPositionY = value; }

	private float playerToWallDistance = 0f;
	float normalizedDistance = 0f;

	Color colorStart = new Color32(0, 151, 167, 255);
	Color colorEnd = new Color32(178, 235, 242, 255);


	float totalTimeElapsed = 0f;
	private int SCORE_INCREMENT_VALUE = 10;

	float commentTimeLeft = 3f;
	private string[] playerCommentTimeAttack = { "Hurry up!!!", "Time is Running!", "Tick\nTock!", "Faster!", "You can do this!" };

	string[] positiveComments = { "Yeah!", "Nice", "Good One!", "Going Great!", "Easy!", "Awesome" };

	string[] cannotSeeComments = { "I can\'t see!", "Turn me straight", "So dark here!", "Its difficult to see further" };

	public enum GameMode
	{
		SPLASH_SCREEN,
		MENU,
		ENDLESS,
		TIME_ATTACK,
		TUTORIAL
	}
	public PauseMenuController pauseMenuController = null;
	private void Start()
	{
		// Tutorial completion status
		getIsTutorialComplete();
		//	Debug.LogError("build index is:" + SceneManager.GetActiveScene().buildIndex);
		score = 0;
		achAnimator = GetComponentInParent<Animator>();
		if (SceneManager.GetActiveScene().buildIndex < 3 && OverlayImage != null)
			OverlayImage.SetActive(false);

		// BG ANIMATOR
		/*if (bgTimeAttack == null)
			bgTimeAttack = new Animator();
		else
			bgTimeAttack.speed = 0.5f;			// animTime = 3, 0.5 is 3 * 2 which is the totalAnimationTime
*/
		if (xTimesText != null)
		{
			xTimesText.gameObject.SetActive(false);
			xTimes = 0;
		}
		//timeBar = GetComponent<Image>();
		//timeLeft = maxTime;

		switch (SceneManager.GetActiveScene().buildIndex)
		{
			case 0: currentScene = GameMode.SPLASH_SCREEN; break;
			case 1: currentScene = GameMode.MENU; break;
			case 2: currentScene = GameMode.ENDLESS; break;
			case 3: currentScene = GameMode.TIME_ATTACK; break;
			case 4: currentScene = GameMode.TUTORIAL; break;
		}
		//Debug.LogError("build index is:" + SceneManager.GetActiveScene().buildIndex);

		//Start the game
		playGame();
	}

	void Awake()
	{

		highscore = PlayerPrefs.GetInt(PLAYER_HIGHSCORE);
		highscoreTimer = PlayerPrefs.GetFloat(PLAYER_HIGHSCORE);
		coins = PlayerPrefs.GetInt(PLAYER_COINS);
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			//DontDestroyOnLoad(gameObject);
		}
	}

	private void OnApplicationQuit()
	{
		score = 0;
	}

	private void OnDestroy()
	{
		score = 0;
	}


	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				// TODO Show Pause menu
				showPauseMenu(false);

				// Temporarily just go back to main screen
				//SceneManager.LoadScene(0);

				if (SceneManager.GetActiveScene().buildIndex == 0)
				{
					// TODO Show exit menu

					// Exit the application
					Application.Quit();
				}
				return;
			}
		}

		switch (currentScene)
		{
			case GameMode.TUTORIAL: gameModeTutorial(); break;
			case GameMode.ENDLESS: gameModeEndless(); break;
			case GameMode.TIME_ATTACK: gameModeTimeAttack(); break;
		}
	}

	public bool isPauseOrPlayTouched()
	{
		return isGamePaused;
		/*bool isTrue = false;

		float x = Input.mousePosition.x;
		float y = Input.mousePosition.y;


		RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), -Vector2.up);
		if (hit.collider != null)
		{
			Debug.Log(hit.collider.name);
			isTrue = hit.collider.name.Equals(playtBtn.name) || hit.collider.name.Equals(pauseBtn.name);
		}
		return isTrue;*/
	}


	public bool isGamePaused = false;
	public void showPauseMenu(bool isDead)
	{
		isGamePaused = true;
		if (!isDead)
			Time.timeScale = 0f;
		else
		{
			// Stop moving camera
			if (PlayerCamera != null)
				PlayerCamera.m_Follow = null;
			else
			{
				cameraTimeMode.StopCameraFollow();
			}

		}
		/*else
		{
			Time.timeScale = 0.01f;
		}*/

		if (currentScene == GameMode.ENDLESS)
		{
			pauseBtn.gameObject.SetActive(false);
			playtBtn.gameObject.SetActive(!isDead);
		}

		pauseMenuPanel?.SetActive(true);
		restartBtn.gameObject.SetActive(isDead);

		// Load banner ad
		AdController.instance.showBannerAd();
	}

	public void playGame()
	{
		if (getGameMode() == GameMode.TUTORIAL) return;
		AudioManager.instance.Play("Bg");
		Time.timeScale = 1f;
		if (pauseMenuPanel != null)
		{
			pauseMenuPanel.SetActive(false);
		}
		if (currentScene == GameMode.ENDLESS)
		{
			pauseBtn.gameObject.SetActive(true);
			branchSpawner.AddNewBranches(0f);

			bowlController?.Create();
		}
		if (AdController.instance != null)
			AdController.instance.hideBannerAd();
		StartCoroutine(resumeClick());
	}

	private IEnumerator resumeClick()
	{
		yield return new WaitForSeconds(0.5f);
		isGamePaused = false;
	}

	public void restartGame()
	{
		isGamePaused = false;
		Time.timeScale = 1f;
		pauseMenuPanel.SetActive(false);
		if (currentScene == GameMode.ENDLESS)
			pauseBtn.gameObject.SetActive(true);
		else
		{
			cameraTimeMode.ResetCameraPosition();
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		AdController.instance.hideBannerAd();
	}

	public void showHome()
	{
		AdController.instance.hideBannerAd();
		updateToLeaderboard();
		SceneManager.LoadScene((int)GameMode.MENU);
	}


	private bool wasLeaderBoardRequested;
	public void ShowLeaderboards()
	{
		if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			wasLeaderBoardRequested = false;
			PlayGamesPlatform.Instance.ShowLeaderboardUI();
		}
		else
		{
			wasLeaderBoardRequested = true;
			PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
		}
	}
	public void SignInCallback(bool success)
	{
		if (success)
		{
			if (wasLeaderBoardRequested)
			{
				ShowLeaderboards();
			}
		}
	}


	private void gameModeTutorial()
	{


	}

	private void gameModeEndless()
	{
		//timer += Time.deltaTime;
		//float seconds = timer % 60;

		updateScore();

		//Debug.Log(player.transform.localRotation.z);

		/*if (achAnimator.GetBool("show_ach"))
		{
			//Debug.Log(achAnimator.GetBool("show_ach"));

			//if () 
			achAnimator.SetBool("show_ach", false);
		}*/

		// Handle time bar ui
		if (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			timeBar.fillAmount = timeLeft / maxTime;
			(xTimesText as Text).text = "" + xTimes + "x";


			if (lastX != xTimes)
			{
				lastX = xTimes;
				//-10 because the score will be already increased as this function is called from CheckEggController
				increaseScore((xTimes * SCORE_INCREMENT_VALUE) - SCORE_INCREMENT_VALUE);
			}

		}
		else
		{
			lastX = 0f;
			timeBar.fillAmount = 0f;
			xTimes = 0;
			xTimesText.gameObject.SetActive(false);
			//Time.timeScale = 0;  // Stops the game
		}
	}

	private void gameModeTimeAttack()
	{
		updateScore();
		if (player == null) return;
		//totalTimeElapsed += Time.deltaTime * 1000;
		totalTimeElapsed += Time.deltaTime * 1000;
		highscoreTimer = totalTimeElapsed;
		timerText.text = FormatTime(totalTimeElapsed);

		if (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
		}


		/*else if (timeLeft <= 0)
		{
			// playerDead();
		}*/

		// Change background color from white to red as distance from player to the bottom wall reduces
		//Debug.Log("wall " + WallPositionY + "      player " + player.position.y);
		// Player position is always greater than position of the wall
		//Mathf.PingPong(Time.time, 1);
		if (WallPositionY == -1f) return;
		playerToWallDistance = WallPositionY - player.position.y;
		normalizedDistance = playerToWallDistance / -10;
		//Debug.Log(normalizedDistance);
		if (currentScene == GameMode.TIME_ATTACK)
			bgTimeAttack.GetComponent<SpriteRenderer>().material.color = Color.Lerp(colorStart, colorEnd, normalizedDistance + 0.1f);


		Debug.Log(timeLeft);
		if (normalizedDistance <= 0.2f)
		{
			if (commentTimeLeft <= 0)
			{
				showPlayerDialog(playerCommentTimeAttack[UnityEngine.Random.Range(0, 5)]);
				commentTimeLeft = 3f;
			}
			else
			{
				commentTimeLeft -= Time.deltaTime;
			}
		}
	}


	public string FormatTime(float time)
	{
		int minutes = (int)time / 60000;
		int seconds = (int)time / 1000 - 60 * minutes;
		int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
		return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
	}


	///////////// Common Funtions ////////////////


	private void updateScore()
	{
		coinsText.text = " " + coins;
		scoreText.text = "Score " + score;
		highScoreText.text = "Highscore " + highscore;
		/*Debug.Log("Score Is::::" + score);
		Debug.Log("HIGHScore Is::::" + highscore);
		Debug.Log("Coins::::" + coins);*/

	}

	public void increaseCoin()
	{
		coins++;
		//AudioManager.instance.Play("coin2");
		PlayerPrefs.SetInt(PLAYER_COINS, coins);
		PlayerPrefs.Save();
	}

	public void rewardCoins()
	{
		coins += 50;
		PlayerPrefs.SetInt(PLAYER_COINS, coins);
		PlayerPrefs.Save();
		AudioManager.instance.Play("coin2");
		//StartCoroutine(PlayCoinSound(0.5f));

		// Continue from last position

	}

	IEnumerator PlayCoinSound(float duration)
	{
		yield return new WaitForSeconds(duration);
	}

	public int getCurrentScore()
	{
		return score;
	}


	public void increaseScore(int bonus)
	{
		if (bonus != -1)
		{
			score += bonus;
		}
		else
		{
			counter += SCORE_INCREMENT_VALUE;
			score += SCORE_INCREMENT_VALUE;

			// Spawn new branches
			if (currentScene != GameMode.TIME_ATTACK)
			{

				if (counter % 30 == 0)
					branchSpawner?.AddNewBranches(4f);

			}
		}

		if (highscore < score)
		{                                                  // storing highscore
			highscore = score;
			PlayerPrefs.SetInt(PLAYER_HIGHSCORE, highscore);
			PlayerPrefs.Save();

			updateToLeaderboard();
		}


		StartCoroutine(waitAndShowComment());
	}

	IEnumerator waitAndShowComment()
	{

		//Debug.Log("Start");

		float playerRotation = player.transform.localRotation.z;
		yield return new WaitForSeconds(0.1f);


		if (playerRotation < 0.50f && playerRotation > -0.50f)
		{
			if (UnityEngine.Random.Range(0, 10) < 2)
				showPlayerDialog(positiveComments[UnityEngine.Random.Range(0, 6)]);

			// Remove overlay here

			if (animatorCannotSeeOverlay != null)
			{
				OverlayImage.SetActive(false);
				animatorCannotSeeOverlay.speed = -1;
				animatorCannotSeeOverlay.SetBool("is_cannot_see", false);
				AudioManager.instance.Play("CanSee");
				Debug.Log("Overlay is gone");
			}

		}
		else
		{
			//if (UnityEngine.Random.Range(0, 10) < 4)
			showPlayerDialog(cannotSeeComments[UnityEngine.Random.Range(0, 3)]);
			//AudioManager.instance.Play("ding");

			// Add overlay here
			if (OverlayImage != null)
			{
				OverlayImage.SetActive(true);
				animatorCannotSeeOverlay.speed = 1;
				animatorCannotSeeOverlay.SetBool("is_cannot_see", true);
				AudioManager.instance.Play("CannotSee");
			}
		}
		//Debug.Log("finish");
	}

	private void updateToLeaderboard()
	{
		if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			if (currentScene == GameMode.ENDLESS)
			{
				PlayGamesPlatform.Instance.ReportScore(PlayerPrefs.GetInt(PLAYER_HIGHSCORE),
					GPGSIds.leaderboard_endless_highscore,
					(bool success) =>
					{
						Debug.Log("Leaderboard update success: " + success);
					});
			}
			else if (currentScene == GameMode.TIME_ATTACK)
			{
				PlayGamesPlatform.Instance.ReportScore((long)PlayerPrefs.GetFloat(PLAYER_HIGHSCORE_TIMER),
					GPGSIds.leaderboard_time_attack_highscore,
					(bool success) =>
					{
						Debug.Log("Leaderboard update success: " + success);
					});
			}
		}
	}


	public void increaseTimer()
	{
		//timeLeft += 3f;

		if (currentScene == GameMode.TIME_ATTACK)
		{
			//bgTimeAttack.speed += 0.1f;
			//float playbackDifference = 1 - (1 / timeLeft);	
			//float currentNormalizedTime = (1 / bgTimeAttack.GetCurrentAnimatorStateInfo(0).normalizedTime);
			//Debug.Log(currentTime +"	|	" +timeLeft);
			//Debug.Log("Anim speed: " + bgTimeAttack.speed + " | Time Diff: " + playbackDifference +" | N_Time: " +currentTime);
			//timeLeft = timeLeft + (totalAnimationTime - (timeLeft + ((currentTime - diffFactor) * totalAnimationTime)));

			//float diffFactor = 0.4f;			 // The amount of time added between 0-1
			//float currentTime = bgTimeAttack.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
			//timeLeft = totalAnimationTime - (currentTime - diffFactor) * totalAnimationTime;
			//bgTimeAttack.Play("Base Layer.TimeAttackBgAnimation", 0, currentTime - diffFactor
		}
		else if (currentScene == GameMode.ENDLESS)
		{
			timeLeft = 3f;
			xTimes += 1;
			xTimesText.gameObject.SetActive(true);
		}
	}


	public void showPlayerDialog(string comment)
	{

		//if (lastTimeCommented )
		//lastTimeCommented = Time.deltaTime;
		if (textComment != null)
		{
			textComment.text = comment;
			imgComment.GetComponent<Animator>().ResetTrigger("show_dialog");
			imgComment.GetComponent<Animator>().SetTrigger("show_dialog");
			AudioManager.instance.Play("Messages");
		}
	}

	public void displayAchievement(string text)
	{
		strAchievement = text;
		achText.text = strAchievement;
		achAnimator.SetBool("show_ach", true);
	}

	public void playerDead()
	{

		timesDead = PlayerPrefs.GetInt(PLAYER_DEAD);
		timesDead++;
		PlayerPrefs.SetInt(PLAYER_DEAD, timesDead);
		PlayerPrefs.Save();
		//AudioManager.instance.sounds[0].pitch = 1f;
		AudioManager.instance.Play("Death");
		AudioManager.instance.Play("fallvoice");

		if (timesDead % 15 == 0)
		{
			AdController.instance.showRewardedVideoAd();

			// Reset the timesDead Stationarycounter. As such not needed but still.
			PlayerPrefs.SetInt(PLAYER_DEAD, 1);
			PlayerPrefs.Save();
		}
		else if (timesDead % 5 == 0)
		{
			AdController.instance.showVideoAd();
		}
		//Debug.Log(timesDead);

		// Check if it is the personal best
		if (highscoreTimer > PlayerPrefs.GetFloat(PLAYER_HIGHSCORE_TIMER))
		{
			PlayerPrefs.SetFloat(PLAYER_HIGHSCORE_TIMER, highscoreTimer);
			PlayerPrefs.Save();
			//Debug.Log("New highscore "+FormatTime(highscoreTimer));
		}

		updateToLeaderboard();

		showPauseMenu(true);
		pauseMenuController.Setup(score, FormatTime(totalTimeElapsed));
		Debug.Log("Game Over is called");
		//SceneManager.LoadScene(0);
	}
	////////// END COMMON FUNCTIONS ////////////
	///

	public void updatePlayer(Rigidbody2D p, Vector3 playerWorldPosition)
	{
		this.player = p;
		updateCommentBoxPosition(playerWorldPosition);
	}


	float resolution;

	float GetDPI()
	{
		AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
		activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);

		return (metrics.Get<float>("xdpi") + metrics.Get<float>("ydpi")) * 0.5f;
	}

	private void updateCommentBoxPosition(Vector3 playerWorldPosition)
	{
		if (imgComment != null && textComment != null)
		{
			imgComment.transform.position = playerWorldPosition;
			Vector2 textSize = CalculateTextSize(textComment.text, textComment);  // Calculate the size of the text
			RectTransform imgRect = imgComment.GetComponent<RectTransform>();// Get the size of the image
			Vector2 imageSize = imgRect.sizeDelta;

			if (textSize.x > imageSize.x || textSize.y > imageSize.y)  // Check if text size exceeds image size and adjust if necessary
			{
				float newWidth = Mathf.Max(textSize.x, imageSize.x);
				float newHeight = Mathf.Max(textSize.y, imageSize.y);
				imgRect.sizeDelta = new Vector2(newWidth, newHeight);   // Update image size to fit the text
			}
		}
	}

	private Vector2 CalculateTextSize(string text, TMP_Text textComponent)
	{
		textComponent.text = text;
		textComponent.ForceMeshUpdate();
		var textSize = textComponent.GetRenderedValues(false);
		return textSize;
	}


	public Rigidbody2D getPlayer()
	{
		return player;
	}

	public GameMode getGameMode()
	{
		return currentScene;
	}

	public void setIsTutorialComplete(bool value)
	{
		PlayerPrefs.SetInt(IS_TUTORIAL_COMPLETE, value ? 1 : 0);
		PlayerPrefs.Save();
	}

	public bool getIsTutorialComplete()
	{
		isTutorialComplete = PlayerPrefs.GetInt(IS_TUTORIAL_COMPLETE) == 1;
		return isTutorialComplete;
	}

	public void MakeSceneTransition(GameMode toScreen)
	{
		StartCoroutine(ScreenTransitionEnumerator((int)toScreen));
	}

	IEnumerator ScreenTransitionEnumerator(int index)
	{
		animatorSceneTransition.SetTrigger("transition_start");

		yield return new WaitForSeconds(1f);

		SceneManager.LoadScene(index);
	}

	public void UpdateWallBottomPosition(float posY)
	{
		wallBottom.transform.position = new Vector3(wallBottom.transform.position.x, posY);
	}

	public static float CalculateSpeedMultiplier()
	{
		if (instance == null || instance.player == null)
		{
			Debug.LogWarning("GameManager or player is not initialized.");
			return 0f; // Return default speed multiplier to avoid breaking the game.
		}
		// Increase speed by 0.1% for every unit of player's y position above 0.
		float baseSpeed = 0.18f;
		float progressionFactor = 0.005f; //  factor to control how quickly speed escalates.
		float heightAboveStart = Mathf.Max(0, instance.player.position.y);
		return baseSpeed + (heightAboveStart * progressionFactor);
	}

	public static int CalculateMaxBowlsToSpawn(float playerHeight)
	{
		
		// Example: Decrease bowls to spawn by 1 for every 500 units of height, but never less than 1
		return Mathf.Max(MinSpawn, instance.maxBowlsPerSpawn - (int)(playerHeight / 500));
	}



}