using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using MySqlConnector;
using MySqlX.XDevAPI;
using OfficeOpenXml.Style;


namespace MyWork2
{
    public class BDWorker
    {
        Form1 mainForm;

        public List<string> problemIDs = new List<string>();

        private MySqlCommand m_sqlCmd;
        private Boolean firstCheck = false;
        public BDWorker(Form1 fm)
        {
            mainForm = fm;
        }


        //\\\\172.16.15.111\\share2\\baza.sqlite
        //\\172.16.15.111\share2\baza.sqlite




        public String getParamConnection()
        {

            string connStr = "Server=" + TemporaryBase.BDPath + ";Port=3306;Database=" + TemporaryBase.sqlDatabase + ";Uid=" + TemporaryBase.sqlLogin + ";Pwd=" + TemporaryBase.sqlPassword + ";Charset=utf8mb4;Convert Zero Datetime=True;SslMode=None;AllowPublicKeyRetrieval=True;";
            //Восстанавливаем путь к базе данных
            if (TemporaryBase.BDPath != "-1")
            {
                if (TemporaryBase.BDPath == "")
                {
                    IniFile INIF = new IniFile("Config.ini");
                    if (INIF.KeyExists("PROGRAMM_SETTINGS", "BDPath"))
                    {
                        TemporaryBase.BDPath = INIF.ReadINI("PROGRAMM_SETTINGS", "BDPath");

                        if (TemporaryBase.BDPath != "")
                        {
                            connStr = "Server=" + TemporaryBase.BDPath + ";Port=3306;Database=" + TemporaryBase.sqlDatabase + ";Uid=" + TemporaryBase.sqlLogin + ";Pwd=" + TemporaryBase.sqlPassword + ";Charset=utf8mb4;Convert Zero Datetime=True;SslMode=None;AllowPublicKeyRetrieval=True;";
                        }
                        else
                        {
                            TemporaryBase.BDPath = "-1";
                        }

                    }
                    //172.16.15.111

                }
                else
                {
                    connStr = "Server=" + TemporaryBase.BDPath + ";Port=3306;Database=" + TemporaryBase.sqlDatabase + ";Uid=" + TemporaryBase.sqlLogin + ";Pwd=" + TemporaryBase.sqlPassword + ";Charset=utf8mb4;Convert Zero Datetime=True;SslMode=None;AllowPublicKeyRetrieval=True;";
                }
            }

            return connStr;
        }



        public MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(getParamConnection());
        }




        public Boolean checkConnection()
        {

            using (MySqlConnection conn = new MySqlConnection(getParamConnection()))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных:\n" + ex.Message, "MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

        }




        //Создание базы данных
        public void CreateBd()
        {
            // Получаем подключение к MySQL
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                // Открываем соединение
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                // Создаём таблицу, если она ещё не существует
                m_sqlCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Catalog (
                id INT AUTO_INCREMENT PRIMARY KEY,
                Data_priema DATETIME NULL,
                Data_vidachi DATETIME NULL,
                Data_predoplaty DATETIME NULL,
                surname VARCHAR(255),
                phone VARCHAR(50),
                AboutUs TEXT,
                whatremont_id INT,
                brand VARCHAR(100),
                model VARCHAR(100),
                SerialNumber VARCHAR(100),
                sostoyanie TEXT,
                komplektonst TEXT,
                polomka TEXT,
                kommentarij TEXT,
                predvaritelnaya_stoimost double DEFAULT 0.00,
                Predoplata double DEFAULT 0.00,
                Zatrati double DEFAULT 0.00,
                okonchatelnaya_stoimost_remonta double DEFAULT 0.00,
                Skidka double DEFAULT 0.00,
                Status_remonta VARCHAR(100),
                master VARCHAR(100),
                vipolnenie_raboti TEXT,
                Garanty VARCHAR(100),
                wait_zakaz VARCHAR(50),
                Adress VARCHAR(255),
                Image_key VARCHAR(255),
                AdressSC VARCHAR(255),
                DeviceColour VARCHAR(50),
                ClientId INT,
                Barcode VARCHAR(20),
                Deleted TINYINT(1) DEFAULT 0
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ";
                m_sqlCmd.ExecuteNonQuery();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу установить соединение с MySQL или выполнить команду:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Всегда закрываем соединение
                if (m_dbConn?.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }
        /*
                public string convertDate(string input)
                {
                    if (string.IsNullOrWhiteSpace(input)) return null;

                    string[] formats = {
                        "dd.MM.yyyy HH:mm:ss",
                        "yyyy-MM-dd HH:mm:ss",
                        "dd.MM.yyyy",
                        "yyyy-MM-dd",
                        "dd-MM-yyyy HH:mm",     
                        "dd-MM-yyyy HH:mm:ss",   
                        "dd-MM-yyyy"              
                    };


                    DateTime dt;
                    if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        return dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    return null;
                }*/

        /*
        public string convertDateRev(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Попробуем стандартный парсинг
            if (DateTime.TryParse(
                    input,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out DateTime dt))
            {
                // Форматируем в обратном виде
                return dt.ToString("yyyy.MM.dd HH:mm");
            }

            // Пользовательские форматы — точно такие же, как в оригинале
            string[] customFormats = new[]
                    {
                // Год-месяц-день
                "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'H:m:s",
                "yyyy-MM-dd HH:mm:ss",      "yyyy-MM-dd H:m:s",
                "yyyy/MM/dd HH:mm:ss",      "yyyy/MM/dd H:m:s",
                "yyyyMMdd HH:mm:ss",        "yyyyMMdd H:m:s",
                "yyyy-MM-dd'T'HH:mm",       "yyyy-MM-dd H:m",
                "yyyy-MM-dd HH:mm",         "yyyy/MM/dd H:m",
                "yyyy/MM/dd HH:mm",         "yyyyMMdd H:m",
                // День.месяц.год
                "dd.MM.yyyy HH:mm:ss",      "dd.MM.yyyy H:m:s",
                "dd.MM.yyyy H:mm:ss",       "dd.MM.yyyy HH:m:s",
                "dd.MM.yyyy HH:mm",         "dd.MM.yyyy H:m",
                // День/месяц/год
                "dd/MM/yyyy HH:mm:ss",      "dd/MM/yyyy H:m:s",
                "dd/MM/yyyy H:m:ss",        "dd/MM/yyyy HH:m:s",
                "dd/MM/yyyy HH:mm",         "dd/MM/yyyy H:m"
            };

            if (DateTime.TryParseExact(
                    input,
                    customFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out dt))
            {
                return dt.ToString("yyyy.MM.dd HH:mm");
            }

            // Если не удалось распарсить — возвращаем null
            return null;
        }


        public string convertDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Попробуем сначала стандартный парсинг (учитывает множество форматов)
            if (DateTime.TryParse(
                    input,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out DateTime dt))
            {
                // Форматируем без секунд
                return dt.ToString("dd.MM.yyyy HH:mm");
            }

            string[] customFormats = new[]
            {
                // Год-месяц-день
                "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'H:m:s",
                "yyyy-MM-dd HH:mm:ss",      "yyyy-MM-dd H:m:s",
                "yyyy/MM/dd HH:mm:ss",      "yyyy/MM/dd H:m:s",
                "yyyyMMdd HH:mm:ss",        "yyyyMMdd H:m:s",
                "yyyy-MM-dd'T'HH:mm",       "yyyy-MM-dd H:m",
                "yyyy-MM-dd HH:mm",         "yyyy/MM/dd H:m",
                "yyyy/MM/dd HH:mm",         "yyyyMMdd H:m",

                "yyyy-MM-dd'T'HH-mm-ss", "yyyy-MM-dd HH-mm-ss",
                "yyyy-MM-dd HH-mm-ss",   "yyyy-MM-dd HH-mm",

                "dd-MM-yyyy HH-mm",  "dd-MM-yyyy HH-mm-ss",
                "dd-MM-yyyy HH:mm",  "dd-MM-yyyy HH:mm:ss",


                // День.месяц.год
                "dd.MM.yyyy HH:mm:ss",      "dd.MM.yyyy H:m:s",
                "dd.MM.yyyy H:mm:ss",       "dd.MM.yyyy HH:m:s",
                "dd.MM.yyyy HH:mm",         "dd.MM.yyyy H:m",
                // День/месяц/год
                "dd/MM/yyyy HH:mm:ss",      "dd/MM/yyyy H:m:s",
                "dd/MM/yyyy H:mm:ss",       "dd/MM/yyyy HH:m:s",
                "dd/MM/yyyy HH:mm",         "dd/MM/yyyy H:m"
            };

            if (DateTime.TryParseExact(
                    input,
                    customFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out dt))
            {
                return dt.ToString("dd.MM.yyyy HH:mm");
            }


            return null;
        }
        */



        private static readonly CultureInfo ruCulture = new CultureInfo("ru-RU");
        private static readonly CultureInfo invCulture = CultureInfo.InvariantCulture;
        // Поддержка нестандартного дефисного времени
        private static readonly string[] extraFormats = new[]
        {
            "dd-MM-yyyy HH-mm",
            "dd-MM-yyyy HH-mm-ss",
            "yyyy-MM-dd HH-mm",
            "yyyy-MM-dd HH-mm-ss"
        };

        public string convertDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            DateTime dt;

            // Быстрый путь — точный русский формат
            if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", ruCulture, DateTimeStyles.None, out dt))
                return dt.ToString("dd.MM.yyyy HH:mm");

            // Общий русский стиль
            if (DateTime.TryParse(input, ruCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("dd.MM.yyyy HH:mm");

            if (DateTime.TryParseExact(input, extraFormats, ruCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("dd.MM.yyyy HH:mm");

            // Последняя попытка
            if (DateTime.TryParse(input, invCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("dd.MM.yyyy HH:mm");

            return null;
        }




        public string convertDateRev(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            DateTime dt;

            // Быстрый путь — точный русский формат
            if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", ruCulture, DateTimeStyles.None, out dt))
                return dt.ToString("yyyy-MM-dd HH:mm");

            // Общий русский стиль
            if (DateTime.TryParse(input, ruCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("yyyy-MM-dd HH:mm");

            if (DateTime.TryParseExact(input, extraFormats, ruCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("yyyy-MM-dd HH:mm");

            // Последняя попытка
            if (DateTime.TryParse(input, invCulture, DateTimeStyles.AssumeLocal, out dt))
                return dt.ToString("yyyy-MM-dd HH:mm");

            return null;
        }



        //Создание базы данных
        public void CreateBd(string incrAutoNumber)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            // Проверяем, что incrAutoNumber непустой и это число > 0
            if (int.TryParse(incrAutoNumber, out int nextValue) && nextValue > 0)
            {
                try
                {
                    m_dbConn.Open();
                    m_sqlCmd.Connection = m_dbConn;

                    // Устанавливаем следующий ID для авто-инкремента
                    m_sqlCmd.CommandText = $"ALTER TABLE Catalog AUTO_INCREMENT = {nextValue};";
                    m_sqlCmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Не удалось обновить AUTO_INCREMENT для таблицы Catalog:\n{ex.Message}",
                        "Ошибка MySQL",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                finally
                {
                    if (m_dbConn.State == ConnectionState.Open)
                        m_dbConn.Close();
                }
            }
            else
            {
                // Ничего не делаем, если передано некорректное значение
                // Можно при желании залогировать или вывести предупреждение
            }
        }






        // получает ID последней записи в базе (MySQL)
        public int BdReadAdvertsDataTop()
        {
            int lastId = 0;
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT id FROM Catalog ORDER BY id DESC LIMIT 1"
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    lastId = Convert.ToInt32(dTable.Rows[0]["id"]);
                }
                else
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: База данных пуста",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении последнего ID из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return lastId;
        }



        // ПРоверяет есть ли такая запись
        // проверяет, существует ли запись с заданным id в таблице Catalog (MySQL)
        public int CatlogIDExists(string catalogId)
        {
            int exists = 0;
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // используем параметризованный запрос, чтобы избежать инъекций
                CommandText = "SELECT EXISTS(SELECT 1 FROM Catalog WHERE id = @id)"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", catalogId);

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    // результат EXISTS — 1 или 0
                    exists = Convert.ToInt32(dTable.Rows[0][0]);
                }
                else
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: База данных пуста",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при проверке наличия записи в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return exists;
        }



        // получает ID первой записи в базе
        // получает ID первой записи в таблице Catalog (MySQL)
        public int BdReadAdvertsDataFirt()
        {
            int firstId = 0;
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT id FROM Catalog LIMIT 1"
            };

            try
            {
                m_dbConn.Open();

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    firstId = Convert.ToInt32(dTable.Rows[0]["id"]);
                }
                else
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: База данных пуста",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении первого ID из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return firstId;
        }


        // Чтение из базы данных
        // читает записи из Catalog: trfl = false — только выданные, trfl = true — только не выданные
        public DataTable BdRead(bool trfl)
        {
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn
            };

            try
            {
                m_dbConn.Open();

                // Если соединение не открылось — выходим
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dTable;
                }

                // Собираем запрос в зависимости от флага trfl
                m_sqlCmd.CommandText = trfl
                    ? "SELECT * FROM Catalog WHERE Data_vidachi IS NULL"
                    : "SELECT * FROM Catalog WHERE Data_vidachi IS NOT NULL";

                // Заполняем DataTable через MySqlDataAdapter
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Что-то пошло не так при чтении из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }



        // Чтение всех записей из базы данных
        // возвращает все записи из таблицы Catalog (MySQL)
        public DataTable BdReadAll()
        {
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM Catalog"
            };

            try
            {
                m_dbConn.Open();

                // Проверяем соединение
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dTable;
                }

                // Заполняем DataTable через MySqlDataAdapter
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Что-то пошло не так при чтении из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }



        // Чтение одной записи из таблицы Catalog по идентификатору (MySQL)
        public DataTable BdReadOneEditor(string id_bd)
        {
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM Catalog WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dTable;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Что-то пошло не так при чтении из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }



        // Чтение из базы, для Графиков
        // Чтение данных для графика между двумя датами по мастеру (MySQL)
        public DataSet BdReadGraf(string calendar1, string calendar2, string master)
        {
            var dSet = new DataSet();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM Catalog
            WHERE Data_vidachi BETWEEN @start AND @end
              AND Data_vidachi IS NOT NULL
              AND master LIKE @master"
            };
            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@start", calendar1);
            m_sqlCmd.Parameters.AddWithValue("@end", calendar2);
            m_sqlCmd.Parameters.AddWithValue("@master", $"%{master.ToUpper()}%");

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dSet;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dSet);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении данных для графика из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dSet;
        }


        // Чтение из базы для графиков с новыми временными форматами (MySQL)
        public List<VirtualClient> BdReadGrafList(string calendar1, string calendar2, string master, string AdressSCINBD, string whatRemont, string brand, string aboutUs)
        {
            List<VirtualClient> vClientList = new List<VirtualClient>();
            DataTable dt1 = new DataTable();
            String sqlQuery;

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            if (m_dbConn.State != ConnectionState.Open)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Соединение с базой данных потеряно, установите соединение" + Environment.NewLine);
                return vClientList;
            }

            try
            {
                sqlQuery = string.Format(@"
            SELECT 
                c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                cm.FIO, cm.Phone, cm.AboutUs,
                w.name, b.name, m.name, c.SerialNumber,
                c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati, c.okonchatelnaya_stoimost_remonta,
                c.Skidka, c.Status_remonta, c.master, c.vipolnenie_raboti,
                c.Garanty, c.wait_zakaz, cm.Adress, c.Image_key, c.AdressSC, c.DeviceColour,
                c.ClientId, c.Barcode, z.name
            FROM Catalog c
            JOIN ClientsMap cm ON c.ClientId = cm.id
            LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
            LEFT JOIN whatremont w ON w.id = c.whatremont_id    
            LEFT JOIN brand b ON b.id = c.brand_id
            LEFT JOIN model m ON m.id = c.model_id
            WHERE c.Data_vidachi IS NOT NULL
              AND c.master LIKE '%{0}%'
              AND c.AdressSC LIKE '%{1}%'
              AND (w.name LIKE @{2} OR w.name IS NULL)
              AND c.brand LIKE '%{3}%'
              AND cm.AboutUs LIKE '%{4}%'",
                      master.ToUpper().Trim(), AdressSCINBD.ToUpper().Trim(), whatRemont.ToUpper().Trim(),
                      brand.ToUpper().Trim(), aboutUs.ToUpper().Trim());

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(sqlQuery, m_dbConn))
                {
                    adapter.Fill(dt1);
                }

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string vidachaDateStr = dt1.Rows[i].ItemArray[2]?.ToString();
                    if (DateTime.TryParse(vidachaDateStr, out DateTime vidachaDate))
                    {
                        DateTime from = DateTime.Parse(calendar1);
                        DateTime to = DateTime.Parse(calendar2);
                        if (vidachaDate.Date >= from.Date && vidachaDate.Date <= to.Date)
                        {
                            VirtualClient vc = new VirtualClient(
                                dt1.Rows[i].ItemArray[0].ToString(),
                                dt1.Rows[i].ItemArray[1].ToString(),
                                dt1.Rows[i].ItemArray[2].ToString(),
                                dt1.Rows[i].ItemArray[3].ToString(),
                                dt1.Rows[i].ItemArray[4].ToString(),
                                dt1.Rows[i].ItemArray[5].ToString(),
                                dt1.Rows[i].ItemArray[6].ToString(),
                                dt1.Rows[i].ItemArray[7].ToString(),
                                dt1.Rows[i].ItemArray[8].ToString(),
                                dt1.Rows[i].ItemArray[9].ToString(),
                                dt1.Rows[i].ItemArray[10].ToString(),
                                dt1.Rows[i].ItemArray[11].ToString(),
                                dt1.Rows[i].ItemArray[12].ToString(),
                                dt1.Rows[i].ItemArray[13].ToString(),
                                dt1.Rows[i].ItemArray[14].ToString(),
                                dt1.Rows[i].ItemArray[15].ToString(),
                                dt1.Rows[i].ItemArray[16].ToString(),
                                dt1.Rows[i].ItemArray[17].ToString(),
                                dt1.Rows[i].ItemArray[18].ToString(),
                                dt1.Rows[i].ItemArray[19].ToString(),
                                dt1.Rows[i].ItemArray[20].ToString(),
                                dt1.Rows[i].ItemArray[21].ToString(),
                                dt1.Rows[i].ItemArray[22].ToString(),
                                dt1.Rows[i].ItemArray[23].ToString(),
                                dt1.Rows[i].ItemArray[24].ToString(),
                                dt1.Rows[i].ItemArray[25].ToString(),
                                dt1.Rows[i].ItemArray[26].ToString(),
                                false,
                                dt1.Rows[i].ItemArray[27].ToString(),
                                dt1.Rows[i].ItemArray[28].ToString(),
                                int.TryParse(dt1.Rows[i].ItemArray[29].ToString(), out int clid) ? clid : -1,
                                dt1.Rows[i].ItemArray[30].ToString(),
                                //ZAKAZCHIK_SUDA
                                dt1.Rows[i].ItemArray[31].ToString()
                            );
                            vClientList.Add(vc);
                        }
                    }
                }

                return vClientList;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при чтении из базы MySQL:\n{ex.Message}", "Ошибка MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return vClientList;
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }





        // Чтение из базы для SMS между двумя датами и по фильтрам (MySQL)
        public List<VirtualClient> BdReadSmsList(string calendar1, string calendar2, string AdressSCINBD, string whatRemont, string brand)
        {
            List<VirtualClient> vClientList = new List<VirtualClient>();
            DataTable dt1 = new DataTable();
            string sqlQuery;

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;

            if (m_dbConn.State != ConnectionState.Open)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Соединение с базой данных потеряно, установите соединение" + Environment.NewLine);
                return vClientList;
            }

            try
            {
                sqlQuery = string.Format(@"
            SELECT c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                   cm.FIO, cm.Phone, cm.AboutUs,
                   w.name, b.name, m.name, c.SerialNumber,
                   c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                   c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati,
                   c.okonchatelnaya_stoimost_remonta, c.Skidka, c.Status_remonta,
                   c.master, c.vipolnenie_raboti, c.Garanty, c.wait_zakaz,
                   cm.Adress, c.Image_key, c.AdressSC, c.DeviceColour,
                   c.ClientId, c.Barcode, z.name
            FROM Catalog c
            JOIN ClientsMap cm ON c.ClientId = cm.id
            LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
            LEFT JOIN brand b ON b.id = c.brand_id
            LEFT JOIN model m ON m.id = c.model_id
            WHERE c.Data_vidachi IS NOT NULL
              AND c.AdressSC LIKE '%{0}%'
              AND (w.name LIKE @{1} OR w.name IS NULL)
              AND (b.name LIKE '%{2}%' OR b.name IS NULL)",

                      AdressSCINBD.ToUpper().Trim(), whatRemont.ToUpper().Trim(), brand.ToUpper().Trim());

                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlQuery, m_dbConn);
                adapter.Fill(dt1);

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    DateTime dataVidachi;
                    if (DateTime.TryParse(dt1.Rows[i].ItemArray[2].ToString(), out dataVidachi))
                    {
                        DateTime start = DateTime.Parse(calendar1);
                        DateTime end = DateTime.Parse(calendar2);
                        if (dataVidachi.Date >= start.Date && dataVidachi.Date <= end.Date)
                        {
                            VirtualClient vc = new VirtualClient(
                                dt1.Rows[i].ItemArray[0].ToString(),
                                dt1.Rows[i].ItemArray[1].ToString(),
                                dt1.Rows[i].ItemArray[2].ToString(),
                                dt1.Rows[i].ItemArray[3].ToString(),
                                dt1.Rows[i].ItemArray[4].ToString(),
                                dt1.Rows[i].ItemArray[5].ToString(),
                                dt1.Rows[i].ItemArray[6].ToString(),
                                dt1.Rows[i].ItemArray[7].ToString(),
                                dt1.Rows[i].ItemArray[8].ToString(),
                                dt1.Rows[i].ItemArray[9].ToString(),
                                dt1.Rows[i].ItemArray[10].ToString(),
                                dt1.Rows[i].ItemArray[11].ToString(),
                                dt1.Rows[i].ItemArray[12].ToString(),
                                dt1.Rows[i].ItemArray[13].ToString(),
                                dt1.Rows[i].ItemArray[14].ToString(),
                                dt1.Rows[i].ItemArray[15].ToString(),
                                dt1.Rows[i].ItemArray[16].ToString(),
                                dt1.Rows[i].ItemArray[17].ToString(),
                                dt1.Rows[i].ItemArray[18].ToString(),
                                dt1.Rows[i].ItemArray[19].ToString(),
                                dt1.Rows[i].ItemArray[20].ToString(),
                                dt1.Rows[i].ItemArray[21].ToString(),
                                dt1.Rows[i].ItemArray[22].ToString(),
                                dt1.Rows[i].ItemArray[23].ToString(),
                                dt1.Rows[i].ItemArray[24].ToString(),
                                dt1.Rows[i].ItemArray[25].ToString(),
                                dt1.Rows[i].ItemArray[26].ToString(),
                                false,
                                dt1.Rows[i].ItemArray[27].ToString(),
                                dt1.Rows[i].ItemArray[28].ToString(),
                                int.TryParse(dt1.Rows[i].ItemArray[29].ToString(), out var clientId) ? clientId : -1,
                                dt1.Rows[i].ItemArray[30].ToString(),
                                //ZAKAZCHIK_SUDA
                                dt1.Rows[i].ItemArray[31].ToString()
                            );

                            vClientList.Add(vc);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при чтении данных из MySQL:\n" + ex.ToString());
            }

            return vClientList;
        }


        private string FormatPhones(string Phone)
        {

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

            return Phone;

        }





        //ZAKAZCHIK_SUDA
        // Чтение из базы для выпадающих списков техники по фильтру AdressSC (MySQL)
        public List<VirtualClient> BdReadListTechnics(string adressSC)
        {
            List<VirtualClient> vClientList = new List<VirtualClient>();
            DataTable dt1 = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();


            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT
                c.id,
                c.Data_priema,
                c.Data_vidachi,
                c.Data_predoplaty,
                cm.FIO,
                cm.Phone,
                cm.AboutUs,
                w.name,
                b.name,
                m.name,
                c.SerialNumber,
                c.Sostoyanie,
                c.komplektonst,
                c.polomka,
                c.kommentarij,
                c.predvaritelnaya_stoimost,
                c.Predoplata,
                c.Zatrati,
                c.okonchatelnaya_stoimost_remonta,
                c.Skidka,
                c.Status_remonta,
                c.master,
                c.vipolnenie_raboti,
                c.Garanty,
                c.wait_zakaz,
                cm.Adress,
                c.Image_key,
                c.AdressSC,
                c.DeviceColour,
                c.ClientId,
                c.Barcode,
                z.name
            FROM Catalog AS c
            JOIN ClientsMap AS cm ON c.ClientId = cm.id
            LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
            LEFT JOIN whatremont w ON w.id = c.whatremont_id
            LEFT JOIN brand b ON b.id = c.brand_id
            LEFT JOIN model m ON m.id = c.model_id
            WHERE UPPER(c.AdressSC) LIKE @adressSC;"
            };

            m_sqlCmd.Parameters.AddWithValue("@adressSC", $"%{adressSC.Trim().ToUpper()}%");

            try
            {
                m_dbConn.Open();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt1);
                }

                foreach (DataRow row in dt1.Rows)
                {

                    string data_priema = "";
                    string data_vidachi = "";
                    string data_predoplaty = "";

                    if (row["Data_priema"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_priema"]);
                        data_priema = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }

                    if (row["Data_vidachi"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_vidachi"]);
                        data_vidachi = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }

                    if (row["Data_predoplaty"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_predoplaty"]);
                        data_predoplaty = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }
                    //VOT
                    VirtualClient vc = new VirtualClient(
                            row.ItemArray[0].ToString(),
                                data_priema,
                                data_vidachi,
                                data_predoplaty,
                            row.ItemArray[4].ToString(),
                            FormatPhones(row.ItemArray[5].ToString()),
                            row.ItemArray[6].ToString(),
                            row.ItemArray[7].ToString(),
                            row.ItemArray[8].ToString(),
                            row.ItemArray[9].ToString(),
                            row.ItemArray[10].ToString(),
                            row.ItemArray[11].ToString(),
                            row.ItemArray[12].ToString(),
                            row.ItemArray[13].ToString(),
                            row.ItemArray[14].ToString(),
                            row.ItemArray[15].ToString(),
                            row.ItemArray[16].ToString(),
                            row.ItemArray[17].ToString(),
                            row.ItemArray[18].ToString(),
                            row.ItemArray[19].ToString(),
                            row.ItemArray[20].ToString(),
                            row.ItemArray[21].ToString(),
                            row.ItemArray[22].ToString(),
                            row.ItemArray[23].ToString(),
                            row.ItemArray[24].ToString(),
                            row.ItemArray[25].ToString(),
                            row.ItemArray[26].ToString(),
                            false,
                            row.ItemArray[28].ToString(),
                            row.ItemArray[29].ToString(),
                            int.TryParse(row.ItemArray[30].ToString(), out var clientId) ? clientId : -1,
                            row.ItemArray[30].ToString(),
                            //ZAKAZCHIK_SUDA
                            row.ItemArray[31].ToString()
                        );



                    vClientList.Add(vc);
                }



            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при чтении техники из MySQL:\n{ex.Message}", "Ошибка MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return vClientList;
        }


        public List<VirtualClient> ExportPhonesVCList(string from, string to)
        {
            List<VirtualClient> vClientList = new List<VirtualClient>();
            DataTable dt1 = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT 
                c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                cm.FIO, cm.Phone, cm.AboutUs,
                w.name, b.name, m.name, c.SerialNumber,
                c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati, 
                c.okonchatelnaya_stoimost_remonta, c.Skidka, c.Status_remonta, c.master,
                c.vipolnenie_raboti, c.Garanty, c.wait_zakaz,
                cm.Adress, c.Image_key, c.AdressSC, c.DeviceColour,
                c.ClientId, c.Barcode, z.name
            FROM Catalog AS c
            JOIN ClientsMap AS cm ON c.ClientId = cm.id
            LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
            LEFT JOIN whatremont w ON w.id = c.whatremont_id
            LEFT JOIN brand b ON b.id = c.brand_id
            LEFT JOIN model m ON m.id = c.model_id
            WHERE c.id BETWEEN @from AND @to;"
            };

            m_sqlCmd.Parameters.AddWithValue("@from", int.Parse(from));
            m_sqlCmd.Parameters.AddWithValue("@to", int.Parse(to));

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt1);
                }

                foreach (DataRow row in dt1.Rows)
                {
                    object[] a = row.ItemArray;


                    string data_priema = "";
                    string data_vidachi = "";
                    string data_predoplaty = "";

                    if (row["Data_priema"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_priema"]);
                        data_priema = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }

                    if (row["Data_vidachi"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_vidachi"]);
                        data_vidachi = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }

                    if (row["Data_predoplaty"] != DBNull.Value)
                    {
                        var dt = Convert.ToDateTime(row["Data_predoplaty"]);
                        data_predoplaty = dt == DateTime.MinValue || dt.Year == 1 ? "" : dt.ToString("yyyy-MM-dd HH:mm");
                    }



                    VirtualClient vc = new VirtualClient(
                            row.ItemArray[0].ToString(),
                                data_priema,
                                data_vidachi,
                                data_predoplaty,
                            row.ItemArray[4].ToString(),
                            FormatPhones(row.ItemArray[5].ToString()),
                            row.ItemArray[6].ToString(),
                            row.ItemArray[7].ToString(),
                            row.ItemArray[8].ToString(),
                            row.ItemArray[9].ToString(),
                            row.ItemArray[10].ToString(),
                            row.ItemArray[11].ToString(),
                            row.ItemArray[12].ToString(),
                            row.ItemArray[13].ToString(),
                            row.ItemArray[14].ToString(),
                            row.ItemArray[15].ToString(),
                            row.ItemArray[16].ToString(),
                            row.ItemArray[17].ToString(),
                            row.ItemArray[18].ToString(),
                            row.ItemArray[19].ToString(),
                            row.ItemArray[20].ToString(),
                            row.ItemArray[21].ToString(),
                            row.ItemArray[22].ToString(),
                            row.ItemArray[23].ToString(),
                            row.ItemArray[24].ToString(),
                            row.ItemArray[25].ToString(),
                            row.ItemArray[26].ToString(),
                            false,
                            row.ItemArray[28].ToString(),
                            row.ItemArray[29].ToString(),
                            int.TryParse(row.ItemArray[30].ToString(), out var clientId) ? clientId : -1,
                            row.ItemArray[30].ToString(),
                            //ZAKAZCHIK_SUDA
                            row.ItemArray[31].ToString()
                        );


                    vClientList.Add(vc);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при чтении из MySQL:\n{ex.Message}", "Ошибка MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return vClientList;
        }



        // Чтение из базы данных
        // Ищет запись по штрихкоду и возвращает её ID (MySQL)
        public string BdReadBarcode(string barcode)
        {
            var dTable = new DataTable();
            string readData = "";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT id FROM Catalog WHERE Barcode = @barcode"
            };
            m_sqlCmd.Parameters.AddWithValue("@barcode", barcode);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return readData;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    return dTable.Rows[0]["id"].ToString();
                }
                else
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Не могу найти запись со штрихкодом «{barcode}»",
                        "Не найдено",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении записи по штрихкоду из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return readData;
        }


        // Проверяет, есть ли в таблице Catalog записи со статусом "Согласование с клиентом" (MySQL)
        public bool BdStatusSaglosSKlient()
        {
            bool exists = false;
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT id FROM Catalog WHERE Status_remonta = @status"
            };
            m_sqlCmd.Parameters.AddWithValue("@status", "Согласование с клиентом");

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                    return false;

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                exists = dTable.Rows.Count > 0;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при проверке статусов в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return exists;
        }



        // Чтение из базы данных
        // Читает одно значение из поля readWhat для записи с указанным id (MySQL)
        public string BdReadOne(string readWhat, string id_bd)
        {
            var dTable = new DataTable();
            string readData = "";

            MySqlConnection m_dbConn = GetMySqlConnection();
            // Собираем команду с именем поля (безопаснее — ограничить допустимые имена)
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = $"SELECT `{readWhat}` FROM Catalog WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return readData;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    readData = dTable.Rows[0][0].ToString();
                }
                else
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Не удалось прочитать поле «{readWhat}» для записи ID {id_bd}",
                        "Не найдено",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return readData;
        }

        //STEP_NEW_FIELD_CLIENTSMAP
        //Запись в базу данных
        // Запись новой садовой заявки в таблицу Catalog (MySQL)
        public void BdWrite(
            string Data_priema,
            string Data_vidachi,
            string Data_predoplaty,
            string surname,
            string phone,
            string AboutUs,
            string WhatRemont,
            string brand,
            string model,
            string SerialNumber,
            string sostoyanie,
            string komplektonst,
            string polomka,
            string kommentarij,
            string predvaritelnaya_stoimost,
            string Predoplata,
            string Zatrati,
            string okonchatelnaya_stoimost_remonta,
            string Skidka,
            string Status_remonta,
            string master,
            string vipolnenie_raboti,
            string Garanty,
            string wait_zakaz,
            string Adress,
            string Image_key,
            string AdressSC,
            string DeviceColour,
            string ClientId,
            string zakazchik
            )
        {


            int zakazchik_id = 0;

            if (zakazchik.Trim().Length > 0)
            {

                zakazchik_id = CheckSpravochnik("zakazchik", zakazchik.Trim());

                if (zakazchik_id < 0)
                {
                    AddSpravochnikNew("zakazchik", zakazchik);
                }

                zakazchik_id = CheckSpravochnik("zakazchik", zakazchik.Trim());

            }



            int whatRemont_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

                if (whatRemont_id < 0)
                {
                    AddSpravochnikNew("whatremont", WhatRemont);
                }

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

            }




            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO Catalog
            (Data_priema, Data_vidachi, Data_predoplaty, surname, phone, AboutUs,
             whatremont_id, brand, model, SerialNumber, sostoyanie, komplektonst,
             polomka, kommentarij, predvaritelnaya_stoimost, Predoplata, Zatrati,
             okonchatelnaya_stoimost_remonta, Skidka, Status_remonta, master,
             vipolnenie_raboti, Garanty, wait_zakaz, Adress, Image_key,
             AdressSC, DeviceColour, ClientId, Barcode, Deleted, zakazchik_id)
            VALUES
            (@dp, @dv, @dd, @surname, @phone, @aboutUs,
             @whatRemont_id, @brand, @model, @sn, @sost, @komp,
             @pol, @komment, @predStoim, @predopl, @zatrati,
             @okonchStoim, @skidka, @status, @master, @vipol,
             @garanty, @wait, @adress, @imgKey,
             @adressSC, @devColor, @clientId, @barcode, @deleted, @zakazchik_id);
        "
            };

            // Генерируем штрихкод
            var now = DateTime.Now;
            string barCODE = now.ToString("ddMMHHmmss");
            barCODE += now.ToString("yyyy").Substring(3);    // последняя цифра года
            barCODE = barcodeLastDigit(barCODE);             // метод расчёта контрольной цифры

            Data_priema = convertDateRev(Data_priema);

            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@dp", Data_priema);
            m_sqlCmd.Parameters.AddWithValue("@dv", convertDateRev(Data_vidachi));
            m_sqlCmd.Parameters.AddWithValue("@dd", convertDateRev(Data_predoplaty));


            m_sqlCmd.Parameters.AddWithValue("@surname", surname.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@aboutUs", AboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@whatRemont_id", whatRemont_id);
            m_sqlCmd.Parameters.AddWithValue("@brand", brand.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@model", model.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@sn", SerialNumber.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@sost", sostoyanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@komp", komplektonst.Trim());
            m_sqlCmd.Parameters.AddWithValue("@pol", polomka.Trim());
            m_sqlCmd.Parameters.AddWithValue("@komment", kommentarij.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predStoim", predvaritelnaya_stoimost.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predopl", Predoplata.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zatrati", Zatrati.Trim());
            m_sqlCmd.Parameters.AddWithValue("@okonchStoim", okonchatelnaya_stoimost_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@skidka", Skidka);
            m_sqlCmd.Parameters.AddWithValue("@status", Status_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@master", master.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@vipol", vipolnenie_raboti.Trim());
            m_sqlCmd.Parameters.AddWithValue("@garanty", Garanty.Trim());
            m_sqlCmd.Parameters.AddWithValue("@wait", wait_zakaz);
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress);
            m_sqlCmd.Parameters.AddWithValue("@imgKey", Image_key);
            m_sqlCmd.Parameters.AddWithValue("@adressSC", AdressSC);
            m_sqlCmd.Parameters.AddWithValue("@devColor", DeviceColour);
            m_sqlCmd.Parameters.AddWithValue("@clientId", ClientId);
            m_sqlCmd.Parameters.AddWithValue("@barcode", barCODE);
            m_sqlCmd.Parameters.AddWithValue("@deleted", 0);
            m_sqlCmd.Parameters.AddWithValue("@zakazchik_id", zakazchik_id);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }


            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }


        }



        //STEP_NEW_FIELD_CLIENTSMAP
        //Добавить новый параметр в справочник
        public void AddSpravochnikNew(string table, string name)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"INSERT INTO " + table + " (name) VALUES (@name);"
            };


            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@name", name);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }


            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }


        }

        //STEP_NEW_FIELD_CLIENTSMAP
        //Добавить новый параметр в справочник
        public int CheckSpravochnik(String table, string name)
        {


            int lastId = 0;
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM  " + table + " WHERE name = \"" + name + "\" ORDER BY id DESC LIMIT 1"
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    lastId = Convert.ToInt32(dTable.Rows[0]["id"]);
                }
                else
                {
                    lastId = -1;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении последнего ID из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }



            return lastId;
        }

        //STEP_NEW_FIELD_CLIENTSMAP
        //Добавить новый параметр в справочник
        public string getNameInSpravochnik(string table, string id)
        {


            string name = "";
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();


            string cmd = "SELECT * FROM " + table + " WHERE id = \"" + id + "\" ORDER BY id DESC LIMIT 1";

            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = cmd
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    name = dTable.Rows[0]["name"].ToString();
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении последнего ID из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }



            return name;
        }





        //ZAKAZCHIK_SUDA
        //Изменение в базе данных
        // Обновление записи в таблице Catalog (MySQL)
        public void BdEdit(
            string data_predoplaty,
            string data_vidachi,
            string surname,
            string phone,
            string AboutUs,
            string WhatRemont,
            string brand,
            string model,
            string SerialNumber,
            string sostoyanie,
            string komplektonst,
            string polomka,
            string kommentarij,
            string predvaritelnaya_stoimost,
            string Predoplata,
            string Zatrati,
            string okonchatelnaya_stoimost_remonta,
            string Skidka,
            string Status_remonta,
            string master,
            string vipolnenie_raboti,
            string Garanty,
            string wait_zakaz,
            string Adress,
            string Image_key,
            string id_bd,
            string AdressSC,
            string DeviceColour,
            string zakazchik
            )
        {

            int zakazchik_id = 0;

            //Добовляем заказчика если он есть
            if (zakazchik.Trim().Length > 0)
            {

                zakazchik_id = CheckSpravochnik("zakazchik", zakazchik.Trim());

                if (zakazchik_id < 0)
                {
                    AddSpravochnikNew("zakazchik", zakazchik);
                }

                zakazchik_id = CheckSpravochnik("zakazchik", zakazchik.Trim());

            }


            int whatRemont_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

                if (whatRemont_id < 0)
                {
                    AddSpravochnikNew("whatremont", WhatRemont);
                }

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

            }


            int brand_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                brand_id = CheckSpravochnik("brand", brand.Trim());

                if (brand_id < 0)
                {
                    AddSpravochnikNew("brand", brand);
                }

                brand_id = CheckSpravochnik("brand", brand.Trim());

            }


            int model_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                model_id = CheckSpravochnik("model", model.Trim());

                if (model_id < 0)
                {
                    AddSpravochnikNew("model", brand);
                }

                model_id = CheckSpravochnik("model", model.Trim());

            }




            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE Catalog SET
                Data_predoplaty = @data_predoplaty,
                Data_vidachi = @data_vidachi,
                surname = @surname,
                phone = @phone,
                AboutUs = @aboutUs,
                whatremont_id = @whatRemont_id,
                brand_id = @brand_id,
                model_id = @model_id,
                SerialNumber = @serialNumber,
                sostoyanie = @sostoyanie,
                komplektonst = @komplektonst,
                polomka = @polomka,
                kommentarij = @kommentarij,
                predvaritelnaya_stoimost = @predvaritelnaya_stoimost,
                Predoplata = @predoplata,
                Zatrati = @zatrati,
                okonchatelnaya_stoimost_remonta = @okonchatelnaya_stoimost_remonta,
                Skidka = @skidka,
                Status_remonta = @status_remonta,
                master = @master,
                vipolnenie_raboti = @vipolnenie_raboti,
                Garanty = @garanty,
                wait_zakaz = @wait_zakaz,
                Adress = @adress,
                Image_key = @image_key,
                AdressSC = @adressSC,
                DeviceColour = @deviceColour,
                zakazchik_id = @zakazchik_id
            WHERE id = @id;
            "
            };

            data_predoplaty = convertDateRev(data_predoplaty);
            data_vidachi = convertDateRev(data_vidachi);


            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@data_predoplaty", data_predoplaty);
            m_sqlCmd.Parameters.AddWithValue("@data_vidachi", data_vidachi);
            m_sqlCmd.Parameters.AddWithValue("@surname", surname.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@aboutUs", AboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@whatRemont_id", whatRemont_id);
            m_sqlCmd.Parameters.AddWithValue("@brand_id", brand_id);
            m_sqlCmd.Parameters.AddWithValue("@model_id", model_id);
            m_sqlCmd.Parameters.AddWithValue("@serialNumber", SerialNumber.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@sostoyanie", sostoyanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@komplektonst", komplektonst.Trim());
            m_sqlCmd.Parameters.AddWithValue("@polomka", polomka.Trim());
            m_sqlCmd.Parameters.AddWithValue("@kommentarij", kommentarij.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predvaritelnaya_stoimost", predvaritelnaya_stoimost.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predoplata", Predoplata.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zatrati", Zatrati.Trim());
            m_sqlCmd.Parameters.AddWithValue("@okonchatelnaya_stoimost_remonta", okonchatelnaya_stoimost_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@skidka", Skidka);
            m_sqlCmd.Parameters.AddWithValue("@status_remonta", Status_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@master", master.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@vipolnenie_raboti", vipolnenie_raboti.Trim());
            m_sqlCmd.Parameters.AddWithValue("@garanty", Garanty.Trim());
            m_sqlCmd.Parameters.AddWithValue("@wait_zakaz", wait_zakaz);
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress);
            m_sqlCmd.Parameters.AddWithValue("@image_key", Image_key);
            m_sqlCmd.Parameters.AddWithValue("@adressSC", AdressSC.ToUpper().Trim());
            m_sqlCmd.Parameters.AddWithValue("@deviceColour", DeviceColour.ToUpper().Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);
            m_sqlCmd.Parameters.AddWithValue("@zakazchik_id", zakazchik_id);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //ZAKAZCHIK_SUDA
        //Изменение в базе данных
        // Обновление данных выдачи по гарантии в таблице Catalog (MySQL)
        public void BdEditVidachiPoGarantii(
            string data_predoplaty,
            string data_vidachi,
            string surname,
            string phone,
            string AboutUs,
            string WhatRemont,
            string brand,
            string model,
            string SerialNumber,
            string sostoyanie,
            string komplektonst,
            string polomka,
            string kommentarij,
            string predvaritelnaya_stoimost,
            string Predoplata,
            string Zatrati,
            string okonchatelnaya_stoimost_remonta,
            string Skidka,
            string Status_remonta,
            string master,
            string vipolnenie_raboti,
            string Garanty,
            string wait_zakaz,
            string Adress,
            string Image_key,
            string id_bd,
            string AdressSC,
            string DeviceColour)
        {




            int whatRemont_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

                if (whatRemont_id < 0)
                {
                    AddSpravochnikNew("whatremont", WhatRemont);
                }

                whatRemont_id = CheckSpravochnik("whatremont", WhatRemont.Trim());

            }


            int brand_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                brand_id = CheckSpravochnik("brand", brand.Trim());

                if (brand_id < 0)
                {
                    AddSpravochnikNew("brand", brand);
                }

                brand_id = CheckSpravochnik("brand", brand.Trim());

            }


            int model_id = 0;

            //Добовляем заказчика если он есть
            if (WhatRemont.Trim().Length > 0)
            {

                model_id = CheckSpravochnik("model", model.Trim());

                if (model_id < 0)
                {
                    AddSpravochnikNew("model", brand);
                }

                model_id = CheckSpravochnik("model", model.Trim());

            }




            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
                UPDATE Catalog SET
                    Data_predoplaty = @data_predoplaty,
                    Data_vidachi = @data_vidachi,
                    surname = @surname,
                    phone = @phone,
                    AboutUs = @aboutUs,
                    whatremont_id = @whatRemont_id,
                    brand_id = @brand_id,
                    model_id = @model_id,
                    SerialNumber = @serialNumber,
                    sostoyanie = @sostoyanie,
                    komplektonst = @komplektonst,
                    polomka = @polomka,
                    kommentarij = @kommentarij,
                    predvaritelnaya_stoimost = @predvaritelnaya_stoimost,
                    Predoplata = @predoplata,
                    Zatrati = @zatrati,
                    okonchatelnaya_stoimost_remonta = @okonchatelnaya_stoimost_remonta,
                    Skidka = @skidka,
                    Status_remonta = @status_remonta,
                    master = @master,
                    vipolnenie_raboti = @vipolnenie_raboti,
                    Garanty = @garanty,
                    wait_zakaz = @wait_zakaz,
                    Adress = @adress,
                    Image_key = @image_key,
                    AdressSC = @adressSC,
                    DeviceColour = @deviceColour
                WHERE id = @id;
            "
            };

            data_predoplaty = convertDateRev(data_predoplaty);
            data_vidachi = convertDateRev(data_vidachi);

            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@data_predoplaty", data_predoplaty);
            m_sqlCmd.Parameters.AddWithValue("@data_vidachi", data_vidachi);
            m_sqlCmd.Parameters.AddWithValue("@surname", surname.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@aboutUs", AboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@whatRemont_id", whatRemont_id);
            m_sqlCmd.Parameters.AddWithValue("@brand_id", brand_id);
            m_sqlCmd.Parameters.AddWithValue("@model_id", model_id);
            m_sqlCmd.Parameters.AddWithValue("@serialNumber", SerialNumber.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@sostoyanie", sostoyanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@komplektonst", komplektonst.Trim());
            m_sqlCmd.Parameters.AddWithValue("@polomka", polomka.Trim());
            m_sqlCmd.Parameters.AddWithValue("@kommentarij", kommentarij.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predvaritelnaya_stoimost", predvaritelnaya_stoimost.Trim());
            m_sqlCmd.Parameters.AddWithValue("@predoplata", Predoplata.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zatrati", Zatrati.Trim());
            m_sqlCmd.Parameters.AddWithValue("@okonchatelnaya_stoimost_remonta", okonchatelnaya_stoimost_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@skidka", Skidka);
            m_sqlCmd.Parameters.AddWithValue("@status_remonta", Status_remonta.Trim());
            m_sqlCmd.Parameters.AddWithValue("@master", master.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@vipolnenie_raboti", vipolnenie_raboti.Trim());
            m_sqlCmd.Parameters.AddWithValue("@garanty", Garanty.Trim());
            m_sqlCmd.Parameters.AddWithValue("@wait_zakaz", wait_zakaz);
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress);
            m_sqlCmd.Parameters.AddWithValue("@image_key", Image_key);
            m_sqlCmd.Parameters.AddWithValue("@adressSC", AdressSC.ToUpper().Trim());
            m_sqlCmd.Parameters.AddWithValue("@deviceColour", DeviceColour.ToUpper().Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи по гарантии в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Изменение в базе данных
        // Обновляет одно поле в записи таблицы Catalog по ID (MySQL)
        public void BdEditOne(string editWhat, string editThis, string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Имя столбца берём динамически, но оборачиваем в обратные апострофы
                CommandText = $"UPDATE Catalog SET `{editWhat}` = @value WHERE id = @id"
            };


            editThis = convertDateRev(editThis);


            m_sqlCmd.Parameters.AddWithValue("@value", editThis);
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении поля в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Изменение в базе данных Убираем NULL
        // Заменяет все NULL-значения в указанном столбце пустой строкой (MySQL)
        public void BdNoNull(string columnName)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Оборачиваем имя столбца в обратные апострофы, чтобы поддерживать MySQL-идентификаторы
                CommandText = $"UPDATE `Catalog` SET `{columnName}` = '' WHERE `{columnName}` IS NULL;"
            };

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении NULL-значений в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Изменение в базе данных Убираем NULL
        // Переименовывает все вхождения старого значения в указанном столбце на новое (MySQL)
        public void BdNoNullRename(string columnName, string newValue, string oldValue)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Имя столбца оборачиваем в обратные апострофы,
                // а сами значения передаём через параметры
                CommandText = $@"
            UPDATE `Catalog`
            SET `{columnName}` = @newVal
            WHERE `{columnName}` = @oldVal;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@newVal", newValue.Trim());
            m_sqlCmd.Parameters.AddWithValue("@oldVal", oldValue);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при переименовании значений в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Удаление из бд
        //Изменение в базе данных
        // Удаляет запись из таблицы Catalog по ID (MySQL)
        public void BdDelete(string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM Catalog WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записи из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //ZAKAZCHIK_SUDA
        //Поиск по базе
        //Поиск по ФИО
        // Поиск записей по фамилии клиента с опцией сортировки/фильтра по дате выдачи (MySQL)
        public DataTable SearchFIO(string FIO, bool check)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dt;
                }

                // Базовый SELECT
                var sb = new StringBuilder();
                sb.Append(@"
                        SELECT
                            c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                            cm.FIO, cm.Phone, cm.AboutUs,
                            w.whatremont, b.name, m.name, c.SerialNumber,
                            c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                            c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati,
                            c.okonchatelnaya_stoimost_remonta, c.Skidka,
                            c.Status_remonta, c.master, c.vipolnenie_raboti,
                            c.Garanty, c.wait_zakaz, cm.Adress, c.Image_key,
                            c.AdressSC, c.DeviceColour, c.ClientId, c.Barcode
                        FROM Catalog AS c
                        JOIN ClientsMap AS cm ON c.ClientId = cm.id
                        LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
                        LEFT JOIN brand b ON b.id = c.brand_id
                        LEFT JOIN whatremont w ON w.id = c.whatremont_id
                        LEFT JOIN model m ON m.id = c.model_id
                        WHERE cm.FIO LIKE @fio
                    ");

                // Добавляем фильтр по выданным/не выданным
                if (check)
                {
                    sb.Append(" AND c.Data_vidachi IS NOT NULL");
                    sb.Append(" ORDER BY c.Data_vidachi DESC");
                }
                else
                {
                    sb.Append(" AND c.Data_vidachi IS NULL");
                }

                m_sqlCmd.CommandText = sb.ToString();
                m_sqlCmd.Parameters.AddWithValue("@fio", $"%{FIO.Trim().ToUpper()}%");

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Что-то пошло не так при проведении поиска в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        //ZAKAZCHIK_SUDA
        // Полный поиск по базе 1
        // Полнотекстовый поиск по множеству полей с множественной логикой (MySQL)
        public DataTable BdReadFullSearch(
                 string FIO, string phone, string TypeOf, string brand, string model,
                 string status, string master, string zakaz, bool trfl,
                 string idInBd, string serialImei, string AdressSC,
                 string garanty = "", string soglasovat = "", string zakazchik = ""
             )
        {
            var dt = new DataTable();
            using (var conn = GetMySqlConnection())
            using (var cmd = conn.CreateCommand())
            {

                conn.Open();

                //ZAKAZCHIK_SUDA

                var sb = new StringBuilder();
                sb.Append(@"
                                SELECT
                                  c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                                  cm.FIO, cm.Phone, cm.AboutUs,
                                  w.name, b.name, m.name, c.SerialNumber,
                                  c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                                  c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati,
                                  c.okonchatelnaya_stoimost_remonta, c.Skidka,
                                  c.Status_remonta, c.master, c.vipolnenie_raboti,
                                  c.Garanty, c.wait_zakaz, cm.Adress, c.Image_key,
                                  c.AdressSC, c.DeviceColour, c.ClientId, c.Barcode, z.name
                                FROM Catalog AS c
                                JOIN ClientsMap AS cm ON c.ClientId = cm.id
                                LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
                                LEFT JOIN whatremont w ON w.id = c.whatremont_id
                                LEFT JOIN brand b ON b.id = c.brand_id
                                LEFT JOIN model m ON m.id = c.model_id
                                WHERE c.Deleted != 1
                                  AND cm.FIO       LIKE @fio
                                  AND cm.Phone     LIKE @phone
                                  AND (w.name LIKE @typeOf OR w.name IS NULL)
                                  AND (b.name LIKE @brand OR b.name IS NULL)
                                  AND (m.name LIKE @model OR m.name IS NULL)
                                  AND c.Status_remonta LIKE @status
                                  AND c.master     LIKE @master
                                  AND c.wait_zakaz LIKE @zakaz
                                  AND c.SerialNumber LIKE @serial
                                  AND c.AdressSC   LIKE @adressSC
                                  AND (z.name LIKE @zakazchik OR z.name IS NULL)
                                ");

                // Общие параметры
                cmd.Parameters.AddWithValue("@fio", $"%{FIO.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@phone", $"%{phone.Trim()}%");
                cmd.Parameters.AddWithValue("@typeOf", $"%{TypeOf.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@brand", $"%{brand.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@model", $"%{model.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@status", $"%{status.Trim()}%");
                cmd.Parameters.AddWithValue("@master", $"%{master.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@zakaz", $"%{zakaz.Trim()}%");
                cmd.Parameters.AddWithValue("@serial", $"%{serialImei.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@adressSC", $"%{AdressSC.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@zakazchik", $"%{zakazchik.Trim()}%");

                // id
                if (!string.IsNullOrWhiteSpace(idInBd))
                {
                    sb.Append(" AND c.id = @id");
                    cmd.Parameters.AddWithValue("@id", idInBd.Trim());
                }
                else
                {
                    sb.Append(" AND c.id LIKE @idLike");
                    cmd.Parameters.AddWithValue("@idLike", $"%{idInBd}%");
                }

                // Гарантийный режим (Image_key)
                sb.Append(" AND c.Image_key LIKE @soglasovat");
                cmd.Parameters.AddWithValue("@soglasovat", $"%{soglasovat.Trim()}%");

                // Фильтр по выдаче и статусу (копируем «странную» логику SQLite)
                if (garanty == "garanty")
                {
                    // ничего дополнительно не добавляем —
                    // уже применили только Image_key
                }
                else if (!trfl)
                {
                    // trfl==false  ⇒ в SQLite делали `Data_vidachi != ''`
                    sb.Append(" AND c.Data_vidachi IS NOT NULL");
                }
                else
                {
                    // trfl==true ⇒
                    // в SQLite была ветка с Data_vidachi == '' + сложный OR по статусу
                    sb.Append(@"
                                  AND (
                                        c.Data_vidachi IS NULL
                                     OR (c.Status_remonta = 'Принят по гарантии')
                                     )
                                  AND c.Status_remonta <> 'Выдан'
                                ");
                }

                cmd.CommandText = sb.ToString();
                using (var adapter = new MySqlDataAdapter(cmd))
                    adapter.Fill(dt);
            }

            return dt;
        }

        //ZAKAZCHIK_SUDA
        //Перегрузка для поиска в выданном 2
        // Полнотекстовый поиск (перегрузка без ветвления гарантии, MySQL)
        public DataTable BdReadFullSearch(
            string FIO,
            string phone,
            string TypeOf,
            string brand,
            string model,
            string status,      // этот параметр больше не используется
            string master,
            string zakaz,
            bool trfl,
            string idInBd,
            string serialImei,
            string AdressSC,
            string garanty = "",
            string soglasovat = "",
            bool vidannoe = false,
            string zakazchik = ""
)
        {
            var dt = new DataTable();
            using (var conn = GetMySqlConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                var sb = new StringBuilder();
                sb.Append(@"
                    SELECT
                      c.id, c.Data_priema, c.Data_vidachi, c.Data_predoplaty,
                      cm.FIO, cm.Phone, cm.AboutUs,
                      w.name, b.name, m.name, c.SerialNumber,
                      c.Sostoyanie, c.komplektonst, c.polomka, c.kommentarij,
                      c.predvaritelnaya_stoimost, c.Predoplata, c.Zatrati,
                      c.okonchatelnaya_stoimost_remonta, c.Skidka,
                      c.Status_remonta, c.master, c.vipolnenie_raboti,
                      c.Garanty, c.wait_zakaz, cm.Adress, c.Image_key,
                      c.AdressSC, c.DeviceColour, c.ClientId, c.Barcode, z.name
                    FROM Catalog AS c
                    JOIN ClientsMap AS cm ON c.ClientId = cm.id
                    LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
                    LEFT JOIN whatremont w ON w.id = c.whatremont_id
                    LEFT JOIN brand b ON b.id = c.brand_id
                    LEFT JOIN model m ON m.id = c.model_id
                    WHERE c.Deleted != 1
                      AND cm.FIO       LIKE @fio
                      AND cm.Phone     LIKE @phone
                      AND (w.name LIKE @typeOf OR w.name IS NULL)
                      AND (b.name LIKE @brand OR b.name IS NULL)
                      AND (m.name LIKE @model OR m.name IS NULL)
                      -- Повторяем SQLite-логику: только «выданные»
                      AND c.Data_vidachi IS NOT NULL
                      -- в оригинале SQLite-версия НЕ фильтровала по статусу
                      AND c.master     LIKE @master
                      AND c.wait_zakaz LIKE @zakaz
                      AND c.Image_key  LIKE @soglasovat
                      AND c.id         LIKE @idLike
                      AND c.SerialNumber LIKE @serial
                      AND c.AdressSC   LIKE @adressSC;
                      AND (z.name LIKE @zakazchik OR z.name IS NULL)
                    ");

                // параметры (статус убрали — он не нужен)
                cmd.Parameters.AddWithValue("@fio", $"%{FIO.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@phone", $"%{phone.Trim()}%");
                cmd.Parameters.AddWithValue("@typeOf", $"%{TypeOf.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@brand", $"%{brand.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@model", $"%{model.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@master", $"%{master.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@zakaz", $"%{zakaz.Trim()}%");
                cmd.Parameters.AddWithValue("@soglasovat", $"%{soglasovat.Trim()}%");
                cmd.Parameters.AddWithValue("@idLike", $"%{idInBd.Trim()}%");
                cmd.Parameters.AddWithValue("@serial", $"%{serialImei.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@adressSC", $"%{AdressSC.Trim().ToUpper()}%");
                cmd.Parameters.AddWithValue("@zakazchik", $"%{zakazchik.Trim()}%");

                cmd.CommandText = sb.ToString();
                using (var adapter = new MySqlDataAdapter(cmd))
                    adapter.Fill(dt);
            }

            return dt;
        }



        // Возвращает все записи со статусом «ожидание» по Image_key = 1 (MySQL)
        public DataTable BdSearchPhoneWaiting()
        {
            var dt = new DataTable();

            //ZAKAZCHIK_SUDA

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT
                c.id,
                c.Data_priema,
                c.Data_vidachi,
                c.Data_predoplaty,
                cm.FIO,
                cm.Phone,
                cm.AboutUs,
                w.name,
                b.name,
                m.name,
                c.SerialNumber,
                c.Sostoyanie,
                c.komplektonst,
                c.polomka,
                c.kommentarij,
                c.predvaritelnaya_stoimost,
                c.Predoplata,
                c.Zatrati,
                c.okonchatelnaya_stoimost_remonta,
                c.Skidka,
                c.Status_remonta,
                c.master,
                c.vipolnenie_raboti,
                c.Garanty,
                c.wait_zakaz,
                cm.Adress,
                c.Image_key,
                c.AdressSC,
                c.DeviceColour,
                c.ClientId,
                c.Barcode
                z.name
            FROM Catalog AS c
            JOIN ClientsMap AS cm ON c.ClientId = cm.id
            LEFT JOIN zakazchik z ON z.id = c.zakazchik_id
            LEFT JOIN whatremont w ON w.id = c.whatremont_id
            LEFT JOIN brand b ON b.id = c.brand_id
            LEFT JOIN model m ON m.id = c.model_id
            WHERE c.Image_key = @imageKey;
        "
            };
            // Передаём параметр вместо строковой конкатенации
            m_sqlCmd.Parameters.AddWithValue("@imageKey", "1");

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при выполнении поиска ожидания в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }





        //Функция заглавной буквы в начале каждого слова
        string FirstLetterToUpper(string krolik)
        {
            string lookup = " \r\n\t";
            var sb = new StringBuilder(krolik.ToLower());

            if (sb.Length > 0 && char.IsLetter(sb[0]))
                sb[0] = char.ToUpper(sb[0]);

            for (int z = 1; z < sb.Length; z++)
            {
                char ch = sb[z];
                if (lookup.Contains(sb[z - 1]) && char.IsLetter(ch))
                    sb[z] = char.ToUpper(ch);
            }
            return sb.ToString();
        }

        //Корректировка дат в верный формат










        //////////////////////////////////////////////////////// СКЛАД /////////////////////////////////////////////////////////////////////////
        //Создание таблицы для склада
        // Создает справочник Stock в MySQL
        public void CreateStock()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            CREATE TABLE IF NOT EXISTS Stock (
                id INT AUTO_INCREMENT PRIMARY KEY,
                Naimenovanie VARCHAR(255),
                Kategoriya VARCHAR(255),
                Podkategoriya VARCHAR(255),
                Colour VARCHAR(50),
                Brand VARCHAR(100),
                Model VARCHAR(100),
                CountOf INT,
                Price DECIMAL(10,2),
                Napominanie TEXT,
                Photo TEXT,
                Primechanie TEXT,
                Photo2 TEXT,
                Photo3 TEXT
            ) ENGINE=InnoDB
              DEFAULT CHARSET=utf8mb4;
        "
            };

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу создать таблицу Stock в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        // Создание таблицы со значениями использованных деталий и записей, в которых они были использованы

        // Создает таблицу StockMap в MySQL
        public void CreateStockMap()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            CREATE TABLE IF NOT EXISTS StockMap (
                id INT AUTO_INCREMENT PRIMARY KEY,
                clientId INT,
                ZIPId INT,
                countOfZIP INT,
                priceOfZIP DECIMAL(10,2)
            ) ENGINE=InnoDB
              DEFAULT CHARSET=utf8mb4;
        "
            };

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу создать таблицу StockMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Запись в базу данных
        // Вставляет новую запись в справочник Stock (MySQL)
        public void BdStockWrite(
            string Naimenovanie,
            string Kategoriya,
            string Podkategoriya,
            string Colour,
            string Brand,
            string Model,
            string CountOf,
            string Napominanie,
            string Price,
            string Primechanie,
            string Photo = "",
            string Photo2 = "",
            string Photo3 = "")
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO Stock
                (Naimenovanie, Kategoriya, Podkategoriya, Colour,
                 Brand, Model, CountOf, Napominanie, Price,
                 Primechanie, Photo, Photo2, Photo3)
            VALUES
                (@name, @cat, @subcat, @colour,
                 @brand, @model, @count, @note, @price,
                 @prime, @photo, @photo2, @photo3);
        "
            };

            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@name", Naimenovanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@cat", Kategoriya.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@subcat", Podkategoriya.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@colour", Colour.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@brand", Brand.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@model", Model.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@count", int.TryParse(CountOf, out var cnt) ? cnt : 0);
            m_sqlCmd.Parameters.AddWithValue("@note", Napominanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@price", decimal.TryParse(Price, out var pr) ? pr : 0m);
            m_sqlCmd.Parameters.AddWithValue("@prime", Primechanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@photo", Photo.Trim());
            m_sqlCmd.Parameters.AddWithValue("@photo2", Photo2.Trim());
            m_sqlCmd.Parameters.AddWithValue("@photo3", Photo3.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в справочник Stock в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Изменение в базе данных
        // Обновляет запись в справочнике Stock (MySQL)
        public void BdStockEdit(
            string Naimenovanie,
            string Kategoriya,
            string Podkategoriya,
            string Colour,
            string Brand,
            string Model,
            string CountOf,
            string Napominanie,
            string Price,
            string Photo,
            string id_bd,
            string Primechanie,
            string Photo2,
            string Photo3)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE Stock SET
                Naimenovanie   = @name,
                Kategoriya     = @category,
                Podkategoriya  = @subcategory,
                Colour         = @colour,
                Brand          = @brand,
                Model          = @model,
                CountOf        = @count,
                Napominanie    = @note,
                Price          = @price,
                Photo          = @photo,
                Primechanie    = @prime,
                Photo2         = @photo2,
                Photo3         = @photo3
            WHERE id = @id;
        "
            };

            // Подготовка параметров
            int count = int.TryParse(CountOf, out var tmpCount) ? tmpCount : 0;
            decimal price = decimal.TryParse(Price, out var tmpPrice) ? tmpPrice : 0m;
            int id = int.TryParse(id_bd, out var tmpId) ? tmpId : 0;

            m_sqlCmd.Parameters.AddWithValue("@name", Naimenovanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@category", Kategoriya.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@subcategory", Podkategoriya.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@colour", Colour.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@brand", Brand.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@model", Model.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@count", count);
            m_sqlCmd.Parameters.AddWithValue("@note", Napominanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@price", price);
            m_sqlCmd.Parameters.AddWithValue("@photo", Photo.Trim());
            m_sqlCmd.Parameters.AddWithValue("@prime", Primechanie.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@photo2", Photo2.Trim());
            m_sqlCmd.Parameters.AddWithValue("@photo3", Photo3.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи в справочнике Stock:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Изменение в базе данных
        // Обновляет одно поле записи в таблице Stock по ID (MySQL)
        public void BdStockEditOne(string columnName, string newValue, string idOfZIP)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Динамическое имя колонки оборачиваем в обратные апострофы
                CommandText = $"UPDATE `Stock` SET `{columnName}` = @value WHERE `id` = @id;"
            };
            m_sqlCmd.Parameters.AddWithValue("@value", newValue);
            m_sqlCmd.Parameters.AddWithValue("@id", idOfZIP);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении в справочнике Stock:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }



        // Полный поиск по базе
        // Полнотекстовый поиск в справочнике Stock (MySQL)
        public DataTable BdStockFullSearch(
            string Naimenovanie,
            string Kategoriya,
            string Podkategoriya,
            string Colour,
            string Brand,
            string Model,
            string CountOf,
            string Napominanie)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM Stock
            WHERE Naimenovanie  LIKE @name
              AND Kategoriya    LIKE @category
              AND Podkategoriya LIKE @subcategory
              AND Colour        LIKE @colour
              AND Brand         LIKE @brand
              AND Model         LIKE @model
              AND CountOf       LIKE @count
              AND Napominanie   LIKE @note;
        "
            };

            // Параметры для поиска
            m_sqlCmd.Parameters.AddWithValue("@name", $"%{Naimenovanie.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@category", $"%{Kategoriya.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@subcategory", $"%{Podkategoriya.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@colour", $"%{Colour.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@brand", $"%{Brand.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@model", $"%{Model.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@count", $"%{CountOf.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@note", $"%{Napominanie.Trim().ToUpper()}%");

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при полном поиске в справочнике Stock в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        // StockEditor
        // Читает запись из справочника Stock по ID (MySQL)
        public DataTable BdStockEditor(string id)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM Stock WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Что-то пошло не так при чтении из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        //Удаление из бд
        // Удаляет запись из справочника Stock по ID (MySQL)
        public void BdStockDelete(string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM Stock WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записи из справочника Stock:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Запись в базу Map данных  clientId TEXT,ZIPId TEXT ,countOfZIP TEXT
        // Вставляет новую запись в связь StockMap (MySQL)
        public void BdStockMapWrite(string clientId, string ZIPId, string countOfZIP, string priceOfZIP)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO StockMap
                (clientId, ZIPId, countOfZIP, priceOfZIP)
            VALUES
                (@clientId, @zipId, @count, @price);
        "
            };

            // Парсим числовые параметры с безопасным fallback
            int cid = int.TryParse(clientId, out var tmpCid) ? tmpCid : 0;
            int zipId = int.TryParse(ZIPId, out var tmpZip) ? tmpZip : 0;
            int count = int.TryParse(countOfZIP, out var tmpCount) ? tmpCount : 0;
            decimal pr = decimal.TryParse(priceOfZIP, out var tmpPr) ? tmpPr : 0m;

            m_sqlCmd.Parameters.AddWithValue("@clientId", cid);
            m_sqlCmd.Parameters.AddWithValue("@zipId", zipId);
            m_sqlCmd.Parameters.AddWithValue("@count", count);
            m_sqlCmd.Parameters.AddWithValue("@price", pr);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в таблицу StockMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Удаление stock map, всех записей о клиенте из бд
        // Удаляет все записи из таблицы StockMap для заданного clientId (MySQL)
        public void BdStockMapDelete(string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM StockMap WHERE clientId = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записей из StockMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        // Отмена затрат
        // Получает записи из StockMap по clientId и ZIPId (MySQL)
        public DataTable BdStockMapZIPDeleteCounter(string idClient, string idZIP)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM StockMap
            WHERE clientId = @clientId
              AND ZIPId    = @zipId;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@clientId", idClient.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zipId", idZIP.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении из StockMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        // Удаление по критериям из StockMap
        public void BdStockMapDeleteZIP(string id_bd, string idZIP)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            if (m_dbConn.State != ConnectionState.Open)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Соединение с базой данных потеряно, установите соединение" + Environment.NewLine);
                return;
            }


            try
            {
                m_sqlCmd.CommandText = "DELETE FROM StockMap WHERE clientId =" + id_bd + " AND ZIPId =" + idZIP;

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Что-то пошло не так, при записи в базу данных " + ex.ToString() + Environment.NewLine);
            }


        }

        // Проверка на наличие использованных запчастей
        // Проверяет, используется ли определённый ZIP у клиента в StockMap (MySQL)
        public bool BdStockMapZIPUsedCheck(string id_bd, string idZIP)
        {
            bool exists = false;

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT EXISTS(
                SELECT 1 
                FROM StockMap 
                WHERE clientId = @clientId 
                  AND ZIPId    = @zipId
            );
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@clientId", id_bd.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zipId", idZIP.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                    return false;

                // ExecuteScalar вернёт 1 или 0
                exists = Convert.ToInt32(m_sqlCmd.ExecuteScalar()) == 1;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при проверке использования ZIP в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return exists;
        }


        // Проверка на наличие использованных запчастей оптимизация
        // Возвращает все записи из таблицы StockMap (MySQL)
        public DataTable BdStockMapZIPUsedCheckOptimised()
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM StockMap"
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении всех записей StockMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        // Проверка на наличие использованных запчастей
        // Считает общее количество ZIP для заданного clientId и ZIPId в таблице StockMap (MySQL)
        public string BdStockMapZIPUsedCoutner(string id_bd, string idZIP)
        {
            string resultCount = "0";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT COALESCE(SUM(countOfZIP), 0)
            FROM StockMap
            WHERE clientId = @clientId
              AND ZIPId    = @zipId;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@clientId", id_bd.Trim());
            m_sqlCmd.Parameters.AddWithValue("@zipId", idZIP.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return resultCount;
                }

                // Выполняем агрегатную функцию
                var scalar = m_sqlCmd.ExecuteScalar();
                resultCount = scalar != null ? scalar.ToString() : "0";
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при подсчёте ZIP в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return resultCount;
        }

        ////////////////////////////////////////////////////////////////////////////////////-------------------------------Статусы
        // Создание таблицы

        // Создает таблицу StatesMap в MySQL
        public void StatesMapTable_Create()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            CREATE TABLE IF NOT EXISTS StatesMap (
                id INT AUTO_INCREMENT PRIMARY KEY,
                clientId INT,
                State VARCHAR(255),
                date DATETIME
            ) ENGINE=InnoDB
              DEFAULT CHARSET=utf8mb4;
        "
            };

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу создать таблицу StatesMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Удаление из бд
        // Удаляет запись из таблицы StatesMap по ID (MySQL)
        public void StatesMapDelete(string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM StatesMap WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записи из StatesMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Запись в базу Map данных  
        // Записывает новое состояние в таблицу StatesMap (MySQL)
        public void StatesMapWrite(string clientId, string state, string date)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
                    INSERT INTO StatesMap
                        (clientId, State, date)
                    VALUES
                        (@clientId, @state, @date);
                "
            };

            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@clientId", clientId.Trim());
            m_sqlCmd.Parameters.AddWithValue("@state", state.Trim());
            m_sqlCmd.Parameters.AddWithValue("@date", date.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в таблицу StatesMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        // Получить все статусы записи
        // Возвращает все записи из таблицы StatesMap для заданного clientId (MySQL)
        public DataTable StatesMapGiver(string idClient)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM StatesMap WHERE clientId = @clientId"
            };
            m_sqlCmd.Parameters.AddWithValue("@clientId", idClient.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении состояний из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        //Изменение в базе данных
        // Обновляет одно поле в записи таблицы StatesMap по ID (MySQL)
        public void StatesMapEdit(string columnName, string newValue, string id_map_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Динамическое имя столбца оборачиваем в обратные апострофы
                CommandText = $"UPDATE `StatesMap` SET `{columnName}` = @value WHERE `id` = @id;"
            };
            m_sqlCmd.Parameters.AddWithValue("@value", newValue.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_map_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи в StatesMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////-------------------------------Клиенты
        // Создание таблицы

        // Создает таблицу ClientsMap в MySQL
        public void ClientsMapTable_Create()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            CREATE TABLE IF NOT EXISTS ClientsMap (
                id INT AUTO_INCREMENT PRIMARY KEY,
                FIO VARCHAR(255),
                Phone VARCHAR(50),
                Adress TEXT,
                Primechanie TEXT,
                Blist VARCHAR(50),
                date DATETIME,
                aboutUs TEXT
            ) ENGINE=InnoDB
              DEFAULT CHARSET=utf8mb4;
        "
            };

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу создать таблицу ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Удаление из бд
        // Удаляет запись из таблицы ClientsMap по ID (MySQL)
        public void ClientsMapDelete(string id_client)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM ClientsMap WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id_client.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записи из ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }


            problemIDs = getProblemIdClients();
        }



        //Удаление из бд записей клиента
        // Удаляет все записи в Catalog для заданного ClientId (MySQL)
        public void ClientsMapZapisiDelete(string id_client)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM Catalog WHERE ClientId = @clientId"
            };

            m_sqlCmd.Parameters.AddWithValue("@clientId", id_client.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении записей из Catalog по ClientId в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Запись в базу Map данных  
        // Добавляет новую запись в таблицу ClientsMap (MySQL)
        public void ClientsMapWrite(
            string FIO,
            string Phone,
            string Adress,
            string Primechanie,
            string Blist,
            string Date,
            string aboutUs)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO ClientsMap
                (FIO, Phone, Adress, Primechanie, Blist, date, aboutUs)
            VALUES
                (@fio, @phone, @adress, @primechanie, @blist, @date, @aboutUs);
        "
            };

            // Подготовка параметров
            m_sqlCmd.Parameters.AddWithValue("@fio", FIO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", Phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress.Trim());
            m_sqlCmd.Parameters.AddWithValue("@primechanie", Primechanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@blist", Blist.Trim());
            // Преобразуем строку даты в формат MySQL DATETIME


            m_sqlCmd.Parameters.AddWithValue("@date", DateTime.Parse(Date.Trim()).ToString("yyyy-MM-dd HH:mm:ss"));

            m_sqlCmd.Parameters.AddWithValue("@aboutUs", aboutUs.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в таблицу ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            problemIDs = getProblemIdClients();

        }


        // Получить данные о клиенте
        // Возвращает запись из ClientsMap по заданному ID (MySQL)
        public DataTable ClientsMapGiver(string idClient)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM ClientsMap WHERE id = @id"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", idClient.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении из таблицы ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }

        // Получить данные о клиентах
        // Возвращает все записи из таблицы ClientsMap (MySQL)
        public DataTable ClientsAllMapGiver()
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM ClientsMap"
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении всех записей из ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }

        // Поиск по фамилии и номеру
        // Поиск записей в ClientsMap по ФИО и телефону (MySQL)
        public DataTable ClientsFIOPhoneSearch(string fio, string phone)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM ClientsMap
            WHERE FIO   LIKE @fio
              AND Phone LIKE @phone;
        "
            };
            // Подготовка шаблонов поиска
            m_sqlCmd.Parameters.AddWithValue("@fio", $"%{fio.Trim().ToUpper()}%");
            m_sqlCmd.Parameters.AddWithValue("@phone", $"%{phone.Trim().Replace(" ", "")}%");

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при поиске клиентов по ФИО/телефону в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }

        //Изменение в базе данных
        // Обновляет одно поле в записи таблицы ClientsMap по ID (MySQL)
        public void ClientsMapEditOne(string columnName, string newValue, string id_map_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                // Оборачиваем имя колонки в обратные апострофы
                CommandText = $"UPDATE `ClientsMap` SET `{columnName}` = @value WHERE `id` = @id;"
            };
            m_sqlCmd.Parameters.AddWithValue("@value", newValue.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_map_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи в ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            problemIDs = getProblemIdClients();

        }


        //Изменение в базе данных, объединение клиентов
        // Переназначает все записи в Catalog от одного клиента к другому (MySQL)
        public void ClientsToClitens(string firstClient, string secondClient)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE `Catalog`
            SET `ClientId` = @newClientId
            WHERE `ClientId` = @oldClientId;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@newClientId", secondClient.Trim());
            m_sqlCmd.Parameters.AddWithValue("@oldClientId", firstClient.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при переназначении записей в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Изменение в базе данных
        // Обновляет все поля в записи ClientsMap по ID (MySQL)
        public void ClientsMapEditAll(
            string FIO,
            string Phone,
            string Adress,
            string Primechanie,
            string Blist,
            string date,
            string aboutUs,
            string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE ClientsMap SET
                FIO          = @fio,
                Phone        = @phone,
                Adress       = @adress,
                Primechanie  = @primechanie,
                Blist        = @blist,
                date         = @date,
                aboutUs      = @aboutUs
            WHERE id = @id;
        "
            };

            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@fio", FIO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", Phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress.Trim());
            m_sqlCmd.Parameters.AddWithValue("@primechanie", Primechanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@blist", Blist.Trim());
            // Преобразуем строку даты в формат MySQL DATETIME


            m_sqlCmd.Parameters.AddWithValue("@date", DateTime.Parse(date.Trim())
                                                                 .ToString("yyyy-MM-dd HH:mm:ss"));

            m_sqlCmd.Parameters.AddWithValue("@aboutUs", aboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            problemIDs = getProblemIdClients();
        }



        //Изменение в базе данных
        // Обновляет поля в записи ClientsMap без изменения даты (MySQL)
        public void ClientsMapEditWithoutDate(
            string FIO,
            string Phone,
            string Adress,
            string Primechanie,
            string Blist,
            string aboutUs,
            string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE ClientsMap SET
                FIO         = @fio,
                Phone       = @phone,
                Adress      = @adress,
                Primechanie = @primechanie,
                Blist       = @blist,
                aboutUs     = @aboutUs
            WHERE id = @id;
        "
            };

            // Параметры для безопасного запроса
            m_sqlCmd.Parameters.AddWithValue("@fio", FIO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", Phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress.Trim());
            m_sqlCmd.Parameters.AddWithValue("@primechanie", Primechanie.Trim());
            m_sqlCmd.Parameters.AddWithValue("@blist", Blist.Trim());
            m_sqlCmd.Parameters.AddWithValue("@aboutUs", aboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении записи ClientsMap (без даты) в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            problemIDs = getProblemIdClients();
        }

        //Изменение в базе данных
        // Обновляет поля FIO, Phone, Adress и aboutUs в записи ClientsMap (MySQL)
        public void ClientsMapEditInEditor(
            string FIO,
            string Phone,
            string Adress,
            string aboutUs,
            string id_bd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE ClientsMap SET
                FIO     = @fio,
                Phone   = @phone,
                Adress  = @adress,
                aboutUs = @aboutUs
            WHERE id = @id;
        "
            };

            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@fio", FIO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", Phone.Trim());
            m_sqlCmd.Parameters.AddWithValue("@adress", Adress.Trim());
            m_sqlCmd.Parameters.AddWithValue("@aboutUs", aboutUs.Trim());
            m_sqlCmd.Parameters.AddWithValue("@id", id_bd.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении клиента в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            problemIDs = getProblemIdClients();
        }

        // Чтение из базы данных
        // Возвращает ID клиента по полному совпадению ФИО и телефона (MySQL)
        public string ClientReadId(string FIO, string Phone)
        {
            string clientId = "";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT id
            FROM ClientsMap
            WHERE FIO   = @fio
              AND Phone = @phone
            LIMIT 1;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@fio", FIO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@phone", Phone.Trim().Replace(" ", ""));

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return "";
                }

                using (var reader = m_sqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                        clientId = reader["id"].ToString();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении ID клиента из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return clientId;
        }

        // Чтение из базы данных
        // Возвращает дату создания записи клиента по его ID (MySQL)
        public string ClientReadDate(string id)
        {
            string clientDate = "";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT `date`
            FROM `ClientsMap`
            WHERE `id` = @id
            LIMIT 1;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@id", id.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return "";
                }

                var result = m_sqlCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    clientDate = result.ToString();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении даты клиента из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return clientDate;
        }


        //Перегрузка функции для подгрузки фамилий
        // Формирует AutoCompleteStringCollection из поля FIO таблицы ClientsMap (MySQL)



        //STEP_NEW_FIELD_CLIENTSMAP 
        public AutoCompleteStringCollection AddCollectionZakazchik()
        {
            var zakazchikAutoColl = new AutoCompleteStringCollection();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT name FROM zakazchik"
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return zakazchikAutoColl;
                }

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }



                foreach (DataRow row in dt.Rows)
                {
                    // Добавляем с заглавной буквы, чтобы избежать дублирования разных регистров
                    var zakazchik = row["name"]?.ToString();

                    if (!string.IsNullOrEmpty(zakazchik))
                    {
                        zakazchikAutoColl.Add(zakazchik);
                    }
                }

            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при загрузке списка заказчиков:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return zakazchikAutoColl;
        }

        public AutoCompleteStringCollection AddCollectionWhat_remont_combo_box()
        {
            var watremontAutoColl = new AutoCompleteStringCollection();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT name FROM whatremont"
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return watremontAutoColl;
                }

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }



                foreach (DataRow row in dt.Rows)
                {
                    // Добавляем с заглавной буквы, чтобы избежать дублирования разных регистров
                    var watRewmont = row["name"]?.ToString();

                    if (!string.IsNullOrEmpty(watRewmont))
                    {
                        watremontAutoColl.Add(watRewmont);
                    }
                }

            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при загрузке списка заказчиков:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return watremontAutoColl;
        }





        public AutoCompleteStringCollection AddCollectionBrandComboBox()
        {
            var brandAutoColl = new AutoCompleteStringCollection();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT name FROM brand"
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return brandAutoColl;
                }

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }



                foreach (DataRow row in dt.Rows)
                {
                    // Добавляем с заглавной буквы, чтобы избежать дублирования разных регистров
                    var brand = row["name"]?.ToString();

                    if (!string.IsNullOrEmpty(brand))
                    {
                        brandAutoColl.Add(brand);
                    }
                }

            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при загрузке списка заказчиков:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return brandAutoColl;
        }


        public AutoCompleteStringCollection AddCollectionModel()
        {
            var modelAutoColl = new AutoCompleteStringCollection();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT name FROM model"
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return modelAutoColl;
                }

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }



                foreach (DataRow row in dt.Rows)
                {
                    // Добавляем с заглавной буквы, чтобы избежать дублирования разных регистров
                    var name = row["name"]?.ToString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        modelAutoColl.Add(name);
                    }
                }

            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при загрузке списка заказчиков:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return modelAutoColl;
        }


        public AutoCompleteStringCollection AddCollectionFIO()
        {
            var surnameAutoColl = new AutoCompleteStringCollection();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT FIO,Blist  FROM ClientsMap"
            };

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return surnameAutoColl;
                }

                var dt = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }



                foreach (DataRow row in dt.Rows)
                {
                    // Добавляем с заглавной буквы, чтобы избежать дублирования разных регистров
                    var fio = row["FIO"]?.ToString();

                    //Айдишник один и тот же почему то
                    String BLiset = row["BList"]?.ToString();

                    if (!string.IsNullOrEmpty(fio))
                    {

                        fio = FirstLetterToUpper(fio);

                        if (BLiset == "1")
                        {
                            fio = fio + " [X]";
                        }

                        surnameAutoColl.Add(fio);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при загрузке списка ФИО из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return surnameAutoColl;
        }

        // Поиск по базы данных ФИО (AddPos)
        // Читает запись клиента (FIO, Phone, Adress, Primechanie, Blist, date, aboutUs) по точному совпадению FIO (MySQL)
        public DataTable BdReadFIOPhone(string fio)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT 
                FIO, 
                Phone, 
                Adress, 
                Primechanie, 
                Blist, 
                `date`, 
                aboutUs
            FROM ClientsMap
            WHERE FIO = @fio;
        "
            };
            cmd.Parameters.AddWithValue("@fio", fio.Trim().ToUpper());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении данных клиента из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }



        public List<string> getProblemIdClients()
        {

            List<string> ids = new List<string>();


            var dt = new DataTable();

            var sb = new StringBuilder();

            MySqlConnection m_dbConn = GetMySqlConnection();
            var cmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT 
                id
            FROM ClientsMap
            WHERE Blist = 1;
            "
            };

            try
            {
                m_dbConn.Open();

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }


                // Проходим по всем строкам и собираем строку вида "#1#2#3"
                foreach (DataRow row in dt.Rows)
                {
                    // Предполагаем, что id — целочисленное или строковое поле, но ToString() сработает.
                    ids.Add(row["id"].ToString());
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении данных клиента из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            if (ids.Count == 0) ids.Add("-1");

            return ids;
        }




        // Возвращает все записи из Catalog для заданного клиента (MySQL)
        public DataTable ClientsShowHistory(string clientId)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM Catalog
            WHERE ClientId = @clientId;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@clientId", clientId.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dt;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при получении истории клиента из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }

        // Читает одно поле из ClientsMap по ID (MySQL)
        public string ClientsReadOne(string readWhat, string clientId)
        {
            string readData = "";

            MySqlConnection m_dbConn = GetMySqlConnection();
            // Имя поля оборачиваем в обратные апострофы
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = $"SELECT `{readWhat}` FROM ClientsMap WHERE id = @id LIMIT 1;"
            };
            m_sqlCmd.Parameters.AddWithValue("@id", clientId.Trim());

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return "";
                }

                var result = m_sqlCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    readData = result.ToString();
                else
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Не могу прочитать поле «{readWhat}» для клиента ID {clientId}",
                        "Не найдено",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении из ClientsMap в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return readData;
        }

        // Поиск клиентов по ФИО (MySQL)
        public DataTable ClientsSearchFIO(string FIO)
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT *
            FROM ClientsMap
            WHERE FIO LIKE @fio;
        "
            };
            m_sqlCmd.Parameters.AddWithValue("@fio", $"%{FIO.Trim().ToUpper()}%");

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        $"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return dt;
                }

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при поиске клиентов по ФИО в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }

        // Вычисляет контрольную цифру UPC-A по первым 11 цифрам и возвращает полный 12-значный штрихкод
        public string barcodeLastDigit(string barcode11)
        {
            if (barcode11 == null || barcode11.Length < 11)
                throw new ArgumentException("Штрихкод должен содержать как минимум 11 цифр.");

            string raw = barcode11.Substring(0, 11);
            int sumOdd = 0, sumEven = 0;

            for (int i = 0; i < raw.Length; i++)
            {
                int digit = raw[i] - '0';
                if (i % 2 == 0)
                    sumOdd += digit * 3;    // позиции 0,2,4,… умножаются на 3
                else
                    sumEven += digit;       // позиции 1,3,5,… напрямую
            }

            int total = sumOdd + sumEven;
            int check = (10 - (total % 10)) % 10;  // если (total mod 10)==0, то контрольная цифра 0

            return raw + check;
        }

        // Генерирует штрихкоды для всех записей без штрихкода и сохраняет их в MySQL,
        // используя точный порядок полей вашей таблицы Catalog и соответствующий конструктор VirtualClient.
        public void bdBarcodeAllGenerator()
        {

            MySqlConnection m_dbConn = GetMySqlConnection();

            // Получаем данные
            DataTable dt1 = mainForm.basa.BdReadAll();
            List<VirtualClient> vc1List = new List<VirtualClient>();

            // Формируем список клиентов из таблицы
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                vc1List.Add(new VirtualClient(
                    dt1.Rows[i][0].ToString(), dt1.Rows[i][1].ToString(), dt1.Rows[i][2].ToString(), dt1.Rows[i][3].ToString(),
                    dt1.Rows[i][4].ToString(), dt1.Rows[i][5].ToString(), dt1.Rows[i][6].ToString(), dt1.Rows[i][7].ToString(), dt1.Rows[i][8].ToString(),
                    dt1.Rows[i][9].ToString(), dt1.Rows[i][10].ToString(), dt1.Rows[i][11].ToString(), dt1.Rows[i][12].ToString(), dt1.Rows[i][13].ToString(),
                    dt1.Rows[i][14].ToString(), dt1.Rows[i][15].ToString(), dt1.Rows[i][16].ToString(), dt1.Rows[i][17].ToString(),
                    dt1.Rows[i][18].ToString(), dt1.Rows[i][19].ToString(), dt1.Rows[i][20].ToString(), dt1.Rows[i][21].ToString(), dt1.Rows[i][22].ToString(),
                    dt1.Rows[i][23].ToString(), dt1.Rows[i][24].ToString(), dt1.Rows[i][25].ToString(), dt1.Rows[i][26].ToString(),
                    true, dt1.Rows[i][27].ToString(), dt1.Rows[i][28].ToString(),
                    int.TryParse(dt1.Rows[i][29]?.ToString(), out var tmpId) ? tmpId : -1,
                    dt1.Rows[i][30].ToString()
                ));
            }

            // Проверка соединения
            if (m_dbConn.State != ConnectionState.Open)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно, установите соединение{Environment.NewLine}");
                return;
            }

            // Начинаем транзакцию
            using (var transaction = m_dbConn.BeginTransaction())
            {
                m_sqlCmd = new MySqlCommand();
                m_sqlCmd.Connection = m_dbConn;
                m_sqlCmd.Transaction = transaction;

                Random random1 = new Random();
                Random random2 = new Random();
                Random random3 = new Random();

                try
                {
                    foreach (var vc1 in vc1List)
                    {
                        if (string.IsNullOrWhiteSpace(vc1.Barcode))
                        {
                            string bcode1 = random1.Next(1111, 9999).ToString();
                            string bcode2 = random2.Next(1111, 9999).ToString();
                            string bcode3 = random3.Next(333, 999).ToString();
                            string bcode = barcodeLastDigit(bcode1 + bcode2 + bcode3);

                            m_sqlCmd.CommandText = "UPDATE Catalog SET Barcode = @barcode WHERE ID = @id";
                            m_sqlCmd.Parameters.Clear();
                            m_sqlCmd.Parameters.AddWithValue("@barcode", bcode);
                            m_sqlCmd.Parameters.AddWithValue("@id", vc1.Id);
                            m_sqlCmd.ExecuteNonQuery();
                        }
                    }

                    // Завершаем транзакцию
                    transaction.Commit();
                }
                catch (MySqlException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при записи в MySQL: {ex.Message}{Environment.NewLine}");
                }
            }
        }



        // ------------------------------------------------------------------------------------Создаём таблицу истории
        public void HistoryBDTable_Create()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();


            try
            {

                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS HistoryBD(id INTEGER PRIMARY KEY AUTOINCREMENT, WHO TEXT, WHAT TEXT, FULLWHAT TEXT, DATA TEXT, IDINCATALOG TEXT)";
                m_sqlCmd.ExecuteNonQuery();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Не могу установить соединение с базой данных " + ex.ToString() + Environment.NewLine);
            }
        }

        //Поиск по ФИО и остальным параметрам
        // Чтение из базы для поиска в истории по ФИО, дате, What и Who (MySQL)
        public DataTable HISTORYSearchFIO(
            string FIO = "",
            string Date = "",
            string WHAT_HistoryBD = "",
            string WHO_HistoryBD = "")
        {
            var dt = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            SELECT
                hbd.*,
                clmap.FIO,
                cat.*
            FROM Catalog AS cat
            JOIN ClientsMap  AS clmap ON cat.ClientId = clmap.id
            JOIN HistoryBD    AS hbd   ON cat.id        = hbd.IDINCATALOG
            WHERE hbd.WHO  LIKE @fio
              AND hbd.WHO  LIKE @who
              AND hbd.WHAT LIKE @what
              AND hbd.DATA LIKE @date;
        "
            };

            // подставляем параметры с шаблонами для LIKE
            m_sqlCmd.Parameters.AddWithValue("@fio", $"%{FIO.ToUpper().Trim()}%");
            m_sqlCmd.Parameters.AddWithValue("@who", $"%{WHO_HistoryBD.ToUpper().Trim()}%");
            m_sqlCmd.Parameters.AddWithValue("@what", $"%{WHAT_HistoryBD.ToUpper().Trim()}%");
            m_sqlCmd.Parameters.AddWithValue("@date", $"%{Date.ToUpper().Trim()}%");

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при поиске в истории из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dt;
        }


        // Запись в историю изменений (MySQL)
        public void HistoryBDWrite(
            string WHO,
            string WHAT,
            string FULLWHAT,
            string IDINCATALOG,
            string DATA = "")
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO HistoryBD
                (WHO, WHAT, FULLWHAT, IDINCATALOG, `DATA`)
            VALUES
                (@who, @what, @fullwhat, @idInCatalog, @data);
        "
            };

            // Подготовка значения поля DATA
            var timestamp = string.IsNullOrEmpty(DATA)
                ? DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                : DATA;

            // Параметры запроса
            m_sqlCmd.Parameters.AddWithValue("@who", WHO.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@what", WHAT.Trim().ToUpper());
            m_sqlCmd.Parameters.AddWithValue("@fullwhat", FULLWHAT.Trim());
            m_sqlCmd.Parameters.AddWithValue("@idInCatalog", IDINCATALOG.Trim());
            m_sqlCmd.Parameters.AddWithValue("@data", timestamp);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при записи в историю MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        // ------------------------------------------------------------------------------- база пользователей (управление)
        // Создаёт таблицу пользователей в MySQL
        public void UsersTable_Create()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            CREATE TABLE IF NOT EXISTS `Users` (
                `id` INT          AUTO_INCREMENT PRIMARY KEY,
                `type` VARCHAR(50)        NOT NULL,
                `name` VARCHAR(100)       NOT NULL,
                `id_gruppi_dostupa` INT   NOT NULL,
                `user_pwd` VARCHAR(255)   NOT NULL
            ) ENGINE=InnoDB
              DEFAULT CHARSET=utf8mb4;
        "
            };

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Не могу создать таблицу Users в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        // Запись нового пользователя в MySQL
        public void UsersBDWrite(string type, string name, string id_gruppi_dostupa, string user_pwd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            INSERT INTO `Users`
                (`type`, `name`, `id_gruppi_dostupa`, `user_pwd`)
            VALUES
                (@type, @name, @groupId, @pwd);
        "
            };

            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@type", type.Trim());
            m_sqlCmd.Parameters.AddWithValue("@name", name.Trim());
            m_sqlCmd.Parameters.AddWithValue("@groupId", id_gruppi_dostupa.Trim());
            m_sqlCmd.Parameters.AddWithValue("@pwd", user_pwd);

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при добавлении пользователя в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Изменение в базе данных
        // Обновление пароля (или любого другого текстового поля) пользователя в MySQL
        public void UsersBdEditPassword(string column, string newValue, string userName)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = $@"
            UPDATE `Users`
            SET `{column}` = @newVal
            WHERE `name` = @userName;
        "
            };

            // Параметры
            m_sqlCmd.Parameters.AddWithValue("@newVal", newValue.Trim());
            m_sqlCmd.Parameters.AddWithValue("@userName", userName.Trim());

            try
            {
                m_dbConn.Open();
                int rowsAffected = m_sqlCmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show(
                        $"Пользователь \"{userName}\" не найден.",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при обновлении поля `{column}` в MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        // Чтение из базы данных
        // Чтение всех пользователей из MySQL
        public DataTable UsersBdRead()
        {
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM `Users`;"
            };

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении пользователей из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }


        // Чтение конкретного пользователя из MySQL по имени
        public DataTable UsersBdRead(string name)
        {
            var dTable = new DataTable();

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT * FROM `Users` WHERE `name` = @name;"
            };
            m_sqlCmd.Parameters.AddWithValue("@name", name.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при чтении пользователя из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }


        // Удаление пользователя из MySQL по имени
        public void UserDelete(string userName)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "DELETE FROM `Users` WHERE `name` = @username"
            };
            m_sqlCmd.Parameters.AddWithValue("@username", userName.Trim());

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(
                    $"{DateTime.Now:HH:mm}: Ошибка при удалении пользователя из MySQL:\n{ex.Message}",
                    "Ошибка MySQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        public void UserBdEditAll(string type, string name, string id_gruppi_dostupa)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = @"
            UPDATE `Users`
            SET `type` = @type,
                `id_gruppi_dostupa` = @groupId
            WHERE `name` = @name"
            };

            m_sqlCmd.Parameters.AddWithValue("@type", type.Trim());
            m_sqlCmd.Parameters.AddWithValue("@groupId", id_gruppi_dostupa.Trim());
            m_sqlCmd.Parameters.AddWithValue("@name", name.Trim());

            try
            {
                m_dbConn.Open();
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при обновлении данных пользователя:\n{ex.Message}",
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        // Чтение из базы данных
        public string UsersGetPass(string name)
        {
            DataTable dTable = new DataTable();
            string password = "-1";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT user_pwd FROM Users WHERE name = @name"
            };
            m_sqlCmd.Parameters.AddWithValue("@name", name.Trim());

            try
            {
                m_dbConn.Open();
                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    password = dTable.Rows[0]["user_pwd"].ToString();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при получении пароля из базы данных:\n{ex.Message}",
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return password;
        }

        // Чтение из базы данных
        public string UsersGetGroupIdByUserName(string name)
        {
            DataTable dTable = new DataTable();
            string groupId = "-1";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand
            {
                Connection = m_dbConn,
                CommandText = "SELECT id_gruppi_dostupa FROM Users WHERE name = @name"
            };
            m_sqlCmd.Parameters.AddWithValue("@name", name.Trim());

            try
            {
                m_dbConn.Open();

                using (var adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    groupId = dTable.Rows[0]["id_gruppi_dostupa"].ToString();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при получении ID группы доступа из MySQL:\n{ex.Message}",
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return groupId;
        }

        // ------------------------------------------------------------------------------- база групп доступа (управление)
        public void GroupDostupTable_Create()
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS GroupDostup (
                id INT AUTO_INCREMENT PRIMARY KEY,
                grName VARCHAR(255),
                delZapis TINYINT(1),
                addZapis TINYINT(1),
                saveZapis TINYINT(1),
                graf TINYINT(1),
                sms TINYINT(1),
                stock TINYINT(1),
                clients TINYINT(1),
                stockAdd TINYINT(1),
                stockDel TINYINT(1),
                stockEdit TINYINT(1),
                clientAdd TINYINT(1),
                clientDel TINYINT(1),
                clientConcat TINYINT(1),
                settings TINYINT(1),
                dates TINYINT(1),
                editDates TINYINT(1)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ";

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Не могу создать таблицу GroupDostup в MySQL\n{ex.Message}",
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        public void GroupDostupBDWrite(string grName, string delZapis, string addZapis, string saveZapis, string graf, string sms, string stock, string clients,
    string stockAdd, string stockDel, string stockEdit, string clientAdd, string clientDel, string clientConcat, string settings, string dates, string editDates)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = @"
            INSERT INTO GroupDostup 
            (grName, delZapis, addZapis, saveZapis, graf, sms, stock, clients, 
             stockAdd, stockDel, stockEdit, clientAdd, clientDel, clientConcat, 
             settings, dates, editDates)
            VALUES 
            (@grName, @delZapis, @addZapis, @saveZapis, @graf, @sms, @stock, @clients, 
             @stockAdd, @stockDel, @stockEdit, @clientAdd, @clientDel, @clientConcat, 
             @settings, @dates, @editDates);
        ";

                // Добавляем параметры
                m_sqlCmd.Parameters.AddWithValue("@grName", grName);
                m_sqlCmd.Parameters.AddWithValue("@delZapis", delZapis);
                m_sqlCmd.Parameters.AddWithValue("@addZapis", addZapis);
                m_sqlCmd.Parameters.AddWithValue("@saveZapis", saveZapis);
                m_sqlCmd.Parameters.AddWithValue("@graf", graf);
                m_sqlCmd.Parameters.AddWithValue("@sms", sms);
                m_sqlCmd.Parameters.AddWithValue("@stock", stock);
                m_sqlCmd.Parameters.AddWithValue("@clients", clients);
                m_sqlCmd.Parameters.AddWithValue("@stockAdd", stockAdd);
                m_sqlCmd.Parameters.AddWithValue("@stockDel", stockDel);
                m_sqlCmd.Parameters.AddWithValue("@stockEdit", stockEdit);
                m_sqlCmd.Parameters.AddWithValue("@clientAdd", clientAdd);
                m_sqlCmd.Parameters.AddWithValue("@clientDel", clientDel);
                m_sqlCmd.Parameters.AddWithValue("@clientConcat", clientConcat);
                m_sqlCmd.Parameters.AddWithValue("@settings", settings);
                m_sqlCmd.Parameters.AddWithValue("@dates", dates);
                m_sqlCmd.Parameters.AddWithValue("@editDates", editDates);

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при записи в таблицу GroupDostup (MySQL):\n" + ex.Message,
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }


        //Изменение в базе данных
        public void GroupDostupBdEditOne(string editWhat, string editThis, string id_in_Usersbd)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                // Проверка на допустимость поля (белый список)
                var allowedFields = new HashSet<string>
                {
                    "grName", "delZapis", "addZapis", "saveZapis", "graf", "sms", "stock", "clients",
                    "stockAdd", "stockDel", "stockEdit", "clientAdd", "clientDel", "clientConcat",
                    "settings", "dates", "editDates"
                };

                if (!allowedFields.Contains(editWhat))
                {
                    MessageBox.Show($"Попытка обновления недопустимого поля: {editWhat}");
                    return;
                }

                m_sqlCmd.CommandText = $"UPDATE GroupDostup SET {editWhat} = @editValue WHERE id = @id";
                m_sqlCmd.Parameters.AddWithValue("@editValue", editThis);
                m_sqlCmd.Parameters.AddWithValue("@id", id_in_Usersbd);

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при обновлении записи в GroupDostup (MySQL):\n" + ex.Message,
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }




        //Изменение в базе данных
        public void GroupDostupBdEditAll(
      string grName, string delZapis, string addZapis, string saveZapis, string graf, string sms, string stock, string clients,
      string stockAdd, string stockDel, string stockEdit, string clientAdd, string clientDel, string clientConcat,
      string settings, string dates, string editDates)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = @"
            UPDATE GroupDostup SET
                delZapis     = @delZapis,
                addZapis     = @addZapis,
                saveZapis    = @saveZapis,
                graf         = @graf,
                sms          = @sms,
                stock        = @stock,
                clients      = @clients,
                stockAdd     = @stockAdd,
                stockDel     = @stockDel,
                stockEdit    = @stockEdit,
                clientAdd    = @clientAdd,
                clientDel    = @clientDel,
                clientConcat = @clientConcat,
                settings     = @settings,
                dates        = @dates,
                editDates    = @editDates
            WHERE grName = @grName";

                m_sqlCmd.Parameters.AddWithValue("@grName", grName.Trim());
                m_sqlCmd.Parameters.AddWithValue("@delZapis", delZapis.Trim());
                m_sqlCmd.Parameters.AddWithValue("@addZapis", addZapis.Trim());
                m_sqlCmd.Parameters.AddWithValue("@saveZapis", saveZapis.Trim());
                m_sqlCmd.Parameters.AddWithValue("@graf", graf.Trim());
                m_sqlCmd.Parameters.AddWithValue("@sms", sms.Trim());
                m_sqlCmd.Parameters.AddWithValue("@stock", stock.Trim());
                m_sqlCmd.Parameters.AddWithValue("@clients", clients.Trim());
                m_sqlCmd.Parameters.AddWithValue("@stockAdd", stockAdd.Trim());
                m_sqlCmd.Parameters.AddWithValue("@stockDel", stockDel.Trim());
                m_sqlCmd.Parameters.AddWithValue("@stockEdit", stockEdit.Trim());
                m_sqlCmd.Parameters.AddWithValue("@clientAdd", clientAdd.Trim());
                m_sqlCmd.Parameters.AddWithValue("@clientDel", clientDel.Trim());
                m_sqlCmd.Parameters.AddWithValue("@clientConcat", clientConcat.Trim());
                m_sqlCmd.Parameters.AddWithValue("@settings", settings.Trim());
                m_sqlCmd.Parameters.AddWithValue("@dates", dates.Trim());
                m_sqlCmd.Parameters.AddWithValue("@editDates", editDates.Trim());

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при обновлении прав доступа в MySQL:\n" + ex.Message,
                                "Ошибка MySQL",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        // Чтение из базы данных
        public DataTable GroupDostupBdRead()
        {
            DataTable dTable = new DataTable();
            string sqlQuery = "SELECT * FROM GroupDostup";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand(sqlQuery, m_dbConn);

            try
            {
                m_dbConn.Open();
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Соединение с базой данных потеряно. Пожалуйста, переподключитесь." + Environment.NewLine);
                    return dTable;
                }

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                return dTable;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при чтении из MySQL: " + ex.Message + Environment.NewLine,
                                "Ошибка MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }

        // Чтение из базы данных
        public DataTable GroupDostupBdRead(string grName)
        {
            DataTable dTable = new DataTable();
            string sqlQuery = "SELECT * FROM GroupDostup WHERE grName = @grName";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand(sqlQuery, m_dbConn);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Соединение с базой данных потеряно. Пожалуйста, переподключитесь." + Environment.NewLine);
                    return dTable;
                }

                m_sqlCmd.Parameters.AddWithValue("@grName", grName.Trim());

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                return dTable;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(DateTime.Now.ToShortTimeString() + ": Ошибка при чтении из MySQL: " + ex.Message + Environment.NewLine,
                                "Ошибка MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

            return dTable;
        }


        // Чтение из базы данных
        public string GroupDostupGetIdByGrNameBdRead(string grName)
        {
            DataTable dTable = new DataTable();
            string sqlQuery = "SELECT id FROM GroupDostup WHERE grName = @grName";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand(sqlQuery, m_dbConn);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show($"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно. Установите соединение." + Environment.NewLine);
                    return "-1";
                }

                m_sqlCmd.Parameters.AddWithValue("@grName", grName.Trim());

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    return dTable.Rows[0]["id"].ToString();
                }

                return "-1";
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при чтении из базы MySQL:\n{ex.Message}", "Ошибка MySQL",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "-1";
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        // Чтение из базы данных
        public string GroupDostupGetgrNameByIdBdRead(string id)
        {
            DataTable dTable = new DataTable();
            string sqlQuery = "SELECT grName FROM GroupDostup WHERE id = @id";

            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand(sqlQuery, m_dbConn);

            try
            {
                m_dbConn.Open();

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show($"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно. Установите соединение." + Environment.NewLine);
                    return "-1";
                }

                m_sqlCmd.Parameters.AddWithValue("@id", id.Trim());

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(dTable);
                }

                if (dTable.Rows.Count > 0)
                {
                    return dTable.Rows[0]["grName"].ToString();
                }

                return "-1";
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Ошибка при чтении grName по ID из MySQL:\n{ex.Message}", "Ошибка MySQL",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "-1";
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }
        }

        //Удаление из бд записей клиента
        public void GroupDostupDelete(string grName)
        {
            MySqlConnection m_dbConn = GetMySqlConnection();
            m_sqlCmd = new MySqlCommand();

            try
            {
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show($"{DateTime.Now:HH:mm}: Соединение с базой данных потеряно. Установите соединение." + Environment.NewLine);
                    return;
                }

                m_sqlCmd.CommandText = "DELETE FROM GroupDostup WHERE grName = @grName";
                m_sqlCmd.Parameters.AddWithValue("@grName", grName.Trim());

                m_sqlCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show($"{DateTime.Now:HH:mm}: Что-то пошло не так при удалении группы доступа:\n{ex.Message}", "Ошибка MySQL",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_dbConn.State == ConnectionState.Open)
                    m_dbConn.Close();
            }

        }


    }


}
