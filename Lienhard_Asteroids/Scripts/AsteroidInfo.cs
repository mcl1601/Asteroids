using UnityEngine;
using System.Collections;

/// <summary>
/// Attatched to each asteroid
/// Holds sprite values to use for collisions
/// </summary>
public class AsteroidInfo : MonoBehaviour 
{
	// sprite renderer of this object
	private SpriteRenderer sRend;

	// radius of the sprite
	public float radius;

	// center of the sprite
	public Vector3 center;

	// scale of the sprite
	private Vector3 scale;

	// asteroid movement script
	private AsteroidMovement aMove;

	// Use this for initialization
	void Start () 
	{
		// get the renderer
		sRend = gameObject.GetComponent<SpriteRenderer> ();
		
		// set the radius as the sprite's y extent shrunk a bit
		radius = sRend.sprite.bounds.extents.y;
		
		// set the center to the ship's position
		center = transform.position;

		// used to scale the sprite
		scale = transform.lossyScale;

		// set the asteroid movement
		aMove = gameObject.GetComponent<AsteroidMovement> ();

		// scale down the sprite for level 2 asteroids
		if (aMove.Level == 2)
			scale = scale * 0.75f;

	}
	
	// Update is called once per frame
	void Update () 
	{
		// get the radius as the sprite's y extent shrunk a bit
		radius = sRend.sprite.bounds.extents.y * scale.y * 0.75f;

		// get the center to the asteroid's position
		center = transform.position;
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

	public Vector3 Scale
	{
		get { return scale; }
	}
}
