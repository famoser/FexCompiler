using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Famoser.FexCompiler.Models.LearningCard;
using Famoser.FexCompiler.Services.Interface;
using Newtonsoft.Json;

namespace Famoser.FexCompiler.Services
{
    public class LearningCardsExportService : IProcessService<bool>
    {
        private readonly LearningCardCollection _learningCardCollection;
        private readonly string _path;

        private const string PathSeparator = "→";

        public LearningCardsExportService(LearningCardCollection learningCardCollection, string path)
        {
            _path = path;
            _learningCardCollection = learningCardCollection;
        }

        public bool Process()
        {
            //create .json file
            var baseFileName = _path.Substring(0, _path.LastIndexOf(".", StringComparison.Ordinal));
            var texFile = baseFileName + ".json";
            File.WriteAllText(texFile, JsonConvert.SerializeObject(_learningCardCollection));

            //create excel file
            CreateExcelFile(baseFileName + ".xlsx");

            return true;
        }


        private void CreateExcelFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var document = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                var sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = _learningCardCollection.MetaDataModel.Title
                };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Constructing header
                var row = new Row();

                row.Append(
                    ConstructCell("Title"),
                    ConstructCell("Children"),
                    ConstructCell("ItemCount"),
                    ConstructCell("Path"),
                    ConstructCell("Identifier")
                );

                // Insert the header row to the Sheet Data
                sheetData.AppendChild(row);
                
                foreach (var learningCard in _learningCardCollection.LearningCards)
                {
                    row = new Row();

                    row.Append(
                        ConstructCell(learningCard.Title),
                        ConstructCell(learningCard.Content),
                        ConstructCell(learningCard.ItemCount.ToString(), CellValues.Number),
                        ConstructCell(learningCard.Path),
                        ConstructCell(learningCard.Identifier)
                    );

                    sheetData.AppendChild(row);
                }

                worksheetPart.Worksheet.Save();
            }
        }

        private Cell ConstructCell(string value, CellValues dataType = CellValues.String)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }
}
