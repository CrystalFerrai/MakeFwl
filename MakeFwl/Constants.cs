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
	/// Version numbers associated with game systems
	/// </summary>
	internal static class Versions
	{
		// Current version numbers can be found in Version in assembly_valheim

		/// <summary>
		/// World format version
		/// </summary>
		/// <remarks>
		/// Referance: int Version.c_WorldGenVersion
		/// </remarks>
		public const int WorldVersion = 41;

		/// <summary>
		/// World generator version
		/// </summary>
		/// <remarks>
		/// Reference: enum Version.World
		/// </remarks>
		public const int GenVersion = 2;
	}
}
