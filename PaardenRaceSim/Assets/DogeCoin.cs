using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DogeCoin : MonoBehaviour
{
	public float m_speed;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.Rotate(0f, m_speed * Time.deltaTime, 0f);
	}
}
