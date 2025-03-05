using JCNET.命令行;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

Arguments arguments = new();

if (true)
{
	RootCommand root_cmd = [];
	root_cmd.Description = "分析指定的可执行文件所依赖的 dll. 会自动排除 windows 系统的 dll" +
		"和已经在指定的可执行文件所在的目录的 dll. 分析完后可以选择将上述的 dll 拷贝到可执行文件所在的目录。";

	Option<string> exe_path_option = new("--exe_path", "可执行文件路径。")
	{
		IsRequired = true,
	};

	Option<bool> copy_dll_option = new("--copy_dll",
		"分析完依赖后，是否将依赖的 dll 拷贝到可执行文件所在的目录。" +
		"（不包括系统 dll 和已经在可执行文件目录中的 dll.）");

	root_cmd.AddOption(exe_path_option);
	root_cmd.AddOption(copy_dll_option);

	root_cmd.SetHandler((InvocationContext context) =>
	{
		arguments.ExePath = context.ParseResult.GetValueForOption(exe_path_option)!;
		arguments.CopyDll = context.ParseResult.GetValueForOption(copy_dll_option);
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

string exe_dir = Path.GetDirectoryName(arguments.ExeFullPath)!.Replace('\\', '/');
Console.WriteLine(exe_dir);

IEnumerable<string> ldd_results = await PowershellExtension.GetNonOSDependentDllFullPathAsync(arguments.ExeFullPath);

foreach (string dependent_dll_full_path in ldd_results)
{
	if (dependent_dll_full_path.StartsWith(exe_dir))
	{
		continue;
	}

	if (arguments.CopyDll)
	{
		string dll_name = Path.GetFileName(dependent_dll_full_path);
		Console.WriteLine($"{dependent_dll_full_path} => {exe_dir}/{dll_name}");
		await using FileStream src_fs = File.OpenRead(dependent_dll_full_path);
		await using FileStream dst_fs = File.OpenWrite($"{exe_dir}/{dll_name}");
		await src_fs.CopyToAsync(dst_fs);
	}
	else
	{
		Console.WriteLine($"{dependent_dll_full_path}");
	}
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

	public bool CopyDll { get; set; } = false;
}
