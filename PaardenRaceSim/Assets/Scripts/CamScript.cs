using UnityEngine;
using System.Collections;

public class CamScript : MonoBehaviour
{

	enum STATES
	{
		FOLLOWING
	}

	STATES m_state;

	GameObject[] m_horses;
	Vector3[] m_distances;

	// Use this for initialization
	void Start()
	{
		m_horses = GameObject.FindGameObjectsWithTag("Horse");
		m_distances = new Vector3[m_horses.Length];
		for(int i = 0; i < m_distances.Length; ++i)
			m_distances[i] = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		
		Vector3 leftMost = Vector3.zero, rightMost = Vector3.zero;
		float leftMostVPX = 0f, rightMostVPX = 0f;
		Vector3 averageHorsePos = new Vector3();
		for(int i = 0; i < m_horses.Length; ++i)
		{
			Vector3 pos = m_horses[i].transform.position;
			averageHorsePos += pos;
			Vector3 v = Camera.main.WorldToViewportPoint(pos);
			if(v.x < leftMostVPX)
			{
				leftMostVPX = v.x;
				leftMost = pos;
			}
			if(v.x > rightMostVPX)
			{
				rightMostVPX = v.x;
				rightMost = pos;
			}
		}
		Vector3 dist = leftMost - rightMost;		
		averageHorsePos /= m_horses.Length;


		Vector3 camDir = averageHorsePos - Vector3.zero;
		camDir = camDir.normalized * dist.magnitude;
		Vector3 newPos = new Vector3((averageHorsePos + camDir).x, transform.position.y, (averageHorsePos + camDir).z);
		transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * .1f);
		transform.LookAt(averageHorsePos);
	}
}
