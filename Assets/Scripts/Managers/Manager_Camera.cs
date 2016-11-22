using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Manager_Camera : MonoBehaviour 
{
	#region Singleton
public static Manager_Camera Instance 
	{
		get 
		{
			if (instance == null)
				instance = FindObjectOfType (typeof(Manager_Camera)) as Manager_Camera;

			return instance;
		}
		set 
		{
			instance = value;
		}
	}
private static Manager_Camera instance;
	#endregion

	private float _brightness = 1.1f, _contrast = 1.1f;
	public Material mat = null;

	public float brightness {get{return _brightness;}set{_brightness = value;}}
	public float contrast {get{return _contrast;}set{_contrast = value;}}

	private Camera myCam = null;

	private void Awake()
	{
		myCam = GetComponent<Camera> ();

		SceneManager.activeSceneChanged += LoadNewSceneMethod;
		if (FindObjectsOfType (GetType ()).Length > 1)
			Destroy (gameObject);
	}

	private void LoadNewSceneMethod(Scene src, Scene dest)
	{
		DontDestroyOnLoad (this);

		if (Manager_Menu.Instance.menuState.Equals (Enums.MenuState.MENU_CONNECTION)) {

			RefContainer refs = FindObjectOfType<RefContainer> ();
			transform.position = refs.mainCamera.transform.position;
			transform.rotation = refs.mainCamera.transform.rotation;
			Destroy (refs.mainCamera);
		}
		if (Manager_Menu.Instance.menuState.Equals (Enums.MenuState.MENU_GAME)) 
		{
			RefContainer refs = FindObjectOfType<RefContainer> ();
			transform.position = refs.mainCamera.transform.position;
			transform.rotation = refs.mainCamera.transform.rotation;

			Camera oldCam = refs.mainCamera.GetComponent<Camera> ();
			myCam.projectionMatrix = oldCam.projectionMatrix;
			myCam.gameObject.AddComponent<Camera_movement> ();

			Destroy (refs.mainCamera);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		mat.SetFloat ("_Brightness", brightness);
		mat.SetFloat ("_Contrast", contrast);
		Graphics.Blit (src, dest, mat);
	}
}
