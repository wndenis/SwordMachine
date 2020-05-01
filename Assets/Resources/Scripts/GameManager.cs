using System.Collections;
using System.Security.Permissions;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance = null;
	public bool RapierTouch;
	//public OculusHaptics LeftHaptics;
	//public OculusHaptics RightHaptics;
    public float DampingDrag = 10f;

    public ScoreBoard scoreBoard;
    public int PlayerScore;
    public int EnemyScore;


    private bool canEnemyGetScore = true;
    private bool canPlayerGetScore = true;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		InitGame();
	}


    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        //scoreBoard.UpdateBoard(true);
    }

    void Start()
    {
        if (scoreBoard == null)
            return;
        scoreBoard.Start();
        scoreBoard.UpdateBoard(true);
    }

    public IEnumerator ScoreDelay(bool isForPlayer = true, float delay = 3f)
    {
        if (isForPlayer)
        {
            canPlayerGetScore = false;
            yield return new WaitForSeconds(delay);
            canPlayerGetScore = true;
        }
        else
        {
            canEnemyGetScore = false;
            yield return new WaitForSeconds(delay);
            canEnemyGetScore = true;
        }
    }

    public void AddScore(bool toPlayer = true, int score = 1)
    {
        if (toPlayer)
        {
            if (canPlayerGetScore)
            {
                PlayerScore += 1;
                StartCoroutine(ScoreDelay());
            }
        }
        else
        {
            if (canEnemyGetScore)
            {
                EnemyScore += 1;
                StartCoroutine(ScoreDelay(false));
            }
        }
        if (scoreBoard != null)
            scoreBoard.UpdateBoard();
    }


	//Update is called every frame.
	void Update()
	{
		
	}
    

    public void RandPitch(AudioSource audioSource, float mid = 1f, float value = 0.15f)
    {
        // mid - mid value for deviation (default: 1f)
        // value - deviation from mid     (default: 0.15f)
        audioSource.pitch = Random.Range(mid - value, mid + value);
    }
}