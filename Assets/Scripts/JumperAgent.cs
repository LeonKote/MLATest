using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class JumperAgent : Agent
{
	private PlayerController controller;

	// Start is called before the first frame update
	void Start()
	{
		controller = GetComponent<PlayerController>();
		controller.OnCollision += OnCollision;
		GameManager.Instance.OnSuccessJump += OnSuccessJump;
	}

	public override void OnEpisodeBegin()
	{
		GameManager.Instance.Restart();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(GameManager.Instance.DistToObstacle);
		sensor.AddObservation(GameManager.Instance.Speed);
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		controller.IsJumping = actions.DiscreteActions[0] == 1;
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var discreteActions = actionsOut.DiscreteActions;
		discreteActions[0] = (int)Input.GetAxis("Jump");
	}

	public void OnCollision()
	{
		AddReward(-1.0f);
		EndEpisode();
	}

	public void OnSuccessJump()
	{
		AddReward(1.0f);
	}
}
