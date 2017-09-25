using UnityEngine;
using System.Collections;

/// <summary>
/// Attatched to a power up
/// Easy acces to radius and center for collision purposes
/// </summary>
public class PowerUpInfo : MonoBehaviour 
{
	// radius of the power up
	private float radius;
	
	// center of the sprite
	private Vector3 center;

	// Use this for initialization
	void Start () 
	{		
		// set the radius 0.6
		radius = 0.6f;
		
		// set the center to the ship's position
		center = transform.position;	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	// properites to be accessed by other classes
	public float Radius
	{
		get { return radius; }
		set { radius = value; }
	}
	
	public Vector3 Center 
	{
		get { return center; }
		set { center = value; }
	}
}
