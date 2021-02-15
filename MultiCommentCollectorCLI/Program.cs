using MCC.Core;
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
        private static bool IsComment = false;

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
            var options = new HashSet<string> { "-add", "-activate", "-inactivate", "-list", "-s", "-h", "exit", "-remove" };

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
                ApplicationClose();
                Environment.Exit(0);
            }

            if (commands.ContainsKey("-list"))
            {
                Console.WriteLine($"接続リスト一覧");

                if (ConnectionManager.GetInstance().Items.Count == 0)
                    Console.WriteLine($"    なし");
                else
                    for (var i = 0; i < ConnectionManager.GetInstance().Items.Count; i++)
                    {
                        var item = ConnectionManager.GetInstance().Items[i];
                        Console.WriteLine($"{i} - {item.PluginName}　URL:{item.URL}　状態:{(item.IsActive.Value ? "有効" : "無効")}");
                    }
            }

            if (commands.TryGetValue("-add", out var url))
                MultiCommentCollector.GetInstance().AddURL(url, true);


            if (commands.TryGetValue("-remove", out var remove))
            {
                if (int.TryParse(remove, out var result) && result < ConnectionManager.GetInstance().Items.Count)
                {
                    var item = ConnectionManager.GetInstance().Items[result];
                    MultiCommentCollector.GetInstance().Inactivate(item);
                    ConnectionManager.GetInstance().Items.Remove(item);
                }
            }
            if (commands.TryGetValue("-activate", out var activate))
            {
                if (int.TryParse(activate, out var result) && result < ConnectionManager.GetInstance().Items.Count)
                {
                    var item = ConnectionManager.GetInstance().Items[result];
                    MultiCommentCollector.GetInstance().Activate(item);
                }
            }
            if (commands.TryGetValue("-inactivate", out var inactivate))
            {
                if (int.TryParse(inactivate, out var result) && result < ConnectionManager.GetInstance().Items.Count)
                {
                    var item = ConnectionManager.GetInstance().Items[result];
                    MultiCommentCollector.GetInstance().Inactivate(item);
                }
            }

            if (commands.TryGetValue("-s", out var s) && s is not null)
            {
                switch (s.ToLower())
                {
                    case "comment":
                        IsComment = true;
                        Console.WriteLine("コメントログを有効化しました。");
                        break;
                }
            }

            if (commands.TryGetValue("-h", out var h) && h is not null)
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

        static void ApplicationStart()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Setting setting = Setting.GetInstance();
            LogManager.GetInstance().Items.CollectionChanged += Log_CollectionChanged;
            CommentManager.GetInstance().Items.CollectionChanged += Comment_CollectionChanged;

            foreach (var item in Setting.GetInstance().ConnectionList)
                ConnectionManager.GetInstance().Items.Add(item);

            MultiCommentCollector.GetInstance().Apply();
            MultiCommentCollector.GetInstance().ServerStart();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            ApplicationClose();
        }

        private static void Log_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var item = LogManager.GetInstance().Items[e.NewStartingIndex];
            Console.WriteLine($"[{item.Date}] {item.SenderName} * {item.Log}");
        }

        private static void Comment_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsComment)
            {
                var item = CommentManager.GetInstance().Items[e.NewStartingIndex];
                Console.WriteLine($"[{item.PostTime}] {item.LiveName} * {item.UserName} - {item.Comment}");
            }
        }

        static void ApplicationClose()
        {
            MCC.Core.MultiCommentCollector.GetInstance().ServerStop();

            Setting.GetInstance().ConnectionList = ConnectionManager.GetInstance().Items;

            try
            {
                XmlSerializer.FileSerialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml", Setting.GetInstance());
            }
            catch
            {

            }
        }
    }
}
