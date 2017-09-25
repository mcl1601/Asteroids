using UnityEngine;
using System.Collections;

/// <summary>
/// Attatched to the spaceship
/// Holds the info for bounding circles
/// </summary>
public class SpriteInfo : MonoBehaviour 
{
	// sprite renderer of the ship
	private SpriteRenderer sRend;

	// radius of the ship
	private float radius;

	// center of the sprite
	private Vector3 center;

	// vehicle movement script to access
	private VehicleMovement vMove;

	// Use this for initialization
	void Start () 
	{
		// get the renderer
		sRend = gameObject.GetComponent<SpriteRenderer> ();

		// set the radius as the sprite's y extent
		radius = sRend.sprite.bounds.extents.y;

		// set the center to the ship's position
		center = transform.position;

		// set the script
		vMove = gameObject.GetComponent<VehicleMovement> ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		// get the radius as the sprite's y extent
		radius = sRend.sprite.bounds.extents.y;
		
		// get the center to the ship's position
		center = transform.position;

		// turn the ship green if they have a power up
		if (vMove.PowerUp) 
		{
			sRend.color = Color.green;
			return;
		}
		else // turn off the color when the power up is not active
		{
			sRend.color = Color.white;
		}

		// change the ship's color if its invulnerable
		if (vMove.Invulnerability) 
		{
			sRend.color = Color.red;
			return;
		} 
		else // turn it off when it's not active
		{
			sRend.color = Color.white;
		}

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
