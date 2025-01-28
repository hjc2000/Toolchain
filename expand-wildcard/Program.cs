using JCNET;
using System.CommandLine;
using System.CommandLine.Invocation;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];
	root_cmd.Description = "展开路径中的通配符。";

	Option<string[]> paths_option = new(["--paths", "--Paths"],
		"要被展开通配符的路径。可以指定多个。")
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
	Console.WriteLine($"{path}");
}

return 0;

internal class Arguments
{
	public string[] Paths { get; set; } = [];
}
