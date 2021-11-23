using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dinah.Core.Processes;

namespace Dinah.Core
{
	public class GitClient
	{
		private DirectoryInfo directoryInfo { get; }

		public GitClient(string directory) : this(new DirectoryInfo(directory)) { }
		public GitClient(DirectoryInfo directoryInfo) => this.directoryInfo = directoryInfo;

		#region get git sync status recursive
		public List<string> GetSyncStatusesRecrusive() => GetSyncStatusesRecrusive(directoryInfo);
		public static List<string> GetSyncStatusesRecrusive(DirectoryInfo directoryInfo)
			=> gitStatus_resurs(directoryInfo, new());

		private static List<string> gitStatus_resurs(DirectoryInfo directoryInfo, List<string> outOfSyncLog)
		{
			var (status, qty) = getGitSyncStatus(directoryInfo.FullName);
			switch (status)
			{
				case GitSyncStatus.OutOfSync:
					outOfSyncLog.Add($"{qty} files out of sync. dir: {directoryInfo.FullName}");
					return outOfSyncLog;
				case GitSyncStatus.NonGit:
					foreach (var subDir in directoryInfo.EnumerateDirectories())
					{
						var name = subDir.Name;

						// ignore symlinks and junction dir.s
						if ((subDir.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint
							// ignore these dir.s
							|| name == ".vs"
							|| name == ".git"
							|| name == "bin"
							|| name == "obj")
							continue;

						outOfSyncLog = gitStatus_resurs(subDir, outOfSyncLog);
					}
					return outOfSyncLog;
				case GitSyncStatus.InSync:
				case GitSyncStatus.NonRoot:
				default:
					return outOfSyncLog;
			}
		}

		private enum GitSyncStatus
		{
			NonGit,
			NonRoot,
			InSync,
			OutOfSync
		}
		private static (GitSyncStatus status, int qty) getGitSyncStatus(string dir)
		{
			var location = getGitLocation(dir);

			if (location == GitLocation.NonGit)
				return (GitSyncStatus.NonGit, 0);

			if (location == GitLocation.NonRoot ||
				location == GitLocation.InsideGitDir)
				return (GitSyncStatus.NonRoot, 0);

			var status = GitSyncStatus.InSync;
			var qty = 0;

			{ // Your branch is ahead of 'origin/master'
				var ahead = RunGitCommand(dir, "git rev-list HEAD@{upstream}..HEAD");
				if (!string.IsNullOrWhiteSpace(ahead.Output))
				{
					status = GitSyncStatus.OutOfSync;
					qty += ahead.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
				}
			}

			{ // Untracked files
				var porcelain = RunGitCommand(dir, "git status --porcelain");
				if (!string.IsNullOrWhiteSpace(porcelain.Output))
				{
					status = GitSyncStatus.OutOfSync;
					qty += porcelain.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
				}
			}

			return (status, qty);
		}

		private enum GitLocation
		{
			NonGit,
			Root,
			NonRoot,
			InsideGitDir
		}
		private static GitLocation getGitLocation(string dir)
		{
			var isInsideGitDir = gitBoolCommand(dir, "git rev-parse --is-inside-git-dir");

			if (!isInsideGitDir.HasValue)
				return GitLocation.NonGit;

			// bare repo root
			var isBareRepository = gitBoolCommand(dir, "git rev-parse --is-bare-repository");
			if (isInsideGitDir.Value && isBareRepository.Value)
				return GitLocation.Root;

			// inside .git
			if (isInsideGitDir.Value)
				return GitLocation.InsideGitDir;

			var toplevel = RunGitCommand(dir, "git rev-parse --show-toplevel").Output;
			var di1 = new DirectoryInfo(dir).FullName;
			var di2 = new DirectoryInfo(toplevel).FullName;
			if (di1 == di2)
				return GitLocation.Root;

			return GitLocation.NonRoot;
		}
		#endregion

		public string GetLatestTag() => GetLatestTag(directoryInfo);
		public static string GetLatestTag(DirectoryInfo directoryInfo)
			=> RunGitCommand(directoryInfo, "git describe --tags --abbrev=0")
			.Output;

		/// <summary>all commits since tag</summary>
		public string GetCommitsSinceTag(string tag) => GetCommitsSinceTag(directoryInfo, tag);
		/// <summary>all commits since tag</summary>
		public static string GetCommitsSinceTag(DirectoryInfo directoryInfo, string tag)
			//   git log <yourlasttag>..HEAD
			// %s == see subject/comments. https://git-scm.com/book/en/v2/Git-Basics-Viewing-the-Commit-History
			=> RunGitCommand(directoryInfo, $"git log --pretty=%s {tag}..HEAD")
			.Output;

		private static bool? gitBoolCommand(string dir, string cmd)
		{
			var result = RunGitCommand(dir, cmd);

			if (result.ExitCode == 128 && (result.Error.StartsWith("fatal: not a git repository") || result.Error.StartsWith("fatal: failed to stat")))
				return null;

			var output = result.Output;

			if (output.EqualsInsensitive($"{true}"))
				return true;

			if (output.EqualsInsensitive($"{false}"))
				return false;

			throw new Exception();
		}



		public ProcessResult RunGitCommand(string cmd) => RunGitCommand(directoryInfo, cmd);
		public static ProcessResult RunGitCommand(DirectoryInfo directoryInfo, string cmd) => RunGitCommand(directoryInfo.FullName, cmd);
		public static ProcessResult RunGitCommand(string directory, string cmd)
		{
			cmd = cmd.Trim();
			if (!cmd.StartsWithInsensitive("git "))
				cmd = "git " + cmd;
			return runCommand(directory, cmd);
		}

		private static ProcessResult runCommand(string directory, string cmd)
		{
			var processStartInfo = new ProcessStartInfo
			{
				WorkingDirectory = directory,
				FileName = "cmd.exe",
				//  "/c" Carries out the command specified by the string and then terminates
				Arguments = "/c " + cmd
			};
			return ProcessRunner.RunHidden(processStartInfo);
		}
	}
}
