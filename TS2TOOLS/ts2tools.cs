using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TS2TOOLS
{
    public partial class ts2tools : Form
    {

        Regex r = new Regex("[^A-Z0-9.$ ]$");
        string menu = "";
        public ts2tools()
        {
            InitializeComponent();
        }

        private void Error(int a)
        {
            string msg = "";
            switch (a)
            {
                case 1:
                    msg = "ไฟล์ไม่ถูกต้อง";
                    break;

            }
            MessageBox.Show(msg, "ERROR!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        public byte[] MarkPacket(byte[] byte_0,byte xor)
        {
            byte[] array = new byte[byte_0.Length - 1 + 1];
            int arg_14_0 = 0;
            int num = byte_0.Length - 1;
            for (int i = arg_14_0; i <= num; i++)
            {
                array[i] = (byte)(byte_0[i] ^ xor);
            }
            return array;
        }


        private string ByteToStringX2(byte[] byte_0)
        {
            string text = "";
            for (int i = 0; i < byte_0.Length; i++)
            {
                byte b = byte_0[i];
                text += b.ToString("X2");
            }
            return text;
        }

        private int ByteArrayToInt4(byte[] byte_0)
        {
            return Convert.ToInt32(ByteToStringX2(new byte[]
            {
            byte_0[3],
            byte_0[2],
            byte_0[1],
            byte_0[0]
            }), 16);
        }

        private int ByteArrayToInt2(byte[] byte_0)
        {
            return Convert.ToInt32(ByteToStringX2(new byte[]
            {
                byte_0[1],
                byte_0[0]
            }), 16);
        }

        private long ByteArrayToLong4(byte[] byte_0)
        {
            return Convert.ToInt64(ByteToStringX2(new byte[]
            {
                byte_0[3],
                byte_0[2],
                byte_0[1],
                byte_0[0]
            }), 16);
        }


        public byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        private long ByteArrayToLong(byte[] byte_0)
        {
            string v = "";
            switch(byte_0.Length)
            {
                case 1:
                    v = ByteToStringX2(new byte[] { byte_0[0] });
                    break;
                case 2:
                    v = ByteToStringX2(new byte[] { byte_0[1], byte_0[0]});
                    break;
                case 4:
                    v = ByteToStringX2(new byte[] { byte_0[3], byte_0[2], byte_0[1], byte_0[0] });
                    break;
                default:
                    v = "0";
                    break;
            }
            return Convert.ToInt64(v,16);
        }

        public string IntTo1BS(int int_2)
        {
            return int_2.ToString("X2");
        }

        public string IntTo2BS(int int_2)
        {
            return int_2.ToString("X4").Substring(2, 2) + int_2.ToString("X4").Substring(0, 2);
        }

        public static string IntTo4BS(int int_2)
        {
            return int_2.ToString("X8").Substring(6, 2) + int_2.ToString("X8").Substring(4, 2) + int_2.ToString("X8").Substring(2, 2) + int_2.ToString("X8").Substring(0, 2);
        }


        public byte[] StringToByteArray(string string_0)
        {
            byte[] array = new byte[(int)Math.Round(unchecked((double)string_0.Length / 2.0 - 1.0)) + 1];
            try
            {
                for (int i = 0; i < string_0.Length; i += 2)
                {
                    array[(int)Math.Round((double)i / 2.0)] = byte.Parse(string_0.Substring(i, 2), NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {

            }
            return array;
        }

        public byte[] CombineArray(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        private byte[] generateKey(int size)
        {
            byte[] buffer = new byte[size];
            new Random().NextBytes(buffer);
            return buffer;
        }

        public byte[] Decode(byte[] data, byte xor)
        {
            byte[] newdata = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                newdata[i] = data[i] ^= xor;
            }
            return newdata;
        }

        private void decryptData(byte[] data, int rounds, bool reverseOrder, bool scan = false)
        {
            if (data == null)
                return;
            for (int index1 = 0; index1 < rounds; ++index1)
            {
                int index2 = reverseOrder ? 0 : data.Length - 1;
                data[index2] = (byte)((int)data[data.Length - 1] ^ (int)byte.MaxValue ^ ((int)data[0] ^ (int)byte.MaxValue) ^ (int)byte.MaxValue);
                if (reverseOrder)
                {
                    for (int index3 = data.Length - 2; index3 > -1; --index3)
                        data[index3 + 1] = (byte)((uint)data[index3 + 1] ^ (uint)data[index3]);
                }
                else
                {
                    for (int index3 = 1; index3 < data.Length; ++index3)
                        data[index3 - 1] = (byte)((uint)data[index3 - 1] ^ (uint)data[index3]);
                }
                for (int index3 = 0; index3 < data.Length; ++index3)
                    data[index3] = (byte)((uint)data[index3] ^ (uint)byte.MaxValue);
                if (scan && data[0] == (byte)120 && data[1] == (byte)156)
                {
                    int num = (int)MessageBox.Show(string.Format("{0}", (object)index1));
                    return;
                }
            }
            if (!scan)
                return;
            int num1 = (int)MessageBox.Show("Round not found");
        }

        private void encryptData(byte[] data, int rounds, bool reverseOrder)
        {
            if (data == null)
                return;
            for (int index1 = 0; index1 < rounds; ++index1)
            {
                int index2 = reverseOrder ? 0 : data.Length - 1;
                for (int index3 = 0; index3 < data.Length; ++index3)
                    data[index3] = (byte)((uint)data[index3] ^ (uint)byte.MaxValue);
                if (reverseOrder)
                {
                    for (int index3 = 0; index3 < data.Length - 1; ++index3)
                        data[index3 + 1] = (byte)((uint)data[index3 + 1] ^ (uint)data[index3]);
                }
                else
                {
                    for (int index3 = data.Length - 1; index3 > 0; --index3)
                        data[index3 - 1] = (byte)((uint)data[index3 - 1] ^ (uint)data[index3]);
                }
                data[index2] = (byte)((int)data[data.Length - 1] ^ (int)byte.MaxValue ^ ((int)data[0] ^ (int)byte.MaxValue) ^ (int)byte.MaxValue);
            }
        }


        private void WriteToCsv(string path, StringBuilder text)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(text);
            }
        }

        private void JmgBtn_Click(object sender, EventArgs e)
        {
            menu = "JMG";
            ContextMenu.Show(JmgBtn, new Point(0, JmgBtn.Height/2));
        }

        private void BzpBtn_Click(object sender, EventArgs e)
        {
            try
            {

                openfile.Filter = "BZP |*.Bzp";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {

                    MessageBox.Show(file);
                    string nfile = file.Split('.').GetValue(0).ToString();

                    using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                    {
                        int start = 2;
                        while (true)
                        {
                            b.BaseStream.Seek(start, SeekOrigin.Begin);
                            int nle = b.ReadByte();
                            b.BaseStream.Seek(start + 1, SeekOrigin.Begin);
                            byte[] name = b.ReadBytes(nle);
                            b.BaseStream.Seek(start + 24, SeekOrigin.Begin);
                            int offset = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(start + 28, SeekOrigin.Begin);
                            int end = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(offset, SeekOrigin.Begin);
                            byte[] data = b.ReadBytes(end);
                            string namefile = Encoding.GetEncoding("big5").GetString(name);
                            start += 32;

                            if (start >= b.BaseStream.Length || name.Length == 0 || !r.IsMatch(namefile))
                                break;
                            if (!Directory.Exists(nfile))
                            {
                                Directory.CreateDirectory(nfile);
                            }
                            File.WriteAllBytes(nfile + "/" + namefile, Ionic.Zlib.ZlibStream.UncompressBuffer(data));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void JmaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "JMA |*.Jma";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {

                    MessageBox.Show(file);
                    string nfile = file.Split('.').GetValue(0).ToString();
                    using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                    {
                        int start = 14;
                        while (true)
                        {
                            b.BaseStream.Seek(start, SeekOrigin.Begin);
                            int nle = b.ReadByte();
                            byte[] name = b.ReadBytes(nle);
                            b.BaseStream.Seek(start+20, SeekOrigin.Begin);
                            int length = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(start + 24, SeekOrigin.Begin);
                            int offsetStart = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(offsetStart, SeekOrigin.Begin);
                            byte[] data = b.ReadBytes(length);


                            //b.BaseStream.Seek(start, SeekOrigin.Begin);
                            //int offset = ByteArrayToInt4(b.ReadBytes(4));
                            //b.BaseStream.Seek(start + 4, SeekOrigin.Begin);
                            //int end = ByteArrayToInt4(b.ReadBytes(4));
                            //b.BaseStream.Seek(start + 8, SeekOrigin.Begin);
                            //int nle = b.ReadByte();
                            //byte[] name = b.ReadBytes(nle);
                            //b.BaseStream.Seek(offset, SeekOrigin.Begin);
                            string namefile = Encoding.GetEncoding("big5").GetString(name);
                            start += 28;
                            MessageBox.Show(start.ToString());
                            if (start >= b.BaseStream.Length || name.Length == 0)
                                break;
                            if (!Directory.Exists(nfile))
                            {
                                Directory.CreateDirectory(nfile);
                            }
                            File.WriteAllBytes(nfile + "/" + namefile, Ionic.Zlib.ZlibStream.UncompressBuffer(data));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void ItemBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT |*.Dat";
                DialogResult dr = this.openfile.ShowDialog();
                ArrayList array = new ArrayList()//offset - Length
                {
                    1,20,//Name
                    21,1,//Func1
                    22,1,//Func2
                    23,2,//Item Id
                    25,2,//Icon_m
                    27,2,//Icon_f
                    29,2,//Model_m
                    31,2,//Model_F
                    33,2,//St1
                    35,2,//St2
                    37,2,//St3
                    39,4,//QtySt1
                    43,4,//QtySt2
                    47,4,//QtySt3
                    51,1,//??
                    52,1,//??
                    53,1,//Position
                    54,1,//??
                    55,4,//Color1
                    59,4,//Color2
                    63,4,//Color3
                    67,4,//Color4
                    71,4,//Color5
                    75,4,//Color6
                    79,4,//Color7
                    83,4,//Color8
                    87,4,//Color9
                    91,4,//Color10
                    95,1,//??
                    96,1,//Lvuse
                    97,4,//Buy
                    101,4,//Sell
                    105,1,//Gender
                    106,1,//??
                    107,1,//??
                    108,1,//Trade
                    109,4,//??
                    110,4,//??
                    117,4,//??
                    121,1,//MaxQty
                    122,2,//Maxdura
                    124,2,//??
                    126,2,//ItemKey
                    128,2,//unk
                    130,2,//Grade,Mapid
                    132,2,//unk
                    134,2,//Job,X
                    136,2,//Aura,Y
                    138,2,//Set
                    140,2,//St5f
                    142,2,//St8f
                    144,2,//St10f
                    146,4,//Qty5f
                    150,4,//Qty8f
                    154,4,//Qty10f
                    158,2,//Skill id
                    159,1,//unk
                    160,1,//Skill lv 
                    161,2,//Npc id
                    163,2,//unk
                    166,254//Des
                };

                await Task.Run(() =>
                {
                    int start = 0;
                    foreach (String file in openfile.FileNames)
                    {
                        if (!file.Contains("Item_TH"))
                            Error(1);
                        string nfile = file.Split('.').GetValue(0).ToString();
                        using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            StringBuilder data = new StringBuilder();
                            ArrayList list = new ArrayList();
                            data.Append("name,func1,func2,item_id,icon_m,icon_f,model_m,model_f,st1,st2,st3,op_st1,op_st2,op_st3,unk1,unk2,eq_pos,unk3,color1,color2,color3,color4,color5,color6,color7,color8,color9,color10," +
                                        "unk4,use_lv,buy,sell,use_gender,unk5,unk6,trade_st,unk7,unk8,unk9,max_qty,max_dura,unk10,item_key,unk12,item_grade|map_id,unk13,use_job|map_x,spirit_el|map_y,item_set,st5f,st8f,st10f,qty5f,qty8f,qty10f,skill_id,unk17,skill_lv,npc_id,unk18,des");
                            for (int i = 0; i < b.BaseStream.Length; i += 420)
                            {
                                data.AppendLine();
                                list.Clear();
                                for (int a = 0; a < array.Count; a += 2)
                                {
                                    int offset = Convert.ToInt32(array[a]);
                                    int Length = Convert.ToInt32(array[a + 1]);
                                    b.BaseStream.Seek(offset + i, SeekOrigin.Begin);
                                    if (Length == 1)
                                    {
                                        list.Add(b.ReadByte());
                                    }
                                    else if (Length == 2)
                                    {
                                        list.Add(ByteArrayToInt2(b.ReadBytes(2)));
                                    }
                                    else if (Length == 4)
                                    {
                                        //if(offset >= 55 && offset <= 91)
                                        //    list.Add(ByteArrayToLong4(b.ReadBytes(4)));
                                        //else
                                        list.Add(ByteArrayToInt4(b.ReadBytes(4)));
                                    }
                                    else
                                    {
                                        if (a == 0)
                                            start++;
                                        list.Add(Encoding.GetEncoding("TIS-620").GetString(b.ReadBytes(Length)).Replace("\n", "").Replace("\r", ""));
                                    }
                                }
                                foreach (var x in list)
                                {
                                    data.Append(x + ",");
                                }
                            }
                            if (data.Length > 0)
                            {
                                File.WriteAllText("Item_TH.csv", data.ToString(), Encoding.UTF8);
                                Logging("อ่านเสร็จสิ้น จำนวนไฟล์ทั้งหมด:" + start);
                                MessageBox.Show("สำเร็จ");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //private async void ItemBtn_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        openfile.Filter = "DAT |*.Dat";
        //        DialogResult dr = this.openfile.ShowDialog();
        //        ArrayList array = new ArrayList()
        //        {
        //            1,20,//Name
        //            21,1,//Func1
        //            22,1,//Func2
        //            23,2,//Item Id
        //            25,2,//Icon_m
        //            27,2,//Icon_f
        //            29,2,//Model_m
        //            31,2,//Model_F
        //            33,2,//St1
        //            35,2,//St2
        //            37,2,//St3
        //            39,4,//QtySt1
        //            43,4,//QtySt2
        //            47,4,//QtySt3
        //            51,1,//??
        //            52,1,//??
        //            53,1,//Position
        //            54,1,//??
        //            55,4,//Color1
        //            59,4,//Color2
        //            63,4,//Color3
        //            67,4,//Color4
        //            71,4,//Color5
        //            75,4,//Color6
        //            79,4,//Color7
        //            83,4,//Color8
        //            87,4,//Color9
        //            91,4,//Color10
        //            95,1,//??
        //            96,1,//Lvuse
        //            97,4,//Buy
        //            101,4,//Sell
        //            105,1,//Gender
        //            106,1,//??
        //            107,1,//??
        //            108,1,//Trade
        //            109,4,//??
        //            110,4,//??
        //            117,4,//??
        //            121,1,//MaxQty
        //            122,2,//Maxdura
        //            124,2,//??
        //            126,2,//ItemKey //Upgrade
        //            128,2,//unk
        //            130,1,//Grade
        //            131,1,//??
        //            132,2,//unk
        //            134,1,//Job
        //            135,1,//??
        //            136,2,//??
        //            138,2,//Set
        //            140,2,//St5f
        //            142,2,//St8f
        //            144,2,//St10f
        //            146,4,//Qty5f
        //            150,4,//Qty8f
        //            154,4,//Qty10f
        //            158,2,//Skill id
        //            159,1,//unk
        //            160,1,//Skill lv 
        //            161,2,//Npc id
        //            163,2,//unk
        //            166,254//Des
        //        };

        //        await Task.Run(() =>
        //        {
        //            int start = 0;
        //            foreach (String file in openfile.FileNames)
        //            {
        //                if (!file.Contains("Item_TH"))
        //                    Error(1);
        //                string nfile = file.Split('.').GetValue(0).ToString();
        //                using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
        //                {
        //                    StringBuilder data = new StringBuilder();
        //                    ArrayList list = new ArrayList();
        //                    data.Append("name|func1|func2|itemid|icon_m|icon_f|model_m|model_f|st1|st2|st3|op_st1|op_st2|op_st3|unk1|unk2|position|unk3|color1|color2|color3|color4|color5|color6|color7|color8|color9|color10|" +
        //                                "unk4|lvuse|buy|sell|gender|unk5|unk6|trade|unk7|unk8|unk9|maxqty|maxdura|unk10|ItemKey|unk11|unk12|grade|unk13|unk14|job|unk15|unk16|set|st5f|st8f|st10f|qty5f|qty8f|qty10f|skill_id|unk17|skill_lv|npcid|unk18|des");
        //                    for (int i = 0; i < b.BaseStream.Length; i += 420)
        //                    {
        //                        data.AppendLine();
        //                        for (int a = 0; a < array.Count; a += 2)
        //                        {
        //                            int offset = Convert.ToInt32(array[a]);
        //                            int Length = Convert.ToInt32(array[a + 1]);
        //                            b.BaseStream.Seek(offset + i, SeekOrigin.Begin);
        //                            if (Length == 1)
        //                            {
        //                                list.Add(b.ReadByte());
        //                            }
        //                            else if (Length == 2)
        //                            {
        //                                list.Add(ByteArrayToInt2(b.ReadBytes(2)));
        //                            }
        //                            else if (Length == 4)
        //                            {
        //                                //if(offset >= 55 && offset <= 91)
        //                                //    list.Add(ByteArrayToLong4(b.ReadBytes(4)));
        //                                //else
        //                                list.Add(ByteArrayToInt4(b.ReadBytes(4)));
        //                            }
        //                            else
        //                            {
        //                                if (a == 0)
        //                                    start++;
        //                                list.Add(Encoding.GetEncoding("TIS-620").GetString(b.ReadBytes(Length)));
        //                            }
        //                        }
        //                        foreach (var x in list)
        //                        {
        //                            data.Append(x + "|");
        //                        }
        //                        list.Clear();
        //                    }
        //                    if (data.Length > 0)
        //                    {
        //                        File.WriteAllText("Item_TH.csv", data.ToString(), Encoding.UTF8);
        //                        Logging("อ่านเสร็จสิ้น จำนวนไฟล์ทั้งหมด:" + start);
        //                        MessageBox.Show("สำเร็จ");
        //                    }
        //                }
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message.ToString());
        //    }
        //}

        private void NpcBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT |*.Dat";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {
                    if (!file.Contains("Npc_TH"))
                        Error(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TalkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT|*.Dat|CSV|*.Csv";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {
                    if (!file.ToLower().Contains("talk"))
                    {
                        Error(1);
                        return;
                    }

                    string extension = Path.GetExtension(file);

                    if (extension.ToLower() == ".dat")
                    {
                        StringBuilder listTalk = new StringBuilder();
                        listTalk.AppendLine("unk,npc_id,speech");
                        byte[] numArray1 = File.ReadAllBytes(file);
                        byte[] data = (byte[])null;

                        this.decryptData(numArray1, 12, false);
                        this.decryptData(data, 12, false);
                        try
                        {
                            numArray1 = ZlibStream.UncompressBuffer(numArray1);
                        }
                        catch
                        {
                            int num = (int)MessageBox.Show("Decryption Failed");
                            return;
                        }
                        for (int index1 = 0; index1 < numArray1.Length; index1 += 258)
                        {
                            numArray1[index1] = (byte)((uint)numArray1[index1] ^ 155U);
                            numArray1[index1 + 1] = (byte)((uint)numArray1[index1 + 1] ^ 241U);
                            numArray1[index1 + 2] = (byte)((uint)numArray1[index1 + 2] ^ 186U);
                            byte[] numArray2 = new byte[(int)numArray1[index1 + 3]];
                            Array.Copy((Array)numArray1, index1 + 4, (Array)numArray2, 0, numArray2.Length);
                            Array.Reverse((Array)numArray2);
                            bool flag = numArray2.Length % 2 == 0;
                            for (int index2 = 0; index2 < numArray2.Length - 3; index2 += 2)
                            {
                                byte num = numArray2[index2 + 1];
                                if (flag)
                                {
                                    numArray2[index2 + 1] = numArray2[index2 + 2];
                                    numArray2[index2 + 2] = num;
                                }
                                else
                                {
                                    numArray2[index2 + 1] = numArray2[index2 + 3];
                                    numArray2[index2 + 3] = num;
                                }
                            }
                            Array.Copy((Array)numArray2, 0, (Array)numArray1, index1 + 4, numArray2.Length);
                            listTalk.AppendLine(ByteArrayToLong(new byte[] { numArray1[index1] }) + "," + ByteArrayToLong(new byte[] { numArray1[index1 + 1], numArray1[index1 + 2] }) + "," + Encoding.GetEncoding("TIS-620").GetString(numArray2).Replace("\n", ""));
                        }
                        File.WriteAllText("Talk_dec.csv", listTalk.ToString(), Encoding.UTF8);
                        int num4 = (int)MessageBox.Show("Done");
                    }
                    else if (extension.ToLower() == ".csv")
                    {
                        using (StreamReader sr = new StreamReader(File.Open(file, FileMode.Open)))
                        {
                            byte[] byteTalk = new byte[0];
                            int ntalk = 0;
                            sr.ReadLine();
                            while (sr.Peek() != -1 && !sr.EndOfStream)
                            {
                                byte[] listTask = new byte[0];
                                string line = sr.ReadLine();
                                List<string> lineValues = line.Split(',').ToList();
                                try
                                {
                                    byte[] speech = Encoding.GetEncoding("TIS-620").GetBytes(lineValues[2]);
                                    listTask = CombineArray(listTask, new byte[] { Convert.ToByte(lineValues[0]) });
                                    listTask = CombineArray(listTask, BitConverter.GetBytes(Convert.ToUInt16(lineValues[1])));
                                    listTask = CombineArray(listTask, new byte[] { Convert.ToByte(speech.Length)});
                                    listTask = CombineArray(listTask, speech);
                                    while (listTask.Length < 258)
                                    {
                                        listTask = CombineArray(listTask, new byte[] { 0x00 });
                                    }
                                    byteTalk = CombineArray(byteTalk, listTask);
                                    ntalk++;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString()+" | "+lineValues.Count.ToString());
                                    return;
                                }
                            }

                            if (byteTalk.Length == 0)
                            {
                                MessageBox.Show("Error");
                                return;
                            }

                            for (int index1 = 0; index1 < byteTalk.Length; index1 += 258)
                            {
                                byteTalk[index1] = (byte)((uint)byteTalk[index1] ^ 155U);
                                byteTalk[index1 + 1] = (byte)((uint)byteTalk[index1 + 1] ^ 241U);
                                byteTalk[index1 + 2] = (byte)((uint)byteTalk[index1 + 2] ^ 186U);
                                byte[] numArray2 = new byte[(int)byteTalk[index1 + 3]];
                                Array.Copy((Array)byteTalk, index1 + 4, (Array)numArray2, 0, numArray2.Length);
                                if (numArray2.Length % 2 == 0)
                                {
                                    for (int index2 = numArray2.Length - 4; index2 >= 0; index2 -= 2)
                                    {
                                        byte num = numArray2[index2 + 1];
                                        numArray2[index2 + 1] = numArray2[index2 + 2];
                                        numArray2[index2 + 2] = num;
                                    }
                                }
                                else
                                {
                                    for (int index2 = numArray2.Length - 5; index2 >= 0; index2 -= 2)
                                    {
                                        byte num = numArray2[index2 + 1];
                                        numArray2[index2 + 1] = numArray2[index2 + 3];
                                        numArray2[index2 + 3] = num;
                                    }
                                }
                                Array.Reverse((Array)numArray2);
                                Array.Copy((Array)numArray2, 0, (Array)byteTalk, index1 + 4, numArray2.Length);
                            }

                            byteTalk = ZlibStream.CompressBuffer(byteTalk);
                            this.encryptData(byteTalk, 12, false);
                            File.WriteAllBytes("Talk_TH.Dat", byteTalk);
                            MessageBox.Show("Done!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void EveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "Emg |*.Emg";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {
                    if (!file.Contains("eve"))
                        Error(1);
                    MessageBox.Show(file);
                    string nfile = file.Split('.').GetValue(0).ToString();
                    using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                    {
                        int start = 6;
                        while (true)
                        {
                            b.BaseStream.Seek(start, SeekOrigin.Begin);
                            int nle = b.ReadByte();
                            b.BaseStream.Seek(start + 1, SeekOrigin.Begin);
                            byte[] name = b.ReadBytes(nle);
                            b.BaseStream.Seek(start + 24, SeekOrigin.Begin);
                            int offset = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(start + 28, SeekOrigin.Begin);
                            int end = ByteArrayToInt4(b.ReadBytes(4));
                            b.BaseStream.Seek(offset, SeekOrigin.Begin);
                            byte xor = GetDecode(b.ReadByte());
                            byte[] data = b.ReadBytes(end);
                            string namefile = Encoding.GetEncoding("big5").GetString(name);
                            if (!Directory.Exists(nfile))
                            {
                                Directory.CreateDirectory(nfile);
                            }
                            listView1.Items.Add("UNPACK:" + namefile);
                            File.WriteAllBytes(nfile + "/" + namefile, Decode(data, xor));
                            start += 32;
                            if (start >= b.BaseStream.Length || name.Length == 0)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Logging(string msg)
        {
            if (listView1.InvokeRequired)
            {
                listView1.Invoke((MethodInvoker)delegate ()
                {
                    listView1.Items.Add(msg);
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                });
            }
            else
            {
                listView1.Items.Add(msg);
            }
        }

        public byte GetDecode(byte a1)
        {
            byte xor = 1;
            do
            {
                byte dxor = Convert.ToByte(a1 ^ xor);
                if (dxor == 0x48)
                    return xor;
                else
                    xor++;
            }
            while (true);
        }

        #region
        /*
             using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            int start = 0;
                            StringBuilder data = new StringBuilder();
                            ArrayList list = new ArrayList();
                            data.Append("name|func1|func2|itemid|model_m|model_f|lmodel_m|lmodel_f|st1|st2|st3|op_st1|op_st2|op_st3|unk1|unk2|position|color1|color2|color3|color4|color5|color6|color7|color8|color9|color10|" +
                                            "unk3|lvuse|buy|sell|gender|unk4|trade|unk5|unk6|unk7|maxqty|maxdura|upgrade|grade|job|set|st5f|st8f|st10f|qty5f|qty8f|qty10f|skill|npcid|unk8|des");
                            while (start < b.BaseStream.Length)
                            {
                                data.AppendLine();
                                b.BaseStream.Seek(start, SeekOrigin.Begin);
                                int nle = b.ReadByte();
                                b.BaseStream.Seek(start + 1, SeekOrigin.Begin);
                                byte[] nameitem = b.ReadBytes(nle);
                                list.Add(Encoding.GetEncoding("big5").GetString(nameitem));
                                b.BaseStream.Seek(start + 21, SeekOrigin.Begin);
                                int func1 = b.ReadByte();
                                list.Add(func1);
                                b.BaseStream.Seek(start + 22, SeekOrigin.Begin);
                                int func2 = b.ReadByte();
                                list.Add(func2);
                                b.BaseStream.Seek(start + 23, SeekOrigin.Begin);
                                int itemid = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(itemid);
                                b.BaseStream.Seek(start + 25, SeekOrigin.Begin);
                                int model_m = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(model_m);
                                b.BaseStream.Seek(start + 27, SeekOrigin.Begin);
                                int model_f = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(model_f);
                                b.BaseStream.Seek(start + 29, SeekOrigin.Begin);
                                int lmodel_m = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(lmodel_m);
                                b.BaseStream.Seek(start + 31, SeekOrigin.Begin);
                                int lmodel_f = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(lmodel_f);
                                b.BaseStream.Seek(start + 33, SeekOrigin.Begin);
                                int st1 = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st1);
                                b.BaseStream.Seek(start + 35, SeekOrigin.Begin);
                                int st2 = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st2);
                                b.BaseStream.Seek(start + 37, SeekOrigin.Begin);
                                int st3 = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st3);
                                b.BaseStream.Seek(start + 39, SeekOrigin.Begin);
                                int op_st1 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(op_st1);
                                b.BaseStream.Seek(start + 43, SeekOrigin.Begin);
                                int op_st2 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(op_st2);
                                b.BaseStream.Seek(start + 47, SeekOrigin.Begin);
                                int op_st3 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(op_st3);
                                b.BaseStream.Seek(start + 51, SeekOrigin.Begin);
                                int unk1 = b.ReadByte();
                                list.Add(unk1);
                                b.BaseStream.Seek(start + 52, SeekOrigin.Begin);
                                int unk2 = b.ReadByte();
                                list.Add(unk2);
                                b.BaseStream.Seek(start + 53, SeekOrigin.Begin);
                                int position = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(position);
                                b.BaseStream.Seek(start + 55, SeekOrigin.Begin);
                                long color1 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color1);
                                b.BaseStream.Seek(start + 60, SeekOrigin.Begin);
                                long color2 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color2);
                                b.BaseStream.Seek(start + 63, SeekOrigin.Begin);
                                long color3 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color3);
                                b.BaseStream.Seek(start + 67, SeekOrigin.Begin);
                                long color4 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color4);
                                b.BaseStream.Seek(start + 71, SeekOrigin.Begin);
                                long color5 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color5);
                                b.BaseStream.Seek(start + 75, SeekOrigin.Begin);
                                long color6 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color6);
                                b.BaseStream.Seek(start + 79, SeekOrigin.Begin);
                                long color7 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color7);
                                b.BaseStream.Seek(start + 83, SeekOrigin.Begin);
                                long color8 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color8);
                                b.BaseStream.Seek(start + 87, SeekOrigin.Begin);
                                long color9 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color9);
                                b.BaseStream.Seek(start + 91, SeekOrigin.Begin);
                                long color10 = ByteArrayToLong4(b.ReadBytes(4));
                                list.Add(color10);
                                b.BaseStream.Seek(start + 95, SeekOrigin.Begin);
                                int unk3 = b.ReadByte();
                                list.Add(unk3);
                                b.BaseStream.Seek(start + 96, SeekOrigin.Begin);
                                int lvuse = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(lvuse);
                                b.BaseStream.Seek(start + 98, SeekOrigin.Begin);
                                int buy = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(buy);
                                b.BaseStream.Seek(start + 102, SeekOrigin.Begin);
                                int sell = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(sell);
                                b.BaseStream.Seek(start + 106, SeekOrigin.Begin);
                                int gender = b.ReadByte();
                                list.Add(gender);
                                b.BaseStream.Seek(start + 107, SeekOrigin.Begin);
                                int unk4 = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(unk4);
                                b.BaseStream.Seek(start + 109, SeekOrigin.Begin);
                                int trade = b.ReadByte();
                                list.Add(trade);
                                b.BaseStream.Seek(start + 110, SeekOrigin.Begin);
                                int unk5 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(unk5);
                                b.BaseStream.Seek(start + 114, SeekOrigin.Begin);
                                int unk6 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(unk6);
                                b.BaseStream.Seek(start + 118, SeekOrigin.Begin);
                                int unk7 = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(unk7);
                                b.BaseStream.Seek(start + 122, SeekOrigin.Begin);
                                int maxqty = b.ReadByte();
                                list.Add(maxqty);
                                b.BaseStream.Seek(start + 123, SeekOrigin.Begin);
                                int maxdura = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(maxdura);
                                b.BaseStream.Seek(start + 127, SeekOrigin.Begin);
                                int upgrade = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(upgrade);
                                b.BaseStream.Seek(start + 131, SeekOrigin.Begin);
                                int grade = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(grade);
                                b.BaseStream.Seek(start + 135, SeekOrigin.Begin);
                                int job = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(job);
                                b.BaseStream.Seek(start + 139, SeekOrigin.Begin);
                                int set = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(set);
                                b.BaseStream.Seek(start + 141, SeekOrigin.Begin);
                                int st5f = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st5f);
                                b.BaseStream.Seek(start + 143, SeekOrigin.Begin);
                                int st8f = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st8f);
                                b.BaseStream.Seek(start + 145, SeekOrigin.Begin);
                                int st10f = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(st10f);
                                b.BaseStream.Seek(start + 147, SeekOrigin.Begin);
                                int qty5f = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(qty5f);
                                b.BaseStream.Seek(start + 151, SeekOrigin.Begin);
                                int qty8f = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(qty8f);
                                b.BaseStream.Seek(start + 155, SeekOrigin.Begin);
                                int qty10f = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(qty10f);
                                b.BaseStream.Seek(start + 159, SeekOrigin.Begin);
                                int skill = ByteArrayToInt2(b.ReadBytes(4));
                                list.Add(skill);
                                b.BaseStream.Seek(start + 162, SeekOrigin.Begin);
                                int npcid = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(npcid);
                                b.BaseStream.Seek(start + 164, SeekOrigin.Begin);
                                int unk8 = ByteArrayToInt2(b.ReadBytes(2));
                                list.Add(unk8);
                                b.BaseStream.Seek(start + 166, SeekOrigin.Begin);
                                int ndes = b.ReadByte();
                                b.BaseStream.Seek(start + 167, SeekOrigin.Begin);
                                byte[] desitem = b.ReadBytes(ndes);
                                list.Add(Encoding.GetEncoding("big5").GetString(desitem));
                                start += 421;
                                foreach (var x in list)
                                {
                                    data.Append(x + "|");
                                }
                                list.Clear();
                                File.WriteAllText("Item_TW.csv", data.ToString(), Encoding.UTF8);

                                if (listView1.InvokeRequired)
                                {
                                    listView1.Invoke((MethodInvoker)delegate ()
                                    {
                                        listView1.Items.Add("ItemId:" + itemid + " | Name:" + Encoding.GetEncoding("big5").GetString(nameitem));
                                        listView1.EnsureVisible(listView1.Items.Count - 1);
                                    });
                                }


                                // break;
                            }
                            MessageBox.Show("สำเร็จ");
                        }

         
         */
        #endregion

        private async void ItemTwBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT |*.Dat";
                DialogResult dr = this.openfile.ShowDialog();
                ArrayList array = new ArrayList()//offset - Length
                {
                    1,20,//Name
                    21,1,//Func1
                    22,1,//Func2
                    23,2,//Item Id
                    25,2,//Icon_m
                    27,2,//Icon_f
                    29,2,//Model_m
                    31,2,//Model_F
                    33,2,//St1
                    35,2,//St2
                    37,2,//St3
                    39,4,//QtySt1
                    43,4,//QtySt2
                    47,4,//QtySt3
                    51,1,//??
                    52,1,//??
                    53,1,//Position
                    54,1,//??
                    55,4,//Color1
                    59,4,//Color2
                    63,4,//Color3
                    67,4,//Color4
                    71,4,//Color5
                    75,4,//Color6
                    79,4,//Color7
                    83,4,//Color8
                    87,4,//Color9
                    91,4,//Color10
                    95,1,//??
                    96,2,//Lvuse
                    98,4,//Buy
                    102,4,//Sell
                    106,1,//Gender
                    107,1,//??
                    108,1,//??
                    109,1,//Trade
                    110,4,//??
                    111,4,//??
                    118,4,//??
                    122,1,//MaxQty
                    123,2,//Maxdura
                    125,2,//??
                    127,2,//ItemKey
                    129,2,//unk
                    131,2,//Grade,Mapid
                    133,2,//unk
                    135,2,//Job,X
                    137,2,//Aura,Y
                    139,2,//Set
                    141,2,//St5f
                    143,2,//St8f
                    145,2,//St10f
                    147,4,//Qty5f
                    151,4,//Qty8f
                    155,4,//Qty10f
                    159,2,//Skill id
                    160,1,//unk
                    161,1,//Skill lv 
                    162,2,//Npc id
                    164,2,//unk
                    167,254//Des
                };


                await Task.Run(() =>
                {
                    int start = 0;
                    foreach (String file in openfile.FileNames)
                    {
                        if (!file.Contains("Item_TW"))
                            Error(1);
                        string nfile = file.Split('.').GetValue(0).ToString();
                        using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            StringBuilder data = new StringBuilder();
                            ArrayList list = new ArrayList();

                            data.Append("name,func1,func2,item_id,icon_m,icon_f,model_m,model_f,st1,st2,st3,op_st1,op_st2,op_st3,unk1,unk2,eq_pos,unk3,color1,color2,color3,color4,color5,color6,color7,color8,color9,color10," +
                                       "unk4,use_lv,buy,sell,use_gender,unk5,unk6,trade_st,unk7,unk8,unk9,max_qty,max_dura,unk10,item_key,unk12,item_grade|map_id,unk13,use_job|map_x,spirit_el|map_y,item_set,st5f,st8f,st10f,qty5f,qty8f,qty10f,skill_id,unk17,skill_lv,npc_id,unk18,des");


                            //data.Append("name,func1,func2,itemid,icon_m,icon_f,model_m,model_f,st1,st2,st3,op_st1,op_st2,op_st3,unk1,unk2,position,unk3,color1,color2,color3,color4,color5,color6,color7,color8,color9,color10," +
                            //            "unk4,lvuse,buy,sell,gender,unk5,unk6,trade,unk7,unk8,unk9,maxqty,maxdura,unk10,unk11,ItemKey,unk12,unk13,grade,mapid,unk15,job,x,aura,y,set,st5f,st8f,st10f,qty5f,qty8f,qty10f,skill_id,unk18,skill_lv,npcid,unk19,des");
                            for (int i = 0; i < b.BaseStream.Length; i += 421)
                            {
                                data.AppendLine();
                                for(int a = 0;a< array.Count;a+=2)
                                {
                                    int offset = Convert.ToInt32(array[a]);
                                    int Length = Convert.ToInt32(array[a+1]);
                                    b.BaseStream.Seek(offset + i, SeekOrigin.Begin);
                                    if(Length == 1)
                                    {
                                        list.Add(b.ReadByte());
                                    }
                                    else if (Length == 2)
                                    {
                                        list.Add(ByteArrayToInt2(b.ReadBytes(2)));
                                    }
                                    else if (Length == 4 )
                                    {
                                        //if(offset >= 55 && offset <= 91)
                                        //    list.Add(ByteArrayToLong4(b.ReadBytes(4)));
                                        //else
                                            list.Add(ByteArrayToInt4(b.ReadBytes(4)));
                                    }
                                    else 
                                    {
                                        if (a == 0)
                                            start++;
                                        list.Add(Encoding.GetEncoding("big5").GetString(b.ReadBytes(Length)).Replace("\n", "").Replace("\r", ""));
                                    }
                                }
                                foreach (var x in list)
                                {
                                    data.Append(x + ",");
                                }
                                list.Clear();
                            }
                            if (data.Length > 0)
                            {
                                File.WriteAllText("Item_TW.csv", data.ToString(), Encoding.UTF8);
                                Logging("อ่านเสร็จสิ้น จำนวนไฟล์ทั้งหมด:"+start);
                                MessageBox.Show("สำเร็จ");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void uNPACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menu.Length == 0)
                return;
            if (menu == "JMG")
                JMG(1);
        }

        private void rEPACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menu.Length == 0)
                return;
            if (menu == "JMG")
                JMG(2);
        }

        private async void JMG(int a)
        {
            try
            {
                if(a == 1)
                {
                    //FolderBrowserDialog diag = new FolderBrowserDialog();
                    //if (diag.ShowDialog() == DialogResult.OK)
                    //{
                    //    foreach (string file in Directory.EnumerateFiles(diag.SelectedPath, "*.Jmg"))
                    //    {
                    //        string nfile = file.Split('.').GetValue(0).ToString();
                    //        using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                    //        {
                    //            int start = 2;
                    //            int qtyfile = 0;
                    //            int amount = 0;
                    //            while (true)
                    //            {
                    //                b.BaseStream.Seek(0, SeekOrigin.Begin);
                    //                qtyfile = ByteArrayToInt2(b.ReadBytes(2));
                    //                b.BaseStream.Seek(start, SeekOrigin.Begin);
                    //                int nle = b.ReadByte();
                    //                byte[] name = b.ReadBytes(nle);
                    //                b.BaseStream.Seek(start + 24, SeekOrigin.Begin);
                    //                int offset = ByteArrayToInt4(b.ReadBytes(4));
                    //                b.BaseStream.Seek(start + 28, SeekOrigin.Begin);
                    //                int end = ByteArrayToInt4(b.ReadBytes(4));
                    //                b.BaseStream.Seek(offset, SeekOrigin.Begin);
                    //                byte[] data = b.ReadBytes(end);
                    //                string namefile = Encoding.GetEncoding("big5").GetString(name);
                    //                start += 32;
                    //                if (start >= b.BaseStream.Length || name.Length == 0 || amount >= qtyfile)
                    //                    break;
                    //                if (!Directory.Exists(nfile))
                    //                {
                    //                    Directory.CreateDirectory(nfile);
                    //                }
                    //                amount++;
                    //                File.WriteAllBytes(nfile + "/" + namefile, data);
                    //            }
                    //            Logging("UNPACK SUCCESS");
                    //        }
                    //    }
                    //}



                    openfile.Filter = "JMG |*.Jmg";
                    DialogResult dr = this.openfile.ShowDialog();
                    foreach (String file in openfile.FileNames)
                    {
                        string nfile = file.Split('.').GetValue(0).ToString();
                        using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            int start = 2;
                            int qtyfile = 0;
                            int amount = 0;
                            while (true)
                            {
                                b.BaseStream.Seek(0, SeekOrigin.Begin);
                                qtyfile = ByteArrayToInt2(b.ReadBytes(2));
                                b.BaseStream.Seek(start, SeekOrigin.Begin);
                                int nle = b.ReadByte();
                                byte[] name = b.ReadBytes(nle);
                                b.BaseStream.Seek(start + 24, SeekOrigin.Begin);
                                int offset = ByteArrayToInt4(b.ReadBytes(4));
                                b.BaseStream.Seek(start + 28, SeekOrigin.Begin);
                                int end = ByteArrayToInt4(b.ReadBytes(4));
                                b.BaseStream.Seek(offset, SeekOrigin.Begin);
                                byte[] data = b.ReadBytes(end);
                                string namefile = Encoding.GetEncoding("big5").GetString(name);
                                start += 32;
                                if (start >= b.BaseStream.Length || name.Length == 0 || amount >= qtyfile)
                                    break;
                                if (!Directory.Exists(nfile))
                                {
                                    Directory.CreateDirectory(nfile);
                                }
                                amount++;
                                File.WriteAllBytes(nfile + "/" + namefile, data);
                            }
                            Logging("UNPACK SUCCESS");
                        }
                    }
                }
                else if (a == 2)
                {
                  
                    int offset = 0;
                    short nfile = 0;
                    int size = 0;
                    FolderBrowserDialog diag = new FolderBrowserDialog();
                    if (diag.ShowDialog() == DialogResult.OK)
                    {
                        nfile = Convert.ToInt16(Directory.EnumerateFiles(diag.SelectedPath, "*.jpg").Count());
                        offset = nfile * 32 + 2;
                        byte[] metadata = new byte[2];
                        byte[] data = new byte[0];
                        metadata = BitConverter.GetBytes(nfile);
                        Task f = Task.Factory.StartNew(() =>
                        {
                            foreach (string file in Directory.EnumerateFiles(diag.SelectedPath, "*.jpg"))
                            {
                                string[] name = file.Split('\\');
                                byte[] bname = Encoding.GetEncoding("big5").GetBytes(name[name.Length - 1]);
                                string namefile = ByteToStringX2(bname);
                                Logging("REPACKING...");

                                //add length name 1 byte
                                Array.Resize(ref metadata, metadata.Length + 1);
                                metadata[metadata.Length-1] = Convert.ToByte(bname.Length);

                                //add bname when < 23
                                if (bname.Length < 23)
                                    Array.Resize(ref bname, 23);

                                //add bname to metadata 
                                metadata = CombineArray(metadata, bname);

                                byte[] fdata = File.ReadAllBytes(diag.SelectedPath + "\\" + name[name.Length - 1]);
                                size += fdata.Length;

                                //add file data read  to data 
                                data = CombineArray(data, fdata);

                                //add offset and  Length  to metadata 
                                metadata = CombineArray(metadata, BitConverter.GetBytes(offset));
                                metadata = CombineArray(metadata, BitConverter.GetBytes(Convert.ToInt32(fdata.Length)));
                                offset += fdata.Length;
                                Logging("REPACK SUCCESS:" + name[name.Length - 1]);
                            }
                            File.WriteAllBytes(diag.SelectedPath + "001.dat", CombineArray(metadata, data));
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void BlissBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT|*.Dat|CSV|*.Csv";
                DialogResult dr = this.openfile.ShowDialog();
                ArrayList array = new ArrayList()//, field Length
                {
                    2, 1,1,2,2,2,1,
                };

                foreach (String file in openfile.FileNames)
                {
                    if (!file.ToLower().Contains("blissbag"))
                    {
                        Error(1);
                        return;
                    }

                    string extension = Path.GetExtension(file);

                    if(extension.ToLower() == ".dat")
                    {
                        byte[] rawData = File.ReadAllBytes(file);
                        if (rawData[0] != 0x78)
                        {
                            MessageBox.Show("Couldn't find round");
                            return;
                        }
                        byte[] data = Ionic.Zlib.ZlibStream.UncompressBuffer(rawData);
                        StringBuilder csvbliss = new StringBuilder();
                        csvbliss.Append("bliss_id,in,get,??,??,??,??,");
                        int x = 1;
                        do
                        {
                            csvbliss.Append("item" + x + ",qty" + x + ",per" + x + ",tag" + x + ",");
                            if (x == 30)
                            {
                                csvbliss.AppendLine();
                                break;
                            }
                            x++;
                        }
                        while (true);
                        for (int index = 0; index < data.Length; index += 221)
                        {
                            StringBuilder bliss = new StringBuilder();
                            int i = index;
                            int field = 0;
                            for (int a = 0; a < array.Count; a++)
                            {
                                if (i >= data.Length - 1 || field >= 127)
                                    break;
                                int Length = Convert.ToInt32(array[a]);
                                switch (Length)
                                {
                                    case 1:
                                        bliss.Append(ByteArrayToLong(new byte[] { data[i] }));
                                        break;
                                    case 2:
                                        bliss.Append(ByteArrayToLong(new byte[] { data[i], data[i + 1] }));
                                        break;
                                }
                                bliss.Append(",");
                                if (a == array.Count - 1)
                                    a = 2;
                                i += Length;
                                field++;
                            }
                            csvbliss.AppendLine(bliss.ToString());
                            bliss.Clear();
                        }
                        File.WriteAllText("blissbag_dec.csv", csvbliss.ToString(), Encoding.UTF8);
                        MessageBox.Show("Done!");
                    }
                    else if (extension.ToLower() == ".csv")
                    {
                        using (StreamReader sr = new StreamReader(File.Open(file, FileMode.Open)))
                        {
                            byte[] blissByte = new byte[0];
                            sr.ReadLine();
                            while (sr.Peek() != -1)
                            {
                                string line = sr.ReadLine();
                                List<string> lineValues = line.Split(',').ToList();
                                int i = 0;
                                int field = 0;
                                for (int a = 0; a < array.Count; a++)
                                {
                                    if (i >= lineValues.Count || field >= 127)
                                        break;
                                    int Length = Convert.ToInt32(array[a]);
                                    switch (Length)
                                    {
                                        case 1:
                                            blissByte = CombineArray(blissByte, new byte[] { Convert.ToByte(lineValues[i]) });
                                            break;
                                        case 2:
                                            blissByte = CombineArray(blissByte, BitConverter.GetBytes(Convert.ToUInt16(lineValues[i])));
                                            break;
                                    }
                                    if (a == array.Count - 1)
                                        a = 2;
                                    i++;
                                    field++;
                                }
                                //break;
                            }
                            blissByte = Ionic.Zlib.ZlibStream.CompressBuffer(blissByte);
                            File.WriteAllBytes("blissbag_dec.Dat", blissByte);
                            MessageBox.Show("Done!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString()+ex.StackTrace.ToString());
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GoodsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT|*.Dat|CSV|*.Csv";
                DialogResult dr = this.openfile.ShowDialog();
                ArrayList array = new ArrayList()//, field Length
                {
                    2, 1,1,2,2,2,1,
                };

                foreach (String file in openfile.FileNames)
                {
                    if (!file.ToLower().Contains("goodssale"))
                    {
                        Error(1);
                        return;
                    }

                    string extension = Path.GetExtension(file);

                    if (extension.ToLower() == ".dat")
                    {
                        byte[] rawData = MarkPacket(File.ReadAllBytes(file), 0x81);

                        if (rawData[0] != 0x78)
                        {
                            MessageBox.Show("Couldn't find round");
                            return;
                        }
                        byte[] data = Ionic.Zlib.ZlibStream.UncompressBuffer(rawData);
                        StringBuilder csvbliss = new StringBuilder();
                        csvbliss.Append("bliss_id,in,get,??,??,??,??,");
                        int x = 1;
                        do
                        {
                            csvbliss.Append("item" + x + ",qty" + x + ",per" + x + ",tag" + x + ",");
                            if (x == 30)
                            {
                                csvbliss.AppendLine();
                                break;
                            }
                            x++;
                        }
                        while (true);
                        for (int index = 0; index < data.Length; index += 221)
                        {
                            StringBuilder bliss = new StringBuilder();
                            int i = index;
                            int field = 0;
                            for (int a = 0; a < array.Count; a++)
                            {
                                if (i >= data.Length - 1 || field >= 127)
                                    break;
                                int Length = Convert.ToInt32(array[a]);
                                switch (Length)
                                {
                                    case 1:
                                        bliss.Append(ByteArrayToLong(new byte[] { data[i] }));
                                        break;
                                    case 2:
                                        bliss.Append(ByteArrayToLong(new byte[] { data[i], data[i + 1] }));
                                        break;
                                }
                                bliss.Append(",");
                                if (a == array.Count - 1)
                                    a = 2;
                                i += Length;
                                field++;
                            }
                            csvbliss.AppendLine(bliss.ToString());
                            bliss.Clear();
                        }
                        File.WriteAllText("bliss_dec.csv", csvbliss.ToString(), Encoding.UTF8);
                        MessageBox.Show("Done!");
                    }
                    else if (extension.ToLower() == ".csv")
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + ex.StackTrace.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Logging("OK");
            //MessageBox.Show("OK");
            //try
            //{
            //    openfile.Filter = "DAT|*.Dat|CSV|*.Csv";
            //    DialogResult dr = this.openfile.ShowDialog();
            //    ArrayList array = new ArrayList()//, field Length
            //    {
            //        2, 1,1,2,2,2,1,
            //    };

            //    foreach (String file in openfile.FileNames)
            //    {
            //        byte[] rawData = File.ReadAllBytes(file);

            //        if (rawData[0] != 0x78)
            //        {
            //            MessageBox.Show("Couldn't find round");
            //            return;
            //        }
            //        byte[] data = Ionic.Zlib.ZlibStream.UncompressBuffer(rawData);
            //        File.WriteAllBytes("sss_dec.Dat", data);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString() + ex.StackTrace.ToString());
            //}
        }

        private void NpcmemBtn_Click(object sender, EventArgs e)
        {
            try
            {
                openfile.Filter = "DAT|*.Dat|CSV|*.Csv";
                DialogResult dr = this.openfile.ShowDialog();
                foreach (String file in openfile.FileNames)
                {
                    if (!file.ToLower().Contains("npc_memoir"))
                    {
                        Error(1);
                        return;
                    }
                    byte[] data = (byte[])null;
                    int rounds = 0;
                    string extension = Path.GetExtension(file);

                    if (extension.ToLower() == ".dat")
                    {
                        StringBuilder listText = new StringBuilder();
                        listText.AppendLine("npc_id,description");
                        byte[] numArray1 = File.ReadAllBytes(file);
                       

                        int length = 406;
                        data = new byte[length];
                        byte[] numArray2 = new byte[numArray1.Length - length];
                        Array.Copy((Array)numArray1, 0, (Array)numArray2, 0, numArray2.Length);
                        Array.Copy((Array)numArray1, numArray2.Length, (Array)data, 0, length);
                        numArray1 = numArray2;
                        rounds = numArray1.Length / 203;


                        int num1 = 1;
                        int num2 = data.Length / 2;
                        for (int index1 = 0; index1 < rounds; ++index1)
                        {
                            for (int index2 = 0; index2 < num2; ++index2)
                                numArray1[index1 * num2 + index2] = (byte)((uint)numArray1[index1 * num2 + index2] ^ (uint)data[num1 * num2 + index2]);
                            num1 = num1 == 1 ? 0 : 1;
                        }

                        for(int index2=0;index2<numArray1.Length;index2+=203)
                        {
                            long id = ByteArrayToLong(new byte[] { numArray1[index2], numArray1[index2 + 1] });
                            byte[] text = new byte[numArray1[index2 + 2]];
                            Array.Copy(numArray1, index2 + 3, text, 0, text.Length);
                            listText.AppendLine(id + "," + Encoding.GetEncoding("TIS-620").GetString(text).Replace("\n", ""));
                            Array.Clear(text,0, text.Length);
                        }
                        File.WriteAllText(Path.GetFileNameWithoutExtension(file) + "_dec.csv", listText.ToString(), Encoding.UTF8);
                        MessageBox.Show("Done");
                    }
                    else if (extension.ToLower() == ".csv")
                    {
                        using (StreamReader sr = new StreamReader(File.Open(file, FileMode.Open)))
                        {
                            byte[] byteText = new byte[0];
                            sr.ReadLine();
                            while (sr.Peek() != -1 && !sr.EndOfStream)
                            {
                                byte[] listText = new byte[0];
                                string line = sr.ReadLine();
                                List<string> lineValues = line.Split(',').ToList();
                                try
                                {
                                    byte[] text = Encoding.GetEncoding("TIS-620").GetBytes(lineValues[1]);
                                    listText = CombineArray(listText, BitConverter.GetBytes(Convert.ToUInt16(lineValues[0])));
                                    listText = CombineArray(listText, new byte[] { Convert.ToByte(text.Length)});
                                    listText = CombineArray(listText, text);
                                    while (listText.Length < 203)
                                    {
                                        listText = CombineArray(listText, new byte[] { 0x00 });
                                    }
                                    byteText = CombineArray(byteText, listText);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString() + " | " + lineValues.Count.ToString());
                                    return;
                                }
                            }

                            if (byteText.Length == 0)
                            {
                                MessageBox.Show("Error");
                                return;
                            }

                            byte[] numArray2;
                            data = this.generateKey(406);
                            rounds = byteText.Length / 203;

                            int num1 = data.Length / 2;
                            for (int index1 = 0; index1 < rounds; ++index1)
                            {
                                int num2 = index1 % 2 == 0 ? 1 : 0;
                                for (int index2 = 0; index2 < num1; ++index2)
                                    byteText[index1 * num1 + index2] = (byte)((uint)byteText[index1 * num1 + index2] ^ (uint)data[num2 * num1 + index2]);
                            }
                            numArray2 = ((IEnumerable<byte>)byteText).Concat<byte>((IEnumerable<byte>)data).ToArray<byte>();
                            File.WriteAllBytes("Npc_Memoir_TH.Dat", numArray2);
                            MessageBox.Show("Done!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
    }
}
