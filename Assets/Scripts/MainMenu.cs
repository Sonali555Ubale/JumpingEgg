using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private Text signInButtonText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text PercentText;
    // Define your tutorial and main game level names here
    private int tutorialSceneIndex = 4; // Example tutorial scene index
    private const string HasCompletedTutorialKey = "HasCompletedTutorial";
    private int GameSceneIndex;
    private void Start()
    {

        Debug.Log("Main menu called.");

        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
    }

    bool wasLeaderBoardRequested = false;

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
            //Debug.Log("Cannot show leaderboard: not authenticated");
        }
    }

    public void SignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();

            // Reset UI
            //signInButtonText.text = "Sign In";
            //authStatus.text = "";
        }
    }

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(Lollygagger) Signed in!");

            if(wasLeaderBoardRequested)
            {
                ShowLeaderboards();
            }
            // Change sign-in button text
            //signInButtonText.text = "Sign out";

            // Show the user's name
            //authStatus.text = "Signed in as: " + Social.localUser.userName;
        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...");

            // Show failure message
            //signInButtonText.text = "Sign in";
            //authStatus.text = "Sign-in failed";
        }
    }

    public void ReadSavedGame(string filename,
                             Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            callback);
    }

    public void WriteSavedGame(ISavedGameMetadata game, byte[] savedData,
                               Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved at: " + System.DateTime.Now);

        // You can add an image to saved game data (such as as screenshot)
        // byte[] pngData = <PNG AS BYTES>;
        // builder = builder.WithUpdatedPngCoverImage(pngData);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
    }


    public void gameModeEndless()
    {
        // StartCoroutine(LoadLevelWithLoadingScreen((int)GameManager.GameMode.ENDLESS));
        GameSceneIndex = (int)GameManager.GameMode.ENDLESS;
        StartSelectedGameLevel(GameSceneIndex);
    }

    public void gameModeTimeAttack()
    {
        // StartCoroutine(LoadLevelWithLoadingScreen((int)GameManager.GameMode.TIME_ATTACK));
        GameSceneIndex = (int)GameManager.GameMode.TIME_ATTACK;
        StartSelectedGameLevel(GameSceneIndex);
    }

    public void StartSelectedGameLevel(int levelSceneIndex)
    {
        if (HasCompletedTutorial())
        {
          //  levelSceneIndex = GameSceneIndex;
            // If the tutorial has been completed, load the selected level directly
            StartCoroutine(LoadLevelWithLoadingScreen(levelSceneIndex));
        }
        else
        {
            // If the tutorial hasn't been completed, load the tutorial scene
            // SceneManager.LoadScene(tutorialSceneIndex);
            StartCoroutine(LoadLevelWithLoadingScreen(tutorialSceneIndex));
        }
    }

    private bool HasCompletedTutorial()
    {
        return PlayerPrefs.GetInt(HasCompletedTutorialKey, 0) == 1;
    }

    public static void MarkTutorialAsCompleted()
    {
        PlayerPrefs.SetInt(HasCompletedTutorialKey, 1);
        PlayerPrefs.Save();
    }

    IEnumerator LoadLevelWithLoadingScreen(int sceneIndex)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            PercentText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void quitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
