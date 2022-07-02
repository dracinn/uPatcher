using System.Diagnostics;
using System.Security.Cryptography;

namespace UTubePatcher.sources
{
    static class APKOps
    {
        private static int topConsoleCursorPosition = 0;
        public static void ClearConsoleStartingFrom(int topConsoleCursorPosition)
        {
            for (int i = topConsoleCursorPosition; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, topConsoleCursorPosition);
        }

        private static string libsPath = "sources/libs";
        public static bool Decompile()
        {
            ConsoleLog.Normal("APK unpacking in progress...", true);
            ConsoleLog.Normal("", true);

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C java -jar {libsPath}/apktool.jar d -f {Main.apkPath} -o {Main.apkDecompiledPath}";
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            string processError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (processError.Length == 0)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("APK succesfully unpacked", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("APK unpacking failed", true);
                ConsoleLog.Error("Press any key to close the app", true);
                Console.ReadKey();
                return false;
            }

            return true;
        }

        public static bool Compile()
        {
            ConsoleLog.Normal("APK building in progress...", true);
            ConsoleLog.Normal("", true);

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C java -jar {libsPath}/apktool.jar b --use-aapt2 {Main.apkDecompiledPath} -o {Main.apkCompiledPath}";
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            string processError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (processError.Length == 0)
            {
                if (Directory.Exists(Main.apkDecompiledPath))
                {
                    Directory.Delete(Main.apkDecompiledPath, true);
                }

                ConsoleLog.Normal("", true);
                ConsoleLog.Success("APK succesfully built", true);
            }
            else
			{
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("APK building failed", true);
                Console.WriteLine(processError, true);
                ConsoleLog.Error("Press any key to close the app", true);
                Console.ReadKey();
                return false;
            }

            return true;
        }

        public static void ZipAlign()
        {
            ConsoleLog.Normal("APK alignment in progress...", true);
            ConsoleLog.Normal("", true);

            string alignedAPKPath = Main.apkCompiledPath.Replace(Main.newPackageName, Main.newPackageName + "_aligned");

            if (File.Exists(Main.apkCompiledPath))
            {
                File.Move(Main.apkCompiledPath, alignedAPKPath, true);
            }

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C sources\\libs\\zipalign.exe -p -f -v 4 {alignedAPKPath} {Main.apkCompiledPath}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            string processOutput = process.StandardOutput.ReadToEnd();
            string processError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (File.Exists(alignedAPKPath))
            {
                File.Delete(alignedAPKPath);
            }

            if (processError.Length == 0)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("APK succesfully aligned", true);
            }
            else
			{
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("APK alignment failed", true);
            }
        }

        public static void Sign()
        {
            topConsoleCursorPosition = Console.GetCursorPosition().Top;

            bool addEmptyLine = false;

            string signKeyFilePath = $"{libsPath}/signkey/signkey.KEYSTORE";
            while (!File.Exists(signKeyFilePath))
            {
                ConsoleLog.Error("Error: signkey.KEYSTORE file not found", true);
                ConsoleLog.Error("Press any key to retry", true);
                Console.ReadKey();
                ClearConsoleStartingFrom(topConsoleCursorPosition);
                addEmptyLine = true;
            }

            topConsoleCursorPosition = Console.GetCursorPosition().Top;

            string signKeyInfoFilePath = $"{libsPath}/signkey/signkeyinfo";
            while (!File.Exists(signKeyInfoFilePath))
			{
                ConsoleLog.Error("Error: signkeyinfo file not found", true);
                ConsoleLog.Error("Press any key to retry", true);
                Console.ReadKey();
                ClearConsoleStartingFrom(topConsoleCursorPosition);
                addEmptyLine = true;
            }

            topConsoleCursorPosition = Console.GetCursorPosition().Top;

            string firstLineSplit = "";
            string secondLineSplit = "";
            string firstStartContent = "Alias=";
            string secondStartContent = "Pass=";
            while ((firstLineSplit == "" && !firstLineSplit.Contains(firstStartContent)) &&
                    (secondLineSplit == "" && !secondLineSplit.Contains(secondStartContent)))
            {
                List<string> signKeyInfoFileContent = File.ReadAllLines(signKeyInfoFilePath).ToList();
                while (signKeyInfoFileContent.Count != 2)
                {
                    ConsoleLog.Error("Error: The length of signkeyinfo file is not equal to 2", true);
                    ConsoleLog.Error("Press any key to retry", true);
                    Console.ReadKey();
                    ClearConsoleStartingFrom(topConsoleCursorPosition);
                    addEmptyLine = true;
                }
                try
                {
                    firstLineSplit = signKeyInfoFileContent[0].Split('"', '"')[1];
                    secondLineSplit = signKeyInfoFileContent[1].Split('"', '"')[1];
                }
                catch (Exception)
                {
                    ConsoleLog.Error("Error: Lines format of signkeyinfo (without extension) file is wrong", true);
                    ConsoleLog.Error("Format to use:", true);
                    ConsoleLog.Error($"{firstStartContent}\"youralias\"", true);
                    ConsoleLog.Error($"{secondStartContent}\"yourpass\"", true);
                    ConsoleLog.Normal("", true);
                    ConsoleLog.Error("Press any key to retry", true);
                    Console.ReadKey();
                    ClearConsoleStartingFrom(topConsoleCursorPosition);
                    addEmptyLine = true;
                }
            }
            if (addEmptyLine)
            {
                ConsoleLog.Normal("", true);
            }

            ConsoleLog.Normal("APK signing in progress...", true);
            ConsoleLog.Normal("", true);

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C java -jar {libsPath}/apksigner.jar sign --ks {signKeyFilePath} --ks-key-alias {firstLineSplit} --ks-pass pass:{secondLineSplit} {Main.apkCompiledPath}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            process.StartInfo = startInfo;
            process.Start();
            string processOutput = process.StandardOutput.ReadToEnd();
            string processError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (processError.Length == 0)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("APK succesfully signed", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("APK signing failed", true);
            }
        }

        public static string GetSmaliPathByName(string smaliName)
		{
            string[] allSmaliDirs = Directory.GetDirectories(Main.apkDecompiledPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string smaliDir in allSmaliDirs)
            {
                if (smaliDir.Contains("smali"))
                {
                    string uncheckedSmali = smaliDir + "\\" + smaliName + ".smali";

                    if (File.Exists(uncheckedSmali))
                    {
                        return uncheckedSmali;
                    }
                }
            }

            return "";
        }

        public static string GetResourceID(String resType, String resName)
        {
            string hexID = "xxxxxxxxxx";

            string publicXMLPath = $"{Main.apkDecompiledPath}\\res\\values\\public.xml";
            
            if (!File.Exists(publicXMLPath))
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: public.xml not found", true);
                return hexID;
            }

            string[] lines = File.ReadAllLines(publicXMLPath);
            bool resExists = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains($"public type=\"{resType}\"") &&
                    lines[i].Contains($"name=\"{resName}\""))
                {
                    return lines[i].Split('\"', '\"')[5];
                }
            }
            if (!resExists)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: resource not found", true);
                return hexID;
            }

            return hexID;
        }

        public static string[] GetVersionCode()
        {
            string[] version = new string[] {
                "0000000000",
                "0.0"
            };

            string apktoolYMLPath = $"{Main.apkDecompiledPath}\\apktool.yml";

            if (!File.Exists(apktoolYMLPath))
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: apktool.yml not found", true);
                return version;
            }

            string[] lines = File.ReadAllLines(apktoolYMLPath);
            bool versionExists = false;
            for (int i = 0; i < lines.Length; i++)
            {
                int steps = 1;
                int newIndex = i + steps < lines.Length ? i + steps : lines.Length;

                string versionName = "versionName: ";
                if (lines[i].Contains($"versionCode: \'") &&
                    lines[newIndex].Contains(versionName))
                {
                    version[0] = lines[i].Split('\'', '\'')[1];
                    version[1] = lines[newIndex].Replace(versionName, "").Replace(" ", "");

                    versionExists = true;
                }
            }
            if (!versionExists)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: versionCode not found", true);
                return version;
            }

            return version;
        }

        public static void SmaliIntegration()
        {
            string oldSmaliFilePath = $"sources\\libs\\smali";
            string newSmaliFilePath = "";

            string[] allDirs = Directory.GetDirectories(Main.apkDecompiledPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string dir in allDirs)
            {
                if (dir.Contains("smali"))
                {
                    newSmaliFilePath = dir;
                }
            }
            if (newSmaliFilePath == "")
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: no smali folder found", true);
            }

            foreach (string dirPath in Directory.GetDirectories(oldSmaliFilePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(newSmaliFilePath + dirPath.Replace(oldSmaliFilePath, ""));
            }
            foreach (string filePath in Directory.GetFiles(oldSmaliFilePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, newSmaliFilePath + filePath.Replace(oldSmaliFilePath, ""), true);
            }
        }

        public static void SmaliBalance()
        {
            ConsoleLog.Normal("Balancing Smali Folders in progress...", true);
            ConsoleLog.Normal("", true);

            List<string> smaliDirs = new List<string>();
            
            string[] allDirs = Directory.GetDirectories(Main.apkDecompiledPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string dir in allDirs)
            {
                if (dir.Contains("smali"))
                {
                    smaliDirs.Add(dir);
                }
            }
            if (smaliDirs.Count == 0)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: no smali folder found", true);
            }

            foreach (string smaliDir in smaliDirs)
			{
                if (smaliDir != smaliDirs[smaliDirs.Count - 1])
                {
                    int smaliDirNameSplitLength = smaliDir.Split(Path.DirectorySeparatorChar).Length;
                    string smaliDirName = smaliDir.Split(Path.DirectorySeparatorChar)[smaliDirNameSplitLength - 1];

                    string[] smaliDirFiles = Directory.GetFiles(smaliDir, "*.*", SearchOption.TopDirectoryOnly);

                    if (smaliDirFiles.Length > 6000)
                    {
                        for (int i = 0; i <= 2000; i++)
                        {
                            File.Move(smaliDirFiles[i], smaliDirs[smaliDirs.Count - 1] + smaliDirFiles[i].Replace(smaliDir, ""));
                        }

                        ConsoleLog.Found($"{smaliDirName} - Fixed", true);
                    }
                    else
					{
                        ConsoleLog.Found($"{smaliDirName} - OK", true);
                    }
                }
			}

            ConsoleLog.Normal("", true);
            ConsoleLog.Success("Smali Folders Balanced succesfully", true);
        }

        public static void IconsIntegration()
        {
            ConsoleLog.Normal("Applying New Icons in progress...", true);
            ConsoleLog.Normal("", true);

            string oldSmaliFilePath = $"sources\\libs\\icons";

            foreach (string filePath in Directory.GetFiles(oldSmaliFilePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, $"{Main.apkDecompiledPath}\\res{filePath.Replace(oldSmaliFilePath, "")}", true);

                ConsoleLog.Found($"{Path.GetFileName(filePath)} - Copied", true);
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Success("New Icons succesfully applied", true);
        }

        public static void UnusedLibsRemoval()
        {
            ConsoleLog.Normal("Removing Unused Libraries in progress...", true);
            ConsoleLog.Normal("", true);

            string libsPath = $"{Main.apkDecompiledPath}\\lib";

            foreach (string dirPath in Directory.GetDirectories(libsPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                int dirNameSplitLength = dirPath.Split(Path.DirectorySeparatorChar).Length;
                string dirName = dirPath.Split(Path.DirectorySeparatorChar)[dirNameSplitLength - 1];

                if (!dirPath.Contains("arm64-v8a"))
				{
                    Directory.Delete(dirPath, true);

                    ConsoleLog.Found($"{dirName} - Removed", true);
                }
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Success("Unused Libraries succesfully removed", true);
        }

        public static string CalculateMD5(string apkPath)
        {
            if (File.Exists(apkPath))
            {
                FileStream stream = File.OpenRead(apkPath);
                string md5 = BitConverter.ToString(MD5.Create().ComputeHash(stream)).Replace("-", "").ToLower();
                stream.Close();
                return md5;
            }
            else
            {
                return "";
            }
        }

        public static void CachingFiles()
		{
            ConsoleLog.Normal("Async Caching Files in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);
            
            int currentFilesIndex = 0;
            int allFilesCount = allFiles.Length;
            int readStepsAmount = 100;
            List<Task<bool>> tasksList = new List<Task<bool>>();
            async Task<bool> DoAsyncTask(string[] allFiles, int startIndex, int endIndex)
            {
                await Task.Delay(1);

                for (int i = startIndex; i < endIndex; i++)
                {
                    string fullContent = File.ReadAllText(allFiles[i]);

                    if (fullContent.Any(char.IsLetterOrDigit))
					{
                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(allFiles[i])}", true);
                    }
                }

                return true;
            };
            for (int i = 0; i <= allFilesCount / readStepsAmount; i++)
            {
                int checkedStep = currentFilesIndex + readStepsAmount <= allFilesCount
                    ?
                    readStepsAmount
                    :
                    allFilesCount - currentFilesIndex;

                tasksList.Add(DoAsyncTask(allFiles, currentFilesIndex, currentFilesIndex + checkedStep));

                currentFilesIndex += checkedStep;
            }
            foreach(Task<bool> task in tasksList)
			{
                while(!task.IsCompletedSuccessfully)
                { }
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Success($"Async Caching Files succesfully completed", true);
        }
    }
}
