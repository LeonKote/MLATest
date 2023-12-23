using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public bool IsJumping { get; set; }
	public event Action OnCollision;

	private Rigidbody2D rb;
	private Animator animator;
	private float jumpDelay;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		if (jumpDelay <= 0)
		{
			if (IsJumping && transform.position.y < -0.01f)
			{
				rb.AddForce(Vector2.up * 1100);
				jumpDelay = 0.1f;
			}
		}
		else
		{
			jumpDelay -= Time.deltaTime;
		}

		if (transform.position.y < -0.01f)
			animator.SetBool("isRunning", true);
		else
			animator.SetBool("isRunning", false);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 0)
			return;

		GameManager.Instance.EndGame();

		OnCollision.Invoke();
	}
}
