using UnityEngine;
using System.Collections;
using Pathfinding;

public class Horse : MonoBehaviour
{
	public Vector3 m_targetPos;
	Vector3 m_lastPos;
	int m_indexPoint = 0;
	RaceTrackPath m_path;
	public float m_maxVelocity;
	public float m_acceleration;
	float m_velocity;

	Animator m_animator;

	// Use this for initialization
	void Start()
	{
		m_lastPos = transform.position;
		m_path = GameObject.Find("Path").GetComponent<RaceTrackPath>();
		m_targetPos = m_path.m_points[++m_indexPoint];
		m_maxVelocity += Random.Range(-.1f, .1f);
		m_animator = GetComponentInChildren<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 vel = transform.position - m_lastPos;
		float velMagnitude = vel.magnitude;
		velMagnitude += m_acceleration * Time.deltaTime;
		if(velMagnitude > m_maxVelocity )
			velMagnitude = m_maxVelocity;
		//vel = vel.normalized * velMagnitude;

		if((m_targetPos - transform.position).sqrMagnitude < 5f)
		{
			if(++m_indexPoint >= m_path.m_points.Length)
				m_indexPoint = 0;
			m_targetPos = m_path.m_points[m_indexPoint];
		}
		Vector3 dir = (m_targetPos - transform.position).normalized;
		//m_lastPos = transform.position;
		//Vector3 newForward = (transform.position - m_lastPos);

		Debug.DrawRay(transform.position, transform.forward * 2f);
		if(Physics.Raycast(transform.position, transform.forward, 2f))
		{
			Quaternion q = Quaternion.AngleAxis( -30f, Vector3.up);
			dir += q * transform.forward;
		}
		Debug.DrawRay(transform.position, dir * 2f, Color.blue);

		if(dir != Vector3.zero)
			transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * 10f);

		m_lastPos = transform.position;
		//rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * velMagnitude);
		m_animator.speed = velMagnitude * 2.5f;
		transform.Translate(transform.forward * velMagnitude, Space.World);
	}

	public void OnPathComplete(Path p)
	{
		Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
	}
}
