using JCNET.命令行;
using System.CommandLine;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];

	Option<string?> debug_option = new("--exe_path", "可执行文件路径。");
	root_cmd.AddOptionAndHandler(debug_option, (string? exe_path) =>
	{
		ArgumentNullException.ThrowIfNull(exe_path);
		arguments.ExePath = exe_path;
	});

	int cmd_parse_result = await root_cmd.InvokeAsync(args);
	if (cmd_parse_result != 0)
	{
		return cmd_parse_result;
	}
}

Console.WriteLine(arguments.ExeFullPath);
IEnumerable<string> ldd_results = await PowershellExtension.GetNonOSDependentDllFullPathAsync(arguments.ExeFullPath);
foreach (string str in ldd_results)
{
	Console.WriteLine(str);
}

return 0;

internal class Arguments
{
	public string ExePath { get; set; } = string.Empty;

	public string ExeFullPath
	{
		get
		{
			return Path.GetFullPath(ExePath);
		}
	}
}
