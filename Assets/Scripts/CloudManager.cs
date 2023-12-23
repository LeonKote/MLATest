using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
	[SerializeField]
	private GameObject cloudPrefab;

	[field: SerializeField]
	public float SpeedMultiplier { get; set; } = 0.25f;

	private List<GameObject> clouds = new List<GameObject>();

	private float spawnDelay;

	public void DoUpdate()
	{
		if (spawnDelay <= 0)
		{
			clouds.Add(Instantiate(cloudPrefab, new Vector3(12, Random.Range(1.0f, 4.0f), 0), Quaternion.identity));
			spawnDelay = Random.Range(16.0f, 24.0f) / GameManager.Instance.Speed;
		}
		else
		{
			spawnDelay -= Time.deltaTime;
		}

		for (int i = 0; i < clouds.Count; i++)
		{
			clouds[i].transform.position += GameManager.Instance.Speed * SpeedMultiplier * Time.deltaTime * Vector3.left;
			if (clouds[i].transform.position.x < -12)
			{
				Destroy(clouds[i]);
				clouds.RemoveAt(i);
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < clouds.Count; i++)
			Destroy(clouds[i]);

		clouds.Clear();
	}
}
