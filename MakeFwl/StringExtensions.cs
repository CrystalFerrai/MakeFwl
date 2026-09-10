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
	internal static class StringExtensions
	{
		// From StringExtensionMethods in assembly_utils
		public static int GetStableHashCode(this string str)
		{
			int num = 5381;
			int num1 = num;
			for (int i = 0; i < str.Length && str[i] != 0; i += 2)
			{
				num = (num << 5) + num ^ str[i];
				if (i == str.Length - 1 || str[i + 1] == 0)
				{
					break;
				}
				num1 = (num1 << 5) + num1 ^ str[i + 1];
			}
			return num + num1 * 1566083941;
		}
	}
}
