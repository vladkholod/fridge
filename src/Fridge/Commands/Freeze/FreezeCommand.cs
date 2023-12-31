﻿using Fridge.Commands.Freeze.LibResolvers;
using Fridge.Extensions;
using Fridge.Models;

namespace Fridge.Commands.Freeze;

public class FreezeCommand : BaseCommand
{
    private static readonly CommandConfig CommandConfig = new()
    {
        Name = "freeze",
        DescriptionFormat = "The `{0}` command is responsible for freezing of libs in package.json.",
        RequiredArguments = new[]
        {
            FreezeCommandArguments.PackageJsonFile
        },
        OptionalArguments = new[]
        {
            FreezeCommandArguments.VersionListFile,
            FreezeCommandArguments.FreezingDependenciesObject,
            FreezeCommandArguments.Force,
            FreezeCommandArguments.DryRun,
        },
    };

    private static readonly string[] FreezeDependencies =
    {
        "dependencies",
        "devDependencies",
        "peerDependencies",
    };
    
    public FreezeCommand() : base(CommandConfig)
    {
    }

    public override void Execute(Parameter[] parameters)
    {
        var packageJsonFile = GetPackageJsonFile(parameters);

        var freezeObjects = GetFreezeDependenciesObjects(parameters);

        var libToVersionMap = GetLibToVersionMap(parameters);

        var tempFilePath = FreezeToTemporaryFile(
            packageJsonFile,
            libToVersionMap,
            freezeObjects);

        ApplyChanges(parameters, packageJsonFile.FullName, tempFilePath);
    }

    private static void ApplyChanges(Parameter[] parameters, string packageJsonFilePath, string tempFilePath)
    {
        var isDryRun = parameters.ContainsArgument(FreezeCommandArguments.DryRun);
        if (isDryRun)
        {
            DisplayChanges(tempFilePath);
        }
        else
        {
            ReplacePackageJson(packageJsonFilePath, tempFilePath);
        }

        File.Delete(tempFilePath);
    }

    private static void DisplayChanges(string tempFilePath)
    {
        var fileData = File.ReadAllText(tempFilePath);

        Console.WriteLine($"package.json after changes:{Environment.NewLine}{fileData}");
    }

    private static void ReplacePackageJson(string packageJsonFilePath, string tempFilePath)
    {
        File.Copy(tempFilePath, packageJsonFilePath, true);
    }

    private static FileInfo GetPackageJsonFile(Parameter[] parameters)
    {
        var parameter =
            parameters.First(parameter => parameter.Argument.Equals(FreezeCommandArguments.PackageJsonFile));

        var file = new FileInfo(parameter.Value!);
        if (!file.Exists)
        {
            throw new FileNotFoundException(file.FullName);
        }

        return file;
    }

    private static string FreezeToTemporaryFile(
        FileInfo packageJsonFile,
        IReadOnlyDictionary<string, string> libToVersionMap,
        string[] freezingObjects)
    {
        var tempFilePath = Path.GetTempFileName();

        using var resultWriter = new StreamWriter(tempFilePath);
        using var packageJsonReader = packageJsonFile.OpenText();

        while (packageJsonReader.ReadLine() is { } line)
        {
            resultWriter.WriteLine(line);

            if (JsonParsingHelper.TryGetKey(line, out var key) && freezingObjects.Contains(key))
            {
                ProcessFreezeObject(
                    packageJsonReader,
                    resultWriter,
                    libToVersionMap);
            }
        }

        return tempFilePath;
    }

    private static void ProcessFreezeObject(
        TextReader packageJsonReader,
        TextWriter resultWriter,
        IReadOnlyDictionary<string, string> libToVersionMap)
    {
        while (packageJsonReader.ReadLine() is { } line)
        {
            if (JsonParsingHelper.ContainsClosingParenthesis(line))
            {
                resultWriter.WriteLine(line);
                return;
            }

            if (!JsonParsingHelper.TryGetKey(line, out var libName))
            {
                resultWriter.WriteLine(line);
                continue;
            }

            if (!libToVersionMap.TryGetValue(libName, out var version))
            {
                resultWriter.WriteLine(line);
                continue;
            }

            var frozenLine = GetFrozenLine(line, version);
            resultWriter.WriteLine(frozenLine);
        }
    }

    private static Dictionary<string, string> GetLibToVersionMap(Parameter[] parameters)
    {
        var versionListFileParameter =
            parameters.FirstOrDefault(parameter => parameter.Argument.Equals(FreezeCommandArguments.VersionListFile));

        ILibResolver libResolver;
        if (versionListFileParameter is not null)
        {
            libResolver = new VersionsFileLibResolver(new FileInfo(versionListFileParameter.Value!));
        }
        else
        {
            var packageJsonFileParameter =
                parameters.First(parameter => parameter.Argument.Equals(FreezeCommandArguments.PackageJsonFile));

            var isForce = parameters.ContainsArgument(FreezeCommandArguments.Force);
            
            libResolver = new InstalledLibResolver(
                new DirectoryInfo(Path.GetDirectoryName(packageJsonFileParameter.Value) ?? string.Empty),
                isForce);
        }

        return libResolver.Resolve();
    }

    private static string[] GetFreezeDependenciesObjects(Parameter[] parameters)
    {
        var objects = parameters
            .Where(parameter => parameter.Argument.Equals(FreezeCommandArguments.FreezingDependenciesObject))
            .Select(parameter => parameter.Value!)
            .ToArray();

        if (objects.Length == 0)
        {
            objects = FreezeDependencies;
        }

        return objects;
    }

    private static string GetFrozenLine(string line, string frozenVersion)
    {
        var currentVersionRange = JsonParsingHelper.GetVersionRange(line);

        var frozenLine = line
            .Remove(currentVersionRange.Start.Value, currentVersionRange.Count())
            .Insert(currentVersionRange.Start.Value, frozenVersion);

        return frozenLine;
    }
}