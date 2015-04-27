using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	public Animator p1Text;

	void Update()
	{
		if( Input.GetButtonDown( "Start" ) )
			p1Text.enabled = true;
	}

	/*
	public Animator startButton;
	public Animator settingsButton;
	public Animator dialog;
	public Animator contentPanel;
	public Animator gearImage;

	void Start()
	{
		RectTransform transform = contentPanel.gameObject.transform as RectTransform;
		Vector2 position = transform.anchoredPosition;
		position.y -= transform.rect.height;
		transform.anchoredPosition = position;
	}

	public void ToggleMenu()
	{
		contentPanel.enabled = true;

		bool isHidden = contentPanel.GetBool( "isHidden" );
		contentPanel.SetBool( "isHidden", !isHidden );

		gearImage.enabled = true;
		gearImage.SetBool( "isHidden", !isHidden );
	}

	public void OpenSettings()
	{
		startButton.SetBool( "isHidden", true );
		settingsButton.SetBool ( "isHidden", true );

		dialog.enabled = true;
		dialog.SetBool( "isHidden", false );
	}

	public void CloseSettings()
	{
		startButton.SetBool( "isHidden", false );
		settingsButton.SetBool( "isHidden", false );
		dialog.SetBool( "isHidden", true );
	}

	public void StartGame()
	{
		Application.LoadLevel("main");
	}
	*/
}
