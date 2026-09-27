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
    internal class Program
    {
        // Entry point
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Options.PrintUsage();
                return OnExit(0);
            }

            Options? options;
            if (!Options.TryParse(args, out options))
            {
                return OnExit(1);
            }
            
            if (!WorldSave.Generate(options))
            {
                return OnExit(1);
            }

            return OnExit(0);
        }

        private static int OnExit(int code)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }
            return code;
        }
    }
}
