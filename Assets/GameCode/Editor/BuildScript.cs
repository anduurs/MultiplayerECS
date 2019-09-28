using UnityEditor;

public class BuildScript
{
	[MenuItem("Build/BuildAll")]
	public static void BuildAll()
	{
		BuildHeadlessWin64Server();
		BuildHeadlessLinux64Server();
		BuildClient();
	}

	[MenuItem("Build/BuildLocal")]
	public static void BuildLocal()
	{
		var buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Local.unity" },
			locationPathName = "Builds/Local/Client.exe",
			target = BuildTarget.StandaloneWindows64
		};

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	[MenuItem("Build/BuildHeadlessWin64Server")]
	public static void BuildHeadlessWin64Server()
	{
		var buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Server.unity" },
			locationPathName = "Builds/Server/Win64Server.exe",
			target = BuildTarget.StandaloneWindows64,
			options = BuildOptions.EnableHeadlessMode
		};

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	[MenuItem("Build/BuildHeadlessLinux64Server")]
	public static void BuildHeadlessLinux64Server()
	{
		var buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Server.unity" },
			locationPathName = "Builds/Server/Linux64Server.exe",
			target = BuildTarget.StandaloneLinux64,
			options = BuildOptions.EnableHeadlessMode
		};

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	[MenuItem("Build/BuildClient")]
	public static void BuildClient()
	{
		var buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Client.unity" },
			locationPathName = "Builds/Client/Client.exe",
			target = BuildTarget.StandaloneWindows64
		};

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}


