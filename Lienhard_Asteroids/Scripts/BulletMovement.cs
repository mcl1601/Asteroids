using UnityEngine;
using System.Collections;

/// <summary>
/// Attatched to a bullet
/// Handles movement and rotation of the bullet
/// </summary>
public class BulletMovement : MonoBehaviour 
{
	// position of the bullet
	private Vector3 bulletPos;
	// direction the bullet will travel in
	private Vector3 direction;
	// velocity of the bullet
	private Vector3 velocity;
	// speed of the bullet
	private float speed;
	// time the bullet has been on screen
	private float time;
	// camera to access
	private Camera cam;
	// position in viewport space
	private Vector3 cameraPos;
	// asteroid manager script to access
	private AsteroidManager man;
	// used to determine which angle to shoot at
	private int spray;

	// Property
	public int Spray
	{
		get { return spray; }
		set { spray = value; }
	}

	// Use this for initialization
	void Start () 
	{
		// set the position to the current location
		bulletPos = transform.position;

		// set direction upwards initially
		direction = transform.rotation * Vector3.up;

		// set direction to rotation
		direction = Quaternion.Euler(0,0, (10f * spray - 20f)) * direction;

		// set speed to 4
		speed = 20f;

		// set time to 0
		time = 0;

		// find and set the main camera
		cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();

		// find the asteroid manager
		man = GameObject.Find ("SceneManager").GetComponent<AsteroidManager> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// update velocity so it is frame rate independent
		velocity = speed * direction * Time.deltaTime;

		// update position
		bulletPos += velocity;

		// wrap the bullet
		Wrap ();

		// transform the position
		transform.position = bulletPos;

		// update the time of the bullet
		time += Time.deltaTime;

		// if the bullet is active for more than 1 second, destroy it
		if (time > 1f) 
		{
			// remove the bullet from the list
			man.Bullets.Remove(gameObject);
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep the bullet on screen at all times.
	/// Recalculates the position based on edge of screen.
	/// </summary>
	void Wrap()
	{
		// convert from world position to camera position
		cameraPos = cam.WorldToViewportPoint(bulletPos);
		
		if (cameraPos.x > 1) // goes off the right of the screen
			cameraPos.x = 0;
		
		if (cameraPos.x < 0) // goes off the left of the screen
			cameraPos.x = 1;
		
		if (cameraPos.y > 1) // goes above the screen
			cameraPos.y = 0;
		
		if (cameraPos.y < 0) // goes below the screen
			cameraPos.y = 1;
		
		// convert back into world space
		bulletPos = cam.ViewportToWorldPoint (cameraPos);
	}
}
