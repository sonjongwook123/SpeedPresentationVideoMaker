using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VideoMakerTool
{
    public partial class VideoEditorControl : UserControl
    {
        private ListBox listParts;
        private Button btnAddVideo, btnRemoveVideo, btnEditVideo;
        public List<VideoPart> Parts { get; private set; } = new List<VideoPart>();
        private int selectedIndex = -1;

        public VideoEditorControl()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.BackColor = Color.White;
            listParts = new ListBox { Location = new Point(30, 30), Size = new Size(300, 500) };
            listParts.SelectedIndexChanged += ListParts_SelectedIndexChanged;

            btnAddVideo = new Button { Text = "영상 추가", Location = new Point(350, 30) };
            btnAddVideo.Click += BtnAddVideo_Click;

            btnRemoveVideo = new Button { Text = "영상 제거", Location = new Point(350, 80), Enabled = false };
            btnRemoveVideo.Click += BtnRemoveVideo_Click;

            btnEditVideo = new Button { Text = "편집", Location = new Point(350, 130), Enabled = false };
            btnEditVideo.Click += BtnEditVideo_Click;

            this.Controls.Add(listParts);
            this.Controls.Add(btnAddVideo);
            this.Controls.Add(btnRemoveVideo);
            this.Controls.Add(btnEditVideo);
        }

        private void ListParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listParts.SelectedIndex;
            btnRemoveVideo.Enabled = selectedIndex >= 0;
            btnEditVideo.Enabled = selectedIndex >= 0;
        }

        private void BtnAddVideo_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Video Files|*.mp4;*.mov;*.avi" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var vp = new VideoPart
                    {
                        FilePath = ofd.FileName,
                        StartTime = TimeSpan.Zero,
                        EndTime = TimeSpan.FromSeconds(10),
                        Duration = TimeSpan.FromSeconds(10),
                        Title = Path.GetFileNameWithoutExtension(ofd.FileName),
                    };
                    Parts.Add(vp);
                    listParts.Items.Add(vp.Title);
                }
        }

        private void BtnRemoveVideo_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < Parts.Count)
            {
                Parts.RemoveAt(selectedIndex);
                listParts.Items.RemoveAt(selectedIndex);
                selectedIndex = -1;
                btnRemoveVideo.Enabled = false;
                btnEditVideo.Enabled = false;
            }
        }

        private void BtnEditVideo_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < Parts.Count)
            {
                var partToEdit = Parts.ElementAt(selectedIndex);
                using (var dialog = new VideoPartEditForm(partToEdit))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        listParts.Items[selectedIndex] = partToEdit.Title;
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            // Designer generated code
        }
    }
}