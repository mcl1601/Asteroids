using UnityEngine;
using System.Collections;

/// <summary>
/// Attactched to an asteroid sprite
/// Handles movement and wrapping
/// </summary>
public class AsteroidMovement : MonoBehaviour 
{
	// Vectors for the asteroid
	private Vector3 astPos;
	private Vector3 velocity;
	private Vector3 direction;

	// Speed of the asteroid
	private float speed;
	// rotation of the gameobject
	private float totalRot;
	// camera to access
	private Camera cam;
	// position in viewport space
	private Vector3 cameraPos;

	// type of asteroid prefab
	private int type;
	// level one or level two asteroid
	private int level;

	// Use this for initialization
	void Start () 
	{
		// find and set the main camera
		cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();

		// zero out rotation
		totalRot = 0f;
	}

	// properties to be accesed by other classes
	public Vector3 Direction
	{
		get { return direction;}
		set { direction = value;}
	}

	public int Level
	{
		get { return level;}
		set { level = value;}
	}

	public int Type
	{
		get { return type;}
		set { type = value;}
	}

	// Update is called once per frame
	void Update () 
	{
		// wrap if off screen
		Wrap ();
		// update the position of the asteroid
		astPos += velocity;
		// set the position
		transform.position = astPos;

		// rotate the asteroid
		Rotate ();
	}
	
	/// <summary>
	/// Wrap this instance.
	/// Keep the asteroid on screen at all times.
	/// Recalculates the position based on edge of screen.
	/// </summary>
	void Wrap()
	{
		// convert from world position to camera position
		cameraPos = cam.WorldToViewportPoint(astPos);
		
		if (cameraPos.x > 1) // goes off the right of the screen
			cameraPos.x = 0;
		
		if (cameraPos.x < 0) // goes off the left of the screen
			cameraPos.x = 1;
		
		if (cameraPos.y > 1) // goes above the screen
			cameraPos.y = 0;
		
		if (cameraPos.y < 0) // goes below the screen
			cameraPos.y = 1;
		
		// convert back into world space
		astPos = cam.ViewportToWorldPoint (cameraPos);
	}

	/// <summary>
	/// Method used when instantiating an asteroid to assign values
	/// </summary>
	/// <param name="s">S.</param>
	/// <param name="pos">Position.</param>
	/// <param name="dir">Dir.</param>
	public void SetValues(float s, Vector3 pos, Vector3 dir, int t, int l)
	{
		// assign values for attributes
		astPos = pos;
		speed = s;
		direction = dir;
		type = t;
		level = l;

		// Calculate the velocity using speed and direction
		velocity = speed * direction * Time.deltaTime;
	}

	/// <summary>
	/// Slightly rotates the asteroid each time it's called
	/// </summary>
	void Rotate()
	{
		// rotate the asteroid slightly
		totalRot += 0.5f;
		// set the rotation
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, totalRot));
	}
}
