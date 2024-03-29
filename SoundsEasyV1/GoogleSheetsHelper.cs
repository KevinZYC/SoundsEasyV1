﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Dynamic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SoundsEasyV1
{
    /* 
     credit: https://developers.google.com/api-client-library/dotnet/apis/sheets/v4
    https://www.hardworkingnerd.com/how-to-read-and-write-to-google-sheets-with-c/


    service email: soundseasysheets@fleet-automata-366622.iam.gserviceaccount.com

    test Google Sheets: 1POh7lSt7QyI45I_16I3An1iTWSc4PsV0rcYP5ExPKhg
     */

    //class with functions that execute different types of data requests
    public class GoogleSheetsHelper
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "GoogleSheetsHelper";

        private BatchUpdateSpreadsheetRequest requestsList = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>()};

    private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId;

        public GoogleSheetsHelper(string credentialFileName, string spreadsheetId)
        {
            var credential = GoogleCredential.FromStream(new FileStream(credentialFileName, FileMode.Open)).CreateScoped(Scopes);

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            _spreadsheetId = spreadsheetId;
        }

        //CURRENTLY UNUSED: function to pull data from a specific range
        public List<ExpandoObject> GetDataFromSheet(GoogleSheetParameters googleSheetParameters)
        {
            googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
            var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
            var columnNames = new List<string>();
            var returnValues = new List<ExpandoObject>();

            if (!googleSheetParameters.FirstRowIsHeaders)
            {
                for (var i = 0; i <= numberOfColumns; i++)
                {
                    columnNames.Add($"Column{i}");
                }
            }

            var response = request.Execute();

            int rowCounter = 0;
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
                    {
                        for (var i = 0; i <= numberOfColumns; i++)
                        {
                            columnNames.Add(row[i].ToString());
                        }
                        rowCounter++;
                        continue;
                    }

                    var expando = new ExpandoObject();
                    var expandoDict = expando as IDictionary<String, object>;
                    var columnCounter = 0;
                    foreach (var columnName in columnNames)
                    {
                        expandoDict.Add(columnName, row[columnCounter].ToString());
                        columnCounter++;
                    }
                    returnValues.Add(expando);
                    rowCounter++;
                }
            }

            return returnValues;
        }

        //Function to pull instrument data from a specific range 
        public void GetInstrumentDataFromSheet(GoogleSheetParameters googleSheetParameters, ref ObservableCollection<Instrument> myList)
        {
            googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
            var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
            var columnNames = new List<string>();
            var returnValues = new List<ExpandoObject>();

            if (!googleSheetParameters.FirstRowIsHeaders)
            {
                for (var i = 0; i <= numberOfColumns; i++)
                {
                    columnNames.Add($"Column{i}");
                }
            }

            
            var response = request.Execute();

            //construct expando objects for each row
            int rowCounter = 0;
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
                    {
                        for (var i = 0; i <= numberOfColumns; i++)
                        {
                            columnNames.Add(row[i].ToString());
                        }
                        rowCounter++;
                        continue;
                    }

                    var expando = new ExpandoObject();
                    var expandoDict = expando as IDictionary<String, object>;
                    var columnCounter = 0;
                    foreach (var columnName in columnNames)
                    {
                        expandoDict.Add(columnName, row[columnCounter].ToString());
                        columnCounter++;
                    }
                    
                    MainWindow.dataSourceInstrument.Add(expToInst(expando, rowCounter));
                    Debug.WriteLine(rowCounter * (100 / values.Count));
                    rowCounter++;
                }
            }

            

        }


        //function to pull student data from a specific range
        public void GetStudentDataFromSheet(GoogleSheetParameters googleSheetParameters, ref ObservableCollection<Student> myList)
        {
            googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
            var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

            SpreadsheetsResource.ValuesResource.GetRequest request =
                _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

            var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
            var columnNames = new List<string>();
            var returnValues = new List<ExpandoObject>();

            if (!googleSheetParameters.FirstRowIsHeaders)
            {
                for (var i = 0; i <= numberOfColumns; i++)
                {
                    columnNames.Add($"Column{i}");
                }
            }

            var response = request.Execute();

            //construct expando objects for each row
            int rowCounter = 0;
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
                    {
                        for (var i = 0; i <= numberOfColumns; i++)
                        {
                            columnNames.Add(row[i].ToString());
                        }
                        rowCounter++;
                        continue;
                    }

                    var expando = new ExpandoObject();
                    var expandoDict = expando as IDictionary<String, object>;
                    var columnCounter = 0;
                    foreach (var columnName in columnNames)
                    {
                        expandoDict.Add(columnName, row[columnCounter].ToString());
                        columnCounter++;
                    }

                    MainWindow.dataSourceStudent.Add(expToStud(expando, rowCounter));
                    rowCounter++;
                }
            }
        }



        //function to add rows of Google Sheet cells
        public void AddCells(GoogleSheetParameters googleSheetParameters, List<GoogleSheetRow> rows)
        {
            //var requests = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

            var sheetId = GetSheetId(_sheetsService, _spreadsheetId, googleSheetParameters.SheetName);

            GridCoordinate gc = new GridCoordinate
            {
                ColumnIndex = googleSheetParameters.RangeColumnStart - 1,
                RowIndex = googleSheetParameters.RangeRowStart - 1,
                SheetId = sheetId
            };

            var request = new Request { UpdateCells = new UpdateCellsRequest { Start = gc, Fields = "*" } };

            var listRowData = new List<RowData>();

            foreach (var row in rows)
            {
                var rowData = new RowData();
                var listCellData = new List<CellData>();
                foreach (var cell in row.Cells)
                {
                    var cellData = new CellData();
                    var extendedValue = new ExtendedValue { StringValue = cell.CellValue };

                    cellData.UserEnteredValue = extendedValue;
                    var cellFormat = new CellFormat { TextFormat = new TextFormat() };

                    if (cell.IsBold)
                    {
                        cellFormat.TextFormat.Bold = true;
                    }

                    cellFormat.BackgroundColor = new Color { Blue = (float)cell.BackgroundColor.B / 255, Red = (float)cell.BackgroundColor.R / 255, Green = (float)cell.BackgroundColor.G / 255 };

                    cellData.UserEnteredFormat = cellFormat;
                    listCellData.Add(cellData);
                }
                rowData.Values = listCellData;
                listRowData.Add(rowData);
            }
            request.UpdateCells.Rows = listRowData;

            // It's a batch request so we can create more than one request and send them all in one batch. Just use reqs.Requests.Add() to add additional requestsList for the same spreadsheet
            
            
            requestsList.Requests.Add(request);

            executeModify();


        }

        public void RemoveRow(int rowStart, int rowEnd)
        {
            var request = new Request
            {
                DeleteDimension = new DeleteDimensionRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId = 0,
                        Dimension = "ROWS",
                        StartIndex = rowStart,
                        EndIndex = rowEnd
                    }
                }
            };


            requestsList.Requests.Add(request);

            executeModify();
        }


        public void executeModify()
        {
            _sheetsService.Spreadsheets.BatchUpdate(requestsList, _spreadsheetId).Execute();
        }

        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];
            return value;
        }

        private GoogleSheetParameters MakeGoogleSheetDataRangeColumnsZeroBased(GoogleSheetParameters googleSheetParameters)
        {
            googleSheetParameters.RangeColumnStart = googleSheetParameters.RangeColumnStart - 1;
            googleSheetParameters.RangeColumnEnd = googleSheetParameters.RangeColumnEnd - 1;
            return googleSheetParameters;
        }

        private int GetSheetId(SheetsService service, string spreadSheetId, string spreadSheetName)
        {
            var spreadsheet = service.Spreadsheets.Get(spreadSheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == spreadSheetName);
            int sheetId = (int)sheet.Properties.SheetId;
            return sheetId;
        }



        
        //expando object to instrument object. Convert to dictionary to read
        private Instrument expToInst(ExpandoObject item, int id)
        {
            var dict = (IDictionary<string, object>)item;
            var type = dict["Type"] as string;
            var make = dict["Make"] as string;
            var caseN = dict["Case Number"] as string;
            var serial = dict["Serial Number"] as string;

            var grade = -1;
            try
            {
                grade = Int32.Parse(dict["Grade"] as string);
            }
            catch (InvalidCastException e)
            {
                
            }

            var sID = dict["Student ID"] as string;
            var repair = dict["Repair Status"] as string;

            Instrument ret = Instrument.CreateInstrument();
            
            ret.id = id;
            ret.type = type;
            ret.make = make;
            ret.caseNum = caseN;
            ret.serialNum = serial;
            ret.grade = grade;
            ret.studentID = sID;
            ret.repairStatus = repair;
            
            

            return ret;
        }
        //expando to student object. Convert into dictionary to read
        private Student expToStud(ExpandoObject item, int id)
        {
            var dict = (IDictionary<string, object>)item;
            var fname = dict["First Name"] as string;
            var lname = dict["Last Name"] as string;
            var course = dict["Class"] as string;
            
            var grade = -1;
            try
            {
                grade = Int32.Parse(dict["Grade"] as string);
            }
            catch (InvalidCastException e)
            {
                
            }

            var email = dict["Email"] as string;

            Student student1 = Student.CreateStudent();
            student1.id = id;
            student1.fname = fname;
            student1.lname = lname;
            student1.course = course;
            student1.grade = grade;
            student1.email = email;

            return student1;

        }
    }

    //cell, row, and parameters objects
    public class GoogleSheetCell
    {
        public string CellValue { get; set; }
        public bool IsBold { get; set; }
        public System.Drawing.Color BackgroundColor { get; set; } = System.Drawing.Color.White;
    }

    public class GoogleSheetParameters
    {
        public int RangeColumnStart { get; set; }
        public int RangeRowStart { get; set; }
        public int RangeColumnEnd { get; set; }
        public int RangeRowEnd { get; set; }
        public string SheetName { get; set; }
        public bool FirstRowIsHeaders { get; set; }
    }

    public class GoogleSheetRow
    {
        public GoogleSheetRow() => Cells = new List<GoogleSheetCell>();

        public List<GoogleSheetCell> Cells { get; set; }
    }
}