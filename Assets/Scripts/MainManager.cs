using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MainManager : MonoBehaviour
{
    [SerializeField] public GameObject container;
    [SerializeField] public TMP_InputField inputField;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText, bestText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private string userName;
    private bool canRestart;
    private bool m_GameOver = false, youWin= false;
    private string youWinText;

    // Start is called before the first frame update

    private void Awake()
    {
        bestText.text = $"Best Score : {GameManager.instance.name} : {GameManager.instance.score}";
        youWinText = $"YOU WIN \n Press Space to Restart";
    }
    void Start()
    {
        container.SetActive(false);
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        GameManager.instance.AddBlock();
    }

    private void Update()
    {
        youWin = GameManager.instance.Winner();
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            GameManager.instance.SavePlayerInformation(m_Points, userName);
            if (canRestart && Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if(youWin)
        {
            GameManager.instance.SavePlayerInformation(m_Points, userName);
            if (canRestart && Input.GetKeyDown(KeyCode.Space))
            {
                Text text = GameOverText.GetComponent<Text>();
                text.text = youWinText.ToString();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void OnApplicationQuit()
    {
        GameManager.instance.SavePlayerInformation(m_Points, userName);

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        container.SetActive(true);
    }

    public void OnGameOver()
    {
        GameOverText.SetActive(true);
    }

    public void GetUserName()
    {
        userName = inputField.text;
        canRestart = true;
    }
}
