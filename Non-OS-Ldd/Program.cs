using System.CommandLine;
using JCNET.命令行;

if (true)
{
	RootCommand root_cmd = [];

	Option<bool> debug_option = new("--debug", "使用此参数表示开启调试模式。");
	root_cmd.AddOptionAndHandler(debug_option, (bool debug) =>
	{
		if (debug)
		{
			Console.WriteLine("调试模式");
		}
		else
		{
			Console.WriteLine("发布模式。");
		}
	});

	int cmd_parse_result = await root_cmd.InvokeAsync(args);
	if (cmd_parse_result != 0)
	{
		return cmd_parse_result;
	}
}

return 0;
