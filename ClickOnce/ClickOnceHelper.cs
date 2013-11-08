using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ScreenGrab.ClickOnce {
    /*
     * Click once is not enabled. Need to set the publish properties and re-enable the check for updates menu option
     * */
    public class ClickOnceHelper {

        public static event UpdateCompleteEventHandler Finished;

        private static Dispatcher dispatcher = null;

        public static void CheckForUpdates() {
            _logMessages = new List<string>();
            _logMessages.Add("CheckForUpdates");
            dispatcher = Dispatcher.CurrentDispatcher;
            Thread t = new Thread(DoTask);
            t.Name = "Upate Check Thread";
            t.IsBackground = true;
            t.Start();
        }

        public static bool CanCheckForUpdates() {
            return ApplicationDeployment.IsNetworkDeployed;
        }

        static void RaiseFinishedTask(bool restart) {
            var action = new Action<object, UpdateCompleteEventArgs>(Finished);
            dispatcher.Invoke(DispatcherPriority.Normal, action, null, new UpdateCompleteEventArgs(restart));
        }

        public static string CurrentVersion {
            get {
                if (ApplicationDeployment.IsNetworkDeployed) {
                    var v = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    return string.Format("v{0}.{1}.{2}", v.Major, v.Minor, v.Revision);
                }
                else {
                    return " [Not Deployed]";
                }
            }
        }

        static void DoTask() {

            bool restart = false;
            _logMessages.Add("DoTask_Start");
            UpdateCheckInfo info = null;

            try {
                if (ApplicationDeployment.IsNetworkDeployed) {
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                    
                    try {
                        info = ad.CheckForDetailedUpdate();
                    } catch (DeploymentDownloadException dde) {
                        _logMessages.Add("DeploymentDownloadException " + dde.Message);
                        MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    } catch (InvalidDeploymentException ide) {
                        _logMessages.Add("InvalidDeploymentException " + ide.Message);
                        MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    } catch (InvalidOperationException ioe) {
                        _logMessages.Add("InvalidOperationException " + ioe.Message);
                        MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    }

                    if (info != null && info.UpdateAvailable) {
                        Boolean doUpdate = true;
                        if (!info.IsUpdateRequired) {
                            var dr = ShowYesNoMessage("An update is available. Would you like to update the application now? You will be prompted to restart the application once it is updated");
                            doUpdate = (dr == MessageBoxResult.Yes);
                        } else {
                            _logMessages.Add("Mandatory update available");
                            // Display a message that the app MUST reboot. Display the minimum required version.
                            MessageBox.Show("This application has detected a mandatory update from your current " +
                                            "version to version " + info.MinimumRequiredVersion +
                                            ". The application will now install the update and restart.",
                                            "Update Available", MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                        }

                        _logMessages.Add("doUpdate: " + doUpdate);

                        if (doUpdate) {
                            try {
                                _logMessages.Add("Updating....");
                                ad.Update();
                                restart = true;
                                //var dr = ShowYesNoMessage("The application has been upgraded. You need to restart to use the updated version. Restart now?");
                                //if (dr == MessageBoxResult.Yes) {
                                    
                                //System.Windows.Forms.Application.Restart();
                                //Application.Current.Shutdown();
                                //}
                            } catch (DeploymentDownloadException dde) {
                                _logMessages.Add("DeploymentDownloadException " + dde.Message);
                                MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            }
                        }

                    } else {
                        _logMessages.Add("No updates...");
                        MessageBox.Show("No updates available at this time", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            } catch (Exception ex) {
                _logMessages.Add("EXCEPTION: " + ex.Message);
            } finally {
                _logMessages.Add(" ***** DONE *****");
                File.WriteAllLines(LogFile, _logMessages);
                RaiseFinishedTask(restart);
            }

        }

        private static string _logFile;
        static string LogFile {
            get {
                if (string.IsNullOrWhiteSpace(_logFile)) {
                    _logFile = Path.GetTempFileName();
                }
                return _logFile;
            }
        }

        static MessageBoxResult ShowYesNoMessage(string msg) {
            _logMessages.Add(msg);
            var res = MessageBox.Show(msg, "Updater", MessageBoxButton.YesNo, MessageBoxImage.Question);
            _logMessages.Add("User Clicked: " + res);
            return res;
        }

        private static List<string> _logMessages;//= new List<string>();

        static void Log(string msg) {
            //File.WriteAllText();
        }
    }
}