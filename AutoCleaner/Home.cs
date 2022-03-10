using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using static AutoCleaner.KnownFolders;

namespace AutoCleaner
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();

            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == "WpnUserService_75cfc")
                {
                    //service.Stop();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ((PictureBox)sender).SendToBack();

            if (checkBox1.Checked)
            {
                //Clear Temp files/folders
                string tempPath = Path.GetTempPath();
                String tempFolder = Environment.ExpandEnvironmentVariables("%TEMP%");
                CleanFilesAndDirs(tempPath);
                checkBox1.ForeColor = Color.Green;
            }
            if(checkBox2.Checked)
            {
                //Empty Recycle Bin
                SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOPROGRESSUI | RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION);
                checkBox2.ForeColor = Color.Green;
            }
            if(checkBox3.Checked)
            {
                //Clear Prefetch files
                String prefetch = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\Prefetch";
                CleanFilesAndDirs(prefetch);
                checkBox3.ForeColor = Color.Green;
            }
            if(checkBox4.Checked)
            {
                //Clear recent activities
                String recent = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\Recent";
                CleanFilesAndDirs(recent);
                checkBox4.ForeColor = Color.Green;
            }
            if(checkBox5.Checked)
            {
                //Clear windows events logs
                ClearWindowsEventLogs();
                checkBox5.ForeColor = Color.Green;
            }
            if(checkBox6.Checked)
            {
                //Clear downloads
                string downloadPath = GetPath(KnownFolder.Downloads);
                CleanFilesAndDirs(downloadPath);
                checkBox6.ForeColor = Color.Green;
            }
            if (checkBox7.Checked)
            {
                //Clear pictures
                string picturesPath = GetPath(KnownFolder.Pictures);
                CleanFilesAndDirs(picturesPath);
                checkBox7.ForeColor = Color.Green;
            }
            if (checkBox8.Checked)
            {
                //Clear videos
                string videosPath = GetPath(KnownFolder.Videos);
                CleanFilesAndDirs(videosPath);
                checkBox8.ForeColor = Color.Green;
            }
            if (checkBox9.Checked)
            {
                //Clear music
                string musicPath = GetPath(KnownFolder.Music);
                CleanFilesAndDirs(musicPath);
                checkBox9.ForeColor = Color.Green;
            }
            if (checkBox10.Checked)
            {
                //Clear links
                string linksPath = GetPath(KnownFolder.Links);
                CleanFilesAndDirs(linksPath);
                checkBox10.ForeColor = Color.Green;
            }
            if (checkBox11.Checked)
            {
                //Clear saved searches
                string searchesPath = GetPath(KnownFolder.SavedSearches);
                CleanFilesAndDirs(searchesPath);
                checkBox11.ForeColor = Color.Green;
            }
            if(checkBox12.Checked)
            {
                //Clear Favorites
                string favoritesPath = GetPath(KnownFolder.Favorites);
                CleanFilesAndDirs(favoritesPath);
                checkBox12.ForeColor = Color.Green;
            }
            if (checkBox13.Checked)
            {
                //Clear Documents
                string documentsPath = GetPath(KnownFolder.Documents);
                CleanFilesAndDirs(documentsPath);
                checkBox13.ForeColor = Color.Green;
            }
            if (checkBox14.Checked)
            {
                //Clear Contacts
                string contactsPath = GetPath(KnownFolder.Contacts);
                CleanFilesAndDirs(contactsPath);
                checkBox14.ForeColor = Color.Green;
            }

            ((PictureBox)sender).BringToFront();
        }


        enum RecycleFlag : int
        {
            SHERB_NOCONFIRMATION = 0x00000001,  // No confirmation, when emptying
            SHERB_NOPROGRESSUI = 0x00000001,    // No progress tracking window during the emptying of the recycle bin
            SHERB_NOSOUND = 0x00000004          // No sound when the emptying of the recycle bin is complete
        }
        [DllImport("Shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        
        private void ClearWindowsEventLogs()
        {
            //Open Event Viewer in windows 10 administrative event logs to view all windows logs
            EventLog[] logs = EventLog.GetEventLogs();
            foreach(EventLog log in logs)
            {
                int totalEntries = log.Entries.Count;
                string logName = log.LogDisplayName;
                log.Clear();
            }

            //Clear operational event logs
            EventLogQuery query = new EventLogQuery("SetUp", PathType.LogName);
            query.ReverseDirection = true; 
            EventLogReader reader = new EventLogReader(query);
            EventRecord eventRecord;
            //while ((eventRecord = reader.ReadEvent()) != null)
            {
                
            }
        }



        private void CleanFilesAndDirs(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            int dirCount = di.GetDirectories().Length;
            int filesCount = di.GetFiles().Length;
            //bool isReadable = CanRead(di.FullName);

            DirectoryInfo[] directories = di.GetDirectories();
            directories[13].Delete(true);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && CanRead(path + "\\" + file.Name))
                    {
                        string name = file.Name;
                        file.Delete();
                        Thread.Sleep(new Random().Next(1,5)*100);
                        label22.Text = name;
                        label22.Invalidate();
                        label22.Refresh();
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    if ((dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        string name = dir.Name;
                        dir.Delete();
                        Thread.Sleep(new Random().Next(1, 5) * 100);
                        label22.Text = name;
                        label22.Invalidate();
                        label22.Refresh();
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                
            }
        }
        
        public bool CanRead(string path)
        {
            try
            {
                var readAllow = false;
                var readDeny = false;
                var accessControlList = Directory.GetAccessControl(path);
                if (accessControlList == null)
                    return false;

                //get the access rules that pertain to a valid SID/NTAccount.
                var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                if (accessRules == null)
                    return false;

                //we want to go over these rules to ensure a valid SID has access
                foreach (System.Security.AccessControl.FileSystemAccessRule rule in accessRules)
                {
                    if ((System.Security.AccessControl.FileSystemRights.Read & rule.FileSystemRights) != System.Security.AccessControl.FileSystemRights.Read) continue;

                    if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Allow)
                        readAllow = true;
                    else if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Deny)
                        readDeny = true;
                }

                return readAllow && !readDeny;
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath shape = new GraphicsPath();
            //Point[] points = new Point[2];
            //points[0] = new Point(0, 0);
            //points[0] = new Point(10, 10);
               
            //shape.AddCurve(points);
            //shape.AddEllipse(-100, 0, this.Width, this.Height);
            
            //this.Region = new Region(shape);
            shape.Dispose();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Show(this, pictureBox1.Location.X + 3, pictureBox1.Location.Y+pictureBox1.Height - 5);   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
