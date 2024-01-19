using AppHealthCheck.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace AppHealthCheck
{
    public partial class Form1 : Form
    {
        AppConfigurationDataModel appConfigurationData;
        private BackgroundWorker backgroundWorker;

        public Form1()
        {
            InitializeComponent();

            //backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += BackgroundWorker_DoWork;
            //backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
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

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            PrepareAndExecuteHealthCheck();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Disable the button while processing
            btnExecute.Enabled = false;

            // Show the spinner or progress bar
            progressBar1.Visible = true;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Hide the spinner or progress bar
            progressBar1.Visible = false;

            // Enable the button after processing
            btnExecute.Enabled = true;

            if (appConfigurationData.SupportTeam.Notify)
            {
                SendMailToSupportTeam();
            }
        }

        private async void PrepareAndExecuteHealthCheck()
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
            string collectionPath = $"{Path.Combine(folderPath, "configs")}\\{application}.postman_collection.json";
            if (!CheckFileExistance("Postman Collection file path", collectionPath)) return;

            // Specify the path to the environment file 
            string environmentPath = $"{Path.Combine(folderPath, "configs")}\\{application}.{environment}.postman_environment.json";
            if (!CheckFileExistance("Postman Environment file path", environmentPath)) return;

            // Specify the path to the JSON file to capture results
            string outputPath = $"{Path.Combine(folderPath, "results")}\\{application}.{environment}.results_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.json";
            UpdateResults($"outputPath: {outputPath}");

            // Build the command to run Newman with the specified reporters
            string command = $"\"{nodePath}\" \"{newmanScriptPath}\" run \"{collectionPath}\"";

            //string targetRequest = "04.GetAssignedTravelers";

            //// Optionally, include the target request within the folder
            //if (!string.IsNullOrEmpty(targetRequest))
            //{
            //    command += $" --request \"{targetRequest}\"";
            //}

            command += $" --environment \"{environmentPath}\" --reporters json --reporter-json-export \"{outputPath}\"";

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
            
            // Start the background worker
            //backgroundWorker.RunWorkerAsync();

            var results = await ExecuteHealthCheck(outputPath, psi);

            UpdateResults($"Execution Completed...");

            if (results.Equals("Success"))
            {
                UpdateResults("Newman execution successful.");
                if (!CheckFileExistance("Postman response file path", outputPath)) return;

                var postmanResponse = LoadJsonFile<PostmanResponseModel>(outputPath);
                //UpdateResults(JsonConvert.SerializeObject(postmanResponse));

                string emailBody = BuildEmailBody(postmanResponse);
                //UpdateResults(emailBody);

                string emailSubject = $"{cbApplication.Text.Split("|")[0].Trim().ToUpper()} : [{cbEnvironment.Text.ToUpper()}] | Health Check - {DateTime.Now.ToString("yyyy/MM/dd")}";
                string recipientToList = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.MailRecepients.ToList).FirstOrDefault();
                string recipientCCList = appConfigurationData.AppConfigurations.Where(x => x.AppName == cbApplication.Text).Select(x => x.MailRecepients.CCList).FirstOrDefault();
                SendEmail(emailSubject, emailBody, recipientToList, recipientCCList);
            }
            else
            {
                UpdateResults(results, true);
            }
        }

        private void SendMailToSupportTeam()
        {
            string emailSubject = $"{cbApplication.Text.Split("|")[0].Trim().ToUpper()} : [{cbEnvironment.Text.ToUpper()}] | Health Check Support Update - {DateTime.Now.ToString("yyyy/MM/dd")}";
            string recipientToList = appConfigurationData.SupportTeam.Email;
            SendEmail(emailSubject, rtbResults.Text, recipientToList, string.Empty);
        }

        private async Task<string> ExecuteHealthCheck(string outputPath, ProcessStartInfo psi)
        {
            string result = string.Empty;
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
                        result = "Success";
                    }
                    else
                    {
                        result = "Newman execution failed.";
                    }
                }
                catch (System.Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
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
                catch (System.Exception ex)
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
            catch (System.Exception ex)
            {
                UpdateResults($"Error loading JSON file: {ex.Message}");
                return default(T);
            }
        }

        private void LoadAppConfigurations()
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string appConfigurationPath = $"{Path.Combine(folderPath, "configs")}\\AppConfigurationData.json";
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
            sb.AppendLine("body { font-family: Verdana, Geneva, Tahoma, sans-serif; }");
            sb.AppendLine("table { border-collapse: collapse; width: 80%; margin-top: 10px; }");
            sb.AppendLine("th, td { border: 1px solid #dddddd; text-align: left; padding: 5px; }");
            sb.AppendLine("th { background-color: #f2f2f2; text-align: center; }");
            sb.AppendLine(".highlightMe { background-color: rgb(247, 186, 186); }");
            sb.AppendLine(".centerText { text-align: center; }");
            sb.AppendLine("span { font-size: .7em; line-height:1.2em; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<P> Hi There, Good day.. </p>");
            sb.AppendLine($"<P>Please find the results of the <b>{lblApplicationName.Text}</b> application health check which was verified on the <b>{cbEnvironment.Text}</b> environment.</P>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th width='2%'>S.NO</th>");
            sb.AppendLine("<th>Request Name With Details</th>");
            sb.AppendLine("<th width='10%'>Status</th>");
            sb.AppendLine("<th width='5%'>Duration</th>");
            sb.AppendLine("</tr>");

            var counter = 1;
            foreach (var execution in postmanResponse.Run.Executions.Skip(2))
            {
                var name = execution.Item.Name;
                var responseCode = execution.Response.Code;
                var responseSatus = "Success";
                var responseString = string.Empty;
                var responeTime = 0;
                var highlightMe = string.Empty;

                if (responseCode != 200 && responseCode != 201)
                {
                    highlightMe = "highlightMe";
                    responseSatus = "Failed";
                    byte[] byteArray = execution.Response.Stream.Data.Select(x => (byte)x).ToArray();
                    responseString = Encoding.ASCII.GetString(byteArray);
                }

                responeTime = execution.Response.ResponseTime;

                sb.AppendLine($"<tr class='{highlightMe}'>");
                sb.AppendLine($"<td class='centerText'>{counter++}</td>");
                sb.AppendLine($"<td>{name} {(string.IsNullOrEmpty(responseString) ? "" : $"<br /><span>{responseString}</span>")}</td>");
                sb.AppendLine($"<td class='centerText'>{responseSatus}</td>");
                sb.AppendLine($"<td class='centerText'>{responeTime} ms</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<br />");
            sb.AppendLine("<p>Regards,<br/>MedSol Team");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private void SendEmail(string emailSubject, string emailBody, string recipientToList, string recipientCCList)
        {
            Outlook.Application? outlookApp = new Outlook.Application();
            Outlook.MailItem? mailItem = outlookApp.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;

            try
            {
                // Set email properties
                mailItem.Subject = emailSubject;
                mailItem.HTMLBody = emailBody;
                mailItem.To = recipientToList;
                if (!string.IsNullOrEmpty(recipientCCList))
                {
                    mailItem.CC = recipientCCList;
                }

                if (chkSendEMail.Checked)
                {
                    // Send the email
                    mailItem.Send();
                    UpdateResults("Email sent successfully.");
                }
                else
                {
                    // Display the email rather than sending it out
                    mailItem.Display();
                    UpdateResults("Email articulated and displayed successfully.");
                }


            }
            catch (System.Exception ex)
            {
                // Handle exceptions as needed
                UpdateResults($"Error sending email: {ex.Message}");
            }
            finally
            {
                // Release the COM objects
                if (mailItem != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
                    mailItem = null;
                }

                if (outlookApp != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
                    outlookApp = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
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