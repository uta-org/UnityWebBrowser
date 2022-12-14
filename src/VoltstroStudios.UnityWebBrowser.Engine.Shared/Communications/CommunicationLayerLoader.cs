// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.IO;
using System.Reflection;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;

internal static class CommunicationLayerLoader
{
    public static ICommunicationLayer GetCommunicationLayerFromAssembly(string assemblyPath)
    {
        Assembly loadedAssembly = LoadAssembly(assemblyPath);
        foreach (Type type in loadedAssembly.GetTypes())
            if (typeof(ICommunicationLayer).IsAssignableFrom(type))
            {
                ICommunicationLayer communicationLayer = Activator.CreateInstance(type) as ICommunicationLayer;
                return communicationLayer;
            }

        //If we get here then no communication layer fround
        throw new DllNotFoundException("Failed to find communication layer in provided assembly!");
    }

    private static Assembly LoadAssembly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException("Communication Layer Assembly not found!");

        CommunicationLayerLoadContext loadContext = new(assemblyPath);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));
    }
}