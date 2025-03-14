﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * PinCodeControl - veryfies an input string the a code string (including numbers)
 * Setup:
 * 	1) place your door and implement Interface (ILockable) for lock() and unlock() calls
 * 	    alternatively you can use the demo "Door" prefab.
 *  2) place your display and ensure to have a Text object to adress to (UI->Text)
 * 		alternatively you can use the demo "PinLockDisplay" prefab.
 * 		NOTE: the prefab does not contain an Eventsystem. Be sure to have one in your scene! (UI->Eventsystem)
 *  3) place your pincode and attach the "PinCodeControl" Script
 * 		you can also use the demo "PinLockPanel" or "PinLockPanelDetailed" prefab.
 * 	4) maintain public parameters of the PinLockControl Script. Be sure to link:
 * 		Game Obj Display Text -> Your previously placed UI-Text
 * 		Game Obj Door -> Your previously placed Door that has the Interface Implementation attached
 * 		Audio* Sources -> for granted and denied warnings.
 * 		Maintain additional parameters to fit your needs.
 * 
 * Config Parameters:
 * 	lockCode		The current passcode that is set
 *  maxAttempts		The amount of attempts allowed befor alerting and lockdown
 *  controlCode		The code that needs to be entered after unlocking the door to change the passcode
 *  LockDownTime	The amount of secondds that the door will remain in lockdown after failed attempts
 *  Display*		The text's that appear on screen in a certain state
 *
 * Note:
 * 	"SUBMIT" and "CLEAR" are dedicated strings that are sent to the panelcontrol by specific buttons.
 *  If you intend to build a character keyboard keep that in mind and possibly change them...
 * */
public class PinCodeControl : MonoBehaviour {
	// Links to other relevant GameObjects
	public Text GameObjDisplayText;
	public GameObject GameObjDoor;

	// settings - change it to your desire
	public string lockCode = "1234";			// the code to unlock

	public string DisplayDefault = "IDLE";		// text to display in IDLE mode
	public string DisplayGranted = "UNLOCKED";	// text to display for access granted
	public string DisplayDenied = "DENIED";		// text to display for access denied / wrong code
	public string DisplayNewCode = "NEW CODE?";	// text to promt for new code

	// audio soundsources
	public AudioSource AudioDenied;
	public AudioSource AudioConfirm;
	public bool noAudio = true;					// disable playing audiosources

	// local minions - you can leave those
	private string inputCode;
	private bool setNewCode;
	private const string ctrlCodeSubmit = "SUBMIT";
	private const string ctrlCodeClear = "CLEAR";
	private ILockable availableInterface;

	// Use this for initialization
	void Start () {
		// get the implemented interface from door gameobject
		inputCode = "";
		setNewCode = false;
		UpdateText (DisplayDefault);
	}
	
	// it's public because door hackers should be able to print special text onto the UI
	public void UpdateText(string newText){
		GameObjDisplayText.text = newText;
	}

	// an input was made, add it to the input value
	public void addKeyInput(string key){
		if (key.Equals (ctrlCodeSubmit)) {
			submit();
			return;
		}

		if (key.Equals (ctrlCodeClear)) {
			clearInput();
			UpdateText (DisplayDefault);
			return;
		}
		inputCode += key;
		UpdateText (inputCode);
	}

    // return was pressed, run checks now
    public void submit() {
        if (inputCode.Length == 0) {
            UpdateText(DisplayDefault);
            clearInput();
            return;
        }
    
		// check for controlcode first (special case)
			if(CheckCode() == false){
				// failed attempt!
				availableInterface.Lock();
				UpdateText(DisplayDenied);
				if(!noAudio)
					AudioDenied.Play();
			}else{
            // grant access
            if (GameObjDoor.tag == "File")
            {
                GameObjDoor.transform.position = new Vector3(GameObjDoor.transform.position.x, 2.533f, GameObjDoor.transform.position.z);
            } else if (GameObjDoor.tag == "Door")
            {
                GameObjDoor.transform.Rotate(0, 90, 0);
            }
            UpdateText(DisplayGranted);
			}
		
		clearInput();
	}

	// clear was pressed, reset input
	public void clearInput(){
		inputCode = "";
	}

	// Check if entered pin code is correct
	private bool CheckCode(){
		if (lockCode.Equals (inputCode)) {
			return true;
		} else {
			return false;
		}
	}
}
