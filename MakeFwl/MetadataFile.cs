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
using System.Net.NetworkInformation;
using System.Text;

namespace MakeFwl
{
	/// <summary>
	/// Utility for creating a metadata (fwl) file
	/// </summary>
	internal static class MetadataFile
	{
		// Current version numbers can be found in Version.Version in assembly_valheim
		private const int WorldVersion = 39; // Indicates version of world format
		private const int GenVersion = 2; // Indicates version of world generator

		private static Random sRandom;

		static MetadataFile()
		{
			sRandom = new();
		}

		/// <summary>
		/// Attempts to create a metadata file using the passed in options and modifiers
		/// </summary>
		public static bool CreateFile(Options options, Modifiers modifiers, [NotNullWhen(true)] out CreateMetadataResult? result)
		{
			string? outputPath = options.OutputPath;
			if (outputPath is null)
			{
				outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{options.Name}.fwl2");
			}
			outputPath = Path.GetFullPath(outputPath);
			string? outputDir = Path.GetDirectoryName(outputPath);
			if (outputDir is null)
			{
				Console.Error.WriteLine($"Could not determine the directory for the output path: {outputPath}");
				result = null;
				return false;
			}
			try
			{
				Directory.CreateDirectory(outputDir);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Could not create the output directory: {outputDir}. [{ex.GetType().FullName}] {ex.Message}");
				result = null;
				return false;
			}

			string? seedName = options.Seed;
			if (seedName is null)
			{
				seedName = GenerateSeedName();
			}

			int seed = seedName.GetStableHashCode();
			long uid = options.Name.GetStableHashCode() + GenerateUID();

			try
			{
				using (FileStream stream = File.Create(outputPath))
				using (BinaryWriter writer = new(stream, Encoding.ASCII, true))
				{
					// Format based on World.SaveWorldMetaData from assembly_valheim

					writer.Write(0); // placeholder for data size

					// Data
					writer.Write(WorldVersion);
					writer.Write(options.Name);
					writer.Write(seedName);
					writer.Write(seed);
					writer.Write(uid);
					writer.Write(GenVersion);
					writer.Write(false); // False means the world DB file does not need to exist to load this world
					modifiers.Serialize(stream);
					writer.Write(0); // Player history count

					// Size
					stream.Seek(0, SeekOrigin.Begin);
					writer.Write((int)stream.Length - 4);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"An error occurred while writing the file. [{ex.GetType().FullName}] {ex.Message}");
				result = null;
				return false;
			}

			result = new(options.Name, seedName, outputPath);
			return true;
		}

		// From World.GenerateSeed in assembly_valheim
		private static string GenerateSeedName()
		{
			string str = "";
			for (int i = 0; i < 10; i++)
			{
				char chr = "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ023456789"[sRandom.Next(0, "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ023456789".Length)];
				str = string.Concat(str, chr.ToString());
			}
			return str;
		}

		// From Utils.GenerateUID in assembly_utils
		private static long GenerateUID()
		{
			string str;
			IPGlobalProperties pGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			str = (pGlobalProperties == null || pGlobalProperties.HostName == null ? "unkown" : pGlobalProperties.HostName);
			string str1 = (pGlobalProperties == null || pGlobalProperties.DomainName == null ? "domain" : pGlobalProperties.DomainName);
			return (long)string.Concat(str, ":", str1).GetHashCode() + (long)sRandom.Next(1, 2147483647);
		}
	}

	/// <summary>
	/// A result from MEtadataFile.CreateFile
	/// </summary>
	internal class CreateMetadataResult
	{
		/// <summary>
		/// The name of the created world
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The seed for the created world
		/// </summary>
		public string Seed { get; }

		/// <summary>
		/// The path to the output metadata file
		/// </summary>
		public string Path { get; }

		public CreateMetadataResult(string name, string seed, string path)
		{
			Name = name;
			Seed = seed;
			Path = path;
		}
	}
}
