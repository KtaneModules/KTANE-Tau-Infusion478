using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class Tau : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable[] Buttons;
   public TextMesh[] Displays;

   private string tau = "628318530717958647692528676655900576839433879875021164194988918461563281257241799725606965068423413596429617302656461329418768921910116446345071881625696223490056820540387704221111928924589790986076392885762195133186689225695129646757356633054240381829129713384692069722090865329642678721452049828254744917401321263117634976304184192565850818343072873578518072002266106109764093304276829390388302321886611454073151918390618437223476386522358621023709614892475992549913470377150544978245587636602389825966734672488131328617204278989279044947438140435972188740554107843435258635350476934963693533881026";
   private string[] digits;
   private string[] encryptedText;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake() {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      /* foreach (KMSelectable Button in Buttons) {
          Button.OnInteract += delegate () { ButtonPress(Button); return false; }; */
      }

	/* void ButtonPress(KMSelectable Button) {
      Debug.Log("Button " + Button + " pressed.");
	} */

	void Start() {
    	ObtainSequence();
		EncryptSequence();
	}

	void ObtainSequence() {
		int i = Bomb.GetSolvableModuleIDs().Contains("pieModule")?Rnd.Range(6, 595):Rnd.Range(0, 589);
	  	digits = ( new char[] { tau[i], tau[i + 1], tau[i + 2], tau[i + 3], tau[i + 4], tau[i + 5]}).Select(x => x.ToString()).ToArray();
	}

	void EncryptSequence() {
		if (Bomb.GetSolvableModuleIDs().Contains("pieFlash")) {
			Array.Reverse(digits);
		}
	}

	void setDisplay(string[] text) {

	}

	void Activate() {
      return;
	}

	void Solve() {
      GetComponent<KMBombModule>().HandlePass();
	}

	void Strike() {
      GetComponent<KMBombModule>().HandleStrike();
	}

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
