using UnityEngine;
using TMPro;
using CrimeCommand.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CrimeCommand.UI;

namespace CrimeCommand.Commands {
	public class CommandInput : MonoBehaviour {
		[SerializeField] private int maxCommandsInStack = 300;
		[SerializeField] private Player player = null;
		[SerializeField] private World.World world = null;
		[SerializeField] private SlideUI map = null;
		[SerializeField] private TMP_InputField commandWindow = null;
		[SerializeField] private TMP_InputField inputField = null;
		[SerializeField] private Scrollbar scrollbar = null;

		private int commandStackIndex = -1;
		private readonly List<string> commandStack = new List<string>();

		private void Start() {
			inputField.ActivateInputField();
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
				UpdateText();
				inputField.ActivateInputField();
			}

			if (!player.IsViewingCam && Input.GetKeyDown(KeyCode.Tab)) {
				map.Toggle();
			}

			if (player.IsViewingCam && Input.GetKeyDown(KeyCode.Escape)) {
				player.ViewCam(false);
				commandWindow.GetComponentInParent<SlideUI>().Toggle();
				inputField.ActivateInputField();
				world.ToggleActiveTime();
			}

			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				if (commandStackIndex + 1 < commandStack.Count) {
					commandStackIndex++;
					inputField.text = commandStack[commandStackIndex];
				}
			}

			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				if (commandStackIndex - 1 >= 0) {
					commandStackIndex--;
					inputField.text = commandStack[commandStackIndex];
				} else {
					commandStackIndex = -1;
					inputField.text = "";
				}
			}
		}

		private void UpdateText() {
			string text = ParseCommand(inputField.text);

			commandWindow.text += text + "\n\n";
			StartCoroutine(nameof(ScrollDown));

			inputField.text = "";
		}

		private string ParseCommand(string text) {
			bool hasArgs = text.IndexOf(" ") > -1;
			string commandString = hasArgs ? text.Substring(0, text.IndexOf(" ")) : text;
			string response = $"'{commandString}' is not a recognized command. Type 'help' for more info.";
			string[] args = hasArgs ? text.Substring(text.IndexOf(" ") + 1).Split(' ') : new string[] { };

			foreach (Command command in Command.commands) {
				if (commandString.ToLower() == command.Title.ToLower()) {
					response = command.Execute(world, command, player, commandWindow, args);
					if (commandStack.Count > maxCommandsInStack) {
						commandStack.RemoveAt(commandStack.Count - 1);
					}
					break;
				}
			}

			commandStack.Insert(0, text);
			commandStackIndex = -1;

			return response;
		}

		private IEnumerator ScrollDown() {
			while (scrollbar.value != 0) {
				yield return null;
			}

			scrollbar.value = 1;
		}
	}
}

