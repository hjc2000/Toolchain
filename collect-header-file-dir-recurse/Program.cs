using JCNET;
using System.CommandLine;
using System.CommandLine.Invocation;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];
	root_cmd.Description = "递归收集 指定路径下的头文件，将它们的所在目录收集起来，放到 cmake 列表字符串中，然后输出。";

	Option<string[]> paths_option = new(["--paths", "--Paths"],
		"要递归收集头文件目录的路径。可以指定多个。")
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

foreach (string path in arguments.Paths)
{
	FileSystemPath foo = new(path);
	if (!foo.IsExistingDirectory)
	{
		throw new Exception("输入的路径只能是目录。");
	}
}

HashSet<FileSystemPath> header_files = [];
foreach (string path in arguments.Paths)
{
	header_files = [.. header_files, .. new FileSystemPath($"{path}/**.h").ExpandWildcard()];
	header_files = [.. header_files, .. new FileSystemPath($"{path}/**.hpp").ExpandWildcard()];
}

HashSet<string> header_dir_set = [];
foreach (FileSystemPath header_file in header_files)
{
	header_dir_set.Add(header_file.DirectoryName.ToString());
}

string cmake_list_string = string.Join(';', header_dir_set);
Console.Write(cmake_list_string);

return 0;

internal class Arguments
{
	public string[] Paths { get; set; } = [];
}
