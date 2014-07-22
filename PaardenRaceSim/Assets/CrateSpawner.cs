using UnityEngine;
using System.Collections;

public class CrateSpawner : MonoBehaviour
{
	public GameObject m_cratePrefab;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject go = Instantiate(
				m_cratePrefab,
				transform.position,
				new Quaternion(
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f))
				) as GameObject;
			go.transform.parent = transform;
		}
	}
}
