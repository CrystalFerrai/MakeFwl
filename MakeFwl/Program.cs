// Copyright 2021 Crystal Ferrai
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

using System;
using System.IO;
using System.Net.NetworkInformation;

namespace MakeFwl
{
    internal class Program
    {
        private const int WorldVersion = 26;
        private const int GenVersion = 1;

        private static Random sRandom;

        // Entry point
        private static int Main(string[] args)
        {
            sRandom = new Random();

            WorldInfo info;
            ParseResult argResult = TryParseArgs(args, out info);
            if (argResult == ParseResult.None) return 0;
            if (argResult == ParseResult.Error) return 1;

            int seed = info.Seed.GetStableHashCode();
            long uid = info.Name.GetStableHashCode() + GenerateUID();

            try
            {
                using (FileStream stream = File.Create(info.Path))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(0); // placeholder for data size

                    // Data
                    writer.Write(WorldVersion);
                    writer.Write(info.Name);
                    writer.Write(info.Seed);
                    writer.Write(seed);
                    writer.Write(uid);
                    writer.Write(GenVersion);

                    // Size
                    stream.Seek(0, SeekOrigin.Begin);
                    writer.Write((int)stream.Length - 4);
                }

                Console.Out.WriteLine($"Created world \"{info.Name}\" with seed \"{info.Seed}\" at {info.Path}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error [{ex.GetType().FullName}] {ex.Message}");
                return 2;
            }

            return 0;
        }

        private static ParseResult TryParseArgs(string[] args, out WorldInfo info)
        {
            info = new WorldInfo();

            if (args.Length < 1)
            {
                Console.Out.WriteLine("Creates a Valheim world seed file. Usage:\n  MakeFwl [world_name] [[seed]] [[-o path]]\n\n    world_name  The name of the world to generate. 5-20 characters.\n\n    seed        (optional) The random seed from which to generate the world.\n                1-10 characters. If ommitted, will use random value.\n\n    -o path     (optional) Output file path. If omitted, will use name of world\n                as file name and place in current directory.");
                return ParseResult.None;
            }

            info.Name = args[0];
            if (info.Name.Length < 5 || info.Name.Length > 20)
            {
                Console.Error.WriteLine("world_name must be between 5 and 20 characters long.");
                return ParseResult.Error;
            }

            if (args.Length > 1)
            {
                if (args[1].Equals("-o", StringComparison.InvariantCultureIgnoreCase) && args.Length > 2)
                {
                    info.Path = args[2];
                }
                else if (args[1].Length > 10)
                {
                    Console.Error.WriteLine("seed must be between 1 and 10 characters long.");
                    return ParseResult.Error;
                }
                else
                {
                    info.Seed = args[1];
                }
            }

            if (args.Length > 3)
            {
                if (args[2].Equals("-o", StringComparison.InvariantCultureIgnoreCase))
                {
                    info.Path = args[3];
                }
            }

            if (info.Seed == null)
            {
                info.Seed = GenerateSeedName();
            }

            if (info.Path == null)
            {
                info.Path = Path.Combine(Directory.GetCurrentDirectory(), $"{info.Name}.fwl");
            }
            else if (Path.GetExtension(info.Path).ToLowerInvariant() != ".fwl")
            {
                info.Path += ".fwl";
            }

            return ParseResult.Success;
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

        private class WorldInfo
        {
            public string Name { get; set; }
            public string Seed { get; set; }
            public string Path { get; set; }
        }

        private enum ParseResult
        {
            None,
            Success,
            Error
        }
    }

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
