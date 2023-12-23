using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private GameObject road;
	[SerializeField]
	private GameObject player;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Text highScoreText;
	[SerializeField]
	private GameObject[] obstaclePrefabs;

	[field: SerializeField]
	public float InitialSpeed { get; set; } = 8.0f;
	[field: SerializeField]
	public float SpeedMultiplier { get; set; } = 0.25f;

	public static GameManager Instance { get; private set; }

	public event System.Action OnSuccessJump;

	[field: SerializeField]
	public float DistToObstacle { get; private set; }
	[field: SerializeField]
	public float Speed { get; private set; }

	private List<GameObject> obstacles = new List<GameObject>();

	private GameObject closestObstacle;
	private float score;
	private float highScore;
	private bool isPaused;

	void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		Application.runInBackground = true;
		Speed = InitialSpeed;
		score = 0;
		StartCoroutine(SpawnObstacles());
	}

	IEnumerator SpawnObstacles()
	{
		while (true)
		{
			int range;
			if (Speed < 12)
				range = 4;
			else
				range = obstaclePrefabs.Length;
			GameObject obstacle = Instantiate(obstaclePrefabs[Random.Range(0, range)]);
			obstacles.Add(obstacle);
			if (closestObstacle == null)
				closestObstacle = obstacle;
            yield return new WaitForSeconds(Random.Range(0.7f, 1.5f));
		}
	}

	void FixedUpdate()
	{
		if (isPaused)
			return;

		MoveRoad();
		ProcessObstacles();
		TrainJump();

		Speed += Time.deltaTime * SpeedMultiplier;
		score += Speed * Time.deltaTime;

		UpdateScoreText();
	}

	private void MoveRoad()
	{
		road.transform.position += Speed * Time.deltaTime * Vector3.left;
		if (road.transform.position.x < -7)
			road.transform.position = new Vector3(0, -0.6f, 0);
	}

	private void ProcessObstacles()
	{
		for (int i = 0; i < obstacles.Count; i++)
		{
			obstacles[i].transform.position += Speed * Time.deltaTime * Vector3.left;
			if (obstacles[i].transform.position.x < -20)
			{
				Destroy(obstacles[i]);
				obstacles.RemoveAt(i);
			}
		}
	}

	private void TrainJump()
	{
		if (closestObstacle == null)
			return;

		DistToObstacle = closestObstacle.transform.position.x - player.transform.position.x;

		if (DistToObstacle < 0 && player.transform.position.y < -0.01f)
		{
			OnSuccessJump.Invoke();
			closestObstacle = GetClosestObstacle();
		}
	}

	private GameObject GetClosestObstacle()
	{
		foreach (var obstacle in obstacles)
			if (obstacle.transform.position.x > player.transform.position.x)
				return obstacle;
		return null;
	}

	public void EndGame()
	{
		isPaused = true;
		UpdateHighScoreText(score);
	}

	public void Restart()
	{
		for (int i = 0; i < obstacles.Count; i++)
			Destroy(obstacles[i]);

		obstacles.Clear();

		Speed = InitialSpeed;
		score = 0;
		isPaused = false;
	}

	private void UpdateScoreText()
	{
		scoreText.text = ((int)score).ToString("D5");
	}

	private void UpdateHighScoreText(float score)
	{
		if (score < highScore)
			return;
		highScore = score;
		highScoreText.text = "HI " + ((int)score).ToString("D5");
	}
}
