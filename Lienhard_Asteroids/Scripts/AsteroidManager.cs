using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Essentially the main game runner
/// Attatched to the scene manager
/// Handles collisions and instantiations
/// </summary>
public class AsteroidManager : MonoBehaviour 
{
	// list of asteroids in the scene
	private List<GameObject> asteroids;

	// prefabs of asteroids
	public GameObject greyAst;
	public GameObject redAst;
	public GameObject brownAst;

	// spaceship gameobject
	public GameObject ship;

	// ship movement
	private VehicleMovement vMove;

	// sprite info scripts to access
	private SpriteInfo sInfo;
	private AsteroidInfo astInfo;

	// Game mechanics script to access score/lives
	private GameMechanics gm;

	// array of prefabs to choose from
	private GameObject[] prefabs;

	// list of powerups in the scene
	private List<GameObject> powerUps;

	// list of bullets in the scene
	private List<GameObject> bullets;

	// prefab for the powerup
	public GameObject pUp;

	// sprite info
	private PowerUpInfo pInfo;

	// Use this for initialization
	void Start () 
	{
		// populate the prefab array with prefabs
		prefabs = new GameObject[3];
		prefabs [0] = greyAst;  // type 0 = grey
		prefabs [1] = redAst;   // type 1 = red
		prefabs [2] = brownAst; // type 2 = brown

		// initialize the asteroid list
		asteroids = new List<GameObject> ();

		// get the info for the ship
		sInfo = ship.GetComponent<SpriteInfo> ();

		// initialize the bullet list
		bullets = new List<GameObject> ();

		// set the ship movement script
		vMove = ship.GetComponent<VehicleMovement> ();

		// set the game mechanics script
		gm = gameObject.GetComponent<GameMechanics> ();

		// initialize the power up list
		powerUps = new List<GameObject> ();
	}

	// property to access the bullet list
	public List<GameObject> Bullets
	{
		get { return bullets; }
		set { bullets = value; }
	}
	
	// Update is called once per frame
	void Update () 
	{
		// don't update if the game is over
		if (gm.GameOver == false) 
		{
			// check how many asteroids are on the screen, if less than
			// 5, spawn more
			while (asteroids.Count < 5) {
				Spawn (1);
			}

			// spawn a power up randomly
			if(Random.Range(0, 500) == 1)
			{
				// instantiate and add to the list
				powerUps.Add((GameObject)Instantiate(pUp, new Vector3(Random.Range(-5f,6f), 
                      Random.Range(-3f,4f), 0f), Quaternion.identity));
			}

			// check for power up collisions
			foreach(GameObject p in powerUps)
			{
				// if it collides
				if(PowUpCollision(p))
				{
					vMove.PowerUp = true; // activate the power up
					vMove.PTimer = 0f; // reset the timer
					Destroy(p); // remove the power up
					powerUps.Remove(p);
					return;
				}
			}

			// check for bullet collisions
			foreach (GameObject asteroid in asteroids) 
			{
				// they collide
				if (BulletCollision (asteroid)) 
				{
					// remove the asteroid from the list
					asteroids.Remove (asteroid);

					// check what level the asteroid is
					if (asteroid.GetComponent<AsteroidMovement> ().Level == 1) 
					{
						// make two smaller asteroids
						Spawn (2, asteroid);
						Spawn (2, asteroid);
					}

					// add score
					gm.Score += GetLevel (asteroid);

					// destroy the asteroid
					Destroy (asteroid);
					return;
				}
			}

			// check if the player hits an asteroid
			foreach (GameObject asteroid in asteroids) 
			{
				// don't check collisions if invulnerability is active
				if (vMove.Invulnerability == true)
					return;

				// they collide
				if (ShipAstCollisionCheck (asteroid)) 
				{
					vMove.VehiclePos = Vector3.zero; // reset the ship
					vMove.Invulnerability = true; // turn on god mode
					vMove.Timer = 0f; // reset the timer for it
					gm.Lives -= 1; // lose a life
				}
			}
		}

		// handles the game over screen
		if (gm.GameOver) 
		{
			// remove all asteroids
			foreach(GameObject ast in asteroids)
				Destroy(ast);

			// remove all powerups
			foreach(GameObject p in powerUps)
				Destroy(p);

			// remove the ship
			Destroy(ship);
		}

		// restarts the game 
		if (gm.Reset) 
		{
			Application.LoadLevel(0);
		}
	}

	/// <summary>
	/// Spawns a new asteroid when there are less than 5
	/// Sets a random direction and assigns other values
	/// </summary>
	void Spawn(int lvl)
	{
		// pick a random number to select a prefab
		int num = Random.Range (0, 3);

		// get a random position
		Vector3 pos = new Vector3 (Random.Range (-14f, 15f), Random.Range (-4f, 5f), 0f);

		// instantiate a new asteroid
		GameObject temp = (GameObject)Instantiate (prefabs [num], pos, Quaternion.identity);

		// access the asteroid movement script on the asteroid
		AsteroidMovement script = temp.GetComponent<AsteroidMovement> ();

		// gives the asteroid a random direction
		Vector3 dir = Quaternion.Euler (0, 0, Random.Range (0, 360F)) * new Vector3 (0, 1, 0); 

		// set the values of the asteroid
		script.SetValues(1f, pos, dir, num, lvl);

		// add it to the asteroids list
		asteroids.Add (temp);
	}

	/// <summary>
	/// Spawns a new level 2 asteroid
	/// Sets a random direction and assigns other values
	/// </summary>
	/// <param name="lvl">Lvl.</param>
	/// <param name="asteroid">Asteroid.</param>
	void Spawn(int lvl, GameObject asteroid)
	{
		// get the type of the destroyed asteroid
		int num = asteroid.GetComponent<AsteroidMovement> ().Type;

		// start at the position of the previous
		Vector3 pos = asteroid.transform.position;

		// instantiate a new asteroid
		GameObject temp = (GameObject)Instantiate (prefabs [num], pos, Quaternion.identity);
		
		// access the asteroid movement script on the asteroids
		AsteroidMovement script = temp.GetComponent<AsteroidMovement> ();
		AsteroidMovement ast = asteroid.GetComponent<AsteroidMovement> ();

		// set a new direction based off the old
		Vector3 dir = Quaternion.Euler (0, 0, Random.Range (-90, 91)) * ast.Direction;

		// set the values
		script.SetValues (2f, pos, dir, num, lvl);

		// scale the object
		temp.transform.localScale = temp.transform.lossyScale * 0.5f;

		// add it to the asteroids list
		asteroids.Add (temp);
	}

	/// <summary>
	/// Checks for collisions between the ship and an asteroid
	/// </summary>
	/// <returns><c>true</c>, if the ship and asteroid collide <c>false</c> otherwise.</returns>
	bool ShipAstCollisionCheck(GameObject asteroid)
	{
		// get the asteroid info script from the asteroid
		astInfo = asteroid.GetComponent<AsteroidInfo> ();

		// check for overlapping
		// get the distance between centers (squared)
		float distance = (astInfo.Center - sInfo.Center).sqrMagnitude;
		
		// if a squared + b squared > c squared, they are colliding
		if (distance < (astInfo.Radius * astInfo.Radius) + (sInfo.Radius * sInfo.Radius)) 
		{
			return true; // they collide
		} 
		else 
		{
			return false;
		}
	}

	/// <summary>
	/// Check for collisions between the bullet and an asteroid
	/// </summary>
	/// <returns><c>true</c>, if collision was bulleted, <c>false</c> otherwise.</returns>
	/// <param name="asteroid">Asteroid.</param>
	bool BulletCollision(GameObject asteroid)
	{
		if (asteroid != null) // fixes an error on restart
		{
			// get the asteroid info script from the asteroid
			astInfo = asteroid.GetComponent<AsteroidInfo> ();

			// loop through the bullets to check for collisions
			foreach (GameObject bul in bullets) 
			{
				// get the distance between the point and center
				float distance = (astInfo.Center - bul.transform.position).sqrMagnitude; 

				// get the square radius
				float radSquared = astInfo.Radius * astInfo.Radius;

				// used to distinguish between big or small asteroids
				if (radSquared > 1.5f) 
				{
					// collides
					if (distance < radSquared) 
					{
						bullets.Remove (bul);
						Destroy (bul);
						return true; // bullet is inside the radius hence colliding
					}
				} 
				else // radii smaller than 1 cause issues
				{
					if (distance < radSquared + 1.5f) 
					{
						bullets.Remove (bul);
						Destroy (bul);
						return true; // bullet is inside the radius hence colliding
					}
				}

			}
		}
		return false; // not colliding
	}

	/// <summary>
	/// Checks for collisions between the ship and a power up
	/// </summary>
	/// <returns><c>true</c>, if up collision was powed, <c>false</c> otherwise.</returns>
	/// <param name="p">P.</param>
	bool PowUpCollision(GameObject p)
	{
		if (p != null) // fixes an error on restart
		{
			// get the sprite info of the power up
			pInfo = p.GetComponent<PowerUpInfo> ();

			// check for overlapping
			// get the distance between centers (squared)
			float distance = (pInfo.Center - sInfo.Center).sqrMagnitude;
		
			// if a squared + b squared > c squared, they are colliding
			if (distance < (pInfo.Radius * pInfo.Radius) + (sInfo.Radius * sInfo.Radius)) 
			{
				return true;
			} 
			else 
			{
				return false;
			}
		}
		return false; // not colliding
	}

	/// <summary>
	/// Takes an asteroid and calculates the score based on its level
	/// Level 1 = 50, Level 2 = 100
	/// </summary>
	/// <returns>The ast type.</returns>
	/// <param name="asteroid">Asteroid.</param>
	int GetLevel(GameObject asteroid)
	{
		// access the asteroid movement script
		int level = asteroid.GetComponent<AsteroidMovement> ().Level;

		// analyze the level and return the proper amount of points
		if (level == 1) 
		{
			return 50;
		} 
		else 
		{
			return 100;
		}
	}
}
