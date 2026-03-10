using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MyWork2
{
    public partial class ClientEditorTrue : Form
    {
        Form1 mainForm;
        string ClID;
        ClientsEditor clientForm;
        public ClientEditorTrue(Form1 mf, string clientID, ClientsEditor ce)
        {
            InitializeComponent();
            mainForm = mf;
            ClID = clientID;
            clientForm = ce;
        }

        private void SaveClientButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Сохранить данные о клиенте?", "Вы уверены?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                String Phone = Regex.Replace(ClientPhoneTextBox.Text, @"[^\d\,]", "");

                mainForm.basa.ClientsMapEditWithoutDate(ClientFioTextBox.Text, PhoneToNorm(Phone), ClientAdressTextBox.Text, PrimechanieTextBox.Text, BlistOfClients(), ClientAboutUsComboBox.Text, ClID);
                TemporaryBase.SearchFULLBegin();
                clientForm.SearchClient();
            }

        }

        //Блэк лист клентов
        private string BlistOfClients()
        {
            if (BlackListComboBox.Text == "Не проблемный")
                return "0";
            else if (BlackListComboBox.Text == "Проблемный")
                return "1";
            else return "-1";
        }

        private void BlackListComboBox_TextChanged(object sender, EventArgs e)
        {
            if (BlackListComboBox.Text == "Проблемный")
            {
                if (TemporaryBase.BlistColor != "")
                {
                    BlackListComboBox.BackColor = Color.FromArgb(int.Parse(TemporaryBase.BlistColor));
                }

                else
                    BlackListComboBox.BackColor = Color.White;
            }
        }
        //Преобразовать номер в номер без пробелов и т.п.
        private string PhoneToNorm(string phone)
        {
            return phone.Replace(" ", "");
        }

        private void ClientEditorTrue_Load(object sender, EventArgs e)
        {

            foreach (SortirovkaSpiskov ssp in TemporaryBase.SortirovkaAboutUs)
            {
                ClientAboutUsComboBox.Items.Add(ssp.SortObj);
            }
            DataTable dt1 = mainForm.basa.ClientsMapGiver(ClID);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        ClientFioTextBox.Text = dt1.Rows[i].ItemArray[1].ToString();

                        String Phone = dt1.Rows[i].ItemArray[2].ToString();





                        if (((Phone.IndexOf('(') < 0) || (Phone.IndexOf('-') < 0)) && (Phone.Length >= 10))
                        {
                            if (Phone.IndexOf(',') > 0)
                            {
                                String currentPhone = "";
                                String lastPhone = Phone + ",";
                                Phone = "";

                                while (lastPhone.IndexOf(',') >= 0)
                                {

                                    if (Phone.Length > 0) Phone = Phone + ", ";

                                    currentPhone = lastPhone.Substring(0, lastPhone.IndexOf(','));
                                    lastPhone = lastPhone.Remove(0, lastPhone.IndexOf(',') + 1);

                                    if (currentPhone[0] == '+')
                                    {
                                        currentPhone = Convert.ToInt64(currentPhone).ToString("+# (###)-###-##-##");
                                    }
                                    else
                                    {
                                        currentPhone = Convert.ToInt64(currentPhone).ToString("# (###)-###-##-##");
                                    }

                                    Phone = Phone + currentPhone;
                                }

                            }
                            else
                            {
                                if (Phone.Length <= 20)
                                {
                                    if (Phone[0] == '+')
                                    {
                                        Phone = Convert.ToInt64(Phone).ToString("+# (###)-###-##-##");
                                    }
                                    else
                                    {
                                        Phone = Convert.ToInt64(Phone).ToString("# (###)-###-##-##");
                                    }
                                }

                            }

                        }





                        ClientPhoneTextBox.Text = Phone;
                        ClientAdressTextBox.Text = dt1.Rows[i].ItemArray[3].ToString();
                        PrimechanieTextBox.Text = dt1.Rows[i].ItemArray[4].ToString();
                        BlackListComboBox.Text = KlientBlistDecoder(dt1.Rows[i].ItemArray[5].ToString());
                        ClientAboutUsComboBox.Text = dt1.Rows[i].ItemArray[7].ToString();
                        label30.Text = "Редактирование клиенат номер: " + dt1.Rows[i].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }
        private string KlientBlistDecoder(string blist)
        {
            if (blist == "0") return "Не проблемный";
            else if (blist == "1") return "Проблемный";
            else return "";

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
