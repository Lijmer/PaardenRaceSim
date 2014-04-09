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

	// Use this for initialization
	void Start()
	{
		m_horses = GameObject.FindGameObjectsWithTag("Horse");
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 averageHorsePos = new Vector3();;
		foreach(GameObject g in m_horses)
			averageHorsePos += g.transform.position;
		averageHorsePos /= m_horses.Length;
		Vector3 camDir = averageHorsePos - Vector3.zero;
		camDir = camDir.normalized * 15f;
		transform.position = new Vector3((averageHorsePos + camDir).x, transform.position.y, (averageHorsePos + camDir).z);
		transform.LookAt(averageHorsePos);
	}
}
