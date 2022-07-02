namespace UTubePatcher.sources
{
    static class Patches
    {
        public static void Debug(bool extendedDebug)
        {
            ConsoleLog.Normal("Debug patching in progress...", true);
            ConsoleLog.Normal("", true);

            List<string> allFiles = new List<string>();

            string[] allDirs = Directory.GetDirectories(Main.apkDecompiledPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string dir in allDirs)
            {
                if (dir.Contains("smali"))
                {
                    allFiles.AddRange(Directory.GetFiles(dir, "*.smali*", SearchOption.TopDirectoryOnly).ToList());
                }
            }

            string firstReference = ".method";
            string firstSubReference = "onClick";
            string secondSubReference = "onTouchEvent";
            string thirdSubReference = "performClick";
            string fourthSubReference = ".locals";
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);

                if (Path.GetFileName(file).Length <= (extendedDebug ? 9 : 10))
                {
                    if (!extendedDebug
                        ?
                        (fullContent.Contains(firstReference) &&
                        fullContent.Contains(firstSubReference)) ||
                        (fullContent.Contains(firstReference) &&
                        fullContent.Contains(secondSubReference)) ||
                        (fullContent.Contains(firstReference) &&
                        fullContent.Contains(thirdSubReference))
                        :
                        fullContent.Contains(firstReference))
                    {
                        bool printFileName = true;

                        List<string> lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if ((!extendedDebug
                                ?
                                (lines[i].Contains(firstReference) &&
                                lines[i].Contains(firstSubReference)) ||
                                (lines[i].Contains(firstReference) &&
                                lines[i].Contains(secondSubReference)) ||
                                (lines[i].Contains(firstReference) &&
                                lines[i].Contains(thirdSubReference))
                                :
                                lines[i].Contains(firstReference)) && lines[i + 1].Contains(fourthSubReference))
                            {
                                if (printFileName)
                                {
                                    ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);
                                    printFileName = false;
                                }

                                string[] patch = {
                                    "invoke-static {}, Lutube/uDebug;->printMethod()V"
                                };

                                lines.InsertRange(i + 2, patch);

                                File.WriteAllLines(file, lines);
                            }
                        }
                    }
                }
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Success("Debug patching succesfully completed", true);
        }

        public static void MicroG()
        {
            ConsoleLog.Normal("Patching MicroG references in progress...", true);
            ConsoleLog.Normal("", true);

            List<string> oldReferences = new List<string>();
            List<string> newReferences = new List<string>();
            void ExtendedString(string oldReference, string newReference)
			{
                oldReferences.Add(oldReference);
                newReferences.Add(newReference);
            };
            string subReference = "com.google.android.gms.accountsettings.action.VIEW_SETTINGS";

            ExtendedString("\"com.google\"", "\"com.mgoogle\"");
            ExtendedString("\"com.google.android.c2dm.intent.RECEIVE\"", "\"com.mgoogle.android.c2dm.intent.RECEIVE\"");
            ExtendedString("\"com.google.android.c2dm.intent.REGISTER\"", "\"com.mgoogle.android.c2dm.intent.REGISTER\"");
            ExtendedString("\"com.google.android.c2dm.intent.REGISTRATION\"", "\"com.mgoogle.android.c2dm.intent.REGISTRATION\"");
            ExtendedString("\"com.google.android.c2dm.permission.RECEIVE\"", "\"com.mgoogle.android.c2dm.permission.RECEIVE\"");
            ExtendedString("\"com.google.android.c2dm.permission.SEND\"", "\"com.mgoogle.android.c2dm.permission.SEND\"");
            ExtendedString("\"com.google.android.gms\"", "\"com.mgoogle.android.gms\"");
            ExtendedString("\"com.google.android.gms.auth.accounts\"", "\"com.mgoogle.android.gms.auth.accounts\"");
            ExtendedString("\"com.google.android.gsf.action.GET_GLS\"", "\"com.mgoogle.android.gsf.action.GET_GLS\"");
            ExtendedString("\"content://com.google.android.gsf.gservices\"", "\"content://com.mgoogle.android.gsf.gservices\"");
            ExtendedString("\"content://com.google.android.gsf.gservices/prefix\"", "\"content://com.mgoogle.android.gsf.gservices/prefix\"");
            ExtendedString("\"com.google.android.gsf.login\"", "\"com.mgoogle.android.gsf.login\"");
            ExtendedString("\"com.google.iid.TOKEN_REQUEST\"", "\"com.mgoogle.iid.TOKEN_REQUEST\"");
            ExtendedString("\"content://com.google.settings/partner\"", "\"content://com.mgoogle.settings/partner\"");
            ExtendedString("android:name=\"com.google.android.youtube.permission.C2D_MESSAGE\"", $"android:name=\"com.{Main.newPackageName.ToLower()}.android.youtube.permission.C2D_MESSAGE\"");
            ExtendedString("android:authorities=\"com.google.android.youtube", $"android:authorities=\"com.{Main.newPackageName.ToLower()}.android.youtube");
            ExtendedString("package=\"com.google.android.youtube", $"package=\"com.{Main.newPackageName.ToLower()}.android.youtube");
            ExtendedString("android:label=\"@string/application_name\"", $"android:label=\"{Main.newPackageName}\"");
            ExtendedString("</queries>", "<package android:name=\"com.mgoogle.android.gms\"/>\n</queries>");
            
            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);
            Array.Resize(ref allFiles, allFiles.Length + 1);
			allFiles[allFiles.Length - 1] = $"{Main.apkDecompiledPath}\\AndroidManifest.xml";

            if (oldReferences.Count == newReferences.Count)
            {
                List<string> lines = new List<string>();
                
                string firstLightWeightAPIReference = "\"The connection to Google Play services was lost\"";
                string firstLightWeightAPISubReference = "const-wide/32";
                string secondLightWeightAPISubReference = "0x493e0";
                string thirdLightWeightAPISubReference = ".locals";
                bool lightWeightAPIPatched = false;
                foreach (string file in allFiles)
                {
                    bool printFileName = true;

                    for (int i = 0; i < oldReferences.Count; i++)
                    {
                        string fullContent = File.ReadAllText(file);

                        if (fullContent.Contains(oldReferences[i]) &&
                            !fullContent.Contains(subReference))
                        {
                            if (printFileName)
                            {
                                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);
                                printFileName = false;
                            }

                            fullContent = fullContent.Replace(oldReferences[i], newReferences[i]);

                            File.WriteAllText(file, fullContent);
                        }

                        if (!lightWeightAPIPatched)
						{
                            if (fullContent.Contains(firstLightWeightAPIReference))
							{
                                lines = File.ReadAllLines(file).ToList();

                                for (int j = lines.Count - 1; j >= 0; j--)
								{
                                    if (lines[j].Contains(firstLightWeightAPIReference))
									{
                                        for (int k = j; k >= 0; k--)
                                        {
                                            if (lines[k].Contains(firstLightWeightAPISubReference) &&
                                                lines[k].Contains(secondLightWeightAPISubReference))
											{
                                                for (int l = k; l >= 0; l--)
                                                {
                                                    if (lines[l].Contains(thirdLightWeightAPISubReference))
                                                    {
                                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)} // Lightweight API", true);

                                                        string[] lightWeightAPIPatch = {
                                                            "return-void"
                                                        };

                                                        lines.InsertRange(l + 1, lightWeightAPIPatch);

                                                        File.WriteAllLines(file, lines);

                                                        lightWeightAPIPatched = true;

                                                        break;
                                                    }
                                                }

                                                if (lightWeightAPIPatched)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (lightWeightAPIPatched)
										{
                                            break;
										}
                                    }
								}
                            }
                        }
                    }
                }
                if (!lightWeightAPIPatched)
                {
                    ConsoleLog.Normal("", true);
                    ConsoleLog.Error("Error during searching and patching the Lightweight API reference", true);

                    Main.buildingFailed = true;
                }

                string settingFragmentPath = $"{Main.apkDecompiledPath}\\res\\xml\\settings_fragment.xml";

                lines = File.ReadAllLines(settingFragmentPath).ToList();

                string[] settingFragmentPatch = {
                    "<Preference android:title=\"microG\">",
                    "<intent android:targetPackage=\"com.mgoogle.android.gms\" android:targetClass=\"org.microg.gms.ui.SettingsActivity\"/>",
                    "</Preference>"
                };

                lines.InsertRange(lines.Count - 2, settingFragmentPatch);

                File.WriteAllLines(settingFragmentPath, lines);

                ConsoleLog.Normal("", true);
                ConsoleLog.Success("MicroG references succesfully patched", true);
            }
            else
			{
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error: The length of the two microG reference lists is not equal", true);

                Main.buildingFailed = true;
            }
        }

        public static void GooglePlaySignature()
        {
            ConsoleLog.Normal("Patching Google Play signature check in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string[] references = new string[] {
                "\"Google Play Store signature invalid.\"",
                "\"The Google Play services resources were not found. Check your project configuration to ensure that the resources are included.\"",
                "\"GooglePlayServices not available due to error \""
            };
            string firstSubReference = ".locals";
            bool firstReferencePatched = false;
            bool secondReferencePatched = false;
            bool thirdReferencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!firstReferencePatched)
                {
                    if (fullContent.Contains(references[0]))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(references[0]))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] firstPatch = {
                                            "const/4 v0, 0x0",
                                            "return-object v0"
                                        };

                                        lines.InsertRange(j + 1, firstPatch);

                                        File.WriteAllLines(file, lines);

                                        firstReferencePatched = true;

                                        break;
                                    }
                                }

                                if (firstReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!secondReferencePatched ||
                    !thirdReferencePatched)
                {
                    if (fullContent.Contains(references[1]))
                    {
                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(references[1]))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        string[] secondPatch = {
                                                "const/4 v0, 0x0",
                                                "return v0"
                                            };

                                        lines.InsertRange(j, secondPatch);

                                        secondReferencePatched = true;

                                        break;
                                    }
                                }

                                if (secondReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(references[2]))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        string[] thirdPatch = {
                                            "return-void"
                                        };

                                        lines.InsertRange(j, thirdPatch);

                                        File.WriteAllLines(file, lines);

                                        thirdReferencePatched = true;

                                        break;
                                    }
                                }

                                if (thirdReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (firstReferencePatched &&
                    secondReferencePatched &&
                    thirdReferencePatched)
                {
                    break;
                }
            }
            if (!firstReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);
            }
            if (!secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the second reference", true);
            }

            if (firstReferencePatched &&
                secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Google Play signature check succesfully patched", true);
            }
            else
			{
                Main.buildingFailed = true;
            }
        }

        public static void LocalConfigAttribute()
        {
            ConsoleLog.Normal("Patching localeConfig attribute in progress...", true);
            ConsoleLog.Normal("", true);

            string localesConfigPath = $"{Main.apkDecompiledPath}\\res\\xml\\locales_config.xml";

            if (File.Exists(localesConfigPath))
            {
                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(localesConfigPath)}", true);

                File.Delete(localesConfigPath);
            }

            string publicXMLPath = $"{Main.apkDecompiledPath}\\res\\values\\public.xml";
            string firstReference = "<public type=\"xml\" name=\"locales_config\"";
            if (File.Exists(publicXMLPath))
            {
                List<string> lines = File.ReadAllLines(publicXMLPath).ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains(firstReference))
                    {
                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(publicXMLPath)}", true);

                        lines.RemoveAt(i);

                        File.WriteAllLines(publicXMLPath, lines);

                        break;
                    }
                }
            }

            string androidManifestPath = $"{Main.apkDecompiledPath}\\AndroidManifest.xml";

            if (File.Exists(androidManifestPath))
            {
                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(androidManifestPath)}", true);

                string text = File.ReadAllText(androidManifestPath);
                string newText = text.Replace("android:localeConfig=\"@xml/locales_config\"", " ");

                File.WriteAllText(androidManifestPath, newText);
            }

            ConsoleLog.Normal("", true);
            ConsoleLog.Success("localeConfig attribute succesfully patched", true);
        }

        public static void EmailAccountText()
        {
            ConsoleLog.Normal("Patching Email Account Text in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string reference = APKOps.GetResourceID("string", "account_switcher_accessibility_label");
            string firstSubReference = "invoke-virtual";
            string secondSubReference = "Landroid/widget/TextView;->setVisibility(I)V";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(reference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(reference))
                            {
                                for (int j = i; j < lines.Count; j++)
                                {
                                    int steps = 52;
                                    int newIndex = j + steps < lines.Count ? j + steps : lines.Count;
                                    for (int k = j; k < newIndex; k++)
                                    {
                                        if (lines[k].Contains(firstSubReference) &&
                                            lines[k].Contains(secondSubReference))
                                        {
                                            ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                            string textViewVisibilityRegister = lines[k].Split(',', '}')[1].Replace(" ", "");

                                            string[] firstPatch = {
                                                $"const/16 {textViewVisibilityRegister}, 0x8"
                                            };

                                            lines.InsertRange(k, firstPatch);

                                            File.WriteAllLines(file, lines);

                                            referencePatched = true;

                                            break;
                                        }
                                    }

                                    if (referencePatched)
                                    {
                                        break;
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Email Account Text succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void BackgroundPlayback()
        {
            ConsoleLog.Normal("Patching Background Playback in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string firstReference = APKOps.GetResourceID("string", "pref_offline_category");
            string secondReference = APKOps.GetResourceID("string", "audio_unavailable");
            string thirdReference = APKOps.GetResourceID("string", "premium_early_access_browse_page_key");
            string fourthReference = "\"Primes instant initialization\"";
            string firstSubReference = ".method public";
            string secondSubReference = "()Z";
            string thirdSubReference = ".line";
            string fourthSubReference = "invoke-static";
            string fifthSubReference = ";)Z";
            string sixthSubReference = ".method";
            string seventhSubReference = ".end method";
            string eighthSubReference = "iget-object v4, v2, Landroidx/preference/Preference;->u:Ljava/lang/String;";
            string ninthSubReference = ", Ljava/lang/Object;-><init>()V";
            bool firstReferencePatched = false;
            bool secondReferencePatched = false;
            bool thirdReferencePatched = false;
            bool fourthReferencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!firstReferencePatched)
                {
                    if (fullContent.Contains(firstReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstReference))
                            {
                                for (int j = i; j < lines.Count; j++)
                                {
                                    if (lines[j].Contains(firstSubReference) &&
                                        lines[j].Contains(secondSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);
                                            
                                        string[] patch = {
                                            "const/4 v0, 0x1",
                                            "return v0"
                                        };

                                        lines.InsertRange(j + 2, patch);

                                        File.WriteAllLines(file, lines);

                                        firstReferencePatched = true;

                                        break;
                                    }
                                }

                                if (firstReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!secondReferencePatched)
                {
                    if (fullContent.Contains(secondReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(secondReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(thirdSubReference))
                                    {
                                        for (int k = j; k < lines.Count; k++)
                                        {
                                            if (lines[k].Contains(fourthSubReference) &&
                                                lines[k].Contains(fifthSubReference))
                                            {
                                                string targetSmali = APKOps.GetSmaliPathByName(lines[k].Split('L', ';')[1]);

                                                if (targetSmali != "") {
                                                    ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(targetSmali)}", true);
                                                    
                                                    List<string> targetSmaliLines = File.ReadAllLines(targetSmali).ToList();

                                                    for (int l = 0; l < targetSmaliLines.Count; l++)
                                                    {
                                                        if (targetSmaliLines[l].Contains(sixthSubReference) &&
                                                            targetSmaliLines[l].Contains($"{lines[k].Split('>', '(')[1]}(") &&
                                                            targetSmaliLines[l].Contains(fifthSubReference))
                                                        {
                                                            string[] patch = {
                                                                "const/4 v0, 0x1",
                                                                "return v0"
                                                            };

                                                            targetSmaliLines.InsertRange(l + 2, patch);

                                                            File.WriteAllLines(targetSmali, targetSmaliLines);

                                                            secondReferencePatched = true;

                                                            break;
                                                        }
                                                    }

                                                    if (secondReferencePatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                                    
                                                if (secondReferencePatched)
                                                {
                                                    break;
                                                }
                                            }

                                            if (lines[k].Contains(seventhSubReference))
                                            {
                                                break;
                                            }
                                        }

                                        if (secondReferencePatched)
                                        {
                                            break;
                                        }
                                    }

                                    if (lines[j].Contains(sixthSubReference))
                                    {
                                        break;
                                    }
                                }

                                if (secondReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!thirdReferencePatched)
                {
                    if (fullContent.Contains(thirdReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(thirdReference))
                            {
                                for (int j = i; j < lines.Count; j++)
                                {
                                    if (lines[j].Contains(eighthSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "invoke-static {v4}, Lutube/NullCheck;->ensureHasFragment(Ljava/lang/String;)Ljava/lang/String;",
                                            "move-result-object v4"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        File.WriteAllLines(file, lines);

                                        thirdReferencePatched = true;

                                        break;
                                    }
                                }

                                if (thirdReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!fourthReferencePatched)
                {
                    if (fullContent.Contains(fourthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(fourthReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(ninthSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "return-void"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        File.WriteAllLines(file, lines);

                                        fourthReferencePatched = true;

                                        break;
                                    }
                                }

                                if (fourthReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (firstReferencePatched &&
                    secondReferencePatched &&
                    thirdReferencePatched &&
                    fourthReferencePatched)
                {
                    break;
                }
            }
            if (!firstReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);
            }
            if (!secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the second reference", true);
            }
            if (!thirdReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the third reference", true);
            }
            if (!fourthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the fourth reference", true);
            }

            if (firstReferencePatched &&
                secondReferencePatched &&
                thirdReferencePatched &&
                fourthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Background Playback succesfully patched", true);
            }
            else
			{
                Main.buildingFailed = true;
            }
        }

        public static void MinimizedPlayerPlayback()
        {
            ConsoleLog.Normal("Patching Minimized Player Playback in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string reference = "Lcom/google/android/apps/youtube/app/watch/playback/MinimizedPlaybackPolicyController";
            string firstSubReference = "const/4";
            string secondSubReference = "0x3";
            string thirdSubReference = ".locals";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                if (!referencePatched)
                {
                    if (file.Contains("MinimizedPlaybackPolicyController.smali"))
                    {
                        List<string> lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(reference))
                            {
                                int steps = 5;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;

                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(firstSubReference) &&
                                        lines[j].Contains(secondSubReference))
                                    {
                                        for (int k = j; k >= 0; k--)
                                        {
                                            if (lines[k].Contains(thirdSubReference))
                                            {
                                                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                                string[] patch = {
                                                    "return-void"
                                                };

                                                lines.InsertRange(k + 1, patch);

                                                File.WriteAllLines(file, lines);

                                                referencePatched = true;

                                                break;
                                            }
                                        }
                                            
                                        if (referencePatched)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Minimized Player Playback succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void CastButtonAndModuleErrors()
        {
            ConsoleLog.Normal("Patching Cast Button in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            bool firstReferencePatched = false;
            string secondReference = "\"com.google.android.gms.cast.framework.internal.CastDynamiteModuleImpl\"";
            string thirdReference = "\"Error fetching CastContext.\"";
            string fourthReference = "\"Failed to load module via V2: \"";
            string firstSubReference = ".method";
            string secondSubReference = "setVisibility(I)V";
            bool secondReferencePatched = false;
            bool thirdReferencePatched = false;
            bool fourthReferencePatched = false;
            List<string> lines = new List<string>();
            foreach (string file in allFiles)
            {
                if (!firstReferencePatched)
                {
                    if (file.Contains("MediaRouteButton.smali"))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstSubReference) &&
                                lines[i].Contains(secondSubReference))
                            {
                                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                string[] patch = {
                                    "const/16 p1, 0x8"
                                };

                                lines.InsertRange(i + 2, patch);

                                File.WriteAllLines(file, lines);

                                firstReferencePatched = true;

                                break;
                            }
                        }
                    }
                }

                string fullContent = File.ReadAllText(file);

                if (!secondReferencePatched)
                {
                    if (fullContent.Contains(secondReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(secondReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "const/4 p0, 0x0",
                                            "return-object p0"
                                        };

                                        lines.InsertRange(j + 2, patch);

                                        File.WriteAllLines(file, lines);

                                        secondReferencePatched = true;

                                        break;
                                    }
                                }

                                if (secondReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!thirdReferencePatched)
                {
                    if (fullContent.Contains(thirdReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(thirdReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "return-void"
                                        };

                                        lines.InsertRange(j + 2, patch);

                                        File.WriteAllLines(file, lines);

                                        thirdReferencePatched = true;

                                        break;
                                    }
                                }

                                if (thirdReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!fourthReferencePatched)
                {
                    if (fullContent.Contains(fourthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(fourthReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "const/4 v0, 0x0",
                                            "return v0"
                                        };

                                        lines.InsertRange(j + 2, patch);

                                        File.WriteAllLines(file, lines);

                                        fourthReferencePatched = true;

                                        break;
                                    }
                                }

                                if (fourthReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (firstReferencePatched &&
                    secondReferencePatched &&
                    thirdReferencePatched &&
                    fourthReferencePatched)
                {
                    break;
                }
            }
            if (!firstReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);
            }
            if (!secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the second reference", true);
            }
            if (!thirdReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the third reference", true);
            }
            if (!fourthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the fourth reference", true);
            }

            if (firstReferencePatched &&
                secondReferencePatched &&
                thirdReferencePatched &&
                fourthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Cast Button succesfully patched", true);
            }
			else
			{
				Main.buildingFailed = true;
			}
        }

        public static void OldQualitySelector()
        {
            ConsoleLog.Normal("Applying Old Quality Selector in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);
            
            string firstReference = APKOps.GetResourceID("string", "quality_label");
            string firstSubReference = "invoke-interface/range";
            string secondSubReference = "Lcom/google/android/libraries/youtube/innertube/model/media/VideoQuality;IIZI";
            string thirdSubReference = "const-string";
            string fourthSubReference = ", \"\"";
            string fifthSubReference = "invoke-";
            string sixthSubReference = ":cond_";
            string seventhSubReference = ".method";
            string eighthSubReference = "iget ";
            string ninthSubReference = ".end method";

            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(firstSubReference) && fullContent.Contains(secondSubReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(thirdSubReference) &&
                                        lines[j].Contains(fourthSubReference))
                                    {
                                        int targetIndex = 0;
                                        bool checkPassed = false;

                                        for (int k = j; k >= 0; k--)
                                        {
                                            if (!checkPassed &&
                                                lines[k].Contains(fifthSubReference))
                                            {
                                                targetIndex = k;
                                                checkPassed = true;
                                            }

                                            if (checkPassed &&
                                                lines[k].Contains(sixthSubReference) &&
                                                targetIndex == k + 1)
                                            {
                                                var methodName = lines[targetIndex].Split('>', ';')[2];

                                                targetIndex = 0;
                                                checkPassed = false;

                                                for (int l = 0; l < lines.Count; l++)
                                                {
                                                    if (!checkPassed &&
                                                        lines[l].Contains(seventhSubReference) &&
                                                        lines[l].Contains(methodName))
                                                    {
                                                        targetIndex = l;
                                                        checkPassed = true;
                                                    }

                                                    if (checkPassed &&
                                                        lines[l].Contains(eighthSubReference) &&
                                                        targetIndex + 8 > l)
                                                    {
                                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                                        string[] patch = {
                                                            $"const/4 {lines[l].Split('t', ',')[1]}, 0x0"
                                                        };

                                                        lines.InsertRange(l + 1, patch);

                                                        File.WriteAllLines(file, lines);

                                                        referencePatched = true;

                                                        break;
                                                    }

                                                    if (checkPassed &&
                                                        lines[l].Contains(ninthSubReference))
                                                    {
                                                        break;
                                                    }
                                                }

                                                if (referencePatched)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (referencePatched || lines[j].Contains(seventhSubReference))
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Old Quality Selector succesfully applied", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void CreateButton()
        {
            ConsoleLog.Normal("Patching Create Button in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string firstReference = APKOps.GetResourceID("layout", "image_only_tab");
            string secondReference = "move-result-object";
            string firstSubReference = "invoke-virtual {";
            string secondSubReference = ", Lcom/google/android/apps/youtube/app/ui/pivotbar/PivotBar;->";
            string thirdSubReference = ")Landroid/view/View";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(firstReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstSubReference) &&
                                lines[i].Contains(secondSubReference) &&
                                lines[i].Contains(thirdSubReference))
                            {
                                string constIntRegister = lines[i].Split(',', '}')[3].Replace(" ", ""); ;

                                int steps = 4;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;

                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(secondReference))
                                    {
                                        string viewRegister = lines[j].Replace(secondReference, "").Replace(" ", "");

                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);
                                            
                                        string[] patch = {
                                            $"const/16 {constIntRegister}, 0x8",
                                            $"invoke-virtual {{{viewRegister}, {constIntRegister}}}, Landroid/view/View;->setVisibility(I)V"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        File.WriteAllLines(file, lines);

                                        referencePatched = true;

                                        break;
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Create Button succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void ShortsButton()
        {
            ConsoleLog.Normal("Patching Shorts Button in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string firstReference = "0x12f9f174";
            string secondReference = "0x10102fe";
            string firstSubReference = "and-int/lit8";
            string secondSubReference = "0x10";
            string thirdSubReference = "move-result-object";
            string fourthSubReference = "invoke-virtual/range {";
            string fifthSubReference = ", Lcom/google/android/apps/youtube/app/ui/pivotbar/PivotBar;->";
            string sixthSubReference = "(Landroid/graphics/drawable/Drawable;Ljava/lang/CharSequence;ZILjava/util/Map;";
            string seventhSubReference = ";Lj$/util/Optional;)Landroid/view/View;";
            bool firstReferencePatched = false;
            bool secondReferencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!firstReferencePatched ||
                    !secondReferencePatched)
                {
                    if (fullContent.Contains(firstReference) &&
                        fullContent.Contains(secondReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(secondReference))
                            {
                                int steps = 83;
                                int newIndex = i - steps > 0 ? i - steps : 0;

                                for (int j = i; j > newIndex; j--)
                                {
                                    if (lines[j].Contains(firstSubReference) &&
                                        lines[j].Contains(secondSubReference))
                                    {
                                        steps = 21;
                                        newIndex = j - steps > 0 ? j - steps : lines.Count;

                                        for (int k = j; k > newIndex; k--)
                                        {
                                            if (lines[k].Contains(thirdSubReference))
                                            {
                                                string enumButtonsRegister = lines[k].Replace(thirdSubReference, "").Replace(" ", "");

                                                string[] patch = {
                                                    $"sput-object {enumButtonsRegister}, Lutube/ADBlocker;->lastPivotTab:Ljava/lang/Enum;"
                                                };

                                                lines.InsertRange(k + 1, patch);

                                                firstReferencePatched = true;

                                                break;
                                            }
                                        }

                                        if (firstReferencePatched)
                                        {
                                            break;
                                        }
                                    }
                                }

                                for (int j = i; j < lines.Count; j++)
                                {
                                    if (lines[j].Contains(fourthSubReference) &&
                                        lines[j].Contains(fifthSubReference) &&
                                        lines[j].Contains(sixthSubReference) &&
                                        lines[j].Contains(seventhSubReference))
                                    {
                                        steps = 4;
                                        newIndex = j + steps < lines.Count ? j + steps : lines.Count;

                                        for (int k = j; k < newIndex; k++)
                                        {
                                            if (lines[k].Contains(thirdSubReference))
                                            {
                                                string viewRegister = lines[k].Replace(thirdSubReference, "").Replace(" ", "");

                                                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                                string[] patch = {
                                                    $"invoke-static {{{viewRegister}}}, Lutube/ADBlocker;->HideShortsButton(Landroid/view/View;)V",
                                                };

                                                lines.InsertRange(k + 1, patch);

                                                File.WriteAllLines(file, lines);

                                                secondReferencePatched = true;

                                                break;
                                            }
                                        }

                                        if (secondReferencePatched)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            if (firstReferencePatched &&
                                secondReferencePatched)
                            {
                                break;
                            }
                        }
                    }
                }

                if (firstReferencePatched &&
                    secondReferencePatched)
                {
                    break;
                }
            }
            if (!firstReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);
            }
            if (!secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the second reference", true);
            }

            if (firstReferencePatched &&
                secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Shorts Button succesfully patched", true);
            }
            else
            {
                Main.buildingFailed = true;
            }
        }

        public static void ShortsCommentsButton()
        {
            ConsoleLog.Normal("Patching Shorts Comments Button in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string firstReference = APKOps.GetResourceID("drawable", "ic_right_comment_32c");
            string secondReference = APKOps.GetResourceID("id", "reel_dyn_comment");
            string firstSubReference = "check-cast";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(firstReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        string constIntRegister = "";

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(secondReference))
                            {
                                constIntRegister = lines[i].Split('t', ',')[1].Replace(" ", "");

                                break;
                            }
                        }

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstReference))
                            {
                                int steps = 9;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;
                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(firstSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string viewRegister = lines[j].Split('t', ',')[1].Replace(" ", "");

                                        string[] patch = {
                                            $"const/16 {constIntRegister}, 0x8",
                                            $"invoke-virtual {{{viewRegister}, {constIntRegister}}}, Landroid/view/View;->setVisibility(I)V"
                                        };

                                        lines.InsertRange(j, patch);

                                        File.WriteAllLines(file, lines);

                                        referencePatched = true;

                                        break;
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Shorts Comments Button succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void LiveChatContent()
        {
            ConsoleLog.Normal("Patching Live Chat Content in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string reference = APKOps.GetResourceID("layout", "live_chat_content");
            string subReference = ".locals";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(reference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(reference))
                            {
                                for (int j = i; j > 0; j--)
                                {
                                    if (lines[j].Contains(subReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] firstPatch = {
                                            $"return-void"
                                        };

                                        lines.InsertRange(j + 1, firstPatch);

                                        File.WriteAllLines(file, lines);

                                        referencePatched = true;

                                        break;
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                
                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Live Chat Content succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void StartVideoPanel()
        {
            ConsoleLog.Normal("Patching Start Video Panel in progress...", true);
            ConsoleLog.Normal("", true);

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string reference = "EngagementPanelController: cannot show EngagementPanel before EngagementPanelController.init() has been called.";
            string subReference = ".locals";
            bool referencePatched = false;
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!referencePatched)
                {
                    if (fullContent.Contains(reference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(reference))
                            {
                                for (int j = i; j > 0; j--)
                                {
                                    if (lines[j].Contains(subReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] secondPatch = {
                                            "if-eqz p4, :cond_hideengagementpanel",
                                            "const/4 v0, 0x0",
                                            "return-object v0",
                                            ":cond_hideengagementpanel"
                                        };

                                        lines.InsertRange(j + 1, secondPatch);

                                        File.WriteAllLines(file, lines);

                                        referencePatched = true;

                                        break;
                                    }
                                }

                                if (referencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (referencePatched)
                {
                    break;
                }
            }
            if (referencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Live Chat Content succesfully patched", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the reference", true);

                Main.buildingFailed = true;
            }
        }

        public static void ADSRemoval()
        {
            ConsoleLog.Normal("Patching ADS in progress...", true);
            ConsoleLog.Normal("", true);

            APKOps.SmaliIntegration();

            string[] allFiles = Directory.GetFiles(Main.apkDecompiledPath, "*.smali*", SearchOption.AllDirectories);

            string firstReference = APKOps.GetResourceID("id", "ad_attribution");
            string secondReference = APKOps.GetResourceID("layout", "endscreen_element_layout_video");
            string thirdReference = APKOps.GetResourceID("layout", "endscreen_element_layout_circle");
            string fourthReference = APKOps.GetResourceID("layout", "endscreen_element_layout_icon");
            string fifthReference = APKOps.GetResourceID("layout", "promoted_video_item_land");
            string sixthReference = APKOps.GetResourceID("layout", "promoted_video_item_full_bleed");
            string seventhReference = "Claiming to use more elements than provided";
            string eighthReference = "loadVideo() called on LocalDirector in wrong state";
            string[] ninthReference = new string[] {
                "\"AnimatedVectorType\"",
                "\"CellType\"",
                "\"CollectionType\"",
                "\"ContainerType\"",
                "\"EditableTextType\"",
                "\"ImageType\"",
                "\"TextType\"",
                "\"ExpandableTextType\"",
                "\"ScrollableContainerType\"",
            };
            string firstSubReference = "invoke-virtual {";
            string secondSubReference = "}, Landroid/view/View;->findViewById(I)Landroid/view/View;";
            string thirdSubReference = "check-cast";
            string fourthSubReference = ", Landroid/widget/FrameLayout;";
            string fifthSubReference = "move-result-object";
            string sixthSubReference = "invoke-direct";
            string seventhSubReference = ";-><init>(";
            string eighthSubReference = ".line";
            string ninthSubReference = "invoke-static";
            string tenthSubReference = "Landroid/view/View;->inflate";
            string eleventhSubReference = "invoke-direct {p0}, Ljava/lang/Object;-><init>()V";
            string twelfthSubReference = ".method";
            string thirteenthSubReference = "invoke-virtual {";
            string fourteenthSubReference = "(Z)V";
            string fifteenthSubReference = "iput-boolean";
            string sixteenthSubReference = "0xaed2868";
            string seventeenthSubReference = "invoke-";
            string eighteenthSubReference = ".end method";
            string ninteenthSubReference = "0xf3a91c5";
            string twentiethSubReference = "invoke-virtual/range";

            string twentyfirstSubReference = "Found an Element with missing debugger id.";
            string twentysecondSubReference = "Element missing type extension";
            string twentythirdSubReference = "invoke-static/range";
            string twentyfourthSubReference = "iget-object";
            //---------------------------------------------------------//
            bool firstReferencePatched = false;
            int firstReferenceAmount = 0;
            List<string> previousPatchedSmalis = new List<string>();
            //---------------------------------------------------------//
            bool secondThirdFourthReferencePatched = false;
            int secondThirdFourthPatchingInteractions = !secondThirdFourthReferencePatched ? 0 : 3;
            var writeNewFile = false;
            //--------------------------------------------------------//
            bool secondReferencePatched = false;
            bool thirdReferencePatched = false;
            bool fourthReferencePatched = false;
            bool fifthReferencePatched = false;
            bool sixthReferencePatched = false;
            bool seventhReferencePatched = false;
            bool eighthReferencePatched = false; //Video ADS
            bool ninthReferencePatched = false; //Litho ADS
            
            foreach (string file in allFiles)
            {
                string fullContent = File.ReadAllText(file);
                List<string> lines = new List<string>();

                if (!firstReferencePatched)
                {
                    if (firstReferenceAmount > 0)
                    {
                        foreach (string previousPatchedSmali in previousPatchedSmalis)
                        {
                            if (!file.Equals(previousPatchedSmali))
                            {
                                firstReferencePatched = false;
                            }
                        }
                    }

                    if (fullContent.Contains(firstReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(firstReference))
                            {
                                int steps = 5;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;

                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(firstSubReference) &&
                                        lines[j].Contains(secondSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        previousPatchedSmalis.Add(file);

                                        string viewRegister = lines[j].Split('{', ',')[1];

                                        string[] patch = {
                                            $"invoke-static {{{viewRegister}}}, Lutube/ADBlocker;->LayoutView(Landroid/view/View;)V"
                                        };

                                        lines.InsertRange(j - 1, patch);

                                        File.WriteAllLines(file, lines);

                                        firstReferencePatched = true;

                                        firstReferenceAmount++;

                                        break;
                                    }
                                }

                                if (firstReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (secondThirdFourthPatchingInteractions < 3)
                {
                    writeNewFile = false;

                    if (fullContent.Contains(secondReference) ||
                        fullContent.Contains(thirdReference) ||
                        fullContent.Contains(fourthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(secondReference) ||
                                lines[i].Contains(thirdReference) ||
                                lines[i].Contains(fourthReference))
                            {
                                int steps = 15;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;

                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(thirdSubReference) &&
                                        lines[j].Contains(fourthSubReference))
                                    {
                                        string checkCastRegister = lines[j].Split('t', ',')[1].Replace(" ", "");

                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            $"invoke-static {{{checkCastRegister}}}, Lutube/ADBlocker;->NormalView(Landroid/view/View;)V"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        if (lines[i].Contains(secondReference)) secondReferencePatched = true;
                                        if (lines[i].Contains(thirdReference)) thirdReferencePatched = true;
                                        if (lines[i].Contains(fourthReference)) fourthReferencePatched = true;

                                        secondThirdFourthPatchingInteractions++;
                                        writeNewFile = true;

                                        break;
                                    }
                                }

                                if (writeNewFile)
                                {
                                    File.WriteAllLines(file, lines);

                                    break;
                                }
                            }
                        }
                    }
                }

                if (!fifthReferencePatched)
                {
                    if (fullContent.Contains(fifthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(fifthReference))
                            {
                                int steps = 7;
                                int newIndex = i + steps < lines.Count ? i + steps : lines.Count;

                                for (int j = i; j < newIndex; j++)
                                {
                                    if (lines[j].Contains(fifthSubReference))
                                    {
                                        string moveResultRegister = lines[j].Replace(fifthSubReference, "").Replace(" ", "");

                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            $"invoke-static {{{moveResultRegister}}}, Lutube/ADBlocker;->LayoutView(Landroid/view/View;)V"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        File.WriteAllLines(file, lines);

                                        fifthReferencePatched = true;

                                        break;
                                    }
                                }

                                if (fifthReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!sixthReferencePatched)
                {
                    if (fullContent.Contains(sixthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(sixthReference))
                            {
                                int steps = 12;
                                int newIndex = i - steps > 0 ? i - steps : 0;

                                int targetIndex = 0;
                                bool checkPassed = false;

                                for (int j = i; j > newIndex; j--)
                                {
                                    if (!checkPassed &&
                                        lines[j].Contains(sixthSubReference) &&
                                        lines[j].Contains(seventhSubReference))
                                    {
                                        targetIndex = j;
                                        checkPassed = true;
                                    }

                                    if (checkPassed &&
                                        lines[j].Contains(eighthSubReference))
                                    {
                                        string targetSmali = APKOps.GetSmaliPathByName(lines[targetIndex].Split('L', ';')[1]);

                                        if (targetSmali != "")
                                        {
                                            List<string> targetSmaliLines = File.ReadAllLines(targetSmali).ToList();

                                            for (int k = 0; k < targetSmaliLines.Count; k++)
                                            {
                                                if (targetSmaliLines[k].Contains(APKOps.GetResourceID("id", "title")))
                                                {
                                                    steps = 10;
                                                    newIndex = k - steps > 0 ? k - steps : 0;

                                                    for (int l = k; l > newIndex; l--)
                                                    {
                                                        if (targetSmaliLines[l].Contains(fifthSubReference))
                                                        {
                                                            steps = 4;
                                                            newIndex = l - steps > 0 ? l - steps : 0;

                                                            for (int m = l; m > newIndex; m--)
                                                            {
                                                                if (targetSmaliLines[m].Contains(ninthSubReference) &&
                                                                    targetSmaliLines[m].Contains(tenthSubReference))
                                                                {
                                                                    string moveResultRegister = targetSmaliLines[l].Replace(fifthSubReference, "").Replace(" ", "");

                                                                    ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(targetSmali)}", true);

                                                                    string[] patch = {
                                                                        $"invoke-static {{{moveResultRegister}}}, Lutube/ADBlocker;->LayoutView(Landroid/view/View;)V"
                                                                    };

                                                                    targetSmaliLines.InsertRange(l + 1, patch);

                                                                    File.WriteAllLines(targetSmali, targetSmaliLines);

                                                                    sixthReferencePatched = true;

                                                                    break;
                                                                }
                                                            }

                                                            if (sixthReferencePatched)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (sixthReferencePatched)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (sixthReferencePatched)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (sixthReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!seventhReferencePatched)
                {
                    if (fullContent.Contains(seventhReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(seventhReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(eleventhSubReference))
                                    {
                                        ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)}", true);

                                        string[] patch = {
                                            "return-void"
                                        };

                                        lines.InsertRange(j + 1, patch);

                                        File.WriteAllLines(file, lines);

                                        seventhReferencePatched = true;

                                        break;
                                    }

                                    if (lines[j].Contains(twelfthSubReference))
                                    {
                                        break;
                                    }
                                }

                                if (seventhReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!eighthReferencePatched)
                {
                    if (fullContent.Contains(eighthReference))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(eighthReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(thirteenthSubReference) &&
                                        lines[j].Contains(fourteenthSubReference)) {

                                        string targetSmali = APKOps.GetSmaliPathByName(lines[j].Split('L', ';')[1]);

                                        if (targetSmali != "")
                                        {
                                            string methodName = lines[j].Split('>', '(')[1];

                                            List<string> targetSmaliLines = File.ReadAllLines(targetSmali).ToList();

                                            int targetIndex = 0;
                                            bool checkPassed = false;

                                            for (int k = 0; k < targetSmaliLines.Count; k++)
                                            {
                                                if (!checkPassed &&
                                                    targetSmaliLines[k].Contains(twelfthSubReference) &&
                                                    targetSmaliLines[k].Contains(lines[j].Split('>', '(')[1]) &&
                                                    targetSmaliLines[k].Contains(fourteenthSubReference))
                                                {
                                                    targetIndex = k + 2;
                                                    checkPassed = true;
                                                }

                                                if (checkPassed &&
                                                    targetSmaliLines[k].Contains(fifteenthSubReference))
                                                {
                                                    string boolRegister = targetSmaliLines[k].Split('n', ',')[1];

                                                    ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(targetSmali)} // Video ADS", true);

                                                    string[] patch = {
                                                            $"const/4 {boolRegister}, 0x0"
                                                        };

                                                    targetSmaliLines.InsertRange(targetIndex, patch);

                                                    File.WriteAllLines(targetSmali, targetSmaliLines);

                                                    eighthReferencePatched = true;

                                                    break;
                                                }
                                            }

                                            if (eighthReferencePatched)
                                            {
                                                break;
                                            }
                                        }

                                        if (eighthReferencePatched)
                                        {
                                            break;
                                        }
                                    }

                                    if (lines[j].Contains(twelfthSubReference))
                                    {
                                        break;
                                    }
                                }

                                if (eighthReferencePatched)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!ninthReferencePatched)
                {
                    if (fullContent.Contains(ninthReference[0]) &&
                        fullContent.Contains(ninthReference[1]) &&
                        fullContent.Contains(ninthReference[2]) &&
                        fullContent.Contains(ninthReference[3]) &&
                        fullContent.Contains(ninthReference[4]) &&
                        fullContent.Contains(ninthReference[5]) &&
                        fullContent.Contains(ninthReference[6]) &&
                        fullContent.Contains(ninthReference[7]) &&
                        fullContent.Contains(ninthReference[8]))
                    {
                        lines = File.ReadAllLines(file).ToList();

                        List<string> getTemplateNameReferences = new List<string>();

                        int startLithoMethodLineIndex = 0;
                        int iterationsAmount = 0;
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (getTemplateNameReferences.Count == 0)
                            {
                                getTemplateNameReferences.Add(
                                    lines[0].Split('L', ';')[1]
                                );
                            }

                            if (lines[i].Contains(sixteenthSubReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(twelfthSubReference))
                                    {
                                        startLithoMethodLineIndex = j;

                                        break;
                                    }
                                }

                                for (int j = i; j < lines.Count; j++)
                                {
                                    if (lines[j].Contains(seventeenthSubReference))
                                    {
                                        getTemplateNameReferences.Add(
                                            lines[j].Substring(lines[j].LastIndexOf(',') + 1)
                                        );

                                        iterationsAmount++;

                                        if (iterationsAmount == 3)
                                        {
                                            break;
                                        }
                                    }

                                    if (lines[j].Contains(eighteenthSubReference))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (iterationsAmount == 4)
                            {
                                break;
                            }
                        }

                        iterationsAmount = 0;

                        for (int i = startLithoMethodLineIndex; i < lines.Count; i++)
                        {
                            if (lines[i].Contains(ninteenthSubReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(twentiethSubReference))
                                    {
                                        getTemplateNameReferences.Add(
                                            lines[j].Substring(lines[j].LastIndexOf(',') + 1)
                                        );

                                        iterationsAmount++;

                                        break;
                                    }

                                    if (lines[j].Contains(twelfthSubReference))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (lines[i].Contains(twentyfirstSubReference))
                            {
                                for (int j = i; j >= 0; j--)
                                {
                                    if (lines[j].Contains(twentiethSubReference))
                                    {
                                        getTemplateNameReferences.Add(
                                            lines[j].Substring(lines[j].LastIndexOf(',') + 1)
                                        );

                                        iterationsAmount++;

                                        break;
                                    }

                                    if (lines[j].Contains(twelfthSubReference))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (iterationsAmount == 2)
                            {
                                break;
                            }

                            if (lines[i].Contains(eighteenthSubReference))
                            {
                                break;
                            }
                        }

                        if (getTemplateNameReferences.Count == 6)
                        {
                            List<string> lithoMethodReferences = new List<string>();

                            lithoMethodReferences.Add(lines[startLithoMethodLineIndex].Split('L', ';')[5]);

                            iterationsAmount = 0;

                            for (int i = startLithoMethodLineIndex; i < lines.Count; i++)
                            {
                                if (lines[i].Contains(twentysecondSubReference))
                                {
                                    for (int j = i; j < lines.Count; j++)
                                    {
                                        if (lines[j].Contains(twentythirdSubReference))
                                        {
                                            lithoMethodReferences.Add(
                                                lines[j].Substring(lines[j].LastIndexOf(',') + 1)
                                            );

                                            iterationsAmount++;
                                        }

                                        if (lines[j].Contains(twentyfourthSubReference))
                                        {
                                            lithoMethodReferences.Add(
                                                lines[j].Substring(lines[j].LastIndexOf(',') + 1)
                                            );

                                            iterationsAmount++;

                                            break;
                                        }

                                        if (lines[j].Contains(eighteenthSubReference))
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (iterationsAmount == 2)
                                {
                                    break;
                                }

                                if (lines[i].Contains(eighteenthSubReference))
                                {
                                    break;
                                }
                            }

                            if (lithoMethodReferences.Count == 3)
                            {
                                ConsoleLog.Found($"Found file: {Path.GetFileNameWithoutExtension(file)} // Litho ADS", true);

                                var lithoMethodLine = lines[startLithoMethodLineIndex];
                                var lithoMethodLineRegistersAmount = lines[startLithoMethodLineIndex + 1];

                                lines[startLithoMethodLineIndex] = "";
                                lines[startLithoMethodLineIndex + 1] = "";

                                string[] patch = {
                                    ".method public static getIsEmpty(Ljava/lang/String;)Z",
                                    ".locals 1",
                                    "if-eqz p0, :cond_firstgetisempty",
                                    "invoke-virtual {p0}, Ljava/lang/String;->isEmpty()Z",
                                    "move-result p0",
                                    "if-eqz p0, :cond_secondgetisempty",
                                    "goto :goto_thirdgetisempty",
                                    ":cond_secondgetisempty",
                                    "const/4 p0, 0x0",
                                    "return p0",
                                    ":cond_firstgetisempty",
                                    ":goto_thirdgetisempty",
                                    "const/4 p0, 0x1",
                                    "return p0",
                                    ".end method",
                                    "",
                                    $".method private static getTemplateName({getTemplateNameReferences[4].Split(',', ';')[0].Replace(" ", "")};)Ljava/lang/String;",
                                    ".locals 2",
                                    $"invoke-virtual {{p0}}, {getTemplateNameReferences[5]}",
                                    "move-result-object p0",
                                    "const v0, 0xaed2868",
                                    $"invoke-static {{p0, v0}}, {getTemplateNameReferences[1]}",
                                    "move-result-object p0",
                                    "if-eqz p0, :cond_templatename",
                                    $"invoke-static {{p0}}, {getTemplateNameReferences[2]}",
                                    "move-result-object p0",
                                    $"invoke-virtual {{p0}}, {getTemplateNameReferences[3]}",
                                    "move-result-object v0",
                                    $"invoke-static {{v0}}, L{getTemplateNameReferences[0]};->getIsEmpty(Ljava/lang/String;)Z",
                                    "move-result v0",
                                    "if-nez v0, :cond_templatename",
                                    $"invoke-virtual {{p0}}, {getTemplateNameReferences[3]}",
                                    "move-result-object p0",
                                    "return-object p0",
                                    ":cond_templatename",
                                    "const/4 p0, 0x0",
                                    "return-object p0",
                                    ".end method",
                                    "",
                                    lithoMethodLine,
                                    lithoMethodLineRegistersAmount,
                                    $"invoke-static/range {{p3 .. p3}}, L{getTemplateNameReferences[0]};->getTemplateName(L{lithoMethodReferences[0]};)Ljava/lang/String;",
                                    "move-result-object v0",
                                    "if-eqz v0, :cond_lithoremoval",
                                    "move-object/from16 v1, p3",
                                    $"iget-object v2, v1, L{lithoMethodReferences[0]};->b:Ljava/nio/ByteBuffer;",
                                    "invoke-static {v0, v2}, Lutube/ADBlocker;->LithoView(Ljava/lang/String;Ljava/nio/ByteBuffer;)Z",
                                    "move-result v1",
                                    "if-eqz v1, :cond_lithoremoval",
                                    "move-object/from16 v2, p1",
                                    $"invoke-static {{v2}}, {lithoMethodReferences[1]}",
                                    "move-result-object v0",
                                    $"iget-object v0, v0, {lithoMethodReferences[2]}",
                                    "return-object v0",
                                    ":cond_lithoremoval"
                                };
                                
                                lines.InsertRange(startLithoMethodLineIndex + 2, patch);

                                File.WriteAllLines(file, lines);

                                ninthReferencePatched = true;
                            }
                        }
                    }
                }

                if (firstReferencePatched &&
                    secondReferencePatched &&
                    thirdReferencePatched &&
                    fourthReferencePatched &&
                    fifthReferencePatched &&
                    sixthReferencePatched &&
                    seventhReferencePatched &&
                    eighthReferencePatched &&
                    ninthReferencePatched)
                {
                    break;
                }
                }
            if (!firstReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the first reference", true);
            }
            if (!secondReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the second reference", true);
            }
            if (!thirdReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the third reference", true);
            }
            if (!fourthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the fourth reference", true);
            }
            if (!fifthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the fifth reference", true);
            }
            if (!sixthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the sixth reference", true);
            }
            if (!seventhReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the seventh reference", true);
            }
            if (!eighthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the eight reference", true);
            }
            if (!ninthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the ninth reference", true);
            }

            if (firstReferencePatched &&
                secondReferencePatched &&
                thirdReferencePatched &&
                fourthReferencePatched &&
                fifthReferencePatched &&
                sixthReferencePatched &&
                seventhReferencePatched &&
                eighthReferencePatched &&
                ninthReferencePatched)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("ADS succesfully patched", true);
            }
			else
			{
				Main.buildingFailed = true;
			}
        }

        public static void AmoledTheme()
        {
            ConsoleLog.Normal("Applying Amoled Theme in progress...", true);
            ConsoleLog.Normal("", true);

            string colorsXMLPath = $"{Main.apkDecompiledPath}\\res\\values\\colors.xml";
            int interactions = 0;
            string[] references = new string[] {
                "\"yt_black1\"",
                "\"yt_black1_opacity95\"",
                "\"yt_black2\"",
                "\"yt_black3\"",
                "\"yt_black4\"",
                "\"yt_selected_nav_label_dark\"",
                "\"yt_status_bar_background_dark\""
            };
            string subReference = "</resources>";
            string getXMLLine(string valueName)
            {
                return $"<color name={valueName}>@android:color/black</color>";
            };
            List<string> lines = File.ReadAllLines(colorsXMLPath).ToList();

            for (int i = 0; i < lines.Count; i++) {
                if (lines[i].Contains(subReference))
                {
                    string[] patch = new string[]
                    {
                        "<color name=\"yt_selected_nav_icon_dark\">#ffff0000</color>",
                        "<color name=\"yt_unselected_nav_icon_dark\">#ff909090</color>"
                    };

                    lines.InsertRange(i - 1, patch);
                    
                    File.WriteAllLines(colorsXMLPath, lines);

                    break;
                }

                for (int j = 0; j < references.Length; j++)
                {
                    if (lines[i].Contains(references[j]))
                    {
                        lines[i] = getXMLLine(references[j]);

                        interactions++;
                    }
                }
            }
            if (interactions == references.Length)
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Success("Amoled Theme succesfully applied", true);
            }
            else
            {
                ConsoleLog.Normal("", true);
                ConsoleLog.Error("Error during searching and patching the references", true);

                Main.buildingFailed = true;
            }
        }
    }
}