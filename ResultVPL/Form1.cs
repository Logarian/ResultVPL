using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace ResultVPL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private SqlCommand cmd;
        private SqlConnection con;
        private string constring = @"Data Source=ServerName;Integrated Security=True";

        public string ReadFromFile(string fName)
        {
            string res;
            try
            {
                StreamReader reader = new StreamReader(fName, Encoding.Default);
                res = reader.ReadToEnd();
                reader.Close();
                return res;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка чтения файла:\n" + ex.Message);
                return "";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
            dataGridView1.Rows.Clear();
            con = new SqlConnection(constring);
            con.Open();
            cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\querys\\getAllDataBaseName.sql"), con);
            cmd.ExecuteNonQuery();
            DataTable dtDBName = new DataTable();
            SqlDataAdapter daDBName = new SqlDataAdapter(cmd);
            daDBName.Fill(dtDBName);
            con.Close();
            
            for (int i = 0; i < dtDBName.Rows.Count; i++)
            {
                con = new SqlConnection(constring);
                con.Open();
                //cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\querys\\searchAllParticipants.sql").Replace("#dataBaseName#", dtDBName.Rows[i]["name"].ToString()).Replace("#surname#", tbSurname.Text.ToString()).Replace("#name#", tbName.Text.ToString()).Replace("#secondName#", tbSecondName.Text.ToString()).Replace("#docSeries#", tbDocSeries.Text.ToString()).Replace("#docNumber#", tbDocNumber.Text.ToString()), con);
                cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\querys\\searchAllParticipantsWithNotExams.sql").Replace("#dataBaseName#", dtDBName.Rows[i]["name"].ToString()).Replace("#surname#", tbSurname.Text.ToString()).Replace("#name#", tbName.Text.ToString()).Replace("#secondName#", tbSecondName.Text.ToString()).Replace("#docSeries#", tbDocSeries.Text.ToString()).Replace("#docNumber#", tbDocNumber.Text.ToString()), con);
                SqlDataReader dr = cmd.ExecuteReader();

                List<string[]> prtData = new List<string[]>();

                while (dr.Read())
                {
                    prtData.Add(new string[4]);

                    prtData[prtData.Count - 1][0] = dr[0].ToString();
                    prtData[prtData.Count - 1][1] = dr[1].ToString();
                    prtData[prtData.Count - 1][2] = dr[2].ToString();
                    prtData[prtData.Count - 1][3] = dr[3].ToString();
                }
                dr.Close();
                con.Close();

                foreach (string[] s in prtData)
                {
                    dataGridView1.Rows.Add(s);
                }
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnCreateBlank_Click(object sender, EventArgs e)
        {
            string FIO = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            string[] FIOarr = FIO.Split(' ');

            var pdfDoc = new Document(PageSize.A4, 45f, 45f, 50f, 35f);
            PdfWriter.GetInstance(pdfDoc, new FileStream(FIOarr[0] + ' ' + FIOarr[1].ToString().Substring(0,1) + '.' + FIOarr[2].ToString().Substring(0,1) + ".pdf", FileMode.OpenOrCreate));
            pdfDoc.Open();

            #region Отступы
            var spacer30 = new Paragraph("")
            {
                SpacingAfter = 30f,
            };
            var spacer10 = new Paragraph("")
            {
                SpacingAfter = 10f,
            };
            var spacer5 = new Paragraph("")
            {
                SpacingAfter = 5f,
            };
            var spacer15 = new Paragraph("")
            {
                SpacingAfter = 15f,
            };
            var spacer20 = new Paragraph("")
            {
                SpacingAfter = 20f,
            };
            var spacer25 = new Paragraph("")
            {
                SpacingAfter = 25f,
            };
            var spacer35 = new Paragraph("")
            {
                SpacingAfter = 35f,
            };
            #endregion

            Paragraph headOrder = new Paragraph();
            BaseFont bf = BaseFont.CreateFont("c:/Windows/Fonts/times.ttf", BaseFont.IDENTITY_H, false);
            iTextSharp.text.Font fntHeadOrder = new iTextSharp.text.Font(bf, 16, 1, iTextSharp.text.BaseColor.BLACK); // Жирный текст
            iTextSharp.text.Font fntFootNote = new iTextSharp.text.Font(bf, 12, 0, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fntFootNote_bold = new iTextSharp.text.Font(bf, 12, 1, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font fntUnderLine = new iTextSharp.text.Font(bf, 8, 0, iTextSharp.text.BaseColor.BLACK); // Простой текст
            headOrder.Alignment = Element.ALIGN_CENTER;
            headOrder.Add(new Chunk("СПРАВКА", fntHeadOrder));
            pdfDoc.Add(headOrder);

            pdfDoc.Add(spacer10);

            Paragraph t1 = new Paragraph();
            t1.Alignment = Element.ALIGN_CENTER;
            t1.Add(new Chunk("о результатах единого государственного экзамена", fntFootNote_bold));
            pdfDoc.Add(t1);

            pdfDoc.Add(spacer10);

            Paragraph t2 = new Paragraph();
            t2.Alignment = Element.ALIGN_CENTER;
            t2.Add(new Chunk(dataGridView1.SelectedRows[0].Cells[1].Value.ToString(), fntFootNote_bold));
            pdfDoc.Add(t2);

            pdfDoc.Add(spacer5);

            Paragraph t3 = new Paragraph();
            t3.Alignment = Element.ALIGN_CENTER;
            t3.Add(new Chunk("документ, удостоверяющий личность: " + dataGridView1.SelectedRows[0].Cells[2].Value.ToString(), fntFootNote));
            pdfDoc.Add(t3);

            pdfDoc.Add(spacer10);

            Paragraph t4 = new Paragraph();
            t4.Alignment = Element.ALIGN_CENTER;
            t4.Add(new Chunk("по результатам сдачи единого государственного экзамена обнаружил(а) следующие знания по общеобразовательным предметам:", fntFootNote));
            pdfDoc.Add(t4);

            pdfDoc.Add(spacer10);

            #region Таблица с результатами экзаменов
            PdfPTable examTable = new PdfPTable(4);
            BaseFont bfTableHeader = BaseFont.CreateFont("c:/Windows/Fonts/times.ttf", BaseFont.IDENTITY_H, false);
            iTextSharp.text.Font fntTableHeader = new iTextSharp.text.Font(bfTableHeader, 12, 1, iTextSharp.text.BaseColor.BLACK); // Жирный текст
            iTextSharp.text.Font fntTableContent = new iTextSharp.text.Font(bfTableHeader, 12, 0, iTextSharp.text.BaseColor.BLACK); // Простой текст
            examTable.WidthPercentage = 100;
            examTable.DefaultCell.HorizontalAlignment = 1;
            examTable.DefaultCell.VerticalAlignment = 1;
            float[] wid = new float[] { 40, 10, 15, 35 };
            examTable.SetWidths(wid);

            PdfPCell cell = new PdfPCell(new Phrase("Наименование предмета", fntTableContent)) { PaddingBottom = 7 };
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            examTable.AddCell(cell);
            cell = new PdfPCell(new Phrase("Балл", fntTableContent)) { PaddingBottom = 7 };
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            examTable.AddCell(cell);
            cell = new PdfPCell(new Phrase("Год сдачи", fntTableContent)) { PaddingBottom = 7 };
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            examTable.AddCell(cell);
            cell = new PdfPCell(new Phrase("Статус результата", fntTableContent)) { PaddingBottom = 7 };
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            examTable.AddCell(cell);

            con = new SqlConnection(constring);
            con.Open();
            cmd = new SqlCommand(ReadFromFile(Environment.CurrentDirectory + "\\querys\\getParticipantExams.sql").Replace("#dataBaseName#", dataGridView1.SelectedRows[0].Cells[3].Value.ToString()).Replace("#participantID#", dataGridView1.SelectedRows[0].Cells[0].Value.ToString()), con);
            cmd.ExecuteNonQuery();
            DataTable dtPartExam = new DataTable();
            SqlDataAdapter daPartExam = new SqlDataAdapter(cmd);
            daPartExam.Fill(dtPartExam);
            con.Close();

            string yearFromDBName = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            string[] year = yearFromDBName.Split('_');

            for (int i = 0; i < dtPartExam.Rows.Count; i++)
            {
                cell = new PdfPCell(new Phrase(dtPartExam.Rows[i]["SubjectName"].ToString(), fntTableContent)) { PaddingBottom = 7 };
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                examTable.AddCell(cell);

                if ((Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) == 20 || 
                    Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) == 21) &&
                    Convert.ToInt32(dtPartExam.Rows[i]["Mark100"]) == 1)
                {
                    cell = new PdfPCell(new Phrase("Зачет", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                else if ((Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) == 20 ||
                    Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) == 21) &&
                    Convert.ToInt32(dtPartExam.Rows[i]["Mark100"]) == 0)
                {
                    cell = new PdfPCell(new Phrase("Незачет", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(dtPartExam.Rows[i]["Mark100"].ToString(), fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }

                cell = new PdfPCell(new Phrase("20" + year[3], fntTableContent)) { PaddingBottom = 7 };
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                examTable.AddCell(cell);

                if ((Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) != 20 &&
                    Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) != 21) &&
                    (Convert.ToInt32(dtPartExam.Rows[i]["Mark100"]) > Convert.ToInt32(dtPartExam.Rows[i]["MarkBorder"])) &&
                    (DateTime.Now.Year - Convert.ToInt32("20" + year[3])) <= 4)
                {
                    cell = new PdfPCell(new Phrase("Действующий", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                else if ((Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) != 20 &&
                    Convert.ToInt32(dtPartExam.Rows[i]["SubjectCode"]) != 21) &&
                    (Convert.ToInt32(dtPartExam.Rows[i]["Mark100"]) < Convert.ToInt32(dtPartExam.Rows[i]["MarkBorder"])) &&
                    (DateTime.Now.Year - Convert.ToInt32("20" + year[3])) <= 4)
                {
                    cell = new PdfPCell(new Phrase("Ниже минимума", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                else if ((DateTime.Now.Year - Convert.ToInt32("20" + year[3])) > 4)
                {
                    cell = new PdfPCell(new Phrase("Не действующий", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase("Действующий", fntTableContent)) { PaddingBottom = 7 };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    examTable.AddCell(cell);
                }
                
            }

            pdfDoc.Add(examTable);
            #endregion

            pdfDoc.Add(spacer10);

            Paragraph t5 = new Paragraph();
            t5.Alignment = Element.ALIGN_LEFT;
            t5.Add(new Chunk("Справка сформирована из РИС ГИА организацией:", fntFootNote));
            pdfDoc.Add(t5);

            Paragraph t6 = new Paragraph();
            t6.Alignment = Element.ALIGN_LEFT;
            t6.Add(new Chunk("Государственное автономное учреждение Иркутской области «Центр оценки профессионального мастерства, квалификаций педагогов и мониторинга качества образования»", fntFootNote));
            pdfDoc.Add(t6);

            pdfDoc.Add(spacer20);

            Paragraph t7 = new Paragraph();
            t7.Alignment = Element.ALIGN_LEFT;
            t7.Add(new Chunk("Дата и время формирования справки: " + DateTime.Now.ToString(), fntFootNote));
            pdfDoc.Add(t7);

            pdfDoc.Add(spacer30);

            Paragraph t8 = new Paragraph();
            t8.Alignment = Element.ALIGN_LEFT;
            t8.Add(new Chunk("Лицо, сформировавшее справку:", fntFootNote));
            pdfDoc.Add(t8);

            pdfDoc.Add(spacer15);

            Paragraph t10 = new Paragraph();
            t10.Alignment = Element.ALIGN_LEFT;
            t10.Add(new Chunk("____________________  ____________________  __________________________________________", fntFootNote));
            pdfDoc.Add(t10);

            Paragraph t11 = new Paragraph();
            t11.Alignment = Element.ALIGN_LEFT;
            t11.SetLeading(11, 0);
            t11.Add(new Chunk("                     (должность)                                          (подпись)                                                                            (Фамилия И.О.)", fntUnderLine));
            pdfDoc.Add(t11);

            pdfDoc.Add(spacer35);

            Paragraph t9 = new Paragraph();
            t9.Alignment = Element.ALIGN_LEFT;
            t9.Add(new Chunk("Дата выдачи «____» ________________  _________ г. регистрационный № ____________________", fntFootNote));
            pdfDoc.Add(t9);

            pdfDoc.Close();

            MessageBox.Show("Справка для " + FIOarr[0] + ' ' + FIOarr[1].ToString().Substring(0, 1) + '.' + FIOarr[2].ToString().Substring(0, 1) + " сформирована!");

        }
    }
}
