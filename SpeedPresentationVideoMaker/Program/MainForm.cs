using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace VideoMakerTool
{
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private TitleEditorControl titleEditor;
        private VideoEditorControl videoEditor;
        private FinalEditorControl finalEditor;
        private ImageBlockEditorControl imageEditor;
        private PdfBlockEditorControl pdfEditor;

        public MainForm()
        {
            InitializeComponent();
            InitUI();
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
        }

        private void InitUI()
        {
            this.Text = "발표 영상 제작 툴";
            this.BackColor = Color.White;
            this.Size = new Size(1200, 800);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(tabControl);

            titleEditor = new TitleEditorControl();
            videoEditor = new VideoEditorControl();
            imageEditor = new ImageBlockEditorControl();
            pdfEditor = new PdfBlockEditorControl();
            finalEditor = new FinalEditorControl();

            tabControl.TabPages.Add(CreatePage("타이틀 만들기", titleEditor));
            tabControl.TabPages.Add(CreatePage("영상", videoEditor));
            tabControl.TabPages.Add(CreatePage("이미지", imageEditor));
            tabControl.TabPages.Add(CreatePage("PDF", pdfEditor));
            tabControl.TabPages.Add(CreatePage("최종 편집", finalEditor));
        }

        private TabPage CreatePage(string title, Control content)
        {
            var page = new TabPage(title);
            page.Controls.Add(content);
            content.Dock = DockStyle.Fill;
            return page;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Text == "최종 편집")
            {
                UpdateBlocks();
            }
        }

        private void UpdateBlocks()
        {
            var allBlocks = new List<Block>();
            allBlocks.AddRange(videoEditor.Parts);
            allBlocks.AddRange(imageEditor.ImageBlocks);
            allBlocks.AddRange(pdfEditor.PdfBlocks);
            finalEditor.SetBlocks(allBlocks, titleEditor.GetTitleBlock());
        }

        private void InitializeComponent()
        {
            // Designer generated code
        }
    }
}