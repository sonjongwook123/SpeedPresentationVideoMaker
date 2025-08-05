using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace VideoMakerTool
{
    public abstract class Block
    {
        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public List<Subtitle> Subtitles { get; set; } = new List<Subtitle>();
    }

    public class TitleBlock : Block
    {
        public string LogoPath { get; set; }
        public string TeamName { get; set; }
        public string TeamMembers { get; set; }
        public TimeSpan TeamNameDuration { get; set; }
        public TimeSpan MembersDuration { get; set; }
        public AnimationType TeamNameAnimation { get; set; }
        public AnimationType MembersAnimation { get; set; }
    }

    public class VideoPart : Block
    {
        public string FilePath { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class ImageBlock : Block
    {
        public string ImagePath { get; set; }
    }

    public class PdfBlock : Block
    {
        public string PdfPath { get; set; }
        public int PageNumber { get; set; }
        public string PdfImagePath { get; set; }
    }

    public class Subtitle
    {
        public string Text { get; set; }
        public TimeSpan StartTime { get; set; }
    }

    public class MusicTrack
    {
        public string FilePath { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public partial class BlockEditForm : Form
    {
        private Block _block;
        private Label lblTitle, lblDuration;
        private TextBox txtTitle;
        private NumericUpDown numDuration;
        private Button btnOk, btnCancel;

        public BlockEditForm(Block block)
        {
            _block = block;
            InitializeComponents();
            txtTitle.Text = _block.Title;
            numDuration.Value = (decimal)_block.Duration.TotalSeconds;
        }

        private void InitializeComponents()
        {
            this.Text = "블록 편집";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 200;

            lblTitle = new Label { Text = "제목:", Location = new Point(20, 20), Width = 100 };
            txtTitle = new TextBox { Location = new Point(120, 20), Width = 150 };

            lblDuration = new Label { Text = "길이 (초):", Location = new Point(20, 60), Width = 100 };
            numDuration = new NumericUpDown { Location = new Point(120, 60), Width = 150, Minimum = 1, Maximum = 60 };

            btnOk = new Button { Text = "확인", Location = new Point(40, 120), Width = 100 };
            btnOk.Click += (s, e) =>
            {
                _block.Title = txtTitle.Text;
                _block.Duration = TimeSpan.FromSeconds((double)numDuration.Value);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancel = new Button { Text = "취소", Location = new Point(150, 120), Width = 100 };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblDuration);
            this.Controls.Add(numDuration);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }
    }

    public partial class VideoPartEditForm : Form
    {
        private VideoPart _part;
        private Label lblFile, lblStart, lblEnd, lblTitle;
        private TextBox txtStart, txtEnd, txtTitle;
        private Button btnOk, btnCancel;

        public VideoPartEditForm(VideoPart part)
        {
            _part = part;
            InitializeComponents();
            txtTitle.Text = _part.Title;
            txtStart.Text = _part.StartTime.ToString(@"hh\:mm\:ss");
            txtEnd.Text = _part.EndTime.ToString(@"hh\:mm\:ss");
        }

        private void InitializeComponents()
        {
            this.Text = "영상 편집";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 250;
            lblTitle = new Label { Text = "제목:", Location = new Point(20, 20), Width = 100 };
            txtTitle = new TextBox { Location = new Point(120, 20), Width = 150 };

            lblFile = new Label { Text = "파일:", Location = new Point(20, 50), Width = 100 };
            Label lblFilePath = new Label { Text = System.IO.Path.GetFileName(_part.FilePath), Location = new Point(120, 50), Width = 150 };

            lblStart = new Label { Text = "시작 시간:", Location = new Point(20, 80), Width = 100 };
            txtStart = new TextBox { Location = new Point(120, 80), Width = 150 };

            lblEnd = new Label { Text = "종료 시간:", Location = new Point(20, 110), Width = 100 };
            txtEnd = new TextBox { Location = new Point(120, 110), Width = 150 };

            btnOk = new Button { Text = "확인", Location = new Point(40, 160), Width = 100 };
            btnOk.Click += (s, e) => {
                if (TimeSpan.TryParse(txtStart.Text, out TimeSpan start) && TimeSpan.TryParse(txtEnd.Text, out TimeSpan end))
                {
                    _part.Title = txtTitle.Text;
                    _part.StartTime = start;
                    _part.EndTime = end;
                    _part.Duration = end - start;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("시간 형식이 올바르지 않습니다. (hh:mm:ss)", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            };

            btnCancel = new Button { Text = "취소", Location = new Point(150, 160), Width = 100 };
            btnCancel.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblFile);
            this.Controls.Add(lblFilePath);
            this.Controls.Add(lblStart);
            this.Controls.Add(txtStart);
            this.Controls.Add(lblEnd);
            this.Controls.Add(txtEnd);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }
    }

    // PDF 페이지 번호를 입력받기 위한 폼을 새로 추가합니다.
    public class PageNumberInputForm : Form
    {
        public int PageNumber { get; private set; } = 1;
        private NumericUpDown numPage;
        private Button btnOk, btnCancel;

        public PageNumberInputForm()
        {
            this.Text = "페이지 번호 입력";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 150;

            var lbl = new Label { Text = "페이지 번호를 입력하세요:", Location = new Point(20, 20), Width = 250 };
            numPage = new NumericUpDown { Location = new Point(20, 50), Width = 250, Minimum = 1, Maximum = 9999 };

            btnOk = new Button { Text = "확인", Location = new Point(40, 80), Width = 100 };
            btnOk.Click += (s, e) =>
            {
                PageNumber = (int)numPage.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancel = new Button { Text = "취소", Location = new Point(150, 80), Width = 100 };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lbl);
            this.Controls.Add(numPage);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }
    }
}