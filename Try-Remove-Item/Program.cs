using JCNET.字符串处理;
using System.CommandLine;
using System.CommandLine.Invocation;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];
	root_cmd.Description = "尝试删除指定路径的目录或文件。如果不存在，则忽略该路径。";

	Option<string[]> path_option = new(["--path", "--Path"],
		"要删除的目录或文件的路径。")
	{
		IsRequired = true,
		AllowMultipleArgumentsPerToken = true,
	};

	root_cmd.AddOption(path_option);

	root_cmd.SetHandler((InvocationContext context) =>
	{
		arguments.Paths = context.ParseResult.GetValueForOption(path_option)!;
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

List<StringPath> full_path_list = [];
foreach (string path in arguments.Paths)
{
	if (path == string.Empty)
	{
		full_path_list.Add(new StringPath("./").FullPath);
	}
	else
	{
		full_path_list.Add(new StringPath(path).FullPath);
	}
}

foreach (StringPath path in full_path_list)
{
	Console.WriteLine(path);
	foreach (StringPath item_path in path.SearchChildItems(path, "*"))
	{
		Console.WriteLine(item_path);
	}
}

return 0;

internal class Arguments
{
	public string[] Paths { get; set; } = [];
}
