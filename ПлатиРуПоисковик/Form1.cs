using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ПлатиРуПоисковик
{

    public partial class Form1 : Form
    {
        string URL = "";
        List<Item> items = new List<Item>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            URL = "http://www.plati.io/api/search.ashx?query="+textBox1.Text+ "&visibleOnly=true&response=json";
            getJSON();
        }

        string get(string url)
        {
            WebRequest request = System.Net.WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }

        void getJSON()
        {
            int c = 0;

            do
            {
                c++;
                string url = "http://www.plati.io/api/search.ashx?visibleOnly=true&pagesize=500&response=json&query=" + textBox1.Text + "&pagenum=" + c;

                string responseFromServer = get(url);
                var js = JsonConvert.DeserializeObject<RootObject>(responseFromServer);
                richTextBox1.AppendText("Всего страниц: " + js.Totalpages + Environment.NewLine);
                foreach (var ret in js.items)
                {
                    if (ret.price_rur <= Convert.ToInt32(textBox3.Text))
                    {
                        items.Add(ret);
                        richTextBox1.AppendText(ret.name + "(" + ret.price_rur + "руб.)" + Environment.NewLine);
                    }

                }
                foreach (var item in items)
                {
                    listBox1.Items.Add(item.name + " (" + item.price_rur + "руб.)");
                    groupBox1.Text = "[" + listBox1.Items.Count + "]";
                }

            } while (listBox1.Items.Count < Convert.ToInt32(textBox2.Text));


        }

        void met1()
        {

        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(items[listBox1.SelectedIndex].url);
            label4.Text = "Скопирована ссылка на <"+ items[listBox1.SelectedIndex].name+">";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string txt = "<html><head>"
                + "<title>Список игр</title>"
                + "<script>"
              + "  document.addEventListener('DOMContentLoaded', () => {"
             + "   const getSort = ({ target }) => {"
            + "    const order = (target.dataset.order = -(target.dataset.order || -1));"
             + "   const index = [...target.parentNode.cells].indexOf(target);"
            + "    const collator = new Intl.Collator(['en', 'ru'], { numeric: true });"
             + "   const comparator = (index, order) => (a, b) => order * collator.compare("
             + "   a.children[index].innerHTML,"
            + "    b.children[index].innerHTML"
            + "    );"
            + "    for (const tBody of target.closest('table').tBodies)"
            + "    tBody.append(...[...tBody.rows].sort(comparator(index, order)));"
            + "      for (const cell of target.parentNode.cells)"
            + "   cell.classList.toggle('sorted', cell === target);"
            + "  };"
            + "   document.querySelectorAll('.table_sort thead').forEach(tableTH => tableTH.addEventListener('click', () => getSort(event)));"
            + "  });"

                + "</script>"
                + "<link rel=\"stylesheet\" href=\"style.css\">"
                + "</head><body>"
                + "<table class=\"table_sort\"><thead><tr><th>Название</th><th>Цена (руб.)</th><th>Описание</th><tbody>";


            foreach (var item in items)
            {
                if (item.name!="")
                txt += "<tr><td><a href=\"" + item.url
                    + "\">" + item.name 
                    + "</a></td> <td>" 
                    + item.price_rur 
                    + "</td><td><details><summary>&nbsp;Подробнее&nbsp;</summary><p>"
                    + item.description
                    + "</p></details></td></tr>" + Environment.NewLine;
            }
            txt += "</tbody></table></body></html>";
            File.WriteAllText("index.html", txt);
            label4.Text = "Сохранено!";
        }
    }
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_eng { get; set; }
        public string name_translit { get; set; }
        public string name_translit_eng { get; set; }
        public double partner_commiss { get; set; }
        public double price_eur { get; set; }
        public double price_rur { get; set; }
        public double price_uah { get; set; }
        public double price_usd { get; set; }
        public int section_id { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string description_eng { get; set; }
        public string image { get; set; }
        public int seller_id { get; set; }
        public string seller_name { get; set; }
        public double seller_rating { get; set; }
        public int numsold { get; set; }
        public int numsold_hidden { get; set; }
        public int count_positiveresponses { get; set; }
        public int count_positiveresponses_hidden { get; set; }
        public int count_negativeresponses { get; set; }
        public int count_negativeresponses_hidden { get; set; }
        public int count_returns { get; set; }
        public int count_returns_hidden { get; set; }
        public string sales_form { get; set; }
    }

    public class RootObject
    {
        public int Pagenum { get; set; }
        public int Pagesize { get; set; }
        public int Totalpages { get; set; }
        public List<Item> items { get; set; }
        public int total { get; set; }
    }
}
