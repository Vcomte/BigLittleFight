using UnityEngine;
using System.Collections;

public class RoomBehaviour : MonoBehaviour {

    private GameObject player1 = null;
    private GameObject player2 = null;

    [SerializeField] private Collider2D goZone1 = null;
    [SerializeField] private Collider2D goZone2 = null;

    [SerializeField] private int roomNumber = 0;

    [SerializeField] private GameObject nextRoomPlayer1 = null;
    [SerializeField] private GameObject nextRoomPlayer2 = null;

	[SerializeField] private GameObject p1Prefab = null, p2Prefab = null;

    //Booleans used to determine who won the whole game
    [SerializeField] private int player1Wins = -1;
    [SerializeField] private int player2wins = 1;

    private GoZoneBehaviour triggeredGoZone = null;

    private bool currentRoom = false;

    //This boolean will be used to store which player won the round, true for player 1
	private bool isP1Winner = true;

    //QTE Sprite
    [SerializeField] private GameObject[] QTEPrefab = null;

    Coroutine QTECoroutine = null;

    // Use this for initialization
    void Start () {
	    if (roomNumber == 0)
        {
            currentRoom = true;
        }
	}
	
	// Update is called once per frame
    // Changer les GetComponent sur les points de vie pour moins d'appels à getcomponent
	void Update () {
        if (currentRoom)
        {
            //QTE
            if (Manager_Game.Instance.isQTEOngoing && QTECoroutine == null)
              QTECoroutine = StartCoroutine(popSpriteQTE());
            else if(!Manager_Game.Instance.isQTEOngoing && QTECoroutine != null)
                StopCoroutine(QTECoroutine);

			if (player1 && player2) {
				if (player1.GetComponent<PlayerScript> ().hpCurrent <= 0 && !goZone2.enabled) 
				{
					player1.GetComponent<PlayerScript> ().delayedKill = true;
                    if (roomNumber != player2wins)
                    {
                        goZone2.enabled = true;
                        triggeredGoZone = goZone2.gameObject.GetComponent<GoZoneBehaviour>();
                        isP1Winner = false;
                    }
                    else
                    {
                        Manager_Game.Instance.UpdateGoAdvantage(0);
                        Manager_Game.Instance.DisplayVictory(2);
                    }
				}
				if (player2.GetComponent<PlayerScript> ().hpCurrent <= 0 && !goZone1.enabled) {
					player2.GetComponent<PlayerScript> ().delayedKill = true;
                    if (roomNumber != player1Wins)
                    {
                        goZone1.enabled = true;
                        triggeredGoZone = goZone1.gameObject.GetComponent<GoZoneBehaviour>();
                        isP1Winner = true;
                    }
                    else
                    {
                        Manager_Game.Instance.UpdateGoAdvantage(0);
                        Manager_Game.Instance.DisplayVictory(1);
                    }
				}
			}
			if (player1 == null && player2 == null)
			{
				player1 = Instantiate(p1Prefab) as GameObject;
				player1.transform.position = this.transform.position + new Vector3(-transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 4, -2.0f, 0);

				player2 = Instantiate(p2Prefab) as GameObject;
				player2.transform.position = this.transform.position + new Vector3(transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 4, -2.0f, 0);
			}
            if (triggeredGoZone)
            {
                if (triggeredGoZone.triggered)
                {
                    if (isP1Winner)
                    {
						player1.GetComponent<PlayerScript> ().toKill = true;
                        nextRoomPlayer1.GetComponent<RoomBehaviour>().currentRoom = true;
                        Camera.main.GetComponent<Camera_movement>().roomNumber += 1;
                    }
                    else
                    {
						player2.GetComponent<PlayerScript>().toKill = true;
                        nextRoomPlayer2.GetComponent<RoomBehaviour>().currentRoom = true;
						Camera.main.GetComponent<Camera_movement>().roomNumber -= 1;
                    }
                    currentRoom = false;
					triggeredGoZone.triggered = false;
                }
            }
        }
    }

    IEnumerator popSpriteQTE()
    {
        int i = 0;
        while (true)
        {
            Vector3 pos = this.transform.position;
            Vector3 newPos = new Vector3(pos.x + Random.Range(-5, 5), pos.y + Random.Range(-2, 6), 0);
            Instantiate(QTEPrefab[i], newPos, this.transform.rotation);
            yield return new WaitForSecondsRealtime(Random.Range(0.1f, 1f));
            ++i;
            if (i == 3)
                i = 0;
        }
    }
}
 