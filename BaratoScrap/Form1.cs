using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaratoScrap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public NpgsqlConnection conn = new NpgsqlConnection();
        private void button3_Click(object sender, EventArgs e)
        {
            List<producto> productos = new List<producto>();

            string filepath = textBox1.Text;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<string> urls = new List<string>();
            foreach (var file in d.GetFiles("*.html"))
            {
                urls.Add(file.FullName);
            }
            try
            {
                foreach (var file in d.GetFiles("*.html"))
                {
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.Load(file.FullName);
                        string filename = Path.GetFileNameWithoutExtension(file.FullName);
                        int parseName = 0;
                        bool descr_name = false;
                        if (!int.TryParse(filename, out parseName))
                        {
                            descr_name = true;
                        }
                        string img_ = "//div[contains(@class, 'swiper-slide')]";
                        int c0 = 0;
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes(img_))
                        {
                            producto producto = new producto();

                            string html0 = node.InnerHtml;
                            var doc1 = new HtmlAgilityPack.HtmlDocument();
                            doc1.LoadHtml(html0);

                            int c1 = 0;
                            HtmlNode node1 = node;
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//div"))
                            {
                                if (c1 == 0)
                                {
                                    int c2 = 0;
                                    string html1 = node2.InnerHtml;
                                    var doc2 = new HtmlAgilityPack.HtmlDocument();
                                    doc2.LoadHtml(html1);

                                    foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//div"))
                                    {
                                        if (c2 == 0)
                                        {
                                            string html2 = node3.InnerHtml;
                                            var doc3 = new HtmlAgilityPack.HtmlDocument();
                                            doc3.LoadHtml(html2);
                                            int c3 = 0;
                                            foreach (HtmlNode node4 in doc3.DocumentNode.SelectNodes("//div"))
                                            {
                                                if (c3 == 0)
                                                {
                                                    string html3 = node4.InnerHtml;
                                                    var doc4 = new HtmlAgilityPack.HtmlDocument();
                                                    doc4.LoadHtml(html3);
                                                    foreach (HtmlNode nodes in doc4.DocumentNode.SelectNodes("//img"))
                                                    {
                                                        string ddd = nodes.Attributes["src"].Value;
                                                        producto.imagen = ddd;
                                                    }
                                                }
                                                c3++;
                                            }
                                        }
                                        if (c2 == 1)
                                        {
                                            string html2 = node3.InnerHtml;
                                            var doc3 = new HtmlAgilityPack.HtmlDocument();
                                            doc3.LoadHtml(html2);
                                            int c3 = 0;
                                            foreach (HtmlNode nodes in doc3.DocumentNode.SelectNodes("//span"))
                                            {
                                                string ddd = nodes.InnerHtml;
                                                if (!string.IsNullOrEmpty(ddd))
                                                {
                                                    if (!ddd.ToLower().Contains("agregar"))
                                                    {
                                                        ddd = ddd.Replace("\t", "").Replace("\n", "");

                                                        if (!ddd.Contains("$"))
                                                        {
                                                            if (string.IsNullOrEmpty(producto.Nombre))
                                                            {
                                                                if (descr_name)
                                                                {
                                                                    producto.Nombre += filename + " ";
                                                                    producto.Nombre += ddd;
                                                                }
                                                                else
                                                                {
                                                                    producto.Nombre += ddd;
                                                                }
                                                                
                                                            }
                                                            else
                                                            {
                                                                producto.Nombre += " " + ddd;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(producto.precio))
                                                            {
                                                                producto.precio += ddd;
                                                            }
                                                            else
                                                            {
                                                                producto.precio += " " + ddd;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            foreach (HtmlNode nodes in doc3.DocumentNode.SelectNodes("//p"))
                                            {
                                                string ddd = nodes.InnerHtml;
                                                if (!string.IsNullOrEmpty(ddd))
                                                {
                                                    if (!ddd.ToLower().Contains("agregar"))
                                                    {
                                                        ddd = ddd.Replace("\t", "").Replace("\n", "");
                                                        if (string.IsNullOrEmpty(producto.descripcion))
                                                        {
                                                            producto.descripcion += ddd;
                                                        }
                                                        else
                                                        {
                                                            producto.descripcion += " " + ddd;
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        c2++;
                                    }
                                }
                                c1++;
                            }
                            productos.Add(producto);
                            c0++;
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {

                        }
                        catch (Exception)
                        {

                        }
                    }
                    
                }
            }
            catch (Exception)
            {
                
            }


            string url_imagen = @"https://lndlexpress.sfo2.digitaloceanspaces.com/Productos/";
            string url_fija = "https://www.lanacionaldelicores.com/TiendaExpress";
            //conn.ConnectionString = "Username=postgres; Password=1234;Host=localhost; Port=5432;Database=metabuscador";
            conn.ConnectionString = "Username=beyodntest; Password=metabyd2018;Host=metabynd.cdsk81lny0di.us-east-2.rds.amazonaws.com; Port=5432;Database=metabuscador";
            string tareaSQL = "INSERT INTO public.tareawebscraper(fechahoraini, fechahorafin, cantidadproductos, idalmacen, productoscopiados)";
            tareaSQL += "VALUES(:param1, :param2, :param3, :param4, :param5);select currval('tareawebscraper_idtarea_seq');";
            

            int idtarea = 0;
            try
            {
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand(tareaSQL, conn);
                cmd1.Parameters.Add(new NpgsqlParameter("param1", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param2", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param3", productos.Count));
                cmd1.Parameters.Add(new NpgsqlParameter("param4", 23));
                cmd1.Parameters.Add(new NpgsqlParameter("param5", true));
                
                idtarea = int.Parse(cmd1.ExecuteScalar().ToString());
                conn.Close();
            }
            catch (PostgresException ex)
            {
                conn.Close();
            }
            foreach (var producto in productos)
            {
                byte[] bytes = Encoding.Default.GetBytes(producto.descripcion);
                producto.descripcion = Encoding.UTF8.GetString(bytes);

                byte[] bytes1 = Encoding.Default.GetBytes(producto.Nombre);
                producto.Nombre = Encoding.UTF8.GetString(bytes1);

                string[] words = producto.imagen.Split('/');
                producto.imagen= url_imagen+ words[2];
                if (producto.descripcion.Length>50)
                {
                    producto.descripcion = "";
                }
                if (producto.descripcion.ToLower().Contains("n-a"))
                {
                    producto.descripcion = "";
                }


                producto.precio = producto.precio.Replace("$", "").Replace(",", "").Replace(" ", "");
                double precio = 0;
                bool existe = false;
                
                if (double.TryParse(producto.precio, out precio))
                {
                    double double_anterior = -1;
                    int id = -1;
                    string url = "";
                    string imagen = "";

                    try
                    {
                        conn.Open();
                        string sql = "";
                        sql += "SELECT h.precio, h.idproducto, h.direccion_imagen, h.url FROM tienda t";
                        sql += " inner join almacen s on t.idtienda = s.idtienda";
                        sql += " inner join tareawebscraper u on u.idalmacen = s.idalmacen";
                        sql += " inner join producto_twebscr_hist h on u.idtarea=h.idtarea where h.nombre='" + producto.Nombre + "' and t.idtienda=4";

                        NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                        NpgsqlDataReader rd = command.ExecuteReader();
                        if (rd.Read())
                        {
                            existe = true;
                            
                            if (double.TryParse(rd[0].ToString(), out double_anterior))
                            {
                            }
                            if (int.TryParse(rd[1].ToString(), out id))
                            {
                            }

                            if (rd[3]!=null)
                            {
                                if (!string.IsNullOrEmpty(rd[3].ToString()))
                                {
                                    url = rd[3].ToString();
                                }
                            }
                            if (rd[2] != null)
                            {
                                if (!string.IsNullOrEmpty(rd[2].ToString()))
                                {
                                    imagen = rd[2].ToString();
                                }
                            }
                        }
                        conn.Close();
                    }
                    catch (PostgresException ex)
                    {
                        conn.Close();
                    }


                    if (!existe)
                    {
                        try
                        {
                            conn.Open();
                            NpgsqlCommand cmd = new NpgsqlCommand("insert into public.producto_twebscr_hist (nombre, detalle, fecha, hora, fechahora, idtarea, direccion_imagen, idcategoria, codigotienda, descripcion, precio, url, activo) values(:nombre, :detalle, :fecha, :hora, :fechahora, :idtarea, :direccion_imagen, :idcategoria, :codigotienda, :descripcion, :precio, :url, :activo)", conn);
                            cmd.Parameters.Add(new NpgsqlParameter("nombre", producto.Nombre));
                            cmd.Parameters.Add(new NpgsqlParameter("detalle", producto.descripcion));
                            cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                            cmd.Parameters.Add(new NpgsqlParameter("hora", DateTime.Now));
                            cmd.Parameters.Add(new NpgsqlParameter("fechahora", DateTime.Now));
                            cmd.Parameters.Add(new NpgsqlParameter("idtarea", idtarea));
                            cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                            cmd.Parameters.Add(new NpgsqlParameter("idcategoria", 10));
                            cmd.Parameters.Add(new NpgsqlParameter("codigotienda", "0"));
                            cmd.Parameters.Add(new NpgsqlParameter("descripcion", ""));
                            cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                            cmd.Parameters.Add(new NpgsqlParameter("url", url_fija));
                            cmd.Parameters.Add(new NpgsqlParameter("activo", true));
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (PostgresException ex)
                        {
                            conn.Close();
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    else
                    {
                        if (id>0 && url_fija!=url && producto.imagen!=imagen)
                        {
                            //actualizar precio, url o imagan
                            try
                            {
                                conn.Open();
                                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE public.producto_twebscr_hist SET direccion_imagen=:direccion_imagen, precio=:precio, url=:url WHERE idproducto=:idproducto", conn);
                                cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                cmd.Parameters.Add(new NpgsqlParameter("url", url_fija));
                                cmd.Parameters.Add(new NpgsqlParameter("idproducto", id));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (PostgresException ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            List<producto> productos = new List<producto>();

            string filepath = textBox2.Text;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<string> urls = new List<string>();
            foreach (var file in d.GetFiles("*.html"))
            {
                urls.Add(file.FullName);
            }


            try
            {
                foreach (var file in d.GetFiles("*.html"))
                {
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.Load(file.FullName);
                        string filename = Path.GetFileNameWithoutExtension(file.FullName);
                        int parseName = 0;
                        bool descr_name = false;
                        if (!int.TryParse(filename, out parseName))
                        {
                            descr_name = true;
                        }
                        string img_ = "//div[@class='product-container-shelf availability-city-checked']";
                        int c0 = 0;
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes(img_))
                        {
                            producto producto = new producto();

                            string html0 = node.InnerHtml;
                            var doc1 = new HtmlAgilityPack.HtmlDocument();
                            doc1.LoadHtml(html0);

                            HtmlNode node1 = node;
                            //url
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//a[contains(@class, 'product-container-shelf__container-image')]"))
                            {
                                producto.url = node2.Attributes["href"].Value;
                                string html1 = node2.InnerHtml;
                                var doc2 = new HtmlAgilityPack.HtmlDocument();
                                doc2.LoadHtml(html1);
                                foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//img"))
                                {
                                    string images = node3.Attributes["src"].Value;
                                    producto.imagen = images;
                                }
                            }
                            //precio
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//span[contains(@class, 'product-container-shelf__best-price')]"))
                            {
                                producto.precio = node2.InnerHtml;
                            }
                            //nombre
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//a[contains(@class, 'product-container-shelf__name')]"))
                            {
                                producto.Nombre = node2.InnerHtml;
                            }
                            productos.Add(producto);
                            c0++;
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {

                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            string url_imagen = @"https://dislicores.vteximg.com.br/arquivos/ids/156225-292-292/";
            string url_fija = "https://www.dislicores.com/es/licores";
            //conn.ConnectionString = "Username=postgres; Password=1234;Host=localhost; Port=5432;Database=metabuscador";
            conn.ConnectionString = "Username=beyodntest; Password=metabyd2018;Host=metabynd.cdsk81lny0di.us-east-2.rds.amazonaws.com; Port=5432;Database=metabuscador";
            string tareaSQL = "INSERT INTO public.tareawebscraper(fechahoraini, fechahorafin, cantidadproductos, idalmacen, productoscopiados)";
            tareaSQL += "VALUES(:param1, :param2, :param3, :param4, :param5);select currval('tareawebscraper_idtarea_seq');";

            int idtarea = 0;
            try
            {
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand(tareaSQL, conn);
                cmd1.Parameters.Add(new NpgsqlParameter("param1", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param2", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param3", productos.Count));
                cmd1.Parameters.Add(new NpgsqlParameter("param4", 24));
                cmd1.Parameters.Add(new NpgsqlParameter("param5", true));
                idtarea = int.Parse(cmd1.ExecuteScalar().ToString());
                conn.Close();
            
            foreach (var producto in productos)
            {
                if (!string.IsNullOrEmpty(producto.descripcion))
                {
                    byte[] bytes = Encoding.Default.GetBytes(producto.descripcion);
                    producto.descripcion = Encoding.UTF8.GetString(bytes);
                }
                if (!string.IsNullOrEmpty(producto.Nombre))
                {
                    byte[] bytes1 = Encoding.Default.GetBytes(producto.Nombre);
                    producto.Nombre = Encoding.UTF8.GetString(bytes1);
                    producto.Nombre = producto.Nombre.Replace("\n             ", "");
                    producto.Nombre = producto.Nombre.Replace("\n            ", "");
                    producto.Nombre = producto.Nombre.Replace("\n           ", "");
                    producto.Nombre = producto.Nombre.Replace("\n          ", "");
                    producto.Nombre = producto.Nombre.Replace("\n         ", "");
                    producto.Nombre = producto.Nombre.Replace("\n        ", "");
                    producto.Nombre = producto.Nombre.Replace("\n       ", "");
                    producto.Nombre = producto.Nombre.Replace("\n      ", "");
                    producto.Nombre = producto.Nombre.Replace("\n     ", "");
                    producto.Nombre = producto.Nombre.Replace("\n    ", "");
                    producto.Nombre = producto.Nombre.Replace("\n   ", "");
                    producto.Nombre = producto.Nombre.Replace("\n  ", "");
                    producto.Nombre = producto.Nombre.TrimStart();
                    producto.Nombre = producto.Nombre.TrimEnd();
                        if (producto.Nombre.Contains("'"))
                        {
                            producto.Nombre = producto.Nombre.Replace("'", "");
                        }
                    }
                    
                    bool existe = false;
                    double precio = 0;
                    producto.precio = producto.precio.Replace("$", "").Replace(".", "").Replace(" ", "");
                    producto.precio=Clean(producto.precio);
                    

                    if (double.TryParse(producto.precio, out precio))
                    {
                        double double_anterior = -1;
                        int id = -1;
                        string url = "";
                        string imagen = "";

                        try
                        {
                            conn.Open();
                            string sql = "";
                            sql += "SELECT h.precio, h.idproducto, h.direccion_imagen, h.url FROM tienda t";
                            sql += " inner join almacen s on t.idtienda = s.idtienda";
                            sql += " inner join tareawebscraper u on u.idalmacen = s.idalmacen";
                            sql += " inner join producto_twebscr_hist h on u.idtarea=h.idtarea where h.nombre='" + producto.Nombre + "' and t.idtienda=6";

                            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                            NpgsqlDataReader rd = command.ExecuteReader();
                            if (rd.Read())
                            {
                                existe = true;

                                if (double.TryParse(rd[0].ToString(), out double_anterior))
                                {
                                }
                                if (int.TryParse(rd[1].ToString(), out id))
                                {
                                }

                                if (rd[3] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[3].ToString()))
                                    {
                                        url = rd[3].ToString();
                                    }
                                }
                                if (rd[2] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[2].ToString()))
                                    {
                                        imagen = rd[2].ToString();
                                    }
                                }
                            }
                            conn.Close();
                        }
                        catch (PostgresException ex)
                        {
                            conn.Close();
                        }


                        if (!existe)
                        {
                            try
                            {
                                conn.Open();
                                NpgsqlCommand cmd = new NpgsqlCommand("insert into public.producto_twebscr_hist (nombre, detalle, fecha, hora, fechahora, idtarea, direccion_imagen, idcategoria, codigotienda, descripcion, precio, url, activo) values(:nombre, :detalle, :fecha, :hora, :fechahora, :idtarea, :direccion_imagen, :idcategoria, :codigotienda, :descripcion, :precio, :url, :activo)", conn);
                                cmd.Parameters.Add(new NpgsqlParameter("nombre", producto.Nombre));
                                cmd.Parameters.Add(new NpgsqlParameter("detalle", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("hora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("fechahora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("idtarea", idtarea));
                                cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                cmd.Parameters.Add(new NpgsqlParameter("idcategoria", 10));
                                cmd.Parameters.Add(new NpgsqlParameter("codigotienda", "0"));
                                cmd.Parameters.Add(new NpgsqlParameter("descripcion", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                cmd.Parameters.Add(new NpgsqlParameter("activo", true));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (PostgresException ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            if (id > 0 && producto.url != url && producto.imagen != imagen)
                            {
                                //actualizar precio, url o imagan
                                try
                                {
                                    conn.Open();
                                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE public.producto_twebscr_hist SET direccion_imagen=:direccion_imagen, precio=:precio, url=:url WHERE idproducto=:idproducto", conn);
                                    cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                    cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                    cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                    cmd.Parameters.Add(new NpgsqlParameter("idproducto", id));
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                                catch (PostgresException ex)
                                {
                                    conn.Close();
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
            }
            }
            catch (PostgresException ex)
            {
                conn.Close();
            }
        }
        private string Clean(string number)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(number, "");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            List<producto> productos = new List<producto>();

            string filepath = textBox3.Text;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<string> urls = new List<string>();
            foreach (var file in d.GetFiles("*.html"))
            {
                urls.Add(file.FullName);
            }


            try
            {
                foreach (var file in d.GetFiles("*.html"))
                {
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.Load(file.FullName);
                        string filename = Path.GetFileNameWithoutExtension(file.FullName);
                        int parseName = 0;
                        bool descr_name = false;
                        if (!int.TryParse(filename, out parseName))
                        {
                            descr_name = true;
                        }
                        string img_ = "//article[@class='mq-product-card mq-default']";
                        int c0 = 0;
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes(img_))
                        {
                            producto producto = new producto();

                            string html0 = node.InnerHtml;
                            var doc1 = new HtmlAgilityPack.HtmlDocument();
                            doc1.LoadHtml(html0);

                            int c1 = 0;
                            HtmlNode node1 = node;
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//div[contains(@class, 'mq-product-img')]"))
                            {
                                string html1 = node2.InnerHtml;
                                var doc2 = new HtmlAgilityPack.HtmlDocument();
                                doc2.LoadHtml(html1);
                                foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//a"))
                                {
                                    producto.url = node3.Attributes["href"].Value;
                                    producto.url = "https://merqueo.com" + producto.url;
                                }
                                foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//img"))
                                {
                                    string images = node3.Attributes["src"].Value;
                                    producto.imagen = images;
                                }
                            }
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//section[contains(@class, 'mq-product-card-data')]"))
                            {
                                string html1 = node2.InnerHtml;
                                var doc2 = new HtmlAgilityPack.HtmlDocument();
                                doc2.LoadHtml(html1);
                                foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//h2[contains(@class, 'mq-product-title')]"))
                                {
                                    string html2 = node3.InnerHtml;
                                    var doc3 = new HtmlAgilityPack.HtmlDocument();
                                    doc3.LoadHtml(html2);
                                    foreach (HtmlNode node4 in doc3.DocumentNode.SelectNodes("//a"))
                                    {
                                        producto.Nombre = node4.InnerText;
                                    }
                                }
                                foreach (HtmlNode node3 in doc2.DocumentNode.SelectNodes("//h3[contains(@class, 'mq-product-subtitle')]"))
                                {
                                    string desc = node3.InnerText;

                                    desc = desc.Replace("\n             ", "");
                                    desc = desc.Replace("\n            ", "");
                                    desc = desc.Replace("\n           ", "");
                                    desc = desc.Replace("\n          ", "");
                                    desc = desc.Replace("\n         ", "");
                                    desc = desc.Replace("\n        ", "");
                                    desc = desc.Replace("\n       ", "");
                                    desc = desc.Replace("\n      ", "");
                                    desc = desc.Replace("\n     ", "");
                                    desc = desc.Replace("\n    ", "");
                                    desc = desc.Replace("\n   ", "");
                                    desc = desc.Replace("\n  ", "");
                                    desc = desc.Replace("\n ", "");
                                    desc = desc.Replace("\n", "");
                                    desc = desc.Replace("\r", "");

                                    producto.Nombre = producto.Nombre +" "+ desc;
                                }
                            }
                            //precio
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//h3[contains(@class, 'mq-product-price')]"))
                            {
                                string precio = node2.InnerHtml;
                                precio = precio.Replace("\n             ", "");
                                precio = precio.Replace("\n            ", "");
                                precio = precio.Replace("\n           ", "");
                                precio = precio.Replace("\n          ", "");
                                precio = precio.Replace("\n         ", "");
                                precio = precio.Replace("\n        ", "");
                                precio = precio.Replace("\n       ", "");
                                precio = precio.Replace("\n      ", "");
                                precio = precio.Replace("\n     ", "");
                                precio = precio.Replace("\n    ", "");
                                precio = precio.Replace("\n   ", "");
                                precio = precio.Replace("\n  ", "");
                                precio = precio.Replace("\r", "");

                                producto.precio = precio;
                            }
                            productos.Add(producto);
                            c0++;
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {

                        }
                        catch (Exception)
                        {

                        }
                    }

                }
            }
            catch (Exception)
            {

            }

            //   
            string url_imagen = @"https://dislicores.vteximg.com.br/arquivos/ids/156225-292-292/";
            string url_fija = "https://www.dislicores.com/es/licores";
            //conn.ConnectionString = "Username=postgres; Password=1234;Host=localhost; Port=5432;Database=metabuscador";
            conn.ConnectionString = "Username=beyodntest; Password=metabyd2018;Host=metabynd.cdsk81lny0di.us-east-2.rds.amazonaws.com; Port=5432;Database=metabuscador";
            string tareaSQL = "INSERT INTO public.tareawebscraper(fechahoraini, fechahorafin, cantidadproductos, idalmacen, productoscopiados)";
            tareaSQL += "VALUES(:param1, :param2, :param3, :param4, :param5);select currval('tareawebscraper_idtarea_seq');";


            int idtarea = 0;
            try
            {
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand(tareaSQL, conn);
                cmd1.Parameters.Add(new NpgsqlParameter("param1", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param2", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param3", productos.Count));
                cmd1.Parameters.Add(new NpgsqlParameter("param4", 25));
                cmd1.Parameters.Add(new NpgsqlParameter("param5", true));
                idtarea = int.Parse(cmd1.ExecuteScalar().ToString());
                conn.Close();

                foreach (var producto in productos)
                {
                    if (!string.IsNullOrEmpty(producto.descripcion))
                    {
                        byte[] bytes = Encoding.Default.GetBytes(producto.descripcion);
                        producto.descripcion = Encoding.UTF8.GetString(bytes);
                    }
                    if (!string.IsNullOrEmpty(producto.Nombre))
                    {
                        byte[] bytes1 = Encoding.Default.GetBytes(producto.Nombre);
                        producto.Nombre = Encoding.UTF8.GetString(bytes1);
                        producto.Nombre = producto.Nombre.Replace("\n             ", "");
                        producto.Nombre = producto.Nombre.Replace("\n            ", "");
                        producto.Nombre = producto.Nombre.Replace("\n           ", "");
                        producto.Nombre = producto.Nombre.Replace("\n          ", "");
                        producto.Nombre = producto.Nombre.Replace("\n         ", "");
                        producto.Nombre = producto.Nombre.Replace("\n        ", "");
                        producto.Nombre = producto.Nombre.Replace("\n       ", "");
                        producto.Nombre = producto.Nombre.Replace("\n      ", "");
                        producto.Nombre = producto.Nombre.Replace("\n     ", "");
                        producto.Nombre = producto.Nombre.Replace("\n    ", "");
                        producto.Nombre = producto.Nombre.Replace("\n   ", "");
                        producto.Nombre = producto.Nombre.Replace("\n  ", "");
                        producto.Nombre = producto.Nombre.TrimStart();
                        producto.Nombre = producto.Nombre.TrimEnd();
                        if (producto.Nombre.Contains("'"))
                        {
                            producto.Nombre = producto.Nombre.Replace("'", "");
                        }
                    }
                    //copiar cada producto
                    bool existe = false;
                    producto.precio = producto.precio.Replace("$", "").Replace(".", "").Replace(" ", "");
                    producto.precio = Clean(producto.precio);
                    double precio = 0;

                    if (double.TryParse(producto.precio, out precio))
                    {
                        double double_anterior = -1;
                        int id = -1;
                        string url = "";
                        string imagen = "";

                        try
                        {
                            conn.Open();
                            string sql = "";
                            sql += "SELECT h.precio, h.idproducto, h.direccion_imagen, h.url FROM tienda t";
                            sql += " inner join almacen s on t.idtienda = s.idtienda";
                            sql += " inner join tareawebscraper u on u.idalmacen = s.idalmacen";
                            sql += " inner join producto_twebscr_hist h on u.idtarea=h.idtarea where h.nombre='" + producto.Nombre + "' and t.idtienda=7";

                            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                            NpgsqlDataReader rd = command.ExecuteReader();
                            if (rd.Read())
                            {
                                existe = true;

                                if (double.TryParse(rd[0].ToString(), out double_anterior))
                                {
                                }
                                if (int.TryParse(rd[1].ToString(), out id))
                                {
                                }

                                if (rd[3] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[3].ToString()))
                                    {
                                        url = rd[3].ToString();
                                    }
                                }
                                if (rd[2] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[2].ToString()))
                                    {
                                        imagen = rd[2].ToString();
                                    }
                                }
                            }
                            conn.Close();
                        }
                        catch (PostgresException ex)
                        {
                            conn.Close();
                        }


                        if (!existe)
                        {
                            try
                            {
                                conn.Open();
                                NpgsqlCommand cmd = new NpgsqlCommand("insert into public.producto_twebscr_hist (nombre, detalle, fecha, hora, fechahora, idtarea, direccion_imagen, idcategoria, codigotienda, descripcion, precio, url, activo) values(:nombre, :detalle, :fecha, :hora, :fechahora, :idtarea, :direccion_imagen, :idcategoria, :codigotienda, :descripcion, :precio, :url, :activo)", conn);
                                cmd.Parameters.Add(new NpgsqlParameter("nombre", producto.Nombre));
                                cmd.Parameters.Add(new NpgsqlParameter("detalle", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("hora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("fechahora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("idtarea", idtarea));
                                cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                cmd.Parameters.Add(new NpgsqlParameter("idcategoria", 10));
                                cmd.Parameters.Add(new NpgsqlParameter("codigotienda", "0"));
                                cmd.Parameters.Add(new NpgsqlParameter("descripcion", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                cmd.Parameters.Add(new NpgsqlParameter("activo", true));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (PostgresException ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            //if (id > 0)
                            if (id > 0 && producto.url != url && producto.imagen != imagen)
                            {
                                //actualizar precio, url o imagan
                                try
                                {
                                    conn.Open();
                                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE public.producto_twebscr_hist SET direccion_imagen=:direccion_imagen, precio=:precio, url=:url WHERE idproducto=:idproducto", conn);
                                    cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                    cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                    cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                    cmd.Parameters.Add(new NpgsqlParameter("idproducto", id));
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                                catch (PostgresException ex)
                                {
                                    conn.Close();
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (PostgresException ex)
            {
                conn.Close();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            List<producto> productos = new List<producto>();

            string filepath = textBox3.Text;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<string> urls = new List<string>();
            List<string> desc = new List<string>();

            urls.Add("https://www.lalicorera.com/productos/cerveza");
            urls.Add("https://www.lalicorera.com/productos/aguardiente");
            urls.Add("https://www.lalicorera.com/productos/champagne");
            urls.Add("https://www.lalicorera.com/productos/brandy");
            urls.Add("https://www.lalicorera.com/productos/coctels-ya-preparados-para-beber");
            urls.Add("https://www.lalicorera.com/productos/especiales");
            urls.Add("https://www.lalicorera.com/productos/ginebra");
            urls.Add("https://www.lalicorera.com/productos/ron");
            urls.Add("https://www.lalicorera.com/productos/tequila");
            urls.Add("https://www.lalicorera.com/productos/vino");
            urls.Add("https://www.lalicorera.com/productos/vodka");
            urls.Add("https://www.lalicorera.com/productos/whisky");

            desc.Add("Cerveza"); desc.Add("Aguardiente");
            desc.Add("Champagne"); desc.Add("Brandy");
            desc.Add("Coctels"); desc.Add("Especiales");
            desc.Add("Ginebra"); desc.Add("Ron");
            desc.Add("Tequila"); desc.Add("Vino");
            desc.Add("Vodka"); desc.Add("Whisky");
            try
            {
                int c0 = 0;
                foreach (var url in urls)
                {
                    
                    try
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var web = new HtmlWeb();
                        var doc = web.Load(url);

                        string img_ = "//div[@class='product-in-list']";
                        
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes(img_))
                        {
                            producto producto = new producto();
                            producto.descripcion = desc[c0];
                            string html0 = node.InnerHtml;
                            var doc1 = new HtmlAgilityPack.HtmlDocument();
                            doc1.LoadHtml(html0);

                            int c1 = 0;
                            HtmlNode node1 = node;
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//a"))
                            {
                                if (c1==0)
                                {
                                    string html1_1 = node2.InnerHtml;
                                    var doc1_1 = new HtmlAgilityPack.HtmlDocument();
                                    doc1_1.LoadHtml(html1_1);
                                    foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//img"))
                                    {
                                        string images = node1_1.Attributes["src"].Value;
                                        producto.imagen = images;
                                    }
                                }
                                
                                c1++;
                            }
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//div[@class='product-center']"))
                            {
                                string html1_1 = node2.InnerHtml;
                                var doc1_1 = new HtmlAgilityPack.HtmlDocument();
                                doc1_1.LoadHtml(html1_1);
                                foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//h3"))
                                {
                                    string html1_2 = node1_1.InnerHtml;
                                    var doc1_2 = new HtmlAgilityPack.HtmlDocument();
                                    doc1_2.LoadHtml(html1_2);
                                    foreach (HtmlNode node1_2 in doc1_2.DocumentNode.SelectNodes("//a"))
                                    {
                                        producto.url = node1_2.Attributes["href"].Value;
                                        producto.Nombre = node1_2.InnerText;
                                    }
                                }
                            }
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//div[@class='product-right']"))
                            {
                                string html1_1 = node2.InnerHtml;
                                var doc1_1 = new HtmlAgilityPack.HtmlDocument();
                                doc1_1.LoadHtml(html1_1);
                                foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//h4"))
                                {
                                    producto.precio = node1_1.InnerText;
                                }
                            }
                            productos.Add(producto);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {

                        }
                        catch (Exception)
                        {

                        }
                    }
                    c0++;
                }
            }
            catch (Exception)
            {

            }

            //   
            string url_imagen = @"https://www.lalicorera.com/";
            string url_fija = "https://www.dislicores.com/es/licores";
            //conn.ConnectionString = "Username=postgres; Password=1234;Host=localhost; Port=5432;Database=metabuscador";
            conn.ConnectionString = "Username=bduserbynd; Password=Metabydaws2019*;Host=metabynd.ccyoqrh9jvkv.us-east-1.rds.amazonaws.com; Port=5432;Database=metabynd";
            string tareaSQL = "INSERT INTO public.tareawebscraper(fechahoraini, fechahorafin, cantidadproductos, idalmacen, productoscopiados)";
            tareaSQL += "VALUES(:param1, :param2, :param3, :param4, :param5);select currval('tareawebscraper_idtarea_seq');";


            int idtarea = 0;
            try
            {
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand(tareaSQL, conn);
                cmd1.Parameters.Add(new NpgsqlParameter("param1", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param2", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param3", productos.Count));
                cmd1.Parameters.Add(new NpgsqlParameter("param4", 26));
                cmd1.Parameters.Add(new NpgsqlParameter("param5", true));
                idtarea = int.Parse(cmd1.ExecuteScalar().ToString());
                conn.Close();
               // return;
                foreach (var producto in productos)
                {
                    if (!string.IsNullOrEmpty(producto.descripcion))
                    {
                        byte[] bytes = Encoding.Default.GetBytes(producto.descripcion);
                        producto.descripcion = Encoding.UTF8.GetString(bytes);
                    }
                    if (!string.IsNullOrEmpty(producto.Nombre))
                    {
                        byte[] bytes1 = Encoding.Default.GetBytes(producto.Nombre);
                        producto.Nombre = Encoding.UTF8.GetString(bytes1);
                        producto.Nombre = producto.Nombre.Replace("\n             ", "");
                        producto.Nombre = producto.Nombre.Replace("\n            ", "");
                        producto.Nombre = producto.Nombre.Replace("\n           ", "");
                        producto.Nombre = producto.Nombre.Replace("\n          ", "");
                        producto.Nombre = producto.Nombre.Replace("\n         ", "");
                        producto.Nombre = producto.Nombre.Replace("\n        ", "");
                        producto.Nombre = producto.Nombre.Replace("\n       ", "");
                        producto.Nombre = producto.Nombre.Replace("\n      ", "");
                        producto.Nombre = producto.Nombre.Replace("\n     ", "");
                        producto.Nombre = producto.Nombre.Replace("\n    ", "");
                        producto.Nombre = producto.Nombre.Replace("\n   ", "");
                        producto.Nombre = producto.Nombre.Replace("\n  ", "");
                        producto.Nombre = producto.Nombre.TrimStart();
                        producto.Nombre = producto.Nombre.TrimEnd();
                    }
                    if (producto.Nombre.Contains("'"))
                    {
                        producto.Nombre = producto.Nombre.Replace("'","");
                    }

                    //copiar cada producto
                    bool existe = false;
                    producto.precio = producto.precio.Replace("$", "").Replace(",", "").Replace(" ", "");
                    producto.precio = Clean(producto.precio);
                    double precio = 0;
                    
                    if (double.TryParse(producto.precio, out precio))
                    {
                        double double_anterior = -1;
                        int id = -1;
                        string url = "";
                        string imagen = "";

                        try
                        {
                            conn.Open();
                            string sql = "";
                            sql += "SELECT h.precio, h.idproducto, h.direccion_imagen, h.url FROM tienda t";
                            sql += " inner join almacen s on t.idtienda = s.idtienda";
                            sql += " inner join tareawebscraper u on u.idalmacen = s.idalmacen";
                            sql += " inner join producto_twebscr_hist h on u.idtarea=h.idtarea where h.nombre='" + producto.Nombre + "' and t.idtienda=8";

                            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                            NpgsqlDataReader rd = command.ExecuteReader();
                            if (rd.Read())
                            {
                                existe = true;

                                if (double.TryParse(rd[0].ToString(), out double_anterior))
                                {
                                }
                                if (int.TryParse(rd[1].ToString(), out id))
                                {
                                }

                                if (rd[3] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[3].ToString()))
                                    {
                                        url = rd[3].ToString();
                                    }
                                }
                                if (rd[2] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[2].ToString()))
                                    {
                                        imagen = rd[2].ToString();
                                    }
                                }
                            }
                            conn.Close();
                        }
                        catch (PostgresException ex)
                        {
                            conn.Close();
                        }


                        if (!existe)
                        {
                            try
                            {
                                conn.Open();
                                NpgsqlCommand cmd = new NpgsqlCommand("insert into public.producto_twebscr_hist (nombre, detalle, fecha, hora, fechahora, idtarea, direccion_imagen, idcategoria, codigotienda, descripcion, precio, url, activo) values(:nombre, :detalle, :fecha, :hora, :fechahora, :idtarea, :direccion_imagen, :idcategoria, :codigotienda, :descripcion, :precio, :url, :activo)", conn);
                                cmd.Parameters.Add(new NpgsqlParameter("nombre", producto.Nombre));
                                cmd.Parameters.Add(new NpgsqlParameter("detalle", producto.descripcion));
                                cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("hora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("fechahora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("idtarea", idtarea));
                                cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", url_imagen+producto.imagen));
                                cmd.Parameters.Add(new NpgsqlParameter("idcategoria", 10));
                                cmd.Parameters.Add(new NpgsqlParameter("codigotienda", "0"));
                                cmd.Parameters.Add(new NpgsqlParameter("descripcion", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                cmd.Parameters.Add(new NpgsqlParameter("url", url_imagen+producto.url));
                                cmd.Parameters.Add(new NpgsqlParameter("activo", true));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (PostgresException ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            if (id > 0 && url_imagen+producto.url != url && url_imagen+producto.imagen != imagen)
                            {
                                //actualizar precio, url o imagan
                                try
                                {
                                    conn.Open();
                                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE public.producto_twebscr_hist SET direccion_imagen=:direccion_imagen, precio=:precio, url=:url WHERE idproducto=:idproducto", conn);
                                    cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", url_imagen+producto.imagen));
                                    cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                    cmd.Parameters.Add(new NpgsqlParameter("url", url_imagen+producto.url));
                                    cmd.Parameters.Add(new NpgsqlParameter("idproducto", id));
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                                catch (PostgresException ex)
                                {
                                    conn.Close();
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (PostgresException ex)
            {
                conn.Close();
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {

                List<producto> productos = new List<producto>();

            string filepath = textBox3.Text;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<string> urls = new List<string>();
            List<string> desc = new List<string>();

            urls.Add("https://andresconrapidez.com/?page_id=36729");
            urls.Add("https://andresconrapidez.com/?page_id=36713");
            urls.Add("https://andresconrapidez.com/?page_id=36731");
            urls.Add("https://andresconrapidez.com/?page_id=36735");
            urls.Add("https://andresconrapidez.com/?page_id=36734");
            urls.Add("https://andresconrapidez.com/?page_id=36732");
            urls.Add("https://andresconrapidez.com/?page_id=36733");
            urls.Add("https://andresconrapidez.com/?page_id=36730");
            urls.Add("https://andresconrapidez.com/?page_id=36724");

            try
            {
                int c0 = 0;
                foreach (var url in urls)
                {

                    try
                    {

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var web = new HtmlWeb();
                        var doc = web.Load(url);

                        string img_ = "//figure[@class='woocom-project']";

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes(img_))
                        {
                            producto producto = new producto();
                            //producto.descripcion = desc[c0];
                            string html0 = node.InnerHtml;
                            var doc1 = new HtmlAgilityPack.HtmlDocument();
                            doc1.LoadHtml(html0);

                            int c1 = 0;
                            HtmlNode node1 = node;

                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//div[@class='woo-buttons-on-img']"))
                            {
                                string html1_1 = node2.InnerHtml;
                                var doc1_1 = new HtmlAgilityPack.HtmlDocument();
                                doc1_1.LoadHtml(html1_1);
                                foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//a"))
                                {
                                    if (c1 == 0)
                                    {
                                        producto.url = node1_1.Attributes["href"].Value;
                                        string html1_2 = node1_1.InnerHtml;
                                        var doc1_2 = new HtmlAgilityPack.HtmlDocument();
                                        doc1_2.LoadHtml(html1_2);
                                        foreach (HtmlNode node1_2 in doc1_2.DocumentNode.SelectNodes("//img"))
                                        {
                                            string images = node1_2.Attributes["data-src"].Value;
                                            producto.imagen = images;
                                        }
                                        c1++;
                                    }
                                }
                            }
                            foreach (HtmlNode node2 in doc1.DocumentNode.SelectNodes("//figcaption[@class='woocom-list-content']"))
                            {
                                string html1_1 = node2.InnerHtml;
                                var doc1_1 = new HtmlAgilityPack.HtmlDocument();
                                doc1_1.LoadHtml(html1_1);
                                foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//h4"))
                                {
                                    string html1_2 = node1_1.InnerHtml;
                                    var doc1_2 = new HtmlAgilityPack.HtmlDocument();
                                    doc1_2.LoadHtml(html1_2);
                                    foreach (HtmlNode node1_2 in doc1_2.DocumentNode.SelectNodes("//a"))
                                    {
                                        producto.Nombre = node1_2.InnerText;
                                    }
                                }
                                foreach (HtmlNode node1_1 in doc1_1.DocumentNode.SelectNodes("//span[@class='woocommerce-Price-amount amount']"))
                                {
                                    producto.precio = node1_1.InnerText;
                                    producto.precio = producto.precio.Replace("&#36;", "");
                                    string[] prices = producto.precio.Split('.');
                                    producto.precio = prices[0];

                                }
                            }
                            productos.Add(producto);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {

                        }
                        catch (Exception)
                        {

                        }
                    }
                    c0++;
                }
            }
            catch (Exception)
            {

            }

            //   
            string url_imagen = @"https://www.lalicorera.com/";
            string url_fija = "https://www.dislicores.com/es/licores";
            //conn.ConnectionString = "Username=postgres; Password=1234;Host=localhost; Port=5432;Database=metabuscador";
            conn.ConnectionString = "Username=bduserbynd; Password=Metabydaws2019*;Host=metabynd.ccyoqrh9jvkv.us-east-1.rds.amazonaws.com; Port=5432;Database=metabynd";
            string tareaSQL = "INSERT INTO public.tareawebscraper(fechahoraini, fechahorafin, cantidadproductos, idalmacen, productoscopiados)";
            tareaSQL += "VALUES(:param1, :param2, :param3, :param4, :param5);select currval('tareawebscraper_idtarea_seq');";


            int idtarea = 0;
            try
            {
                conn.Open();
                NpgsqlCommand cmd1 = new NpgsqlCommand(tareaSQL, conn);
                cmd1.Parameters.Add(new NpgsqlParameter("param1", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param2", DateTime.Now));
                cmd1.Parameters.Add(new NpgsqlParameter("param3", productos.Count));
                cmd1.Parameters.Add(new NpgsqlParameter("param4", 27));
                cmd1.Parameters.Add(new NpgsqlParameter("param5", true));
                idtarea = int.Parse(cmd1.ExecuteScalar().ToString());
                conn.Close();
                foreach (var producto in productos)
                {
                    if (!string.IsNullOrEmpty(producto.descripcion))
                    {
                        byte[] bytes = Encoding.Default.GetBytes(producto.descripcion);
                        producto.descripcion = Encoding.UTF8.GetString(bytes);
                    }
                    if (!string.IsNullOrEmpty(producto.Nombre))
                    {
                        producto.Nombre = producto.Nombre.Replace("\n             ", "");
                        producto.Nombre = producto.Nombre.Replace("\n            ", "");
                        producto.Nombre = producto.Nombre.Replace("\n           ", "");
                        producto.Nombre = producto.Nombre.Replace("\n          ", "");
                        producto.Nombre = producto.Nombre.Replace("\n         ", "");
                        producto.Nombre = producto.Nombre.Replace("\n        ", "");
                        producto.Nombre = producto.Nombre.Replace("\n       ", "");
                        producto.Nombre = producto.Nombre.Replace("\n      ", "");
                        producto.Nombre = producto.Nombre.Replace("\n     ", "");
                        producto.Nombre = producto.Nombre.Replace("\n    ", "");
                        producto.Nombre = producto.Nombre.Replace("\n   ", "");
                        producto.Nombre = producto.Nombre.Replace("\n  ", "");
                        producto.Nombre = producto.Nombre.TrimStart();
                        producto.Nombre = producto.Nombre.TrimEnd();
                    }
                    if (producto.Nombre.Contains("'"))
                    {
                        producto.Nombre = producto.Nombre.Replace("'", "");
                    }

                    //copiar cada producto
                    bool existe = false;
                    producto.precio = producto.precio.Replace("$", "").Replace(",", "").Replace(" ", "");
                    producto.precio = Clean(producto.precio);
                    double precio = 0;

                    if (double.TryParse(producto.precio, out precio))
                    {
                        double double_anterior = -1;
                        int id = -1;
                        string url = "";
                        string imagen = "";

                        try
                        {
                            conn.Open();
                            string sql = "";
                            sql += "SELECT h.precio, h.idproducto, h.direccion_imagen, h.url FROM tienda t";
                            sql += " inner join almacen s on t.idtienda = s.idtienda";
                            sql += " inner join tareawebscraper u on u.idalmacen = s.idalmacen";
                            sql += " inner join producto_twebscr_hist h on u.idtarea=h.idtarea where h.nombre='" + producto.Nombre + "' and t.idtienda=9";

                            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
                            NpgsqlDataReader rd = command.ExecuteReader();
                            if (rd.Read())
                            {
                                existe = true;

                                if (double.TryParse(rd[0].ToString(), out double_anterior))
                                {
                                }
                                if (int.TryParse(rd[1].ToString(), out id))
                                {
                                }

                                if (rd[3] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[3].ToString()))
                                    {
                                        url = rd[3].ToString();
                                    }
                                }
                                if (rd[2] != null)
                                {
                                    if (!string.IsNullOrEmpty(rd[2].ToString()))
                                    {
                                        imagen = rd[2].ToString();
                                    }
                                }
                            }
                            conn.Close();
                        }
                        catch (PostgresException ex)
                        {
                            conn.Close();
                        }


                        if (!existe)
                        {
                            try
                            {
                                conn.Open();
                                NpgsqlCommand cmd = new NpgsqlCommand("insert into public.producto_twebscr_hist (nombre, detalle, fecha, hora, fechahora, idtarea, direccion_imagen, idcategoria, codigotienda, descripcion, precio, url, activo) values(:nombre, :detalle, :fecha, :hora, :fechahora, :idtarea, :direccion_imagen, :idcategoria, :codigotienda, :descripcion, :precio, :url, :activo)", conn);
                                cmd.Parameters.Add(new NpgsqlParameter("nombre", producto.Nombre));
                                cmd.Parameters.Add(new NpgsqlParameter("detalle", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("fecha", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("hora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("fechahora", DateTime.Now));
                                cmd.Parameters.Add(new NpgsqlParameter("idtarea", idtarea));
                                cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                cmd.Parameters.Add(new NpgsqlParameter("idcategoria", 10));
                                cmd.Parameters.Add(new NpgsqlParameter("codigotienda", "0"));
                                cmd.Parameters.Add(new NpgsqlParameter("descripcion", ""));
                                cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                cmd.Parameters.Add(new NpgsqlParameter("activo", true));
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                            catch (PostgresException ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            if (id > 0 && producto.url != url && producto.imagen != imagen)
                            {
                                //actualizar precio, url o imagan
                                try
                                {
                                    conn.Open();
                                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE public.producto_twebscr_hist SET direccion_imagen=:direccion_imagen, precio=:precio, url=:url WHERE idproducto=:idproducto", conn);
                                    cmd.Parameters.Add(new NpgsqlParameter("direccion_imagen", producto.imagen));
                                    cmd.Parameters.Add(new NpgsqlParameter("precio", precio));
                                    cmd.Parameters.Add(new NpgsqlParameter("url", producto.url));
                                    cmd.Parameters.Add(new NpgsqlParameter("idproducto", id));
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                                catch (PostgresException ex)
                                {
                                    conn.Close();
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (PostgresException ex)
            {
                conn.Close();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }
    }
    
}
