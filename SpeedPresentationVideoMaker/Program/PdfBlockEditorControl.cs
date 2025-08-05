using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace VideoMakerTool
{
    public partial class PdfBlockEditorControl : UserControl
    {
        private ListBox listPdfs;
        private Button btnAddPdf, btnRemovePdf, btnEditPdf;
        public List<PdfBlock> PdfBlocks { get; private set; } = new List<PdfBlock>();
        private int selectedIndex = -1;

        public PdfBlockEditorControl()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.BackColor = Color.White;
            listPdfs = new ListBox { Location = new Point(30, 30), Size = new Size(300, 500) };
            listPdfs.SelectedIndexChanged += ListPdfs_SelectedIndexChanged;

            btnAddPdf = new Button { Text = "PDF 페이지 추가", Location = new Point(350, 30) };
            btnAddPdf.Click += BtnAddPdf_Click;

            btnRemovePdf = new Button { Text = "페이지 제거", Location = new Point(350, 80), Enabled = false };
            btnRemovePdf.Click += BtnRemovePdf_Click;

            btnEditPdf = new Button { Text = "편집", Location = new Point(350, 130), Enabled = false };
            btnEditPdf.Click += BtnEditPdf_Click;

            this.Controls.Add(listPdfs);
            this.Controls.Add(btnAddPdf);
            this.Controls.Add(btnRemovePdf);
            this.Controls.Add(btnEditPdf);
        }

        private void ListPdfs_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listPdfs.SelectedIndex;
            btnRemovePdf.Enabled = selectedIndex >= 0;
            btnEditPdf.Enabled = selectedIndex >= 0;
        }

        private void BtnAddPdf_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "PDF Files|*.pdf" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var inputForm = new PageNumberInputForm())
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK)
                        {
                            var pageNumber = inputForm.PageNumber;
                            var pb = new PdfBlock
                            {
                                PdfPath = ofd.FileName,
                                PageNumber = pageNumber,
                                Duration = TimeSpan.FromSeconds(5),
                                Title = $"{Path.GetFileNameWithoutExtension(ofd.FileName)} (Page {pageNumber})",
                            };
                            PdfBlocks.Add(pb);
                            listPdfs.Items.Add(pb.Title);
                        }
                    }
                }
        }

        private void BtnRemovePdf_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < PdfBlocks.Count)
            {
                PdfBlocks.RemoveAt(selectedIndex);
                listPdfs.Items.RemoveAt(selectedIndex);
                selectedIndex = -1;
                btnRemovePdf.Enabled = false;
                btnEditPdf.Enabled = false;
            }
        }

        private void BtnEditPdf_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < PdfBlocks.Count)
            {
                var blockToEdit = PdfBlocks.ElementAt(selectedIndex);
                using (var dialog = new BlockEditForm(blockToEdit))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        listPdfs.Items[selectedIndex] = blockToEdit.Title;
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