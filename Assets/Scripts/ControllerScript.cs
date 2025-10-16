/* Unity Game Programming
 * RoboDash Project
 * RoboDash Author
 * Project Date
 */

using UnityEngine;
using TMPro;


public class ControllerScript : MonoBehaviour 
{
	// public configuration variables
	public float chargeInterval = 3.0f;
	public float monsterInterval = 2.0f;
	public float maxMonsterSpeed = 1.0f;

	// public references to external objects
	public GameObject chargePrefab;
	public GameObject monsterPrefab;
	public TextMeshProUGUI messageText;
	public TextMeshProUGUI scoreText;

    // internal variables to track score and game state
	private int score;
	private bool gameOver;

	// Use this for initialization
	void Start()
	{
		// calculate random delays in spawning initial objects
		float initialChargeDelay = Random.value * chargeInterval;
		float initialMonsterDelay = Random.value * monsterInterval;

		// launch a new power-up charge periodically
		InvokeRepeating ("CreateCharge", initialChargeDelay, chargeInterval);

		// launch a new monster periodically
		InvokeRepeating ("CreateMonster", initialMonsterDelay, monsterInterval);

		// reset message text, score, and gameOver flags
		messageText.text = "";
		gameOver = false;
		score = 0;
		updateScoreText ();

	}

	// this function is invoked periodically
	void CreateCharge()
	{
		// create a charge prefab just off the right side of the screen
		createPrefab (chargePrefab);
	}

	// this function is invoked periodically
	void CreateMonster()
	{
		// create a monster prefab just off the right side of the screen
		GameObject monster = createPrefab (monsterPrefab);

		// we want monsters to move a little bit in relation to
		// the background to make the game more interesting.

		// randomize the monster's speed across the screen
		float xSpeed = Random.value * -maxMonsterSpeed;

		// set monster's speed in the -X direction (right to left)
		Rigidbody2D rb = monster.GetComponent<Rigidbody2D>();
		rb.linearVelocity = new Vector2(xSpeed, 0);
	}

	// utility function to create the specified prefab 
	// just off the right side of the screen
	// anbd between the top of the screen and top of the platform
	GameObject createPrefab(GameObject prefab)
	{
		// calculate the height and width of the screen
		float worldHeight = Camera.main.orthographicSize * 2.0F;
		float worldWidth = worldHeight * Camera.main.aspect;

		// create new prefab and get Renderer
		GameObject newObj = Instantiate (prefab);
		Renderer objR = newObj.GetComponent<Renderer> ();

		// *** student code to finish this function goes here

		// get world Y-coordinate of top of the platform
		GameObject platform = GameObject.Find("PlatformQuad");
		Renderer platformR = platform.GetComponent<Renderer> ();
		float platformTopY = platformR.bounds.max.y;

		// calculate x position just off the side of the screen
		float x = (worldWidth / 2.0F) + objR.bounds.size.x;

		// Calculate random Y position between top of screen
		// and top of platform.  
		// Shrink the range by half the object's height so 
		// object will always be fully visible within that range.
		float maxY = (worldHeight / 2.0F) - (objR.bounds.size.y / 2.0f);
		float minY = platformTopY + (objR.bounds.size.y / 2.0f);

		// get new random Y position between minY and maxY
		float y = Random.Range (minY, maxY);

		// update new prefab's position
		newObj.transform.position = new Vector3 (x, y, 0);

		// send back reference to calling function
		return newObj;  
	}

	// Update is called once per frame
	void Update ()
	{
		// get references to the platform layer
		GameObject platform = GameObject.Find ("PlatformQuad");
		Renderer r = platform.GetComponent<Renderer> ();

		// figure out change in X for the platform
		QuadScroller platformScript = platform.GetComponent<QuadScroller> ();
		float xChange = r.bounds.size.x * Time.deltaTime * platformScript.speed;

		// update x position of all non-player objects in the game

		// find world coordinate of left edge of the screen
		float worldHeight = Camera.main.orthographicSize * 2.0F;
		float worldWidth = worldHeight * Camera.main.aspect;

		// for each tagged object we can find
		updateObjectPositions ("charge", xChange, -worldWidth / 2.0F);
		updateObjectPositions ("shot", xChange, -worldWidth / 2.0F);
		updateObjectPositions ("monster", xChange, -worldWidth / 2.0F);
	}

	// this function will move a group of objects indiciated by the tag
	// the specified amount to the left.  If the object passes the specified
	// left boundary, it will be destroyed.
	void updateObjectPositions(string tag, float xChange, float xLeftBoundary)
	{
		// get all objects with specified tag
		GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

		// *** student code to finish this function goes here
		// for each object
		foreach (GameObject obj in objects) 
		{
			// move this object
			obj.transform.Translate (-xChange, 0, 0);

			// Get the object's Renderer component
			Renderer r = obj.GetComponent<Renderer>();

			// if object X position is less than the left boundary coordinate
			// minus the width of the object (to ensure it's fully off the screen)
			if (obj.transform.position.x < xLeftBoundary - r.bounds.size.x) 
			{
				Destroy (obj);  // destroy this object
			}
		}

	}

	// public function can be called by other scripts
	// to signal that the game is over
	public void GameOver()
	{
		// set flag
		gameOver = true;

		// quit spawning new objects
		CancelInvoke ();	

		// update message text
		messageText.text = "Game Over; good run!";

		// delete all existing monsters and charges
		deleteObjects("charge");
		deleteObjects("shot");
		deleteObjects("monster");
	}

	// this utility function will delete all objects
	// that have the specified tag
	void deleteObjects(string tag)
	{
		// get all objects with tag
		GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

		// for each object
		foreach (GameObject obj in objects) 
		{
			Destroy (obj);  // destroy this object
		}
	}

	// this public function will let other scripts know
	// if the game is over
	public bool IsGameOver()
	{
		return gameOver;
	}

	// this public function can be called by other scripts
	// to increase the score
	public void IncreaseScore()
	{
		score++;
		updateScoreText ();
	}

	// this private utility function will 
	// update the score text
	void updateScoreText()
	{
		scoreText.text = "Score: " + score;
	}


}
