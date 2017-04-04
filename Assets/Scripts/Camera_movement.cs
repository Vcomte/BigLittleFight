using UnityEngine;
using System.Collections;

public class Camera_movement : MonoBehaviour {

    [SerializeField] private float _roomSize = 0.0f;
	[SerializeField] private float _roomHeight = 0.0f;
    [SerializeField] private int _roomNumber = 0;
	[SerializeField] private int p1FinishRoom = 2;
	[SerializeField] private int p2FinishRoom = -2;

    private int previous_room = 0;
    private Vector3 originalPosition = Vector3.zero;
	private float yOffset = 0.0f;
    private float initialSize;

    public int roomNumber { get { return _roomNumber; } set { _roomNumber = value; } }

    // Use this for initialization
    void Start () {
        if (originalPosition == Vector3.zero)
        {
            originalPosition = this.transform.position;
        }
		GameObject room = GameObject.FindGameObjectWithTag ("Room");
		_roomSize = room.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().bounds.size.x +
			room.transform.GetChild (1).gameObject.GetComponent<Collider2D> ().bounds.size.x * 2;
		_roomHeight = room.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().bounds.size.y;
        initialSize = this.GetComponent<Camera>().orthographicSize;
    }

	// Update is called once per frame
	void FixedUpdate () 
	{
        if(previous_room != roomNumber)
        {
			Vector3 target = Vector3.zero;

			if (roomNumber == p1FinishRoom)
				target = originalPosition + new Vector3 ((roomNumber - 1) * _roomSize, -yOffset + _roomHeight, 0);
			else if (roomNumber == p2FinishRoom)
				target = originalPosition + new Vector3 ((roomNumber + 1) * _roomSize, -yOffset - _roomHeight, 0);
			else
				target = originalPosition + new Vector3 (roomNumber * _roomSize, -yOffset, 0);

			this.transform.position = Vector3.Lerp(this.transform.position, target, 0.05f);

			if (Mathf.Abs(this.transform.position.x - target.x) < 0.02f && Mathf.Abs(this.transform.position.y - target.y) < 0.02f) {
				previous_room = roomNumber;
			}
        }
        else
        {
            GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
            GameObject player2 = GameObject.FindGameObjectWithTag("Player2");

            if (player1 == null && player2 != null)
                player1 = player2;
            else if (player2 == null && player1 != null)
                player2 = player1;
            else if (player1 == null && player2 == null)
                return;

            Vector3 camPosition = this.transform.position;

            float newSize = this.GetComponent<Camera>().orthographicSize;
            float differenceX = Mathf.Abs(player1.transform.position.x - player2.transform.position.x) - 14.6f;
            if(differenceX > 1f) {
                newSize = initialSize + Mathf.Log(differenceX);
            }else
            {
                newSize = initialSize;
            }

            Vector3 player1RelativePosition = new Vector3(player1.transform.position.x, camPosition.y, camPosition.z);
            Vector3 player2RelativePosition = new Vector3(player2.transform.position.x, camPosition.y, camPosition.z);

            this.GetComponent<Camera>().orthographicSize = newSize;
            this.transform.position = Vector3.Lerp(this.transform.position, (player1RelativePosition - player2RelativePosition) * 0.5f + player2RelativePosition, 0.05f);
        }
    }
}