using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Text livesTxt;

    private Player player;
    private Invaders invaders;
    private Bunker[] bunkers;

    public int score { get; private set; }
    public int lives { get; private set; }

    Vector3 startPosition = new Vector3(0, -13, 0);

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        bunkers = FindObjectsOfType<Bunker>();

        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreTxt.text = score.ToString().PadLeft(4,'0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesTxt.text = this.lives.ToString();
    }

    public void PlayerKilled(Player player)
    {
        SetLives(lives - 1);

        if (lives > 0) 
        {
            Invoke(nameof(NewRound), 1f);

        }
        else
        {
            GameOver();
        }
    }

    public void InvaderKilled(Invader invader) 
    { 
        invader.gameObject.SetActive(false);

        SetScore(score + invader.score);

        if (invaders.invadersAlive == 0)
        {
            NewRound();
        }
    }

    public void Boundary()
    {
        if (invaders.gameObject.activeSelf)
        { 
            invaders.gameObject.SetActive(false);
            PlayerKilled(player);
        }
    }
}
