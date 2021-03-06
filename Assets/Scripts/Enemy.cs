﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	[SerializeField]
	private Transform exitPoint;
	[SerializeField]
	private Transform[] waypoints;
	[SerializeField]
	private float navigationUpdate;

	[SerializeField]
	private int healthPoints;

	[SerializeField]
	private int rewardAmount;

	private int target = 0;
	private Transform enemy;
	private Collider2D enemyCollider;
	private Animator animation;
	private float navigationTime = 0;
	private bool isDead = false;

	public bool IsDead {
		get {
			return isDead;
		}
	}

	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform> ();
		enemyCollider = GetComponent<Collider2D>();
		animation = GetComponent<Animator>();
		GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(waypoints != null && !isDead) {
			navigationTime += Time.deltaTime;
			if(navigationTime > navigationUpdate) {
				if(target < waypoints.Length) {
					enemy.position = Vector2.MoveTowards(enemy.position, waypoints[target].position, navigationTime);
				} else {
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
				}
				navigationTime = 0;
			}
		}
	}

	
	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "CheckPoint") {
			target += 1;
		} else if (other.tag == "Finish") {
			GameManager.Instance.RoundEscaped += 1;
			GameManager.Instance.TotalEscaped += 1;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.IsWaveOver();
		} else if (other.tag == "Projectile") {
			Projectile newP = other.gameObject.GetComponent<Projectile>();
			enemyHit(newP.AttackStrength);
			Destroy(other.gameObject);
		}
	}

	public void enemyHit(int hitPoint) {
		if(healthPoints - hitPoint > 0) {
			healthPoints -= hitPoint;
			GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
			// call hurt animation
			animation.Play("Hurt");

		} else {
			// die animation
			animation.SetTrigger("didDie");

			enemyDie();
		}
	}

	public void enemyDie() {
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled += 1;
		GameManager.Instance.AddMoney(rewardAmount);
		GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
		GameManager.Instance.IsWaveOver();
	}
	
}
