using System.Diagnostics;
using Fridge.Exceptions;

namespace Fridge.Commands.Freeze.LibResolvers;

public class InstalledLibResolver : ILibResolver
{
    private readonly DirectoryInfo _packageDirectory;
    private readonly bool _suppressErrors;

    public InstalledLibResolver(DirectoryInfo packageDirectory, bool suppressErrors)
    {
        if (!packageDirectory.Exists)
        {
            throw new DirectoryNotFoundException(packageDirectory.FullName);
        }

        _packageDirectory = packageDirectory;
        _suppressErrors = suppressErrors;
    }

    public Dictionary<string, string> Resolve()
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _packageDirectory.FullName,
        };

        try
        {
            return ResolveLibs(process);
        }
        catch (Exception exception)
        {
            throw new ResolveLibsException(exception);
        }
    }

    private Dictionary<string, string> ResolveLibs(Process process)
    {
        process.Start();

        RunNpmList(process);

        VerifyErrors(process, _suppressErrors);

        var libs = ParseOutput(process);

        process.WaitForExit();

        return libs;
    }

    private static void RunNpmList(Process process)
    {
        using var inputWriter = process.StandardInput;

        inputWriter.WriteLine("npm list");
    }

    private static void VerifyErrors(Process process, bool suppressErrors)
    {
        using var errorReader = process.StandardError;

        var containsError = errorReader.Read() != -1;
        if (containsError && !suppressErrors)
        {
            throw new Exception("`npm list` executed with error.");
        }
    }

    private static Dictionary<string, string> ParseOutput(Process process)
    {
        using var stream = process.StandardOutput;

        return NpmListHelper.ParseStream(stream);
    }
}