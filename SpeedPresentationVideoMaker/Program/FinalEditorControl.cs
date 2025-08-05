using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VideoMakerTool
{
    public partial class FinalEditorControl : UserControl
    {
        private Button btnAddMusic, btnExport;
        private ListBox listBlocks, listMusic;
        private List<Block> allBlocks;
        private List<MusicTrack> musicTracks;
        private TitleBlock titleBlock;

        public FinalEditorControl()
        {
            InitializeComponent();
            allBlocks = new List<Block>();
            musicTracks = new List<MusicTrack>();
            titleBlock = new TitleBlock();
            InitUI();
        }

        private void InitUI()
        {
            this.BackColor = Color.White;
            listBlocks = new ListBox { Location = new System.Drawing.Point(30, 30), Size = new System.Drawing.Size(300, 500) };
            listMusic = new ListBox { Location = new System.Drawing.Point(350, 30), Size = new System.Drawing.Size(300, 500) };
            btnAddMusic = new Button { Text = "음악 추가", Location = new System.Drawing.Point(350, 540) };
            btnAddMusic.Click += BtnAddMusic_Click;
            btnExport = new Button { Text = "영상 뽑기", Location = new System.Drawing.Point(350, 600) };
            btnExport.Click += BtnExport_Click;

            Controls.Add(listBlocks);
            Controls.Add(listMusic);
            Controls.Add(btnAddMusic);
            Controls.Add(btnExport);
        }

        private void BtnAddMusic_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Audio Files|*.mp3;*.wav" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var track = new MusicTrack { FilePath = ofd.FileName, StartTime = TimeSpan.Zero, EndTime = TimeSpan.FromMinutes(3) };
                    musicTracks.Add(track);
                    listMusic.Items.Add(Path.GetFileName(track.FilePath));
                }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (allBlocks.Count == 0 && titleBlock == null)
            {
                MessageBox.Show("추가된 블록이 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "MP4 Video|*.mp4" })
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        RenderEngine.ExportFinalVideo(allBlocks, musicTracks, titleBlock, sfd.FileName);
                        MessageBox.Show("영상 내보내기 시작. FFmpeg 콘솔을 확인하세요.", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"영상 내보내기 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
        }

        public void SetBlocks(List<Block> blocks, TitleBlock titleBlock)
        {
            this.allBlocks = blocks;
            this.titleBlock = titleBlock;
            listBlocks.Items.Clear();
            if (titleBlock != null && !string.IsNullOrEmpty(titleBlock.TeamName))
            {
                listBlocks.Items.Add($"타이틀: {titleBlock.TeamName}");
            }
            foreach (var block in allBlocks)
            {
                listBlocks.Items.Add(block.Title);
            }
        }

        private void InitializeComponent()
        {
            // Designer generated code
        }
    }
}