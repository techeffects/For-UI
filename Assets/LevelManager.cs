using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public GameObject antPrefab;  // Reference to the ant prefab
    public GameObject flyPrefab;
    public GameObject locustPrefab;
    public GameObject roachPrefab;

    public Transform spawnArea;   // The area where ants will spawn
    public Text levelText;        // UI Text to display current level
    public Text timeText;         // UI Text to display remaining time
    public Text pointText;        // UI Text to display points
    public Text quotaText;        // UI Text to display quota
    public float levelTime = 15f; // Starting time for each level
    public float timeMultiplier = 1f;

    public GameObject resultPanel; // Reference to the result panel
    public Text resultLevelText, resultPointsText, outcomeText; // Text elements to display results
    public Button nextLevelButton, mainMenuButton; // Buttons for user interaction

    public int level = 1;
    public int quota = 10;       // Starting number of ants to be clicked
    public float timeRemaining;
    public int antsClicked = 0;
    public int points = 0;       // Starting points

    void Start()
    {
        StartLevel();
        resultPanel.SetActive(false);
    }

    void Update() {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeText();
        }
        else
        {
            ShowResults(false);
            //FailLevel();
        }

        if (Input.GetKeyDown("r")) {  // Reload scene, for testing purposes
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Title Screen");
        }
    }

    void StartLevel() {
        // antsClicked = 0;
        timeRemaining = levelTime;
        UpdateLevelText();
        UpdatePointText();
        UpdateTimeText();
        UpdateQuotaText();
        CalculateSpawnAreaBounds();
        SpawnCreatures(quota + 5);
    }

    void CalculateSpawnAreaBounds() {
        if (Camera.main.orthographic) {
            var vertExtent = Camera.main.orthographicSize;  
            var horzExtent = vertExtent * Screen.width / Screen.height;
            var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            var topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
            spawnArea.localScale = new Vector3(horzExtent * 2, vertExtent * 2, spawnArea.localScale.z);
            spawnArea.position = Camera.main.transform.position + new Vector3(0, 0, 10);  // Adjust Z to be within the visible range
        }
    }

    void SpawnCreatures(int number) {
        float minX = spawnArea.position.x - spawnArea.localScale.x / 2;
        float maxX = spawnArea.position.x + spawnArea.localScale.x / 2;
        float minY = spawnArea.position.y - spawnArea.localScale.y / 2;
        float maxY = spawnArea.position.y + spawnArea.localScale.y / 2;

        GameObject[] creaturePrefabs = GetCreaturePrefabsForLevel();
        for (int i = 0; i < number; i++) {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), spawnArea.position.z);
            GameObject creature = Instantiate(creaturePrefabs[Random.Range(0, creaturePrefabs.Length)], spawnPosition, Quaternion.identity);
            AssignLevelManager(creature);
        }
    }

    void AssignLevelManager(GameObject creature) {
        // Determine type and assign the LevelManager reference accordingly
        if (creature.GetComponent<antMove>() != null) {
            creature.GetComponent<antMove>().levelMan = this;
        } else if (creature.GetComponent<flyMove>() != null) {
            creature.GetComponent<flyMove>().levelMan = this;
        } else if (creature.GetComponent<locustMove>() != null) {
            creature.GetComponent<locustMove>().levelMan = this;
        } else if (creature.GetComponent<roachMove>() != null) {
            creature.GetComponent<roachMove>().levelMan = this;
        }
    }

    GameObject[] GetCreaturePrefabsForLevel() {
        switch (level) {
            case 1:
                return new GameObject[] { antPrefab };
            case 2:
                return new GameObject[] { antPrefab, flyPrefab };
            case 3:
                return new GameObject[] { antPrefab, flyPrefab, locustPrefab };
            case 5:
                return new GameObject[] { antPrefab, flyPrefab, locustPrefab, roachPrefab };
            default:
                return new GameObject[] { antPrefab, flyPrefab, locustPrefab, roachPrefab }; // Default to only spawning ants
        }
    }

    public void AntClicked() {
        antsClicked++;
        points++;  // Increment points for each ant clicked
        UpdatePointText();
        if (antsClicked >= quota) {
            //NextLevel();
            ShowResults(true);
        }
    }

    void ShowResults(bool isSuccess) {
        if (resultPanel.activeSelf)
            return;

        resultPanel.SetActive(true); // Show the result panel

        resultLevelText.text = "Level Reached: " + level.ToString();
        resultPointsText.text = "Total Points: " + points.ToString();
        
        nextLevelButton.onClick.RemoveAllListeners();
        if (isSuccess) {
            outcomeText.text = "Level Won! Go to Next";
            nextLevelButton.onClick.AddListener(NextLevel);
            nextLevelButton.GetComponentInChildren<Text>().text = "Next Level";
        } else {
            outcomeText.text = "Game Over! Retry";
            nextLevelButton.onClick.AddListener(RestartLevel);
            nextLevelButton.GetComponentInChildren<Text>().text = "Try Again";
        }

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.GetComponentInChildren<Text>().text = "Main Menu";
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("Title Screen"));
    }

    void NextLevel() {
        level++;
        timeMultiplier++;
        quota += 30;
        levelTime = 15f * timeMultiplier;
        resultPanel.SetActive(false);
        StartLevel();
    }

    void RestartLevel() {
        resultPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // StartLevel();
    }

    void FailLevel() {
        Debug.Log("Time's up! Try again.");
        resultPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reloads scene from start
    }

    void UpdateLevelText() {
        levelText.text = "Level: " + level.ToString();
    }

    void UpdateTimeText() {
        timeText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
    }

    void UpdatePointText() {
        pointText.text = "Points: " + points.ToString();
    }

    void UpdateQuotaText() {
        quotaText.text = "Quota: " + quota.ToString();
    }
}
