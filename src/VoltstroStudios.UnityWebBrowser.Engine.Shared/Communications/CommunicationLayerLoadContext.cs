// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
//
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

//using System.Runtime.Loader;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;

internal class CommunicationLayerLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver resolver;

    public CommunicationLayerLoadContext(string dllPath)
    {
        resolver = new AssemblyDependencyResolver(dllPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
}

public abstract class AssemblyLoadContext
{
    protected AssemblyLoadContext()
    {
        Default = this;
    }

    public static AssemblyLoadContext Default { get; private set; }

    //public static AssemblyName GetAssemblyName(string assemblyPath);
    //public static AssemblyLoadContext GetLoadContext(Assembly assembly);
    //public static void InitializeDefaultContext(AssemblyLoadContext context);
    protected abstract Assembly Load(AssemblyName assemblyName);

    public Assembly LoadFromAssemblyName(AssemblyName assemblyName) => Assembly.LoadFrom(assemblyName.Name);

    public static Assembly LoadFromAssemblyPath(string assemblyFullPath)
    {
        var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(assemblyFullPath);
        var fileName = Path.GetFileName(assemblyFullPath);
        var directory = Path.GetDirectoryName(assemblyFullPath);

        //var inCompileLibraries = DependencyContext.Default.CompileLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));
        //var inRuntimeLibraries = DependencyContext.Default.RuntimeLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));

        var assembly =
            //(inCompileLibraries || inRuntimeLibraries)
            Assembly.Load(new AssemblyName(fileNameWithOutExtension));
        //: AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFullPath);

        if (assembly != null)
            LoadReferencedAssemblies(assembly, fileName, directory);

        return assembly;
    }

    private static void LoadReferencedAssemblies(Assembly assembly, string fileName, string directory)
    {
        var filesInDirectory = Directory.GetFiles(directory).Where(x => x != fileName).Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
        var references = assembly.GetReferencedAssemblies();

        foreach (var reference in references)
        {
            if (filesInDirectory.Contains(reference.Name))
            {
                var loadFileName = reference.Name + ".dll";
                var path = Path.Combine(directory, loadFileName);
                var loadedAssembly = LoadFromAssemblyPath(path);
                if (loadedAssembly != null)
                {
                    LoadReferencedAssemblies(loadedAssembly, loadFileName, directory);
                }
            }
        }
    }

    //protected Assembly LoadFromNativeImagePath(string nativeImagePath, string assemblyPath);
    //protected Assembly LoadFromStream(Stream assembly);
    //protected Assembly LoadFromStream(Stream assembly, Stream assemblySymbols);
    //protected virtual IntPtr LoadUnmanagedDll(string unmanagedDllName);
    //public void SetProfileOptimizationRoot(string directoryPath);
    //public void StartProfileOptimization(string profile);
}

public interface IDependencyResolver
{
    public string ResolveAssemblyToPath(AssemblyName assemblyName);

    public string ResolveUnmanagedDllToPath(string unmanagedDllName);
}

public class AssemblyDependencyResolver : IDependencyResolver
{
    private string dllPath;

    public AssemblyDependencyResolver(string dllPath)
    {
        this.dllPath = dllPath;
    }

    public string ResolveAssemblyToPath(AssemblyName assemblyName)
    {
        throw new NotImplementedException();
    }

    public string ResolveUnmanagedDllToPath(string unmanagedDllName)
    {
        throw new NotImplementedException();
    }
}