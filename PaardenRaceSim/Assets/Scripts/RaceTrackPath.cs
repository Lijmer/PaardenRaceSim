﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RaceTrackPath : MonoBehaviour
{
	public Vector3[] m_points;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		for(int i = 1; i < m_points.Length; ++i)
		{			
			Debug.DrawLine(m_points[i-1], m_points[i], Color.magenta);
		}
		Debug.DrawLine(m_points[m_points.Length-1], m_points[0], Color.magenta);
	}
}
