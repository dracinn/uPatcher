using HtmlAgilityPack;
using System.Diagnostics;
using System.Net;
using UTubePatcher.sources;

Console.Title = "uPatcher";
Main.Init();

static class Main
{
    public static bool debugBuild = false;
    public static bool printAtConsoleCenter = true;
    private static int topConsoleCursorPosition = 0;
    public static (int, int) initConsoleSize = (Console.WindowWidth, Console.WindowHeight);
    private static int dividerLength = initConsoleSize.Item1 / 2;
    private static string divider = $"{new string('\\', dividerLength)}{new string('/', dividerLength)}";
    private static string minVersion = "1529474496";
    private static string lastMD5 = "lastSourceFileMD5";
    public static string apkPath = "";
    public static string apkDecompiledPath = "";
    public static string apkCompiledPath = "";
    public static string newPackageName = "uTube";
    public static bool buildingFailed = false;

    public static void Init()
    {
        if (checkJavaInstallation())
        {
            ConsoleLog.Error("Error: Java is not installed on your system.", true);
            ConsoleLog.Error("Press any key to close the app", true);
            Console.ReadKey();
            Environment.Exit(0);
        }

        apkPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\youtube_source.apk";
        apkDecompiledPath = Path.GetDirectoryName(Main.apkPath) + "\\" + Path.GetFileNameWithoutExtension(Main.apkPath); ;
        apkCompiledPath = Path.GetDirectoryName(Main.apkPath) + "\\" + Main.newPackageName + ".apk";

        //----------Debug----------//
        //APKOps.Decompile();
        //Console.ReadKey();
        //APKOps.Compile();
        //Console.ReadKey();
        //APKOps.ZipAlign();
        //Console.ReadKey();
        //APKOps.Sign();
        //Console.ReadKey();
        //Patches.ADSRemoval();
        //Console.ReadKey();
        //ConsoleLog.Normal("".Split('t', ',')[1].Replace(" ", ""), true);
        //Console.ReadKey();

        ConsoleLog.Normal("", true);
        ConsoleLog.Warning(divider, true);
        ConsoleLog.Normal("", true);

        if (!debugBuild)
        {
            bool downloadSourceFileAgain = false;
            if (File.Exists(lastMD5))
            {
                string previousMD5 = File.ReadAllText(lastMD5);

                if (!File.Exists(apkPath) || (File.Exists(apkPath) && !previousMD5.Contains(APKOps.CalculateMD5(apkPath))))
                {
                    downloadSourceFileAgain = true;
                }
            }
            else
            {
                downloadSourceFileAgain = true;
            }
            if (downloadSourceFileAgain)
            {
                DownloadLastAPKVersion(apkPath);

                ConsoleLog.Normal("", true);
                ConsoleLog.Warning(divider, true);
                ConsoleLog.Normal("", true);
            }

            if (!APKOps.Decompile())
		    {
                Environment.Exit(0);
		    }

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            if (Int32.Parse(APKOps.GetVersionCode()[0]) >= Int32.Parse(minVersion))
            {
                ConsoleLog.Success($"OK: version {APKOps.GetVersionCode()[1]} should be compatible", true);
            }
            else
            {
                ConsoleLog.Error($"Error: version {APKOps.GetVersionCode()[1]} is not compatible", true);
                return;
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            APKOps.CachingFiles();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.MicroG();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.GooglePlaySignature();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.LocalConfigAttribute();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.EmailAccountText();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.BackgroundPlayback();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.MinimizedPlayerPlayback();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.CastButtonAndModuleErrors();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.OldQualitySelector();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.CreateButton();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.ShortsButton();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.ShortsCommentsButton();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.LiveChatContent();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.StartVideoPanel();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.ADSRemoval();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.AmoledTheme();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            APKOps.IconsIntegration();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            APKOps.UnusedLibsRemoval();

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            APKOps.SmaliBalance();
        }
        else
        {
            if (!APKOps.Decompile())
            {
                Environment.Exit(0);
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Warning(divider, true);
            ConsoleLog.Normal("", true);

            Patches.Debug(false);
        }

        ConsoleLog.Normal("", true);
        ConsoleLog.Warning(divider, true);
        ConsoleLog.Normal("", true);

        if (!APKOps.Compile())
        {
            Environment.Exit(0);
        }

        ConsoleLog.Normal("", true);
        ConsoleLog.Warning(divider, true);
        ConsoleLog.Normal("", true);

        APKOps.ZipAlign();

        ConsoleLog.Normal("", true);
        ConsoleLog.Warning(divider, true);
        ConsoleLog.Normal("", true);

        APKOps.Sign();

        ConsoleLog.Normal("", true);
        ConsoleLog.Warning(divider, true);
        ConsoleLog.Normal("", true);

        if (!buildingFailed)
        {
            ConsoleLog.Success("Building done!", true);
            ConsoleLog.Success("Press any key to close the patcher.", true);
        }
        else
		{
            ConsoleLog.Error("Building failed!", true);
			ConsoleLog.Error("Check console output for further informations", true);
            ConsoleLog.Error("Press any key to close the patcher.", true);
        }
        Console.ReadKey();
        ConsoleLog.Normal("", false);
    }

    private static bool checkJavaInstallation()
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/C java -version";
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        process.StartInfo = startInfo;
        process.Start();
        string processOutput = process.StandardOutput.ReadToEnd();
        string processError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return processError.Length == 0;
    }

    private static void DownloadLastAPKVersion(string apkPath)
    {
        topConsoleCursorPosition = Console.GetCursorPosition().Top;

        string version = "";
        string webMD5Value = "";
        bool fileIntegrityOK = false;
        bool downloadSourceFileCompleted = false;
        while (!fileIntegrityOK)
        {
            downloadSourceFileCompleted = false;

            HtmlWeb hw = new HtmlWeb();
            
            string youtubeSectionURL = $"https://www.apkmirror.com/apk/google-inc/youtube/";

            if (version == "")
            {
                ConsoleLog.Normal("Retrieving last YouTube version  →", true);

                HtmlDocument sourceFileVersionDoc = hw.Load(youtubeSectionURL);
                HtmlNodeCollection sourceFileVersionNodes = sourceFileVersionDoc.DocumentNode.SelectNodes("//h5");
                List<int> sourceFileVersionsIntList = new List<int>();

                for (int i = 0; i < sourceFileVersionNodes.Count; i++)
                {
                    if (!sourceFileVersionNodes[i].InnerText.Contains("APK Mirror"))
                    {
                        sourceFileVersionsIntList.Add(Int32.Parse(sourceFileVersionNodes[i].InnerText.Where(Char.IsDigit).ToArray()));
                    }
                    else
                    {
                        break;
                    }
                }

                if (sourceFileVersionsIntList.Count > 1)
                {
                    string newerVersion = sourceFileVersionsIntList.Max().ToString().Insert(2, ".").Insert(5, ".");
                    
                    if (newerVersion.Length < 8)
                    {
                        ConsoleLog.Error("Error: YouTube newerversion string length is less than 8", true);
                        ConsoleLog.Error("Press any key to close the app", true);
                        Console.ReadKey();
                        Environment.Exit(0);
                    }

                    ConsoleLog.Success($"{newerVersion} Found!", true);
                    ConsoleLog.Normal("", true);

                    version = newerVersion;
                }
                else
				{
                    ConsoleLog.Error("Error: YouTube version array length is less than 2", true);
                    ConsoleLog.Error("Press any key to close the app", true);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            else
			{
                for (int i = topConsoleCursorPosition; i < Console.WindowHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    ConsoleLog.Normal(new string(' ', Console.WindowWidth), false);
                }
                Console.SetCursorPosition(0, topConsoleCursorPosition);
            }

            HtmlDocument youtubeVersionSectionDoc = hw.Load($"{youtubeSectionURL}youtube-{version}-release/");
            string youtubeVersionSectionURL = youtubeVersionSectionDoc.DocumentNode.SelectSingleNode("//span[@class='apkm-badge']").ParentNode.ChildNodes[1].GetAttributeValue("href", "null");
            string[] youtubeVersionSectionURLSplits = youtubeVersionSectionURL.Split('/', '/');

            string sourceFileDownloadURL = $"{youtubeSectionURL}youtube-{version}-release/{youtubeVersionSectionURLSplits[youtubeVersionSectionURLSplits.Length - 2]}/";
            
            HtmlDocument sourceFileDoc = hw.Load($"{sourceFileDownloadURL}download/?forcebaseapk=true");
            HtmlNode sourceFileNode = sourceFileDoc.DocumentNode.SelectSingleNode("//form[@id='filedownload']");

            if (sourceFileNode != null)
            {
                if (webMD5Value == "")
                {
                    ConsoleLog.Normal("Retrieving package MD5  →", true);

                    HtmlDocument webMD5Doc = hw.Load(sourceFileDownloadURL);
                    HtmlNode webMD5Node = webMD5Doc.DocumentNode.SelectSingleNode("//div[@id='safeDownload']");

                    string safeDownloadNodeText = webMD5Node.ChildNodes[1].InnerText.Replace("\n", "");
                    for (int i = safeDownloadNodeText.IndexOf("APK file hashes", 0); i < safeDownloadNodeText.Length; i++)
                    {
                        int steps = 4;
                        bool indexLowerMaxLength = i + steps < safeDownloadNodeText.Length ? true : false;

                        if ((safeDownloadNodeText[i] == 'M' &&
                            safeDownloadNodeText[i + 1] == 'D' &&
                            safeDownloadNodeText[i + 2] == '5' &&
                            safeDownloadNodeText[i + 3] == ':') &&
                            indexLowerMaxLength)
                        {
                            string tempWebMD5Value = "";

                            for (int j = i + 4; j < safeDownloadNodeText.Length; j++)
                            {
                                steps = 4;
                                indexLowerMaxLength = j + steps < safeDownloadNodeText.Length ? true : false;

                                if (!(safeDownloadNodeText[j] == 'S' &&
                                    safeDownloadNodeText[j + 1] == 'H' &&
                                    safeDownloadNodeText[j + 2] == 'A' &&
                                    safeDownloadNodeText[j + 3] == '-' &&
                                    safeDownloadNodeText[j + 4] == '1') &&
                                    indexLowerMaxLength)
                                {
                                    tempWebMD5Value += safeDownloadNodeText[j];
                                }
                                else
                                {
                                    break;
                                }
                            }

                            webMD5Value = tempWebMD5Value.Replace(" ", "");

                            ConsoleLog.Success("Done!", true);
                            ConsoleLog.Normal("", true);

                            break;
                        }
                    }
                }

#pragma warning disable SYSLIB0014
                HttpWebRequest webPageRequest = (HttpWebRequest)WebRequest.Create($"https://www.apkmirror.com/wp-content/themes/APKMirror/download.php?id={sourceFileNode.ChildNodes[1].GetAttributeValue("value", "null").Replace(" ", "")}&forcebaseapk");

                webPageRequest.AllowAutoRedirect = true;
                webPageRequest.MaximumAutomaticRedirections = 1;

                ConsoleLog.Success($"Downloading YouTube {version}  →", true);

                WebClient webClient = new WebClient();
                DownloadProgress dP = new DownloadProgress();
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    dP.Progression(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
                };
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    webClient.CancelAsync();
                    webClient.Dispose();

                    downloadSourceFileCompleted = true;
                };
                webClient.DownloadFileAsync(webPageRequest.GetResponse().ResponseUri,
                    apkPath);
#pragma warning restore SYSLIB0014

                while (!downloadSourceFileCompleted)
                { }

                dP.Dispose();

                if (webMD5Value == APKOps.CalculateMD5(apkPath))
                {
                    File.WriteAllText(lastMD5, webMD5Value);

                    ConsoleLog.Success("Done!", true);

                    fileIntegrityOK = true;
                }
                else
                {
                    ConsoleLog.Error("Integrity error!", true);

                    if (File.Exists(apkPath))
                    {
                        File.Delete(apkPath);
                    }
                }

                Thread.Sleep(1000);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: Unable to get resource file download link", true);
                ConsoleLog.Error("Press any key to close the app", true);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}