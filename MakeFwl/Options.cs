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

namespace MakeFwl
{
	/// <summary>
	/// Program options gathered from the command line
	/// </summary>
	internal class Options
	{
		/// <summary>
		/// The name of the world
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The world generation seed
		/// </summary>
		public string? Seed { get; set; }

		/// <summary>
		/// The path to where the file should be generated
		/// </summary>
		public string? OutputPath { get; set; }

		/// <summary>
		/// The path to a modifiers.txt file to load
		/// </summary>
		public string? ModifiersPath { get; set; }

		private Options()
		{
			Name = null!;
		}

		/// <summary>
		/// Parse command line parameters and output an Options object if successful.
		/// </summary>
		public static bool TryParse(string[] args, [NotNullWhen(true)] out Options? options)
		{
			options = null;

			if (args.Length == 0)
			{
				PrintUsage();
				return false;
			}

			Options instance = new();

			int posIndex = 0;

			for (int i = 0; i < args.Length; ++i)
			{
				if (args[i].StartsWith('-'))
				{
					if (args[i].Length != 2)
					{
						Console.Error.WriteLine($"Unrecognized switch {args[i]}");
						return false;
					}

					// Switch argument
					switch (args[i][1])
					{
						case 'm':
							if (i + 1 >= args.Length)
							{
								Console.Error.WriteLine($"Missing parameter for switch {args[i]}");
								return false;
							}
							++i;
							instance.ModifiersPath = args[i];
							break;
						case 'o':
							if (i + 1 >= args.Length)
							{
								Console.Error.WriteLine($"Missing parameter for switch {args[i]}");
								return false;
							}
							++i;
							instance.OutputPath = args[i];
							break;
						default:
							Console.Error.WriteLine($"Unrecognized flag {args[i]}");
							return false;
					}
				}
				else
				{
					// Positional argument
					switch (posIndex)
					{
						case 0:
							instance.Name = args[i];
							break;
						case 1:
							instance.Seed = args[i];
							break;
						default:
							Console.Error.WriteLine("Too many arguments");
							return false;
					}

					++posIndex;
				}
			}

			if (posIndex == 0)
			{
				Console.Error.WriteLine("Missing required parameter 'world_name'");
				return false;
			}

			options = instance;
			return true;
		}

		/// <summary>
		/// Print program usage
		/// </summary>
		public static void PrintUsage()
		{
			Console.Out.WriteLine(
				"Creates a Valheim world seed file. Usage:\n" +
				"  MakeFwl [world_name] [[seed]] [[-m path]] [[-o path]]\n" +
				"\n" +
				"    world_name  The name of the world to generate. 5-20 characters.\n" +
				"\n" +
				"    seed        (optional) The random seed from which to generate the world.\n" +
				"                1-10 characters. If ommitted, will use random value.\n" +
				"\n" +
				"    -m path     (optional) Modifiers file path. If omitted, will default settings.\n" +
				"                See modifiers.example.txt for an example of a modifiers file.\n" +
				"\n" +
				"    -o path     (optional) Output file path. If omitted, will use name of world\n" +
				"                as file name and place in current directory."
				);
		}
	}
}
