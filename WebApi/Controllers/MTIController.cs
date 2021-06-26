
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MTIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MTIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"Select  MsgID, '(' + CAST( MTICode as varchar(5)) + ' - ' + CAST( FunctionCode as varchar(5)) + ') ' + Description Description from LT_MCMessages";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCmd = new SqlCommand(query, mycon))
                {
                    myReader = myCmd.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetElements")]
        public JsonResult GetElements(string MessageType)
        {
            string[] parameter = MessageType.Split(new char[] { '(', '-', ')' });

            string MTICode = parameter[1];
            string FunctionCode = parameter[2];
            DataTable table = new DataTable();

            if (!string.IsNullOrEmpty(MTICode))
            {
                string query = $"Select ID, DataElementNO, ParentElementNO, DataElementName, ColumnName,  Format, Length, Justification, IsVariableLength, VariableLength, Dst, '' InputValue FROM LT_MCIncomingMessageSetting MS 	INNER JOIN LT_MCMessages M 		ON MS.MsgId = M.MsgID 	INNER JOIN LT_MCElements E 		ON MS.ElementID = E.ID Where DataElementNO Is Not null AND DataElementNO NOT IN (-1,1)  AND M.MTICode = {MTICode} AND FunctionCode = {FunctionCode}";

                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection mycon = new SqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (SqlCommand myCmd = new SqlCommand(query, mycon))
                    {
                        myReader = myCmd.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        mycon.Close();
                    }
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        [Route("FormatElement")]
        public JsonResult FormatElement(object messageType)
        {
            var jsonObj = JObject.Parse(messageType.ToString());

            List<Element8583> elements = new List<Element8583>();
            if (jsonObj.ContainsKey("ExistingData"))
            {
                elements = JsonConvert.DeserializeObject<List<Element8583>>(jsonObj["ExistingData"].ToString());
            }
            var Element8583 = JsonConvert.DeserializeObject<Element8583>(messageType.ToString());
            elements.Add(Element8583);

            return new JsonResult(JsonConvert.SerializeObject(elements, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }


        [HttpPost]
        [Route("CreateFile")]
        public ActionResult CreateFile(object messageType)
        {
            var jsonObj = JObject.Parse(messageType.ToString());

            List<Element8583> elements = JsonConvert.DeserializeObject<List<Element8583>>(jsonObj["ExistingData"].ToString());
            StringBuilder sb = new StringBuilder();

            foreach (var element in elements)
            {
                string eleJson = JsonConvert.SerializeObject(element, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var root = (JContainer)JToken.Parse(eleJson);
                

                var SelectedElementIndex = root.DescendantsAndSelf().OfType<JProperty>()
                                        .Where(w => w.Name != "DE0")
                                        .Select(p => Convert.ToInt32(p.Name.Replace("DE", ""))).ToList();

                var list = root.DescendantsAndSelf().OfType<JProperty>()
                                .Select(p => p.Value.Value<string>()).ToList();

                var BitmapByte = BuildBitMap(SelectedElementIndex);
                var hexString = BitmapByte.HexBytesToString();
                var ASCIIString = HexToASCII(hexString);
                list.Insert(1, ASCIIString);

                //SelectedElementIndex.Insert(0, 1);
                //CreateBitmap(SelectedElementIndex, out string BitmapPrimary, out string BitmapSecondary);
                //list.Insert(1, BitmapSecondary);
                //list.Insert(1, BitmapPrimary);

                string data = string.Join("", list);

                string hexHeader = CreateHeader(data.Length);
                sb.Append($"{HexToASCII(hexHeader)}{data}");

            }
            string fileData = sb.ToString();
            fileData = fileData + HexToASCII("00000000");
            fileData = fileData.PadRight(((fileData.Length / 1014) + 1) * 1014, '@');

            string filePath = $"E:\\{DateTime.Now.ToString("ddMMyyhhmmss")}.log";
            System.IO.File.WriteAllBytes(filePath, Encoding.Latin1.GetBytes(fileData));

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/plain", System.IO.Path.GetFileName(filePath));
            //return new JsonResult(sb);
        }

        private string CreateHeader(int length)
        {
            string hexString = "";
            int Counter = 0;
            do
            {
                if (length > 256)
                {
                    Counter++;
                    length = length - 256;
                }
                else
                {
                    //hexString = $"{CreateHeader(Counter)} {length.ToString().PadLeft(2, '0')}";
                    if (Counter > 0)
                        hexString += CreateHeader(Counter);
                    else
                        hexString += length.IntToHexValue(2);
                }
            }
            while (length > 256);

            return hexString.PadLeft(8, '0');
        }


        //Iso8583 _iso8583 = new Iso8583();
        //ASCIIMessage asciiMessage = new ASCIIMessage
        //{
        //    Field4 = "000000004444",
        //    Field11 = "000021",
        //    Field12 = "104212",
        //    Field13 = "0529",
        //    Field22 = "021",
        //    Field37 = "000000001015",
        //    Field41 = "JI091003",
        //    Field42 = "000000000111111"
        //};

        //var messageBytes = _iso8583.Build<ASCIIMessage>(asciiMessage, "0100", IsoFields.F39);
        private void CreateBitmap(List<int> selEleIndex, out string BitmapPrimary, out string BitmapSecondary)
        {
            string Binary = "0".PadLeft(128, '0');
            var eleIndex = Binary.ToCharArray().ToList();
            //List<int> selEleIndex = new List<int>() { 1, 7, 127 };
            var newList = eleIndex.Select((item, index) => (index + 1).In(selEleIndex.ToArray()) ? 1 : 0).ToArray();
            string newBinary = string.Join("", newList);

            string BitmapPrimaryBinary = newBinary.Substring(0, 64);
            string BitmapSecondaryBinary = newBinary[64..];

            string BitmapPrimaryHex = BinaryStringToHexString(BitmapPrimaryBinary);
            string BitmapSecondaryHex = BinaryStringToHexString(BitmapSecondaryBinary);

            string BitmapPrimaryAscii = HexToASCII(BitmapPrimaryHex);
            string BitmapSecondaryAscii = HexToASCII(BitmapSecondaryHex);

            BitmapPrimary = BitmapPrimaryAscii;
            BitmapSecondary = BitmapSecondaryAscii;

            //  BitmapPrimary = HexToChar(BitmapPrimaryAscii);
            //   BitmapSecondary = HexToChar(BitmapSecondaryAscii);

            //var chararry =  StringToByteArray(BitmapPrimaryHex);
        }

        private byte[] BuildBitMap(IEnumerable<int> orderedFields)
        {
            //[IsoField(position: IsoFields.BitMap, maxLen: 8, lengthType: LengthType.FIXED, contentType: ContentType.B, dataType: DataType.HEX)]
            var secondBitRequired = orderedFields.Any(pos => pos > 65 && pos < 128);

            char[] bitmapBinaryArray = null;

            if (secondBitRequired)
            {
                bitmapBinaryArray = new char[129];
                bitmapBinaryArray[1] = '1';
            }
            else
            {
                bitmapBinaryArray = new char[65];
                bitmapBinaryArray[1] = '0';
            }
            //Building BitMap
            for (var i = 2; i < bitmapBinaryArray.Length; i++)
            {
                if (orderedFields.Contains(i))
                    bitmapBinaryArray[i] = '1';
                else
                    bitmapBinaryArray[i] = '0';
            }

            var bitmapString = new string(bitmapBinaryArray);
            var bitMap = Convert.ToInt64(bitmapString.Substring(1, 64), 2).ToString("X").PadLeft(16, '0');

            if (secondBitRequired)
            {
                bitMap = bitMap + Convert.ToInt64(bitmapString.Substring(65, 64), 2).ToString("X").PadLeft(16, '0');
            }

            //[IsoField(position: IsoFields.BitMap, maxLen: 8, lengthType: LengthType.FIXED, contentType: ContentType.B, dataType: DataType.HEX)]
            return BuildFieldValue(LengthType.FIXED, MaxLen: 8, LenDataType: DataType.HEX, encoding: EncodingType.None, dataType: DataType.HEX, bitMap);
        }
        //https://github.com/CBidis/CSharp8583

        internal static byte[] BuildFieldValue(LengthType lengthType, int MaxLen, DataType LenDataType, EncodingType encoding, DataType dataType, string fieldValue)
        {
            var fieldBytes = new List<byte>();

            try
            {
                switch (lengthType)
                {
                    case LengthType.FIXED:
                        if (fieldValue.Length < MaxLen)
                            fieldValue = fieldValue?.PadRight(MaxLen);
                        break;

                    case LengthType.LVAR:
                    case LengthType.LLVAR:
                    case LengthType.LLLVAR:
                    case LengthType.LLLLVAR:
                        if (LenDataType == DataType.ASCII)
                        {
                            var valueLen = fieldValue?.Length.ToString().PadLeft((int)lengthType, '0');
                            fieldBytes.AddRange(valueLen.FromASCIIToBytes(encoding));
                        }
                        else if (LenDataType == DataType.HEX)
                        {
                            var valueLen = fieldValue?.Length.IntToHexValue((int)lengthType);
                            fieldBytes.AddRange(valueLen.FromASCIIToBytes(encoding));
                        }
                        else if (LenDataType == DataType.BCD)
                        {
                            var valueLen = fieldValue?.Length.ToString().ConvertToBinaryCodedDecimal(false);
                            fieldBytes.AddRange(valueLen);
                        }
                        break;

                        //default:
                        //    throw new BuildFieldException(isoFieldProperties, $"Cannot Parse Length value for {isoFieldProperties?.Position} and Len Type {isoFieldProperties?.LenDataType}");
                }

                switch (dataType)
                {
                    case DataType.BIN:
                        fieldBytes.AddRange(fieldValue.ToBinaryStringFromHex().ToBytesFromBinaryString());
                        break;
                    case DataType.BCD:
                        if (fieldValue.Length % 2 == 1)
                            fieldValue += '0';
                        var bcdValue = fieldValue.ConvertToBinaryCodedDecimal(false);
                        fieldBytes.AddRange(bcdValue);
                        break;
                    case DataType.ASCII:
                    case DataType.HEX:
                        fieldBytes.AddRange(fieldValue.FromASCIIToBytes(encoding));
                        break;
                        //default:
                        //    throw new BuildFieldException(isoFieldProperties, $"Cannot Parse value for {isoFieldProperties?.Position} and Type {isoFieldProperties?.DataType}");
                }
            }
            catch (Exception ex)
            {
                /*throw new BuildFieldException(isoFieldProperties, $"Cannot Parse value for {isoFieldProperties?.Position} and Type //{isoFieldProperties?.DataType}", ex);*/
            }

            return fieldBytes.ToArray();
        }
        public string HexToASCII(string hexValue)
        {
            string value = null;

            if (!string.IsNullOrEmpty(hexValue) && !hexValue.Equals("null"))
            {
                //StringBuilder output = new StringBuilder("");
                //for (int i = 0; i < hexValue.length(); i += 2)
                //{
                //    String str = hexValue.substring(i, i + 2);
                //    output.append((char)Integer.parseInt(str, 16));
                //}
                //value = output.toString();


                StringBuilder output = new StringBuilder();
                for (int a = 0; a < hexValue.Length; a = a + 2)
                {
                    string Char2Convert = hexValue.Substring(a, 2);
                    output.Append(((char)Convert.ToInt32(Char2Convert, 16)).ToString());
                }
                value = output.ToString(); ;
            }

            return value;
        }

        public static char[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToChar(Convert.ToByte(hex.Substring(x, 2), 16)))
                             .ToArray();
        }


        public static string HexToChar(string Hex)
        {
            string str = "";
            for (int i = 0; i < Hex.Length; i++)
                str += (char)Hex[i];

            return str;
        }

        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }
    }


}
