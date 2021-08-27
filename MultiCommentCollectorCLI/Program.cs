using MCC.Core;
using MCC.Core.Manager;
using MCC.Utility.IO;
using MCC.Utility.Text;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace MultiCommentCollectorCLI
{
    class Program
    {
        static bool IsComment = false;

        [MTAThread]
        static void Main(string[] args)
        {
            // 設定読み込み
            ApplicationStart();

            // コマンド解析
            ExecuteCommand(AnalyzeCommand(args));

            bool exit = false;

            while (!exit)
            {
                var read = Console.ReadLine();

                var command = read.Split(" ");

                ExecuteCommand(AnalyzeCommand(command));
            }
        }

        static Dictionary<string, string> AnalyzeCommand(string[] args)
        {
            var options = new HashSet<string> { "-add", "-activate", "-inactivate", "-list", "-show", "-hide", "exit", "-remove" };

            string key = null;
            return args
                .GroupBy(s => options.Contains(s) ? key = s : key) // 副作用！
                .ToDictionary(g => g.Key ?? "!NULL!", g => g.Skip(1).FirstOrDefault()); // 1番目はキーなのでSkip
        }

        static void ExecuteCommand(Dictionary<string, string> commands)
        {
            if (commands is null)
                return;

            if (commands.ContainsKey("exit"))
            {
                ApplicationExit();
                Environment.Exit(0);
            }

            if (commands.ContainsKey("-list"))
            {
                Console.WriteLine($"接続リスト一覧");

                if (ConnectionManager.Instance.Count == 0)
                    Console.WriteLine($"    なし");
                else
                    for (var i = 0; i < ConnectionManager.Instance.Count; i++)
                    {
                        var item = ConnectionManager.Instance[i];
                        Console.WriteLine($"{i} - {item.Plugin.PluginName}　URL:{item.URL}　状態:{(item.IsActive.Value ? "有効" : "無効")}");
                    }
            }

            if (commands.TryGetValue("-add", out var url))
                MultiCommentCollector.Instance.AddURL(url, true);


            if (commands.TryGetValue("-remove", out var remove))
            {
                if (int.TryParse(remove, out var result) && result < ConnectionManager.Instance.Count)
                {
                    var item = ConnectionManager.Instance[result];
                    MultiCommentCollector.Instance.Inactivate(item);
                    ConnectionManager.Instance.Remove(item);
                }
            }
            if (commands.TryGetValue("-activate", out var activate))
            {
                if (int.TryParse(activate, out var result) && result < ConnectionManager.Instance.Count)
                {
                    var item = ConnectionManager.Instance[result];
                    MultiCommentCollector.Instance.Activate(item);
                }
            }
            if (commands.TryGetValue("-inactivate", out var inactivate))
            {
                if (int.TryParse(inactivate, out var result) && result < ConnectionManager.Instance.Count)
                {
                    var item = ConnectionManager.Instance[result];
                    MultiCommentCollector.Instance.Inactivate(item);
                }
            }

            if (commands.TryGetValue("-show", out var s) && s is not null)
            {
                switch (s.ToLower())
                {
                    case "comment":
                        IsComment = true;
                        Console.WriteLine("コメントログを有効化しました。");
                        break;
                }
            }

            if (commands.TryGetValue("-hide", out var h) && h is not null)
            {
                switch (h.ToLower())
                {
                    case "comment":
                        IsComment = false;
                        Console.WriteLine("コメントログを無効化しました。");
                        break;
                }
            }
        }

        static void Log_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var item = LogManager.Instance[e.NewStartingIndex];
            Console.WriteLine($"[{item.Date}] {item.SenderName} * {item.Log}");
        }

        static void Comment_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsComment)
            {
                var item = CommentManager.Instance[e.NewStartingIndex];
                Console.WriteLine($"[{item.PostTime}] {item.LiveName} * {item.UserName} - {item.Comment}");
            }
        }

        static void ApplicationStart()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Setting setting = Setting.Instance;
            LogManager.Instance.CollectionChanged += Log_CollectionChanged;
            CommentManager.Instance.CollectionChanged += Comment_CollectionChanged;

            foreach (var item in Setting.Instance.ConnectionList)
                ConnectionManager.Instance.Add(item);

            MultiCommentCollector.Instance.Apply();
            MultiCommentCollector.Instance.ServerStart();
        }

        static void ApplicationExit()
        {
            MCC.Core.MultiCommentCollector.Instance.ServerStop();

            Setting.Instance.ConnectionList = ConnectionManager.Instance;

            foreach (var item in PluginManager.Instance)
                item.PluginClose();

            try
            {
                XmlSerializer.FileSerialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml", Setting.Instance);
            }
            catch
            {

            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            ApplicationExit();
        }
    }
}
