using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GnoPatch.Tests.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GnoPatch.Tests
{
    public abstract class BaseFixture
    {
        
        protected object Run(PatchGroup patch, string targetTypeName)
        {
            var patcher = new Patcher();

            var result = patcher.Apply(patch, new[] {Environment.CurrentDirectory});

            if (!result.Success)
            {
                Assert.Fail("Patcher reported failure: " + result.Details.First(d => !d.Success).Message);
            }

            PEVerify(result.FinalAssembly);

            var bytes = File.ReadAllBytes(result.FinalAssembly);

            var assembly = Assembly.Load(bytes);

            var invocationResult = ExecuteModifiedAssembly(assembly, targetTypeName);

            File.Delete(result.FinalAssembly);

            return invocationResult;
        }
        
        private object ExecuteModifiedAssembly(Assembly assembly, string targetType)
        {
            
            var modifiedType = assembly.GetTypes().FirstOrDefault(t => t.FullName == targetType);

            if (modifiedType == null) Assert.Fail("Couldn't find modified type " + targetType);

            var instance = Activator.CreateInstance(modifiedType);

            if (instance == null) Assert.Fail("The object couldn't be instantiated.");

            var actor = instance as IActor;

            if (actor == null) Assert.Fail("The target type no longer implements IActor.");

            return actor.Act();
        }

        protected void PEVerify(string assemblyLocation)
        {
            var pathKeys = new[]
            {
                "sdkDir",
                "altSdkDir"
            };

            var peVerifyLocation = GetPEVerifyLocation(pathKeys);

            if (!File.Exists(peVerifyLocation))
                Assert.Fail("Could not find PEVerify.exe; make sure you specify a valid location in app settings. Add a new one if it's in an unusual location for you.");

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = peVerifyLocation,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(assemblyLocation) ?? AppDomain.CurrentDomain.BaseDirectory,
                Arguments = "\"" + assemblyLocation + "\" /nologo",
                CreateNoWindow = true
            });

            var processOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var result = string.Format("PEVerify Exit Code: {0}", process.ExitCode);

            Console.WriteLine(GetType().FullName + ": " + result);

            if (process.ExitCode == 0)
                return;

            Console.WriteLine(processOutput);
            Assert.Fail("Verification failed; PEVerify output: " + Environment.NewLine + processOutput, result);
        }

        private static string GetPEVerifyLocation(IEnumerable<string> pathKeys)
        {
            foreach (var key in pathKeys)
            {
                var directory = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(directory))
                    continue;

                var target = Path.Combine(directory, "peverify.exe");

                if (File.Exists(target)) return target;
            }
            return string.Empty;
        }
    }
}