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

namespace MakeFwl
{
	/// <summary>
	/// Utility for creating a new Valheim world save
	/// </summary>
	internal static class WorldSave
	{
		public static bool Generate(Options options)
		{
			Modifiers? modifiers = null;
			if (!Modifiers.TryLoad(options.ModifiersPath, out modifiers))
			{
				return false;
			}

			string? outputDirectory = options.OutputPath ?? Directory.GetCurrentDirectory();

			string outputPath;
			try
			{
				outputDirectory = Path.GetFullPath(outputDirectory);
				outputPath = Path.Combine(outputDirectory, options.Name);
				Directory.CreateDirectory(outputPath);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Could not create the output directory. Ensure the output path is valid, you have write access to it, and your world name does not contain any characters which cannot be used in a file system path. [{ex.GetType().FullName}] {ex.Message}");
				return false;
			}

			string fwlPath = Path.Combine(outputPath, "_main.0.fwl2");

			CreateMetadataResult? result;
			if (!MetadataFile.CreateFile(fwlPath, options.Name, options.Seed, modifiers, out result))
			{
				return false;
			}

			Console.Out.WriteLine($"Created world \"{result.Name}\" with seed \"{result.Seed}\" at {outputPath}");

			return true;
		}
	}
}
