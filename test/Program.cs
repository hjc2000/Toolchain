using JCNET.命令行;

List<string> results = await PowershellExtension.LddAsync("F:/cpp-lib-build-scripts/msys/.total-install/bin/sync-time.exe");
results = await PowershellExtension.Cygpath_GetWindowsPath_Async(results);
foreach (string result in results)
{
	Console.WriteLine(result);
}
