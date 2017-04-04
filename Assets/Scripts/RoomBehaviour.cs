using UnityEngine;
using System.Collections;

public class RoomBehaviour : MonoBehaviour {

    private GameObject player1 = null;
    private GameObject player2 = null;

    [SerializeField] private GoZoneBehaviour goZone1 = null;
    [SerializeField] private GoZoneBehaviour goZone2 = null;

    [SerializeField] private int roomNumber = 0;

    [SerializeField] private GameObject nextRoomPlayer1 = null;
    [SerializeField] private GameObject nextRoomPlayer2 = null;

	[SerializeField] private GameObject p1Prefab = null, p2Prefab = null;

    //Booleans used to determine who won the whole game
    [SerializeField] private int player1Wins = -1;
    [SerializeField] private int player2Wins = 1;

    private GoZoneBehaviour triggeredGoZone = null;
    private bool endGame = false;

    private bool currentRoom = false;

    //This boolean will be used to store which player won the round, true for player 1
	private bool isP1Winner = true;

    // Use this for initialization
    void Start () {
	    if (roomNumber == 0)
        {
            currentRoom = true;
        }
	}
	
	private void Update () 
	{
        if (currentRoom)
        {
            // TODO : refactor this shit because it's useless now. delayedKill should go to playerscript
			if (player1 && player2) 
			{
				if (!player1.GetComponent<PlayerScript>().isAlive) 
				{
					player1.GetComponent<PlayerScript>().delayedKill = true;
				}
				if (!player2.GetComponent<PlayerScript> ().isAlive) {
					player2.GetComponent<PlayerScript> ().delayedKill = true;
				}
			}
            // Initial pop
			if (player1 == null && player2 == null)
			{
				player1 = Instantiate(p1Prefab) as GameObject;
				player1.transform.position = this.transform.position + new Vector3(-transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 4, -2.0f, 0);

				player2 = Instantiate(p2Prefab) as GameObject;
				player2.transform.position = this.transform.position + new Vector3(transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 4, -2.0f, 0);
			}
            // Repop when killed
            else if (player1 == null && Mathf.Abs(player2.transform.position.x - goZone2.transform.position.x) > 6f && !endGame)
            {
                player1 = Instantiate(p1Prefab) as GameObject;
                player1.transform.position = player2.transform.position + new Vector3(-6f, 0, 0);
            }
            else if (player2 == null && Mathf.Abs(player1.transform.position.x - goZone1.transform.position.x) > 6f && !endGame)
            {
                player2 = Instantiate(p2Prefab) as GameObject;
                player2.transform.position = player1.transform.position + new Vector3(6f, 0, 0);
            }

           
            if (goZone1.triggered)
            {
                if (roomNumber != player1Wins)
                {
                    player1.GetComponent<PlayerScript>().toKill = true;
                    player2.GetComponent<PlayerScript>().toKill = true;
                    nextRoomPlayer1.GetComponent<RoomBehaviour>().currentRoom = true;
                    Camera.main.GetComponent<Camera_movement>().roomNumber += 1;
                    currentRoom = false;
                    goZone1.triggered = false;
                }
                else
                {
                    endGame = true;
                    Manager_Game.Instance.UpdateGoAdvantage(0);
                    Manager_Game.Instance.DisplayVictory(1);
                }
            }
            else if(goZone2.triggered)
            {
                if (roomNumber != player2Wins)
                {
                    player2.GetComponent<PlayerScript>().toKill = true;
                    player1.GetComponent<PlayerScript>().toKill = true;
                    nextRoomPlayer2.GetComponent<RoomBehaviour>().currentRoom = true;
                    Camera.main.GetComponent<Camera_movement>().roomNumber -= 1;
                    currentRoom = false;
                    goZone2.triggered = false;
                }
                else
                {
                    endGame = true;
                    Manager_Game.Instance.UpdateGoAdvantage(0);
                    Manager_Game.Instance.DisplayVictory(2);
                }
            }
                
        }
    }

}
 