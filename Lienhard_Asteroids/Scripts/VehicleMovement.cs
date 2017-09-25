using UnityEngine;
using System.Collections;

/// <summary>
/// Attatched to the spaceship
/// Handles driving and shooting
/// </summary>
public class VehicleMovement : MonoBehaviour 
{
	// vector to calculate movement
	private Vector3 vehiclePos;
	private Vector3 velocity;
	private Vector3 direction;
	private Vector3 acceleration;

	// position in viewport space
	private Vector3 cameraPos;

	// rate of movement
	public float maxSpeed;
	public float accelRate;

	// camera to access
	public Camera cam;
	// rotation of the gameobject
	private float totalRot;

	// bullet to instantiate
	public GameObject prefab;

	// asteroid manager script to access
	private AsteroidManager man;

	// firing cooldown
	private float cooldown;

	// give god mode on respawn
	private bool invuln;

	// timer for god mode
	private float timer;

	// is the power up active?
	private bool powerUp;

	// timer for the power up
	private float pTimer;

	// Use this for initialization
	void Start () 
	{
		// initiate at 0,0,0
		vehiclePos = new Vector3 (0, 0, 0);
		velocity = Vector3.zero;
		acceleration = Vector3.zero;

		// set the direction upwards
		direction = new Vector3 (0,1,0); 

		// set the speed to 1
		maxSpeed = 1;

		// set rotation to 0
		totalRot = 0;

		// set the acceleration rate
		accelRate = 0.2f;

		// zero out cooldown and timer
		cooldown = 0f;
		timer = 0f;

		// find the asteroid manager
		man = GameObject.Find ("SceneManager").GetComponent<AsteroidManager> ();

		// start off with invulnerability
		invuln = true;

		// start without the powerup
		powerUp = false;

		// set the timer to 0;
		pTimer = 0f;
	}

	// properties to be accessed by other scripts
	public Vector3 VehiclePos
	{
		get { return vehiclePos; }
		set { vehiclePos = value; }
	}

	public bool Invulnerability
	{
		get { return invuln; }
		set { invuln = value; }
	}

	public float Timer
	{
		get { return timer; }
		set { timer = value; }
	}

	public bool PowerUp
	{
		get { return powerUp; }
		set { powerUp = value; }
	}

	public float PTimer
	{
		get { return pTimer; }
		set { pTimer = value; }
	}

	// Update is called once per frame
	void Update () 
	{
		RotateVehicle ();
		Drive ();
		Wrap ();
		SetTransform ();

		// increase the cooldown timer
		cooldown += Time.deltaTime;

		// shoot if the cooldown allows it
		if (Input.GetKeyDown (KeyCode.Space) && cooldown > 0.5f)
		{
			Fire ();
			cooldown = 0f;
		}

		// keeps track of the god mode timer
		if (invuln) 
		{
			timer += Time.deltaTime;
			if(timer > 3f)
				invuln = false;
		}

		// keeps track of the power up timer
		if (powerUp) 
		{
			pTimer += Time.deltaTime;
			if(pTimer > 6f)
				powerUp = false;
		}
	}

	/// <summary>
	/// Rotates the vehicle.
	/// Change the vehicle's direction when keys are pressed
	/// </summary>
	void RotateVehicle()
	{
		// press J, move 3 degrees to the left
		if (Input.GetKey (KeyCode.LeftArrow)) 
		{
			direction = Quaternion.Euler(0,0,3) * direction;
			totalRot += 3f;
		}
		// press K, move 3 degrees to the right
		if (Input.GetKey (KeyCode.RightArrow)) 
		{
			direction = Quaternion.Euler(0,0,-3) * direction;
			totalRot -= 3f;
		}
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep the vehicle on screen at all times.
	/// Recalculates the position based on edge of screen.
	/// </summary>
	void Wrap()
	{
		// convert from world position to camera position
		cameraPos = cam.WorldToViewportPoint(vehiclePos);

		if (cameraPos.x > 1) // goes off the right of the screen
			cameraPos.x = 0;

		if (cameraPos.x < 0) // goes off the left of the screen
			cameraPos.x = 1;

		if (cameraPos.y > 1) // goes above the screen
			cameraPos.y = 0;

		if (cameraPos.y < 0) // goes below the screen
			cameraPos.y = 1;

		// convert back into world space
		vehiclePos = cam.ViewportToWorldPoint (cameraPos);
	}

	/// <summary>
	/// Drive this instance.
	/// Calculate the velocity based on acceleration and decceleration.
	/// Updates the position based on the velocity vector.
	/// </summary>
	void Drive()
	{
		// accelerate when holding I
		if (Input.GetKey (KeyCode.UpArrow)) 
		{
			// calculate a = rate * direction
			acceleration = accelRate * direction * Time.deltaTime;
			// add accel to velocity
			velocity += acceleration;

			// limit velocity
			velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		} 
		else  // Decelerate when you're not holding I
		{
			velocity = velocity * 0.98f;

			if(velocity.sqrMagnitude < 0.001f)
			{
				velocity = Vector3.ClampMagnitude(velocity, 0f);
			}
		}

		// get the new position
		vehiclePos += velocity;
	}

	/// <summary>
	/// Sets the transform.
	/// Update position and rotation.
	/// </summary>
	void SetTransform()
	{
		// draw the object at the new position
		transform.position = vehiclePos;

		// draw the current rotation
		transform.rotation = Quaternion.Euler (0,0,totalRot);
	}

	/// <summary>
	/// Shoots a bullet, or 5 if the power up is on
	/// </summary>
	void Fire()
	{
		// shoot five with the power up
		if (powerUp) 
		{
			// loop through making each bullet
			for(int i = 0; i < 5; i++)
			{
				// instantiate a new bullet
				GameObject temp = (GameObject)Instantiate (prefab, new Vector3 (transform.position.x, transform.position.y, 1f), 
				       transform.rotation);

				// set the spray value in the bullet movement script
				temp.GetComponent<BulletMovement>().Spray = i;

				// add the bullet to the bullet list
				man.Bullets.Add (temp);
			}
		} 
		else // power up is off, shoot one bullet
		{			
			// instantiate a new bullet
			GameObject temp = (GameObject)Instantiate (prefab, new Vector3 (transform.position.x, transform.position.y, 1f),
                       transform.rotation);

			// set the spray value in the bullet movement script
			temp.GetComponent<BulletMovement>().Spray = 2;

			// add the bullet to the bullet list
			man.Bullets.Add (temp);
		}
	}
}
