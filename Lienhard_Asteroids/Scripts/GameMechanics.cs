using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Handles non-gameplay functions like score and lives
/// Attatched to the scene manager to help run the game
/// </summary>
public class GameMechanics : MonoBehaviour 
{
	// score of the player
	private int score;

	// number of lives -> max = 3
	private int lives;

	// game over bool ends the game if lives = 0
	private bool gameOver;

	// camera to access
	public Camera cam;

	// text component for the Score canvas
	private Text uiScore;

	// text component for the game over screen
	private Text uiGO;

	// resets the game on command
	private bool reset;

	// properties to be accessed by other classes
	public int Score
	{
		get { return score; }
		set { score = value; }
	}

	public int Lives
	{
		get { return lives; }
		set { lives = value; }
	}

	public bool Reset
	{
		get { return reset; }
		set { reset = value; }
	}

	public bool GameOver
	{
		get { return gameOver; }
		set { gameOver = value; }
	}
	// Use this for initialization
	void Start () 
	{
		// set score to 0
		score = 0;

		// set lives to 3
		lives = 3;

		// don't immediately end the game
		gameOver = false;

		// set the Text components
		uiScore = GameObject.Find ("Score_Lives").GetComponent<Text> ();
		uiGO = GameObject.Find ("GameOver").GetComponent<Text> ();

		// don't reset the game
		reset = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// if the player loses all their lives: game over
		if (lives == 0) 
		{
			gameOver = true;
		}

		// set the text for the UI
		uiScore.text = "Score: " + score + "         Lives = " + lives;
		uiGO.text = "GAME OVER \n Score: " + score + "\nPress ENTER to play again";

		// enable the score/lives but not the game over UI
		uiGO.enabled = false;
		uiScore.enabled = true;

		// when game over occurs, show the game over screen
		if (gameOver) 
		{
			// enable/disable the correct UIs
			uiGO.enabled = true;
			uiScore.enabled = false;

			// restart the game when the player hits return
			if(Input.GetKeyDown(KeyCode.Return))
			{
				reset = true;
				gameOver = false;
			}
		}
	}


}
