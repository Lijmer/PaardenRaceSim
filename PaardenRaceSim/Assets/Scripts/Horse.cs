using UnityEngine;
using System.Collections;
using Pathfinding;

public class Horse : MonoBehaviour
{
	public Vector3 m_targetPos;
	Vector3 m_lastPos;
	int m_indexPoint = 0;
	RaceTrackPath m_path;
	public float m_speed;
	// Use this for initialization
	void Start()
	{
		m_lastPos = transform.position;
		m_path = GameObject.Find("Path").GetComponent<RaceTrackPath>();
		m_targetPos = m_path.m_points[++m_indexPoint];
		m_speed += Random.Range(-1f, 1f);
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 newForward = (transform.position - m_lastPos);
		if(newForward != Vector3.zero)
			transform.forward = Vector3.Lerp(transform.forward, newForward.normalized, .1f);
		if((m_targetPos - transform.position).sqrMagnitude < 2f)
		{
			if(++m_indexPoint >= m_path.m_points.Length)
				m_indexPoint = 0;
			m_targetPos = m_path.m_points[m_indexPoint];
		}
		Vector3 dir = (m_targetPos - transform.position).normalized * m_speed * Time.deltaTime;
		m_lastPos = transform.position;
		rigidbody.MovePosition(transform.position + dir);
		//transform.Translate(dir, Space.World);
	}

	public void OnPathComplete(Path p)
	{
		Debug.Log("Yey, we got a path back. Did it have an error? "+p.error);
	}
}
