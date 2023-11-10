using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
class Program
{
    static async Task Main(string[] args)
    {
        // Step 1: Parse command-line arguments
        string folderPath = args.Length > 0 ? args[0] : string.Empty;

        // Step 2: Check if the specified folder exists
        if (Directory.Exists(folderPath))
        {
            // Step 3: Check if the specified folder is not empty
            if (Directory.EnumerateFileSystemEntries(folderPath).Any())
            {
                // Step 4: Define the target folder path in the "Process" directory
                string targetFolderPath = Path.Combine("Process");

                // Step 5: Create the target folder if it doesn't exist
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                // Step 6: Copy the contents of the specified folder to the target folder
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    string fileName = Path.GetFileName(filePath);
                    string targetFilePath = Path.Combine(targetFolderPath, fileName);
                    File.Copy(filePath, targetFilePath, true);
                }

                // Step 7: Find main.bat file in the specified folder
                string batchScriptPath = Path.Combine(targetFolderPath, "main.bat");

                if (File.Exists(batchScriptPath))
                {
                    // Step 8: Read configuration from config.json
                    string configFilePath = Path.Combine(targetFolderPath, "config.json");

                    if (File.Exists(configFilePath))
                    {
                        // Step 9: Extract arguments from config.json
                        string configJson = File.ReadAllText(configFilePath);
                        dynamic config = JObject.Parse(configJson);
                        JArray argsArray = config.args;
                        string cli = "/c main.bat " + string.Join(' ', argsArray.Select(arg => $"\"{arg}\""));

                        // Step 10: Start a new process for batch script execution with arguments
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = cli,
                            WorkingDirectory = targetFolderPath,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        // Step 11: Start process with async 
                        Process process = new Process { StartInfo = psi };
                        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data); // Use NLog for logging
                        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine($"Error: {e.Data}"); };
                        Console.WriteLine($"Command Calling: > cmd.exe {cli}");
                        Console.WriteLine("==================================================");
                        process.Start();

                        // Step 12: Begin asynchronous reading of standard output
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        // Step 13: Wait for process exit
                        await process.WaitForExitAsync();
                        Console.WriteLine("==================================================");

                        // Step 14: Logging and error handling
                        int exitCode = process.ExitCode;
                        Console.WriteLine(exitCode == 0 ? "Batch script executed successfully." : $"Error: Batch script execution failed with exit code {exitCode}.");

                        // Step 15: Move contents of "Process" folder to a new folder with datetime tag
                        string currentDateTimeTag = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string destinationFolderPath = Path.Combine("LogStorage", currentDateTimeTag);

                        if (!Directory.Exists(destinationFolderPath))
                        {
                            Directory.CreateDirectory(destinationFolderPath);
                        }

                        foreach (string filePath in Directory.GetFiles(targetFolderPath))
                        {
                            string fileName = Path.GetFileName(filePath);
                            string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                            File.Move(filePath, destinationFilePath);
                        }

                        // Step 16: Confirm successful move and cleanup
                        if (Directory.GetFiles(targetFolderPath).Length == 0)
                        {
                            Console.WriteLine($"Log: {currentDateTimeTag}");
                            Directory.Delete(targetFolderPath);
                        }
                        else
                        {
                            Console.WriteLine("Error: Unable to move contents to the destination folder.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: config.json not found in the specified folder.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: main.bat not found in the specified folder.");
                }
            }
            else
            {
                Console.WriteLine("Error: The specified folder is empty. There's nothing to copy.");
            }
        }
        else
        {
            Console.WriteLine("Error: Specified folder path does not exist.");
        }
    }
}
