using JCNET;
using System.CommandLine;
using System.CommandLine.Invocation;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];
	root_cmd.Description = "尝试删除指定路径的目录或文件。如果不存在，则忽略该路径。";

	Option<string[]> paths_option = new(["--paths", "--Paths"],
		"要删除的目录或文件的路径。可以指定多个。")
	{
		IsRequired = true,
		AllowMultipleArgumentsPerToken = true,
	};

	root_cmd.AddOption(paths_option);

	root_cmd.SetHandler((InvocationContext context) =>
	{
		arguments.Paths = context.ParseResult.GetValueForOption(paths_option)!;
	});

	int cmd_parse_result = await root_cmd.InvokeAsync(args);
	if (cmd_parse_result != 0)
	{
		return cmd_parse_result;
	}

	if (args.Contains("-h") ||
		args.Contains("--help") ||
		args.Contains("?") ||
		args.Contains("--version"))
	{
		return 0;
	}
}

HashSet<FileSystemPath> expand_result = [];
foreach (string path in arguments.Paths)
{
	expand_result = [.. expand_result, .. new FileSystemPath(path).ExpandWildcard()];
}

foreach (FileSystemPath path in expand_result)
{
	path.SetFileAttributesNormalRecursively();

	if (path.IsExistingFile)
	{
		File.Delete(path.ToString());
		Console.WriteLine($"删除文件：{path}");
	}
	else if (path.IsExistingDirectory)
	{
		Directory.Delete(path.ToString(), true);
		Console.WriteLine($"删除目录：{path}");
	}
	else
	{
		Console.WriteLine($"{path} 不是一个存在的项目，无法删除");
	}
}

return 0;

internal class Arguments
{
	public string[] Paths { get; set; } = [];
}
