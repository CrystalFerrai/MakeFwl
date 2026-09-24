// Copyright 2026 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MakeFwl
{
	/// <summary>
	/// Loads world modifiers from a file and serializes them to the main output file
	/// </summary>
	internal class Modifiers
	{
		public Modifier_Preset Preset { get; set; } = Modifier_Preset.Unset;

		public Modifier_Combat Combat { get; set; } = Modifier_Combat.Unset;

		public Modifier_DeathPenalty DeathPenalty { get; set; } = Modifier_DeathPenalty.Unset;

		public Modifier_Resources Resources { get; set; } = Modifier_Resources.Unset;

		public Modifier_Raids Raids { get; set; } = Modifier_Raids.Unset;

		public Modifier_Portals Portals { get; set; } = Modifier_Portals.Unset;

		public bool? NoBuildCost { get; set; } = null;

		public bool? PlayerEvents { get; set; } = null;

		public bool? Fire {  get; set; } = null;

		public bool? PassiveMobs { get; set; } = null;

		public bool? NoMap { get; set; } = null;

		// Modifier research notes
		// Names map to entries in the GlobalKeys enumeration
		// Game.UpdateWorldRates maps modifiers to actual game values
		// ServerOptionsGUI is the UI for configuring modifiers.
		//  - Values are not present in code. Must be in assets somewhere.

		/// <summary>
		/// Attempt to load a modifiers.txt file and output a Modifiers object if successful
		/// </summary>
		public static bool TryLoad(string? path, [NotNullWhen(true)] out Modifiers? modifiers)
		{
			if (path is null)
			{
				modifiers = new();
				return true;
			}

			try
			{
				using (FileStream modifiersFile = File.OpenRead(path))
				{
					return TryLoad(modifiersFile, out modifiers);
				}
			}
			catch
			{
				Console.Error.WriteLine($"Error reading modifiers file: {path}");

				modifiers = null;
				return false;
			}
		}

		/// <summary>
		/// Attempt to load a modifiers.txt file and output a Modifiers object if successful
		/// </summary>
		public static bool TryLoad(Stream stream, [NotNullWhen(true)] out Modifiers? modifiers)
		{
			modifiers = null;
			Modifiers instance;

			try
			{
				using StreamReader reader = new(stream, Encoding.UTF8, leaveOpen: true);

				instance = new();
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine()!.Trim();

					// Skip comments
					if (line.StartsWith('#')) continue;

					// Skip empty lines
					if (string.IsNullOrEmpty(line)) continue;

					string[] parts = line.Split('=');
					if (parts.Length != 2)
					{
						Console.Error.WriteLine($"Modifier input line could not be parsed. Expected 'name = value'. Line: '{line}'");
						return false;
					}

					string name = parts[0].Trim().ToLowerInvariant();
					string value = parts[1].Trim().ToLowerInvariant();

					switch (name)
					{
						case "preset":
							instance.Preset = ParseEnum<Modifier_Preset>(name, value);
							break;
						case "combat":
							instance.Combat = ParseEnum<Modifier_Combat>(name, value);
							break;
						case "deathpenalty":
							instance.DeathPenalty = ParseEnum<Modifier_DeathPenalty>(name, value);
							break;
						case "resources":
							instance.Resources = ParseEnum<Modifier_Resources>(name, value);
							break;
						case "raids":
							instance.Raids = ParseEnum<Modifier_Raids>(name, value);
							break;
						case "portals":
							instance.Portals = ParseEnum<Modifier_Portals>(name, value);
							break;
						case "nobuildcost":
							instance.NoBuildCost = ParseBool(name, value);
							break;
						case "playerevents":
							instance.PlayerEvents = ParseBool(name, value);
							break;
						case "fire":
							instance.Fire = ParseBool(name, value);
							break;
						case "passivemobs":
							instance.PassiveMobs = ParseBool(name, value);
							break;
						case "nomap":
							instance.NoMap = ParseBool(name, value);
							break;
						default:
							Console.Error.WriteLine($"Unexpected modifier property '{name}' will be skipped.");
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Failed to load modifiers. Error: [{ex.GetType().FullName}] {ex.Message}");
				return false;
			}

			modifiers = instance;
			return true;
		}

		/// <summary>
		/// Write the current modifiers to the main output stream
		/// </summary>
		public void Serialize(Stream stream)
		{
			IReadOnlyList<string> data = Serialize();

			using BinaryWriter writer = new(stream, Encoding.ASCII, true);
			writer.Write(data.Count);

			foreach (string datum in data)
			{
				writer.Write(datum);
			}
		}

		private IReadOnlyList<string> Serialize()
		{
			Dictionary<string, string> presets = new()
			{
				{ "combat", "default" },
				{ "deathpenalty", "default" },
				{ "resources", "default" },
				{ "raids", "default" },
				{ "portals", "default" }
			};

			HashSet<string> flags = new();
			
			switch (Preset)
			{
				case Modifier_Preset.Easy:
					presets["combat"] = "easy";
					presets["raids"] = "less";
					break;
				case Modifier_Preset.Normal:
					break;
				case Modifier_Preset.Hard:
					presets["combat"] = "hard";
					presets["raids"] = "more";
					break;
				case Modifier_Preset.Hardcore:
					presets["combat"] = "veryhard";
					presets["deathpenalty"] = "hardcore";
					presets["raids"] = "more";
					presets["portals"] = "hard";
					flags.Add("nomap");
					break;
				case Modifier_Preset.Casual:
					presets["combat"] = "veryeasy";
					presets["deathpenalty"] = "casual";
					presets["resources"] = "more";
					presets["raids"] = "none";
					presets["portals"] = "casual";
					flags.Add("playerevents");
					flags.Add("passivemobs");
					break;
				case Modifier_Preset.Hammer:
					presets["raids"] = "none";
					flags.Add("nobuildcost");
					flags.Add("passivemobs");
					break;
				case Modifier_Preset.Immersive:
					presets["portals"] = "veryhard";
					flags.Add("fire");
					flags.Add("nomap");
					break;
			}

			if (Combat != Modifier_Combat.Unset)
			{
				presets["combat"] = Combat.ToString().ToLowerInvariant();
			}
			if (DeathPenalty != Modifier_DeathPenalty.Unset)
			{
				presets["deathpenalty"] = DeathPenalty.ToString().ToLowerInvariant();
			}
			if (Resources != Modifier_Resources.Unset)
			{
				presets["resources"] = Resources.ToString().ToLowerInvariant();
			}
			if (Raids  != Modifier_Raids.Unset)
			{
				presets["raids"] = Raids.ToString().ToLowerInvariant();
			}
			if (Portals != Modifier_Portals.Unset)
			{
				presets["portals"] = Portals.ToString().ToLowerInvariant();
			}

			Dictionary<string, int> modifiers = new();
			foreach (var pair in presets)
			{
				switch (pair.Key)
				{
					case "combat":
						switch (pair.Value)
						{
							case "veryeasy":
								modifiers["playerdamage"] = 125;
								modifiers["enemydamage"] = 50;
								modifiers["enemyspeedsize"] = 90;
								break;
							case "easy":
								modifiers["playerdamage"] = 110;
								modifiers["enemydamage"] = 75;
								modifiers["enemyspeedsize"] = 95;
								break;
							case "hard":
								modifiers["playerdamage"] = 85;
								modifiers["enemydamage"] = 150;
								modifiers["enemyspeedsize"] = 110;
								modifiers["enemyleveluprate"] = 120;
								break;
							case "veryhard":
								modifiers["playerdamage"] = 70;
								modifiers["enemydamage"] = 200;
								modifiers["enemyspeedsize"] = 120;
								modifiers["enemyleveluprate"] = 140;
								break;
						}
						break;
					case "deathpenalty":
						switch (pair.Value)
						{
							case "casual":
								flags.Add("deathkeepequip");
								modifiers["skillreductionrate"] = 15;
								break;
							case "veryeasy":
								modifiers["skillreductionrate"] = 15;
								break;
							case "easy":
								modifiers["skillreductionrate"] = 50;
								break;
							case "hard":
								modifiers["skillreductionrate"] = 150;
								flags.Add("deathdeleteunequipped");
								break;
							case "hardcore":
								flags.Add("deathdeleteitems");
								flags.Add("deathskillsreset");
								break;
						}
						break;
					case "resources":
						switch (pair.Value)
						{
							case "muchless":
								modifiers["resourcerate"] = 50;
								break;
							case "less":
								modifiers["resourcerate"] = 75;
								break;
							case "more":
								modifiers["resourcerate"] = 150;
								break;
							case "muchmore":
								modifiers["resourcerate"] = 200;
								break;
							case "most":
								modifiers["resourcerate"] = 300;
								break;
						}
						break;
					case "raids":
						switch (pair.Value)
						{
							case "none":
								modifiers["eventrate"] = 0;
								break;
							case "muchless":
								modifiers["eventrate"] = 200;
								break;
							case "less":
								modifiers["eventrate"] = 150;
								break;
							case "more":
								modifiers["eventrate"] = 60;
								break;
							case "muchmore":
								modifiers["eventrate"] = 30;
								break;
						}
						break;
					case "portals":
						switch (pair.Value)
						{
							case "casual":
								flags.Add("teleportall");
								break;
							case "hard":
								flags.Add("nobossportals");
								break;
							case "veryhard":
								flags.Add("noportals");
								break;
						}
						break;
				}
			}

			CheckFlag("nobuildcost", NoBuildCost, flags);
			CheckFlag("playerevents", PlayerEvents, flags);
			CheckFlag("fire", Fire, flags);
			CheckFlag("passivemobs", PassiveMobs, flags);
			CheckFlag("nomap", NoMap, flags);

			if (modifiers.Any() || flags.Any())
			{
				List<string> results = new();

				foreach (var pair in modifiers)
				{
					results.Add($"{pair.Key} {pair.Value}");
				}
				foreach (string flag in flags)
				{
					results.Add(flag);
				}

				results.Add("preset " + string.Join(':', presets.Select(p => $"{p.Key}_{p.Value}")));

				return results;
			}

			return [];
		}

		private static TEnum ParseEnum<TEnum>(string name, string value) where TEnum : struct
		{
			TEnum modifier;
			if (Enum.TryParse(value, true, out modifier) && !(modifier.Equals(default(TEnum))))
			{
				return modifier;
			}

			Console.Error.WriteLine($"Invalid modifier value '{value}' for property '{name}'.");
			return default;
		}

		private static bool? ParseBool(string name, string value)
		{
			bool modifier;
			if (bool.TryParse(value, out modifier))
			{
				return modifier;
			}

			Console.Error.WriteLine($"Invalid modifier value '{value}' for property '{name}'.");
			return null;
		}

		private static void CheckFlag(string name, bool? value, ISet<string> flags)
		{
			if (value.HasValue)
			{
				if (value.Value)
				{
					flags.Add(name);
				}
				else
				{
					flags.Remove(name);
				}
			}
		}
	}

	internal enum Modifier_Preset
	{
		Unset,
		Easy,
		Normal,
		Hard,
		Hardcore,
		Casual,
		Hammer,
		Immersive
	}

	internal enum Modifier_Combat
	{
		Unset,
		VeryEasy,
		Easy,
		Normal,
		Hard,
		VeryHard
	}

	internal enum Modifier_DeathPenalty
	{
		Unset,
		Casual,
		VeryEasy,
		Easy,
		Normal,
		Hard,
		Hardcore
	}

	internal enum Modifier_Resources
	{
		Unset,
		MuchLess,
		Less,
		Normal,
		More,
		MuchMore
	}

	internal enum Modifier_Raids
	{
		Unset,
		None,
		MuchLess,
		Less,
		Normal,
		More,
		MuchMore
	}

	internal enum Modifier_Portals
	{
		Unset,
		Casual,
		Normal,
		Hard,
		VeryHard
	}
}
