using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	#region private Vars

	private int _hitCounter = 0;
	private int _HpCurrent = 0;
	private float _powerCurrent = 0.0f;
    private bool _isStunned = false, _isGrounded = true, _canJump = true, _isDashing = false;
	private bool _canAttack = true;
	private Coroutine _StunCoroutine;
	private Coroutine _RegenHPCooldownCoroutine;
	private Coroutine _RegenHPCoroutine;
	private Coroutine _AttackCoroutine;
	private Coroutine _ChangeSizeCoroutine;
	private Coroutine _DashCoroutine;
	private HitTrigger _HighHitTrigger = null, _LowHitTrigger = null;
	private Enums.AttackType _ParryDirection = Enums.AttackType.ATTACK_NONE;
	private Enums.CharSize _SizeCurrent = Enums.CharSize.SIZE_BIG;
	private int _QTECurrentPoints = 0;
	private Rigidbody2D myBody = null;
	private bool isPaused = false;

	private bool throwNextFrame = false;
	private bool _throwing = false;
	private GameObject _toThrow = null;
	private bool _toKill = false;
    private bool _delayedKill = false;

    private AudioSource audioSource = null;
	private Animator myAnim = null;

    private float idleTimer = 0.0f;

    #endregion

    #region accessors

    public int qTECurrentPoints		{get{return _QTECurrentPoints;}set{_QTECurrentPoints = value;}}
	public int hitCounter 			{get{return _hitCounter;}set{_hitCounter = value;}}
	public int hpCurrent 			{get{return _HpCurrent;}set{_HpCurrent = value;}}
	public int hpMax 				{get{return _HpMax;}set{_HpMax = value;}}

	public bool isStunned			{get{return _isStunned;}set{_isStunned = value; }}
    public bool canJump             { get { return _canJump; } set { _canJump = value; } }
    public bool isGrounded			{get{return _isGrounded;}set{_isGrounded = value; }}
	public bool isDashing			{get{return _isDashing;}set{_isDashing = value; }}
	public bool canAttack			{get{return _canAttack;}set{_canAttack = value; }}
	public bool faceLeft 			{get{return _faceLeft;}set{_faceLeft = value; }}
	public bool throwing 			{get{return _throwing;}set{_throwing = value; }}
	public bool toKill 				{get{return _toKill;}set{_toKill = value;}}
    public bool delayedKill         {get{return _delayedKill; }set{_delayedKill = value;}}


    public float powerCurrent 		{get{return _powerCurrent;}set{_powerCurrent = value;}}
	public float powerMax 			{get{return _powerMax;}set{_powerMax = value;}}
	public float stunTimer			{get{return _stunTime;}set{_stunTime = value;}}
	public float regenTime			{get{return _regenTime;}set{_regenTime = value;}}
	public float regenCooldown		{get{return _regenCooldown;}set{_regenCooldown = value;}}
	public float dashCooldown		{get{return _dashCooldown;}set{_dashCooldown = value;}}
	public float powerRegenRate		{get{return _powerRegenRate;}set{_powerRegenRate = value;}}
	public float powerDecreaseRate	{get{return _powerDecreaseRate;}set{_powerDecreaseRate = value;}}
	public float dashDamage			{get{return _dashDamage; }set{_dashDamage = value; }}

	public GameObject arrow 		{get{return _arrow;}set{_arrow = value;}}
	public GameObject toThrow 		{get{return _toThrow;}set{_toThrow = value;}}

	public string playerID			{get{return _playerID; }set{_playerID = value; }}

	#endregion

	#region serializable Vars

	[SerializeField] private string _playerID = "P1";
	[SerializeField] private float _stunTime = 0.0f;
	// Time it takes before the player starts regenerating
	[SerializeField] private float _regenCooldown = 0.0f;
	// Time it takes to fully regen to max HP
	[SerializeField] private float _regenTime = 0.0f;
	// Time it takes after a dash to be able to dash again
	[SerializeField] private float _dashCooldown = 0.0f;


	[SerializeField] private int _HpMax = 0;
	[SerializeField] private float _powerMax = 0;

	//Combat stats
	// Time it takes before player can attack again
	[SerializeField] private float attackCooldown = 0.0f;
	[SerializeField] private GameObject hitTrigger_High_Prefab = null, hitTrigger_Low_Prefab = null;
	[SerializeField] private Transform hitTrigger_High_Pos = null, hitTrigger_Low_Pos = null;
	[SerializeField] private Vector3 size_Big = Vector3.zero, size_Small = Vector3.zero;

	//Time it takes to perform a size change
	[SerializeField] private float _sizeChangeTime = 0.35f;
	[SerializeField] private float _powerRegenRate = 0.0f;
	[SerializeField] private float _powerDecreaseRate = 0.0f;

	[SerializeField] private GameObject qTECollider = null;
	[SerializeField] private Vector2 dashForce = Vector2.zero;
	[SerializeField] private GameObject dashCollider = null;
	[SerializeField] private float _dashDamage = 0.0f;

	[Header("SOUND")]
    [SerializeField] private AudioClip footStep = null;
    [SerializeField] private AudioClip strike = null;
    [SerializeField] private AudioClip kick = null;
    [SerializeField] private AudioClip dash = null;
    [SerializeField] private AudioClip jump = null;
    [SerializeField] private AudioClip shrink = null;
    [SerializeField] private AudioClip takeDamage = null;

    [Header("MOVEMENT")]
	[SerializeField] private float smallSpeed = 0.0f;
	[SerializeField] private float jumpSpeed = 0.0f;
	[SerializeField] private float speed = 0.0f;
	[SerializeField] private bool _faceLeft = false;
	[SerializeField] private GameObject _arrow = null;
	[SerializeField] private float throwForce = 500.0f;

	#endregion

	private void Awake()
	{
		Manager_Game.Instance.onPause += Handler_OnPause;
	}

	private void Start()
	{
		_faceLeft = _playerID == "P1" ? false : true;
		myBody = GetComponent<Rigidbody2D> ();
        audioSource = GetComponent<AudioSource>();
		myAnim = transform.GetChild (0).GetComponent<Animator> ();

		hpCurrent = hpMax;
		powerCurrent = 0;
		Manager_Game.Instance.UpdateHealthBars (_playerID.Equals ("P1") ? 1 : 2, hpCurrent, hpMax);
        Manager_Game.Instance.UpdateGoAdvantage(0);
	}

	private void Update()
	{
		if (Manager_Game.Instance.isQTEOngoing || isPaused)
			return;
        idleTimer += Time.deltaTime;

        Manager_Game.Instance.UpdateHealthBars (_playerID.Equals ("P1") ? 1 : 2, hpCurrent, hpMax);

		#region SPECIFIC BIG INTERACTIONS
		if(_SizeCurrent.Equals(Enums.CharSize.SIZE_BIG))
		{
			ResolveInput_Big();
		}

		#endregion

		#region SPECIFIC SMALL INTERACTIONS
		else if(_SizeCurrent.Equals(Enums.CharSize.SIZE_SMALL))
		{
			ResolveInput_Small();
		}

		#endregion

		#region GENERAL INPUT

		ResolveInput_General();
		if (throwing)
			throwNextFrame = true;

		if(_toKill || _delayedKill)
		{
            StartCoroutine(killSelf());
            StartCoroutine(flash());
            _toKill = false;
            _delayedKill = false;
            isStunned = true;
		}
		
		#endregion
	}

	private void ResolveInput_Big()
	{
		if (canAttack && !isStunned) 
		{
			float attackAxis = Input.GetAxis (_playerID + "Attack");
			float parryAxis = Input.GetAxis (_playerID + "Parry");

			//ATTACK CODE
			if (attackAxis != 0.0f && parryAxis == 0.0f) 
			{
				_ParryDirection = Enums.AttackType.ATTACK_NONE;
				//Attack button is pressed: Check for attack direction
				float directionAxis = Input.GetAxis (_playerID + "DirectionY");
                if (directionAxis > 0.15f)
                {
                    //High Attack

                    canAttack = false;
                    Invoke("AttackHigh", 0.2f);
                    myAnim.SetBool("isPunching", true);
                    myAnim.SetBool("isKicking", false);
                    myAnim.SetBool("isRunning", false);

                }
                else if (directionAxis < -0.15f)
                {
                    //Low Attack

                    canAttack = false;
                    Invoke("AttackLow", 0.2f);
                    myAnim.SetBool("isKicking", true);
                    myAnim.SetBool("isPunching", false);
                    myAnim.SetBool("isRunning", false);
                }
            }

            //PARRY CODE 
            if (parryAxis != 0.0f && attackAxis == 0.0f)
            {
                float directionAxis = Input.GetAxis(_playerID + "DirectionY");
                if (directionAxis > 0.15f)
                {
                    // HIGH PARRY
                    _ParryDirection = Enums.AttackType.ATTACK_HIGH;
                    myAnim.SetBool("isPunching", false);
                    myAnim.SetBool("isKicking", false);
                    myAnim.SetBool("isParryingUp", true);
                    myAnim.SetBool("isParrying", true);
                }
                else if (directionAxis < -0.15f)
                {
                    // LOW PARRY
                    _ParryDirection = Enums.AttackType.ATTACK_LOW;
                    myAnim.SetBool("isPunching", false);
                    myAnim.SetBool("isKicking", false);
                    myAnim.SetBool("isParryingUp", false);
                    myAnim.SetBool("isParrying", true);
                }
            }
            else if (parryAxis == 0.0f)
            {
                _ParryDirection = Enums.AttackType.ATTACK_NONE;
                myAnim.SetBool("isParrying", false);
                myAnim.SetBool("isParryingUp", false);
            }
        }

		if(powerCurrent < powerMax)
		{
			powerCurrent += _powerRegenRate * Time.deltaTime;
			Manager_Game.Instance.UpdatePowerBars (_playerID.Equals ("P1") ? 1 : 2, powerCurrent, powerMax);
		}
	}

	private void ResolveInput_Small()
	{
		if(powerCurrent > 0.0f)
		{
			powerCurrent -= powerDecreaseRate * Time.deltaTime;
			Manager_Game.Instance.UpdatePowerBars (_playerID.Equals ("P1") ? 1 : 2, powerCurrent, powerMax);
		}
		else if(powerCurrent <= 0.0f)
		{
			ChangeSize ();
		}
	}

	private void ResolveInput_General()
	{
		float horizontal = Input.GetAxis(_playerID + "DirectionX");
		float vertical = Input.GetAxis(_playerID + "DirectionY");

		// Y BUTTON
		if (Input.GetButtonDown (_playerID + "YButton") && !isStunned && canAttack) 
		{
			ChangeSize ();
		}

		// MOVEMENT
		if (!isDashing && !isStunned)
		{
			float sSpeed = _SizeCurrent.Equals (Enums.CharSize.SIZE_SMALL) ? smallSpeed : 0f;
			float ySpeed;
			bool jump = Input.GetButtonDown (_playerID + "AButton");

			if (jump & _canJump)
			{
				myAnim.SetBool("isJumping", true);
                ySpeed = jumpSpeed + sSpeed;
			} else
			{
				myAnim.SetBool("isJumping", false);
                ySpeed = myBody.velocity.y;
			}

            float attackAxis = Input.GetAxis(_playerID + "Attack");
            float parryAxis = Input.GetAxis(_playerID + "Parry");

            if (attackAxis < 0.15f && attackAxis > -0.15f && parryAxis < 0.15f && parryAxis > -0.15f)
            {
                if (horizontal > 0.25f || horizontal < -0.25f)
                {
                    myAnim.SetBool("isRunning", true);
                    idleTimer = 0.0f;
                }
                else
                {
                    if (idleTimer > 0.1f)
                        myAnim.SetBool("isRunning", false);
                }

                Vector2 velocity = new Vector2(horizontal * (speed + sSpeed), ySpeed);
                myBody.velocity = velocity;
            }
        }

		if (!isStunned)
		{
			//Flipping the player vertically
			if (horizontal > 0.15f && _faceLeft)
			{
				_faceLeft = !_faceLeft;
				this.gameObject.transform.Rotate (new Vector3 (0, 180, 0));
			} else if (horizontal < -0.15f && !_faceLeft)
			{
				_faceLeft = !_faceLeft;
				this.gameObject.transform.Rotate (new Vector3 (0, -180, 0));
			}

			//Vertical commands for aiming
			if (horizontal > 0.01f || vertical > 0.01f || horizontal < -0.01f || vertical < -0.01f)
				arrow.transform.up = (new Vector3 (horizontal, vertical, 0f));
		
			//Commands for throwing an object
			if (Input.GetButtonDown (_playerID + "XButton") && throwNextFrame)
			{
				toThrow.GetComponent<interactiveItem> ().thrown = true;
				toThrow.transform.SetParent (null);
				Rigidbody2D toThrowBody = toThrow.GetComponent<Rigidbody2D> ();
				toThrowBody.isKinematic = false;
				toThrowBody.AddForce (arrow.transform.up * throwForce);

				toThrow.transform.GetComponent<Collider2D> ().isTrigger = false;

				toThrow = null;
				throwing = false;
				throwNextFrame = false;

				arrow.GetComponentInChildren<SpriteRenderer> ().enabled = false;
			}
		}
	}

	private void Handler_OnPause(bool pause)
	{
		// pause the game
		if (pause)
		{
			isPaused = true;
			Time.timeScale = 0;
		}

		//unpause
		else
		{
			isPaused = false;
			Time.timeScale = 1;
		}
	}

	#region Physics
	private void FixedUpdate()
	{
		if (_SizeCurrent.Equals (Enums.CharSize.SIZE_SMALL))
		{
			if(!isDashing)
			{
				float horizontal = Input.GetAxis
					(_playerID + "DirectionX");
				float vertical = Input.GetAxis(_playerID + "DirectionY");

				float attackAxis = Input.GetAxis (_playerID + "Attack");

				if(attackAxis < -0.5f)
				{
					Dash();
					myBody.velocity = Vector2.zero;
					myBody.velocity += (new Vector2(dashForce.x * horizontal, dashForce.y * vertical));
				}
			}
		}
	}

	#endregion

	#region Combat Methods

	private void AttackHigh()
	{
		GameObject newHighTrigger = GameObject.Instantiate (hitTrigger_High_Prefab,
			hitTrigger_High_Pos.position, Quaternion.identity, transform) as GameObject;
		
		_HighHitTrigger = newHighTrigger.GetComponent<HitTrigger> ();
		_HighHitTrigger.Set (_playerID.Equals ("P1") ? "Player2" : "Player1");

		_AttackCoroutine = StartCoroutine (AttackCoroutine (_HighHitTrigger));
	}

	private void AttackLow()
	{
		GameObject newLowTrigger = GameObject.Instantiate (hitTrigger_Low_Prefab,
			hitTrigger_Low_Pos.position, Quaternion.identity, transform) as GameObject;
		
		_LowHitTrigger = newLowTrigger.GetComponent<HitTrigger> ();
		_LowHitTrigger.Set (_playerID.Equals ("P1") ? "Player2" : "Player1");

		_AttackCoroutine = StartCoroutine (AttackCoroutine (_LowHitTrigger));
	}

	public void TakeHit(Enums.AttackType attackType, int attackDamage)
	{
		if (attackType.Equals (_ParryDirection)) 
		{
			
		}
		else
		{
			TakeDamage (attackDamage);
		}
	}

	public void TakeHit()
	{
		if(_SizeCurrent.Equals(Enums.CharSize.SIZE_BIG))
            TakeDamage(30);
	}

	public void TakeDamage(int damageToTake)
	{
		if (isStunned)
			return;

		++hitCounter;
		if (hitCounter >= 3)
		{
			damageToTake *= 3;
			hitCounter = 0;
		}

		hpCurrent -= damageToTake;
		if (_RegenHPCooldownCoroutine != null)
			StopCoroutine (_RegenHPCooldownCoroutine);
		if(_RegenHPCoroutine != null)
			StopCoroutine (_RegenHPCoroutine);

		if (hpCurrent <= 0)
		{
			hpCurrent = 0;
			Stun ();
		}
		else
		{
			//Then start the timer to regen health
			_RegenHPCooldownCoroutine = StartCoroutine (RegenHPCooldownCoroutine ());
		}

		Manager_Game.Instance.UpdateHealthBars (_playerID.Equals ("P1") ? 1 : 2, hpCurrent, hpMax);
	}

	public void StartHpRegenCooldown()
	{
		if (_RegenHPCooldownCoroutine != null)
			StopCoroutine (_RegenHPCooldownCoroutine);

		_RegenHPCooldownCoroutine = StartCoroutine (RegenHPCooldownCoroutine ());
	}

	public void RegenHP()
	{
		if (_RegenHPCoroutine != null)
			StopCoroutine (_RegenHPCoroutine);
		
		_RegenHPCoroutine = StartCoroutine (RegenHPCoroutine());
	}

	public void Stun()
	{

		Manager_Game.Instance.UpdateGoAdvantage(_playerID.Equals("P1") ? 2 : 1);
		hitCounter = 0;
		if (_StunCoroutine != null)
			StopCoroutine (_StunCoroutine);

		_StunCoroutine = StartCoroutine (StunCoroutine ());
	}

	public void Dash()
	{
		transform.GetChild (0).gameObject.layer = 16; /*Dashing*/
        isDashing = true;
		dashCollider.SetActive (true);

		if (_DashCoroutine != null)
			StopCoroutine (_DashCoroutine);

		_DashCoroutine = StartCoroutine (DashCoroutine ());
	}

	#endregion

	#region Size Change Methods

	private void ChangeSize()
	{
		if (_SizeCurrent.Equals(Enums.CharSize.SIZE_BIG)) 
		{
			isStunned = true;
			if (_ChangeSizeCoroutine != null)
				StopCoroutine (_ChangeSizeCoroutine);

			_SizeCurrent = Enums.CharSize.SIZE_SMALL;
			_ChangeSizeCoroutine = StartCoroutine (ChangeSizeCoroutine ());
            transform.GetChild(0).gameObject.layer = 10; // "SmallPlayer"
            myAnim.speed = 2;

			this.transform.GetChild (0).gameObject.GetComponent<TrailRenderer> ().enabled = true;
		}
		else 
		{
			isStunned = true;
			if (_ChangeSizeCoroutine != null)
				StopCoroutine (_ChangeSizeCoroutine);

			_SizeCurrent = Enums.CharSize.SIZE_BIG;
			_ChangeSizeCoroutine = StartCoroutine (ChangeSizeCoroutine ());
            transform.GetChild(0).gameObject.layer = 11; // "Player"
            myAnim.speed = 1;

			this.transform.GetChild (0).gameObject.GetComponent<TrailRenderer> ().enabled = false;
		}

		/*isStunned = true;
		_ChangeSizeCoroutine = StartCoroutine (ChangeSizeCoroutine (_SizeCurrent.Equals(Enums.CharSize.SIZE_BIG) ? size_Small : size_Big));
		_SizeCurrent = _SizeCurrent.Equals (Enums.CharSize.SIZE_BIG) ? Enums.CharSize.SIZE_SMALL : Enums.CharSize.SIZE_BIG;
		*/
	}

	#endregion

	#region Combat Coroutines
	private IEnumerator StunCoroutine()
	{
		isStunned = true;
		float timer = 0.0f;

		while (timer < stunTimer)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		isStunned = false;
		RegenHP ();
		yield return null;
	}

	private IEnumerator RegenHPCooldownCoroutine()
	{
		float timer = 0.0f;
		while(timer < regenCooldown)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		RegenHP ();
		yield return null;
	}

	private IEnumerator RegenHPCoroutine()
	{
		float currentLerpTime = 0.0f;
		float startingHP = (float)hpCurrent;

		while(currentLerpTime < _regenTime)
		{
			currentLerpTime += Time.deltaTime;
			float t = currentLerpTime / regenTime;
			t = Mathf.Sin(t * Mathf.PI * 0.5f);
			hpCurrent = Mathf.RoundToInt(Mathf.Lerp(startingHP, hpMax, t));

			Manager_Game.Instance.UpdateHealthBars (_playerID.Equals ("P1") ? 1 : 2, hpCurrent, hpMax);
			yield return null;
		}

		hpCurrent = hpMax;
		yield return null;
	}

	private IEnumerator AttackCoroutine(HitTrigger trig)
	{
		float timer = 0.0f;

		while(timer < attackCooldown)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		canAttack = true;
		trig.Deactivate ();

        float attackAxis = Input.GetAxis(_playerID + "Attack");
        float directionAxis = Input.GetAxis(_playerID + "DirectionY");

        if (attackAxis < 0.15f && attackAxis > -0.15f)
        {
            //if (directionAxis > -0.15f && directionAxis < 0.15f)
            //{
            myAnim.SetBool("isKicking", false);
            myAnim.SetBool("isPunching", false);
            //}
        }

        yield return null;
	}

	private IEnumerator DashCoroutine()
	{
		float timer = 0.0f;

		while (timer < dashCooldown)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		dashCollider.SetActive (false);
		isDashing = false;
		transform.GetChild (0).gameObject.layer = 10 /*SmallPlayer*/;
        yield return null;
	}

	#endregion

	#region Change Size Coroutines

	private IEnumerator ChangeSizeCoroutine()
	{
		bool isBig = _SizeCurrent.Equals (Enums.CharSize.SIZE_BIG) ? true : false;

		float currentLerpTime = 0.0f;
		Vector3 startSize = Vector3.zero;
		Vector3 targetSize = Vector3.zero;

		if (!isBig)
		{
			startSize = size_Big;
			targetSize = size_Small;
		}
		else
		{
			startSize = size_Small;
			targetSize = size_Big;
		}

		while (currentLerpTime < _sizeChangeTime)
		{
			currentLerpTime += Time.deltaTime;
			float t = currentLerpTime / _sizeChangeTime;
			t = Mathf.Sin(t * Mathf.PI * 0.5f);

			Vector3 target = Vector3.Lerp (startSize, targetSize, t);
			transform.localScale = target;

			yield return null;
		}

		transform.localScale = targetSize;

		if (isBig)
		{
			qTECollider.SetActive (false);
			dashCollider.SetActive (false);
		}
		else
		{
			qTECollider.SetActive (true);
			dashCollider.SetActive (false);
		}

		isStunned = false;
		yield return null;
	}

    #endregion

    #region Self destrucion Coroutines
    IEnumerator killSelf()
    {
        myAnim.Play(playerID.Equals("P1") ? "Antman_Stunned" : "YJ_Stunned");
        Destroy(this.gameObject, delayedKill ? 2 : 0);
        yield return null;
    }

    IEnumerator flash()
    {
        float alpha = transform.GetChild(0).GetComponent<SpriteRenderer>().color.a;
        int coeff = -1;
        while (true)
        {
            alpha = alpha + Time.deltaTime * 3f * coeff;
            Vector4 color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;

            transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Vector4(color.x, color.y, color.z, alpha);
            if (alpha >= 0.99f)
                coeff = -1;
            else if (alpha <= 0.01f)
                coeff = 1;
            yield return null;
        }
    }
    #endregion

    #region collisions

    private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 9 /*ground*/) 
		{
			_isGrounded = true;
            _canJump = true;
		}else if (!_isGrounded && (collision.gameObject.layer == 12 /* bigitems*/ || collision.gameObject.layer == 13 /* smallItems */ || collision.gameObject.layer == 14 /*Scenery*/ || 
            _SizeCurrent.Equals(Enums.CharSize.SIZE_SMALL) && collision.gameObject.layer == 15 /*Small Scenery*/))
        {
            _canJump = true;
        }
	}

	private void OnCollisionExit2D(Collision2D collision){
        if (collision.gameObject.layer == 9 /*ground*/)
        {
            _isGrounded = false;
            _canJump = false;
        }
        else if (!_isGrounded && (collision.gameObject.layer == 12 /* bigitems*/ || collision.gameObject.layer == 13 /* smallItems */ || collision.gameObject.layer == 14 /*Scenery*/ ||
           _SizeCurrent.Equals(Enums.CharSize.SIZE_SMALL) && collision.gameObject.layer == 15 /*Small Scenery*/))
        {
            _canJump = false;
        }
    }

	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.CompareTag (playerID.Equals("P1")? "Player2DashCollider" : "Player1DashCollider"))
		{
			TakeDamage (Mathf.RoundToInt(c.transform.parent.GetComponent<PlayerScript> ().dashDamage));
		}
	}
	#endregion


}
