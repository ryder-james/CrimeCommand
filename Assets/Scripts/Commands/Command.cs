using TMPro;
using CrimeCommand.World;
using CrimeCommand.UI;

namespace CrimeCommand.Commands {
	public class Command {
		public delegate string ExecuteCommand(World.World world, 
			Command caller, Player player, TMP_InputField commandWindow, string[] args);

		public string Title { get; set; }
		public string Usage { get; set; }
		public string Description { get; set; }
		public ExecuteCommand Execute { get; set; }

		public static readonly Command[] commands = new Command[] {
			new Command() {
				Title = "Clear",
				Usage = "clear",
				Description = "Clears the command window.",
				Execute = (world, caller, player, commandWindow, args) => {
					commandWindow.text = "";
					return "";
				}
			}, // Clear
			new Command() {
				Title = "Move",
				Usage = "Usage: move forward|back|left|right",
				Description = "Tells the operative to move in a given direction.",
				Execute = (world, caller, player, commandWindow, args) => {
					if (args.Length < 1) {
						return "Unknown direction. " + caller.Usage;
					}

					string dir;

					args[0] = args[0].ToLower();

					switch (args[0]) {
					case "forward":
					case "f":
					case "^":
						player.Move(Direction.Forward);
						dir = "forward";
						break;
					case "back":
					case "b":
					case "v":
						player.Move(Direction.Back);
						dir = "backward";
						break;
					case "left":
					case "l":
					case "<":
						player.Move(Direction.Left);
						dir = "left";
						break;
					case "right":
					case "r":
					case ">":
						player.Move(Direction.Right);
						dir = "right";
						break;
					default:
						return "Unknown direction. " + caller.Usage;
					}

					return "Moved " + dir;
				}
			}, // Strafe
			new Command() {
				Title = "Turn",
				Usage = "turn around|left|right",
				Description = "Tells the operative to turn a given direction.",
				Execute = (world, caller, player, commandWindow, args) => {
					if (args.Length < 1) {
						return "Unknown direction. " + caller.Usage;
					}

					string dir;
					args[0] = args[0].ToLower();
					switch (args[0]) {
					case "around":
					case "a":
					case "v":
						player.Turn(Direction.Back);
						dir = "around";
						break;
					case "left":
					case "l":
					case "<":
						player.Turn(Direction.Left);
						dir = "left";
						break;
					case "right":
					case "r":
					case ">":
						player.Turn(Direction.Right);
						dir = "right";
						break;
					default:
						return "Unknown direction. " + caller.Usage;
					}
					return "Turned " + dir;
				}
			}, // Turn
			new Command() {
				Title = "Hack",
				Usage = "hack <camera name>",
				Description = "Hacks a specific camera.",
				Execute = (world, caller, player, commandWindow, args) => {
					if (player.Hack(args[0])) {
						return $"{args[0]} hacked!";
					}

					return $"{args[0]} not found.";
				}
			}, // Hack
			new Command() {
				Title = "View",
				Usage = "view",
				Description = "Changes the viewpoint from the operative to the current hacked camera./n" +
							  "<color=\"red\">WARNING!! Time turns back on while looking through a camera!<color=\"white\">",
				Execute = (world, caller, player, commandWindow, args) => {
					if (player.ViewCam(true)) {
						commandWindow.GetComponentInParent<SlideUI>().Toggle();
						world.ToggleActiveTime();
						return "";
					}

					return "No camera hacked!";
				}
			}, // View
			new Command() {
				Title = "Steal",
				Usage = "steal",
				Description = "Steals a painting infront of you.",
				Execute = (world, caller, player, commandWindow, args) => {
					if (player.WallChecker.TouchPainting) {
						player.WallChecker.Painting.SetActive(false);
						return $"Stolen!";
					}

					return $"No painting in range.";
				}
			},

			new Command() {
				Title = "Wait",
				Usage = "wait",
				Description = "Allows one cycle to pass without action.",
				Execute = (world, caller, player, commandWindow, args) => {
					world.Step();
					return "Waiting...";
				}
			}, // Wait
			new Command() {
				Title = "Help",
				Usage = "clear",
				Description = "Clears the command window.",
				Execute = (world, caller, player, commandWindow, args) => {
					string response = "";

					foreach (Command c in commands) {
						if (c.Title != caller.Title && c.Title != "Dance") {
							response += $"<color=\"green\">{c.Usage}<color=\"white\">:\n";
							response += $"{c.Description}\n\n";
						}
					}

					return response.Substring(0, response.Length - 2);
				}
			}, // Help
		};
	}
}