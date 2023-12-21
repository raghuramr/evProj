using AppSmokeTesting.Models;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace AppSmokeTesting
{
    public partial class Form1 : Form
    {
        AppConfigurationDataModel appConfigurationData;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EnvironmentValidations();
            LoadAppConfigurations();
            appConfigurationData.AppConfigurations.ForEach(config =>
            {
                cbApplication.Items.Add(config.AppName);
            });
        }

        private void cbApplication_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblApplicationName.Text = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.ApplicationName).FirstOrDefault();
            cbEnvironment.Items.Clear();
            var environments = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.Environments).ToList();
            environments.ForEach(cbEnvironment.Items.AddRange);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (cbApplication.SelectedIndex == -1)
            {
                UpdateResults("Please select the application, to proceed", true);
                return;
            }

            if (cbEnvironment.SelectedIndex == -1)
            {
                UpdateResults("Please select the environment, to proceed", true);
                return;
            }

            var application = cbApplication.Text;
            var environment = cbEnvironment.Text;
            var fileCheck = false;

            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Specify the path to the Node.js executable
            string nodePath = @"C:\Program Files\nodejs\node.exe";

            // Specify the path to the Newman script
            string newmanScriptPath = @"C:\Users\rraichooti\AppData\Roaming\npm\node_modules\newman\bin\newman.js";

            // Specify the path to your Postman collection file
            string collectionPath = $"{(Path.Combine(folderPath, "configs"))}\\{application}.postman_collection.json";
            if (!CheckFileExistance("Postman Collection file path", collectionPath)) return;

            // Specify the path to the environment file 
            string environmentPath = $"{(Path.Combine(folderPath, "configs"))}\\{application}.{environment}.postman_environment.json";
            if (!CheckFileExistance("Postman Environment file path", environmentPath)) return;

            // Specify the path to the JSON file to capture results
            string outputPath = $"{(Path.Combine(folderPath, "results"))}\\{application}.{environment}.results_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.json";
            UpdateResults($"outputPath: {outputPath}");

            // Build the command to run Newman with the specified reporters
            string command = $"\"{nodePath}\" \"{newmanScriptPath}\" run \"{collectionPath}\" --environment \"{environmentPath}\" --reporters json --reporter-json-export \"{outputPath}\"";

            // Start the process
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/C \"{command}\""
            };

            UpdateResults($"Execution Started...");

            using (var process = new Process { StartInfo = psi })
            {
                try
                {
                    process.Start();

                    // Read the output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Display the output and error
                    UpdateResults("Output:");
                    UpdateResults(output);

                    UpdateResults("Error:");
                    UpdateResults(error);

                    // Check the exit code
                    int exitCode = process.ExitCode;
                    UpdateResults($"Exit Code: {exitCode}");

                    if (exitCode == 0)
                    {
                        UpdateResults("Newman execution successful.");
                        if (!CheckFileExistance("Postman response file path", outputPath)) return;

                        var postmanResponse = LoadJsonFile<PostmanResponseModel>(outputPath);
                        UpdateResults(JsonConvert.SerializeObject(postmanResponse));

                        var emailBody = BuildEmailBody(postmanResponse);
                        UpdateResults(emailBody);
                        SendEmail(emailBody);
                    }
                    else
                    {
                        UpdateResults("Newman execution failed.");
                    }
                }
                catch (Exception ex)
                {
                    UpdateResults(ex.Message);
                }
            }
        }

        private bool CheckFileExistance(string typeOfFile, string filePathToCheck)
        {
            var result = true;

            if (!File.Exists(filePathToCheck))
            {
                UpdateResults($"Unable to find the specified configuration file: {filePathToCheck}", true);
                result = false;
            }
            else
            {
                UpdateResults($"{typeOfFile}: {filePathToCheck}");
            }

            return result;
        }

        private void EnvironmentValidations()
        {
            if (IsNodeInstalled())
            {
                UpdateResults("Node.js is installed on the system.");
            }
            else
            {
                UpdateResults("Node.js is not installed on the system.");
                UpdateResults("Do you want to install Node.js? (y/n)");

                string response = Console.ReadLine();
                if (response.ToLower() == "y")
                {
                    InstallNode();
                }
            }
        }

        private static bool IsNodeInstalled()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "node",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "--version"
            };

            using (Process process = new Process { StartInfo = psi })
            {
                try
                {
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        static void InstallNode()
        {
            // Provide the code to install Node.js based on the user's operating system
            // This could involve downloading the installer and running it
            Console.WriteLine("Node.js installation process goes here.");
        }

        private void UpdateResults(string message, bool highlight = false)
        {
            if (highlight)
            {
                rtbResults.SelectionStart = rtbResults.Text.Length;
                rtbResults.SelectionLength = message.Length;
                rtbResults.SelectionColor = Color.Red;
            }
            else
            {
                rtbResults.SelectionStart = rtbResults.Text.Length;
                rtbResults.SelectionLength = message.Length;
                rtbResults.SelectionColor = Color.Black;
            }

            rtbResults.AppendText($"{DateTime.UtcNow} - {message}\n");
        }

        private T LoadJsonFile<T>(string filePath)
        {
            try
            {
                // Read the entire file content
                string jsonContent = File.ReadAllText(filePath);

                // Deserialize the JSON into the specified type
                T result = JsonConvert.DeserializeObject<T>(jsonContent);

                return result;
            }
            catch (Exception ex)
            {
                UpdateResults($"Error loading JSON file: {ex.Message}");
                return default(T);
            }
        }

        private void LoadAppConfigurations()
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string appConfigurationPath = $"{(Path.Combine(folderPath, "configs"))}\\AppConfigurationData.json";
            if (!CheckFileExistance("Application Configuration file path", appConfigurationPath)) return;

            appConfigurationData = LoadJsonFile<AppConfigurationDataModel>(appConfigurationPath);
        }

        private string BuildEmailBody(PostmanResponseModel postmanResponse)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("table {");
            sb.AppendLine("font-family: Arial, sans-serif;");
            sb.AppendLine("border-collapse: collapse;");
            sb.AppendLine("width: 50%;");
            sb.AppendLine("margin-top: 20px;");
            sb.AppendLine("}");
            sb.AppendLine("th, td {");
            sb.AppendLine("border: 1px solid #dddddd;");
            sb.AppendLine("text-align: left;");
            sb.AppendLine("padding: 8px;");
            sb.AppendLine("}");
            sb.AppendLine("th {");
            sb.AppendLine("background-color: #f2f2f2;");
            sb.AppendLine("}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>S.NO</th>");
            sb.AppendLine("<th>Request Name</th>");
            sb.AppendLine("<th>Status</th>");
            sb.AppendLine("</tr>");

            var counter = 1;
            foreach (var execution in postmanResponse.Run.Executions)
            {
                var name = execution.Item.Name;
                var responseCode = execution.Response.Code;
                var responseSatus = execution.Response.Status;
                var responseString = string.Empty;

                if (responseCode != 200 && responseCode != 201)
                {
                    byte[] byteArray = execution.Response.Stream.Data.Select(x => (byte)x).ToArray();
                    responseString = Encoding.ASCII.GetString(byteArray);
                }

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{counter++}</td>");
                sb.AppendLine($"<td>{name}</td>");
                sb.AppendLine($"<td>{responseCode} | {responseSatus} | {responseString} </td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private void SendEmail(string emailBody)
        {
            string recipientToList = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.MailRecepients.ToList).FirstOrDefault();
            string recipientCCList = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.MailRecepients.ToList).FirstOrDefault();
            string subject = $"{cbApplication.Text.Split("|")[0].Trim().ToUpper()} : [{cbEnvironment.Text.ToUpper()}] | Smoke test - {DateTime.Now.ToString("yyyy/MM/dd")}";
            string body = emailBody;

            // Create an Outlook application instance
            Outlook.Application outlookApp = new();

            // Create a new mail item
            Outlook.MailItem mailItem = outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            // Set email properties
            mailItem.Subject = subject;
            mailItem.HTMLBody = body;
            mailItem.To = recipientToList;
            mailItem.CC = recipientCCList;

            if (chkSendEMail.Checked)
            {
                // Send the email
                mailItem.Send();
            }
            else
            {
                // display the email rather than sending it out
                mailItem.Display();
            }

            // Release the COM objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);

            UpdateResults("Email sent successfully.");
        }

        private void chkSendEMail_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSendEMail.Checked)
            {
                chkSendEMail.Text = "Email will be sent directly";
            }
            else
            {
                chkSendEMail.Text = "Composed mail will be display on screen";
            }
        }
    }
}