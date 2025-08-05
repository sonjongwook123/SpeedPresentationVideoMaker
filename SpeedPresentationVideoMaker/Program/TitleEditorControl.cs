using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VideoMakerTool
{
    public partial class TitleEditorControl : UserControl
    {
        private PictureBox picLogo;
        private TextBox txtTeamName;
        private TextBox txtMembers;
        private ComboBox cmbTeamAnim, cmbMemberAnim;
        private Button btnPreview, btnLogo;

        private const string TeamNamePlaceholder = "팀 이름 입력";
        private const string MembersPlaceholder = "팀원 이름 입력 (쉼표 구분)";

        public TitleEditorControl()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.BackColor = Color.White;

            picLogo = new PictureBox { BorderStyle = BorderStyle.FixedSingle, Size = new Size(200, 200), Location = new Point(30, 30), BackColor = Color.LightGray };
            btnLogo = new Button { Text = "로고 선택", Location = new Point(30, 240) };
            btnLogo.Click += (s, e) => {
                using (var ofd = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg" })
                    if (ofd.ShowDialog() == DialogResult.OK) picLogo.Image = Image.FromFile(ofd.FileName);
            };

            txtTeamName = new TextBox { Text = TeamNamePlaceholder, ForeColor = Color.Gray, Location = new Point(260, 30), Width = 300 };
            txtTeamName.GotFocus += (s, e) => { if (txtTeamName.Text == TeamNamePlaceholder) { txtTeamName.Text = ""; txtTeamName.ForeColor = Color.Black; } };
            txtTeamName.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtTeamName.Text)) { txtTeamName.Text = TeamNamePlaceholder; txtTeamName.ForeColor = Color.Gray; } };

            cmbTeamAnim = new ComboBox { Location = new Point(260, 80), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTeamAnim.Items.AddRange(new[] { "FadeIn", "ZoomIn" });
            cmbTeamAnim.SelectedIndex = 1;

            txtMembers = new TextBox { Text = MembersPlaceholder, ForeColor = Color.Gray, Location = new Point(260, 150), Width = 300 };
            txtMembers.GotFocus += (s, e) => { if (txtMembers.Text == MembersPlaceholder) { txtMembers.Text = ""; txtMembers.ForeColor = Color.Black; } };
            txtMembers.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtMembers.Text)) { txtMembers.Text = MembersPlaceholder; txtMembers.ForeColor = Color.Gray; } };

            cmbMemberAnim = new ComboBox { Location = new Point(260, 200), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMemberAnim.Items.AddRange(new[] { "FadeIn", "ZoomIn" });
            cmbMemberAnim.SelectedIndex = 0;

            btnPreview = new Button { Text = "프리뷰 재생", Location = new Point(260, 260) };
            btnPreview.Click += BtnPreview_Click;

            this.Controls.Add(picLogo);
            this.Controls.Add(btnLogo);
            this.Controls.Add(txtTeamName);
            this.Controls.Add(cmbTeamAnim);
            this.Controls.Add(txtMembers);
            this.Controls.Add(cmbMemberAnim);
            this.Controls.Add(btnPreview);
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            var titleBlock = GetTitleBlock();

            picLogo.Visible = picLogo.Image != null;

            if (picLogo.Image != null)
            {
                AnimationHelper.Animate(picLogo, AnimationType.FadeIn);
            }
        }

        public TitleBlock GetTitleBlock()
        {
            var titleBlock = new TitleBlock
            {
                LogoPath = picLogo.ImageLocation,
                TeamName = txtTeamName.Text != TeamNamePlaceholder ? txtTeamName.Text : string.Empty,
                TeamMembers = txtMembers.Text != MembersPlaceholder ? txtMembers.Text : string.Empty,
                TeamNameAnimation = (AnimationType)Enum.Parse(typeof(AnimationType), cmbTeamAnim.SelectedItem.ToString()),
                MembersAnimation = (AnimationType)Enum.Parse(typeof(AnimationType), cmbMemberAnim.SelectedItem.ToString()),
                TeamNameDuration = TimeSpan.FromSeconds(3),
                MembersDuration = TimeSpan.FromSeconds(3)
            };
            return titleBlock;
        }

        private void InitializeComponent()
        {
            // Designer generated code
        }
    }
}