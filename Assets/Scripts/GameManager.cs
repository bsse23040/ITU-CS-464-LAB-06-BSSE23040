using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// What the player has to do to finish a level. Every level picks one in the Inspector.
public enum Objective { CollectCoins, ReachGoal, CollectThenGoal, Survive }

/// <summary>
/// One GameManager per level. It owns the score, the lives, the timer and the three states of the game:
/// playing, won and lost. Every level scene has its own copy with its own objective.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("This level")]
    [SerializeField] string levelName = "Level";
    [SerializeField] Objective objective = Objective.CollectCoins;
    [SerializeField] int coinTarget = 8;
    [SerializeField] float timeLimit = 0f;      // 0 = no timer. For Survive it is how long you have to last.

    [Header("Player")]
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;

    [Header("UI")]
    [SerializeField] TMP_Text levelText, objectiveText, timerText, statusText;
    [SerializeField] GameObject winPanel, losePanel;
    [SerializeField] TMP_Text winText, nextLabel, loseText;

    int score;
    float timeLeft;
    int shownSeconds = -1;
    bool goalOpen, ended, respawning;

    public bool GoalOpen => goalOpen;
    public bool IsRespawning => respawning;

    void Awake() => Instance = this;

    void Start()
    {
        timeLeft = timeLimit;
        goalOpen = objective == Objective.ReachGoal;
        levelText.text = levelName;

        bool lastLevel = SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1;
        winText.text = lastLevel ? "YOU BEAT THE GAME!" : "LEVEL COMPLETE!";
        nextLabel.text = lastLevel ? "Play Again" : "Next Level";
        UpdateUI();
    }

    void Update()
    {
        if (ended || timeLimit <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            if (objective == Objective.Survive) Win();     // lasting until the end is the goal
            else Lose("Time is up!");
        }

        int seconds = Mathf.CeilToInt(timeLeft);
        if (seconds != shownSeconds)                       // the text changes when the number changes
        {
            shownSeconds = seconds;
            UpdateUI();
        }
    }

    // ---------------------------------------------------------------- score and objective
    public void AddScore(int amount)
    {
        score += amount;
        if (objective == Objective.CollectCoins && score >= coinTarget) Win();
        if (objective == Objective.CollectThenGoal && score >= coinTarget) goalOpen = true;
        UpdateUI();
    }

    // called by the Goal when the player stands on it
    public void GoalReached()
    {
        if (ended || !goalOpen) return;
        Win();
    }

    public void UpdateUI()
    {
        switch (objective)
        {
            case Objective.CollectCoins:
                objectiveText.text = $"Collect every coin: {score} / {coinTarget}";
                break;
            case Objective.ReachGoal:
                objectiveText.text = "Reach the green goal";
                break;
            case Objective.CollectThenGoal:
                objectiveText.text = score < coinTarget
                    ? $"Collect the coins: {score} / {coinTarget}"
                    : "The exit is open! Reach the goal";
                break;
            case Objective.Survive:
                objectiveText.text = "Survive until the timer runs out";
                break;
        }

        timerText.text = timeLimit > 0f ? $"{Mathf.CeilToInt(timeLeft)}" : "";
        statusText.text = $"Lives: {lives}    Health: {Mathf.Max(player.Current, 0)} / {player.Max}" + (player.shielded ? "    SHIELD" : "");
    }

    // ---------------------------------------------------------------- losing a life
    public void PlayerDied()
    {
        if (ended || respawning) return;     // a pit and a guard can both kill in the same moment
        lives--;
        UpdateUI();
        if (lives <= 0) Lose("Out of lives");
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        respawning = true;
        yield return new WaitForSeconds(respawnDelay);   // wait a beat: the knockback of the hit that killed us is still queued
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
        respawning = false;
    }

    // ---------------------------------------------------------------- the end of a level
    void Win()
    {
        if (ended) return;
        ended = true;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose(string reason)
    {
        if (ended) return;
        ended = true;
        loseText.text = reason;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // wired to the buttons on the panels
    public void Restart()
    {
        Time.timeScale = 1f;                             // timeScale survives a scene load, so put it back first
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}
