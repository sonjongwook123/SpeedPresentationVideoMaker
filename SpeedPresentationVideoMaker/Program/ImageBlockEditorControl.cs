using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace VideoMakerTool
{
    public partial class ImageBlockEditorControl : UserControl
    {
        private ListBox listImages;
        private Button btnAddImage, btnRemoveImage, btnEditImage;
        public List<ImageBlock> ImageBlocks { get; private set; } = new List<ImageBlock>();
        private int selectedIndex = -1;

        public ImageBlockEditorControl()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.BackColor = Color.White;
            listImages = new ListBox { Location = new Point(30, 30), Size = new Size(300, 500) };
            listImages.SelectedIndexChanged += ListImages_SelectedIndexChanged;

            btnAddImage = new Button { Text = "이미지 추가", Location = new Point(350, 30) };
            btnAddImage.Click += BtnAddImage_Click;

            btnRemoveImage = new Button { Text = "이미지 제거", Location = new Point(350, 80), Enabled = false };
            btnRemoveImage.Click += BtnRemoveImage_Click;

            btnEditImage = new Button { Text = "편집", Location = new Point(350, 130), Enabled = false };
            btnEditImage.Click += BtnEditImage_Click;

            this.Controls.Add(listImages);
            this.Controls.Add(btnAddImage);
            this.Controls.Add(btnRemoveImage);
            this.Controls.Add(btnEditImage);
        }

        private void ListImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listImages.SelectedIndex;
            btnRemoveImage.Enabled = selectedIndex >= 0;
            btnEditImage.Enabled = selectedIndex >= 0;
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var ib = new ImageBlock
                    {
                        ImagePath = ofd.FileName,
                        Duration = TimeSpan.FromSeconds(5),
                        Title = Path.GetFileNameWithoutExtension(ofd.FileName),
                    };
                    ImageBlocks.Add(ib);
                    listImages.Items.Add(ib.Title);
                }
        }

        private void BtnRemoveImage_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < ImageBlocks.Count)
            {
                ImageBlocks.RemoveAt(selectedIndex);
                listImages.Items.RemoveAt(selectedIndex);
                selectedIndex = -1;
                btnRemoveImage.Enabled = false;
                btnEditImage.Enabled = false;
            }
        }

        private void BtnEditImage_Click(object sender, EventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < ImageBlocks.Count)
            {
                var blockToEdit = ImageBlocks.ElementAt(selectedIndex);
                using (var dialog = new BlockEditForm(blockToEdit))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        listImages.Items[selectedIndex] = blockToEdit.Title;
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