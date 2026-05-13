using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The namespace for the project is StarCooperation. For compatibility, all classes should be within this namespace.
/// The namespace can be extended, e.g. StarCooperation.Localization.
/// </summary>
namespace StarCooperation
{
	public class Styleguide : MonoBehaviour
	{
		// The coding styleguide mainly uses the standard .NET Framework naming convention system (https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/x2dbyw72(v=vs.71)
		// and tries to use the Unity standard as far as possible. Exceptions and other cases are denoted in this script.

		/////////////////////////////////////////////
		// Order of class elements in each script:
		//
		// nested classes
		// public static fields
		// public events
		// public enums
		// serialized(public or protected/private + [SerializeField]) fields
		// public properties

		// private fields
		// private properties

		// Monobehaviour methods
		// Class methods

		/////////////////////////////////////////////
		// Naming convention for functions/variables:
		// Functions start with a big letter, e.g. MyFunction().
		// Member variables start with a small letter, e.g. myVariable.
		// Private and public fields are -not- separated by a leading "m" or any other identifier.



		// All class members and functions should denote is accessibility, e.g. private, public, etc.
		public static Styleguide instance;

		// Serialized fields for inspector should be grouped under descriptive headers
		[Header("Descriptive Header")]

		// Fields and any kind of variables should be named from general purpose/meaning to special use-case, e.g.
		public float speedPlayerRotation;			// good
		public float speedPlayerTranslation;		// good
		public float speedCameraRotation;			// also good, depends what is more general, what is more use-case
		public Color fontColor;						// also good, because it's basically one word/phrase
		public float playerDuckJumpSpeedRotation;	// not good. Whats stored in here?

		// Names can be abbreviated where it helps the readability, but not be too short for others to understand.
		public Vector3 tempPos;						// this is perfectly fine
		public float tSpeedAbr;                     // what does this mean?

		// Properties can start with an uppercase, but don't have to
		private bool backingType;
		public bool MyType							// Could be "myType" as well
		{
			// Exepction from curly braces rule: Single-line braces in property accessors
			get	{ return backingType; }	
			set
			{
				backingType = value;
				// Do more stuff here;
			}
		}

		// At least one line space between blocks that belong together (e.g. public/private fields or same kind of logic)
		// Bools should start with an "is", however this is -not- mandatory
		private bool isOn = true;
		private bool isOff = true;

		private Vector3 startPos;

		private void Awake()
		{
			// Every initialization that affects this Monobehaviour only should be done in Awake, so the Start() routine can be used for inter-script access.
			instance = this;
			startPos = transform.position;
		}

		private void Start()
		{
			// Start() is for anything that must be called after Awake, e.g. access to other scripts.
			if (MoveCamera.instance != null)
			{
				ShowMeTheStyleguide();
			}
		}

		// Update is called once per frame
		private void Update()
		{

		}

		public void ShowMeTheStyleguide()
		{
			/////////////////////////////////////////////
			// Blocks of code that CAN contain curly braces should always use curly braces on a dedicated line.
			// This is valid for if, while and any other kind of code block.

			// This is CORRECT:
			if (isOn == isOff)
			{
				UpdatePosition();
			}
			else
			{
				return;
			}

			UnityEngine.UI.Toggle myToggle = GetComponent<UnityEngine.UI.Toggle>();
			myToggle.onValueChanged.AddListener(delegate
			{
				// DoDelegatedStuff();
			});

			// This is INCORRECT:
			if (isOn == isOff)
				UpdatePosition();
			else
				return;

			if (isOn == isOff)	{
				UpdatePosition();
			}
			else {
				return;
			}
	}

		/// <summary>
		/// Each function that is not self-explanatory by its name or needs clarification in behavior, requires a proper summary like this one.
		/// All functions should begin with a verb and clearly state what they do.
		/// </summary>
		private void UpdatePosition()
		{
			StartCoroutine(DoUpdatePosition());
		}

		/// <summary>
		/// When a non-coroutine function simply calls a coroutine, this coroutine should have the same name as the calling function, preceeded by a "Do" or a "Co".
		/// </summary>
		/// <returns></returns>
		private IEnumerator DoUpdatePosition()
		{
			yield return null;
		}

		/// <summary>
		/// Exception from curly braces rule: Empty funtions.
		/// </summary>
		public virtual void EmptyMethod() { }
	}
}
