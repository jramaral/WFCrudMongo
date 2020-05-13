using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace WFCrudMongo
{
    public partial class Form1 : Form
    {
        private byte[] iBytes;
        private string nfile;
        private ObjectId idDocumento;
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            //salvar os dados 
            
            

            IMongoClient cliente = new MongoClient("mongodb://localhost");
            IMongoDatabase database = cliente.GetDatabase("cad_prodb");

            IMongoCollection<Produto> colProds = database.GetCollection<Produto>("produto");

            Produto prod = new Produto();
            try
            {
                prod.NomeProduto = txt_nome_produto.Text;
                prod.Descricao = txt_descricao.Text;
                prod.DataCadastro = Convert.ToDateTime(txt_data.Text);
                prod.Ativo = txt_ativo.Checked;
                prod.Valor = Convert.ToDouble(txt_valor.Text);
                prod.Posicao = Convert.ToInt16(txt_posicao.Text);
                prod.DocumentoIncorporado = idDocumento;

                colProds.InsertOne(prod);

                atualizarGrid();



            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
           
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
            DestroyHandle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            atualizarGrid();

        }

        public void atualizarGrid()
        {
            try
            {
                var dao = new Produto();
                dataGridView1.DataSource = Listar("");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

            }
        }

        public IList<Produto> Listar(string txt)
        {
            

            IMongoClient cliente = new MongoClient("mongodb://localhost");
            IMongoDatabase database = cliente.GetDatabase("cad_prodb");

            IMongoCollection<Produto> colProds = database.GetCollection<Produto>("produto");

            Expression<Func<Produto, bool>> filter = x => x.NomeProduto.Contains("");
            IList<Produto> items = colProds.Find(filter).ToList();

            return items;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            dlg.Title = "Selecione uma imagem";

            dlg.ShowDialog();

            if (!string.IsNullOrEmpty(dlg.FileName))
            {

                pictureBox1.BackgroundImage = null;
                pictureBox1.Load(dlg.FileName);

                var stream = new MemoryStream();
                pictureBox1.Image.Save(stream, ImageFormat.Jpeg);

               nfile = dlg.FileName;

               IMongoClient cliente = new MongoClient("mongodb://localhost");
               IMongoDatabase database = cliente.GetDatabase("cad_prodb");

               var fs = new GridFSBucket(database);

               idDocumento = UploadFile(fs, stream);

             // DownloadFile(fs, idDocumento);
           



            }
        }
        private  ObjectId UploadFile(GridFSBucket fs, Stream stream)
        {
                var file = File.OpenRead(nfile);
                var t = Task.Run<ObjectId>(() => fs.UploadFromStreamAsync("test.txt", file));
                return t.Result;
            
        }
        private void DownloadFile(GridFSBucket fs, ObjectId id)
        {

            var x = fs.DownloadAsBytesAsync(id);
            Task.WaitAll(x);
            var bytes = x.Result;

            var stream = new MemoryStream(bytes);
            pictureBox2.Image = Image.FromStream(stream);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            ObjectId id = default;
            
            if (dataGridView1.CurrentRow != null)
            {
                id = (ObjectId) dataGridView1.CurrentRow.Cells["DocumentoIncorporado"].Value;
            }


            IMongoClient cliente = new MongoClient("mongodb://localhost");
            IMongoDatabase database = cliente.GetDatabase("cad_prodb");

            var fs = new GridFSBucket(database);

            DownloadFile(fs, id);
        }
    }
}
