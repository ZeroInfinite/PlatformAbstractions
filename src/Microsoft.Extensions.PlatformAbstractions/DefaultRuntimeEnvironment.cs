﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions.Native;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.PlatformAbstractions
{
    public class DefaultRuntimeEnvironment : IRuntimeEnvironment
    {
        public DefaultRuntimeEnvironment()
        {
            OperatingSystem = PlatformApis.GetOSName();
            OperatingSystemVersion = PlatformApis.GetOSVersion();
            OperatingSystemPlatform = PlatformApis.GetOSPlatform();

            RuntimePath = GetLocation(typeof(object).GetTypeInfo().Assembly);
            RuntimeType = GetRuntimeType();
            RuntimeVersion = typeof(object).GetTypeInfo().Assembly.GetName().Version.ToString();
            RuntimeArchitecture = GetArch();
        }

        public Platform OperatingSystemPlatform { get; }

        public string OperatingSystemVersion { get; }

        public string OperatingSystem { get; }

        public string RuntimeArchitecture { get; }

        public string RuntimePath { get; }

        public string RuntimeType { get; }

        public string RuntimeVersion { get; }

        private string GetRuntimeType()
        {
#if NET451
            return Type.GetType("Mono.Runtime") != null ? "Mono" : "CLR";
#else
            return "CoreCLR";
#endif
        }

        private string GetLocation(Assembly assembly)
        {
            var assemblyLocation = (string)typeof(Assembly).GetProperty("Location")?.GetGetMethod().Invoke(assembly, parameters: null);
            return string.IsNullOrEmpty(assemblyLocation) ? null : Path.GetDirectoryName(assemblyLocation);
        }

        private static string GetArch()
        {
#if NET451
            return Environment.Is64BitProcess ? "x64" : "x86";
#else
            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.X86:
                    return "x86";
                case Architecture.X64:
                    return "x64";
                case Architecture.Arm:
                    return "arm";
                default:
                    return null;
            }
#endif
        }

    }
}
