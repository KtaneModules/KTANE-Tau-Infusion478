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

   public string[] Sounds = {"C", "D", "E", "F", "G", "Chord"};
   public Color tauPurple = new Color(0.424f, 0f, 1f, 1f);

   private string tau = "628318530717958647692528676655900576839433879875021164194988918461563281257241799725606965068423413596429617302656461329418768921910116446345071881625696223490056820540387704221111928924589790986076392885762195133186689225695129646757356633054240381829129713384692069722090865329642678721452049828254744917401321263117634976304184192565850818343072873578518072002266106109764093304276829390388302321886611454073151918390618437223476386522358621023709614892475992549913470377150544978245587636602389825966734672488131328617204278989279044947438140435972188740554107843435258635350476934963693533881026";
   private string[] digits;
   private List<string> encryptedPositions = new List<string> {};
   private List<string> encryptedText = new List<string> {};
   private string alphabet = "ZABCDEFGHIJKLMNOPQRSTUVWXY";
   private int digitSeq;
   private int initial;
   private string[] finalDigits;
   private string[] inputDigits;
   private List<int> buttonOrder = new List<int>() {};
   private List<int> buttonsPressed = new List<int>() {};
   private int stage = 0;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake() {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      foreach (KMSelectable Button in Buttons) {
         Button.OnInteract += delegate () { ButtonPress(Button); return false; };
      }
   }

	void ButtonPress(KMSelectable Button) {
      if (ModuleSolved) {
         return;
      }
      for (int i = 0; i < 5; i++) {
         if (Button == Buttons[i]) {
            if (buttonsPressed.Contains(i)) {
               return;
            }
            if (buttonOrder[stage] == i) {
               Displays[i].color = Color.gray;
               Audio.PlaySoundAtTransform(Sounds[stage], Button.transform);
               buttonsPressed.Add(i);
               stage++;
               Debug.LogFormat("[Tau #{0}] Button {1} pressed. Correct.", ModuleId, i + 1);
            } else {
               Strike();
               Debug.LogFormat("[Tau #{0}] Button {1} incorrectly pressed when {2} was expected. Strike!", ModuleId, i + 1, buttonOrder[stage] + 1);
            }
         }
      }
      if (stage == 5) {
         Solve();
      }
	}

	void Start() {
    	ObtainSequence();
		EncryptSequence();
      ContinueSequence();
      inputDigits = ( new int[] { (int.Parse(finalDigits[0]) + int.Parse(finalDigits[1])) % 10, (int.Parse(finalDigits[1]) + int.Parse(finalDigits[2])) % 10, (int.Parse(finalDigits[2]) + int.Parse(finalDigits[3])) % 10, (int.Parse(finalDigits[3]) + int.Parse(finalDigits[4])) % 10, (int.Parse(finalDigits[4]) + int.Parse(finalDigits[5])) % 10}).Select(x => x.ToString()).ToArray();
      Debug.LogFormat("[Tau #{0}] Adding each pair of digits results in {1}.", ModuleId, inputDigits.Join(""));
      FindOrder();
	}

	void ObtainSequence() {
		int i = Bomb.GetSolvableModuleIDs().Contains("pieModule")?Rnd.Range(6, 595):Rnd.Range(0, 589);
      initial = i;
	  	digits = ( new char[] { tau[i], tau[i + 1], tau[i + 2], tau[i + 3], tau[i + 4], tau[i + 5]}).Select(x => x.ToString()).ToArray();
	}

	void EncryptSequence() {
		digitSeq = int.Parse(digits[0] + digits[1] + digits[2] + digits[3] + digits[4] + digits[5]);
      int i = ((digitSeq % 1000000) < 881376)?Rnd.Range(0, 12):Rnd.Range(0, 11);
      digitSeq += (i * 1000000);
      int initialSeq = digitSeq;
      encryptedPositions.Add((digitSeq % 26).ToString());
      for (int j = 0; j < 4; j++) {
         digitSeq = (digitSeq / 26);
         encryptedPositions.Insert(0, (digitSeq % 26).ToString());
      }
      for (int j = 0; j < 5; j++) {
         encryptedText.Add(alphabet[int.Parse(encryptedPositions[j])].ToString());
      }
      Debug.LogFormat("[Tau #{0}] The module displays the letters {1}.", ModuleId, encryptedText.Join(""));
      Debug.LogFormat("[Tau #{0}] This decrypts to {1}, which corresponds to the sequence {2} in tau.", ModuleId, initialSeq, digits.Join(""));
      for (int j = 0; j < 5; j++) {
         Displays[j].text = encryptedText[j];
      }
	}

   void ContinueSequence() {
      int i = initial;
      if (Bomb.GetSolvableModuleIDs().Contains("pieModule")) {
         finalDigits = ( new char[] { tau[i - 6], tau[i - 5], tau[i - 4], tau[i - 3], tau[i - 2], tau[i - 1]}).Select(x => x.ToString()).ToArray();
         Debug.LogFormat("[Tau #{0}] There is a Pie module present, so the continued sequence is {1}.", ModuleId, finalDigits.Join(""));
      } else {
         finalDigits = ( new char[] { tau[i + 6], tau[i + 7], tau[i + 8], tau[i + 9], tau[i + 10], tau[i + 11]}).Select(x => x.ToString()).ToArray();
         Debug.LogFormat("[Tau #{0}] There is no Pie module present, so the continued sequence is {1}.", ModuleId, finalDigits.Join(""));
      }
      if (Bomb.GetSolvableModuleIDs().Contains("pieFlash")) {
         Array.Reverse(finalDigits);
         Debug.LogFormat("[Tau #{0}] However, there is a Pie Flash present, so the sequence is reversed to become {1}.", ModuleId, finalDigits.Join(""));
      }
   }

	void FindOrder() {
      int seqPosition = 0;
      while (buttonOrder.Count() < 5) {
         for (int i = seqPosition; i < 600; i++) {
            if (inputDigits.Contains(tau[i].ToString())) {
               for (int j = 0; j < 5; j++) {
                  if (inputDigits[j] == tau[i].ToString()) {
                  buttonOrder.Add(j);
                  inputDigits[j] = ".";
                  seqPosition = i + 1;
                  break;
                  }
               }
            break;
            }
         }
      }
      Debug.LogFormat("[Tau #{0}] The order to press the buttons is {1}{2}{3}{4}{5}.", ModuleId, buttonOrder[0] + 1, buttonOrder[1] + 1, buttonOrder[2] + 1, buttonOrder[3] + 1, buttonOrder[4] + 1);
	}

	void Activate() {
      return;
	}

	void Solve() {
      ModuleSolved = true;
      StartCoroutine(SolveAnimation());
	}

	void Strike() {
      GetComponent<KMBombModule>().HandleStrike();
	}

   IEnumerator SolveAnimation() {
      yield return new WaitForSeconds(.5f);
      foreach(TextMesh Display in Displays) {
         Display.color = tauPurple;
      }
      Audio.PlaySoundAtTransform("G", transform);
      yield return new WaitForSeconds(.2f);
      foreach(TextMesh Display in Displays) {
         Display.color = Color.gray;
      }
      Audio.PlaySoundAtTransform("F", transform);
      yield return new WaitForSeconds(.2f);
      foreach(TextMesh Display in Displays) {
         Display.color = tauPurple;
      }
      Audio.PlaySoundAtTransform("D", transform);
      yield return new WaitForSeconds(.2f);
      foreach(TextMesh Display in Displays) {
         Display.color = Color.white;
      }
      Audio.PlaySoundAtTransform("Chord", transform);
      yield return new WaitForSeconds(.2f);
      foreach(TextMesh Display in Displays) {
         Display.color = tauPurple;
      }
      Audio.PlaySoundAtTransform("C", transform);
      Displays[0].text = "";
      Displays[1].text = "";
      Displays[2].text = "τ";
      Displays[3].text = "";
      Displays[4].text = "";
      Debug.LogFormat("[Tau #{0}] All buttons have been pressed correctly. Module solved!", ModuleId);
      GetComponent<KMBombModule>().HandlePass();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} 3 4 2 5 1 to press the buttons in a certain order. Buttons are numbered from left to right.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      string[] Commands = Command.Split(' ');
      if (Commands.Length != (5 - buttonsPressed.Count())) {
         yield return "sendtochaterror You must input all " + (5 - buttonsPressed.Count()) + " remaining buttons in one command.";
         yield break;
      }
      for (int i = 0; i < (5 - buttonsPressed.Count()); i++) {
         if (!"12345".Contains(Commands[i])) {
            yield return "sendtochaterror Invalid command.";
            yield break;
         }
      }
      for (int i = 0; i < 5; i++) {
         Buttons[(int.Parse(Commands[i])) - 1].OnInteract();
         yield return new WaitForSeconds(.2f);
         if (buttonsPressed.Count() == 5) {
            yield break;
         }
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      for (int i = 0; i < 5; i++) {
         Buttons[buttonOrder[i]].OnInteract();
         yield return new WaitForSeconds(.2f);
      }
   }
}
