using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Famoser.FexCompiler.Enum;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Models.ContentTypes;
using Famoser.FexCompiler.Models.TextRepresentation;
using Newtonsoft.Json.Linq;

namespace Famoser.FexCompiler.Helpers
{
    public class FexService
    {
        private readonly DocumentModel _document;
        public FexService(DocumentModel document)
        {
            _document = document;
        }

        public DocumentModel ParseDocument()
        {
            var fexLines = ConvertToFexLines(_document.RawLines);
            NormalizeFexLineLevels(fexLines);
            TightenFexLineLevels(fexLines);
            ProcessColon(fexLines);

            Section section = new Section(null);
            FillSection(fexLines, section, 0, fexLines.Count - 1, 0);
            document.Content = section.Content;

            return document;
        }

        private static void FillSection(List<FexLine> lines, Section section, int startIndex, int stopIndex, int levelCorrection)
        {
            int i = startIndex;
            for (; i <= stopIndex; i++)
            {
                if (lines[i].IsCode)
                {
                    section.Content.Add(new Code(lines[i].Text));
                }
                else
                {
                    var nextLevel = i < stopIndex ? lines[i + 1].Level - levelCorrection : -100;

                    //if nextLevel is bigger we recursively call
                    if (nextLevel > 0)
                    {
                        var newStartIndex = i + 1;
                        var newStopIndex = stopIndex;
                        int j = newStartIndex;
                        for (; j <= stopIndex; j++)
                        {
                            if (lines[j].Level - levelCorrection == 0)
                            {
                                newStopIndex = j - 1;
                                break;
                            }
                        }

                        //wops we really need a new section
                        var newSection = new Section(section) { Title = GetLineNodeSimple(lines[i].Text, true) };
                        FillSection(lines, newSection, newStartIndex, newStopIndex, levelCorrection + 1);
                        i = newStopIndex;
                        section.Content.Add(newSection);
                    }
                    else
                    {
                        section.Content.Add(new Paragraph(GetLineNodeSimple(lines[i].Text, false)));
                    }
                }
            }
        }

        /// <summary>
        /// convert the array to fexLine objects
        /// cleans up empty lines, takes care of headers and code sections
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns></returns>
        private List<FexLine> ConvertToFexLines(string[] fileInput)
        {
            var res = new List<FexLine>();
            for (var index = 0; index < fileInput.Length; index++)
            {
                var currentLine = fileInput[index];

                //ignore emtpy lines
                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    continue;
                }

                //create our line
                var fexLine = new FexLine();

                //set line level
                while (currentLine.StartsWith("\t"))
                {
                    currentLine = currentLine.Substring(1);
                    fexLine.Level++;
                }

                //set text
                fexLine.Text = currentLine.Trim();

                //code detected
                if (fexLine.Text.StartsWith("CODE_START"))
                {
                    //skip code header
                    fexLine.Text = "";
                    index++;

                    //mark node as to be code
                    fexLine.IsCode = true;

                    //prefix which can be cleared up
                    var removePrefix = new string('\t', fexLine.Level);

                    //collapse rest of code into this line
                    for (; index < fileInput.Length;)
                    {
                        if (fileInput[index].Trim() == "CODE_STOP")
                        {
                            break;
                        }

                        //try to correct level
                        if (fileInput[index].StartsWith(removePrefix))
                        {
                            //prefix found; therefore remove
                            fexLine.Text += fileInput[index].Substring(removePrefix.Length);
                        }
                        else
                        {
                            //prefix not found; just add it like this
                            fexLine.Text += fileInput[index];
                        }
                    }
                }

                //if level is 0, this could be a header
                if (fexLine.Level == 0)
                {
                    //check for next line if header mark is set
                    if (index + 1 < fileInput.Length)
                    {
                        var nextLine = fileInput[index + 1];
                        //if next line is only ===, but at least 3 then set level to -1
                        if (nextLine.StartsWith("==="))
                        {
                            fexLine.Level = -2;
                            //skip nextLine
                            index++;
                        }

                        if (nextLine.StartsWith("---"))
                        {
                            fexLine.Level = -1;
                            //skip nextLine
                            index++;
                        }
                    }
                }

                res.Add(fexLine);
            }

            return res;
        }

        /// <summary>
        /// make sure the level starts at 0
        /// </summary>
        /// <param name="lines"></param>
        private void NormalizeFexLineLevels(List<FexLine> lines)
        {
            //normalize indexes
            //0: ensure all levels >= 0
            var smallestNumber = lines.Min(t => t.Level);
            foreach (var fexLine in lines)
            {
                fexLine.Level -= smallestNumber;
            }
        }

        /// <summary>
        /// makes sure the levels are set as tight as possible
        /// </summary>
        /// <param name="lines"></param>
        private void TightenFexLineLevels(List<FexLine> lines)
        {
            //1: ensure levels are as tight as possible
            if (lines.Count < 1)
            {
                return;
            }

            var strangeLinesFound = false;
            do
            {
                var lastLevel = lines[0].Level;
                for (int i = 1; i < lines.Count; i++)
                {
                    if (lines[i].Level > lastLevel && lines[i].Level - 1 > lastLevel)
                    {
                        strangeLinesFound = true;

                        //increase by more than one level in one line; this is wrong
                        var faultyLevel = lines[i].Level;
                        //1 is allowed, but this offset is bigger than that
                        var offSet = faultyLevel - lastLevel;
                        Debug.Assert(offSet > 1);
                        //needed correction
                        var correction = offSet - 1;
                        Debug.Assert(correction >= 1);

                        //correct all lines with level 
                        for (int j = i; j < lines.Count; j++)
                        {
                            if (lines[j].Level >= faultyLevel)
                            {
                                lines[j].Level -= correction;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    lastLevel = lines[i].Level;
                }
            } while (strangeLinesFound);

        }

        /// <summary>
        /// applies colon rules
        /// double colon escapes colon
        /// split at colon if not inside brackets
        /// </summary>
        /// <param name="lines"></param>
        private void ProcessColon(List<FexLine> lines)
        {
            //split on colon if there is content afterwards
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsCode)
                {
                    continue;
                }
                
                if (lines[i].Text.Contains(":"))
                {
                    //only split if its single :
                    var textToProcess = lines[i].Text;
                    var splitIndex = -1;
                    var insideBracketLevel = 0; //(), {}
                    for (int j = 0; j < textToProcess.Length - 1; j++)
                    {
                        switch (textToProcess[j])
                        {
                            case ':':
                                if (textToProcess[j + 1] == ':')
                                    //remove, because :: treated as escape for :
                                    textToProcess = textToProcess.Remove(j, 1);
                                else if (insideBracketLevel == 0)
                                    //not inside brackets, therefore can split here
                                    splitIndex = j;

                                break;
                            case '(':
                            case '{':
                                insideBracketLevel++;
                                break;
                            case ')':
                            case '}':
                                insideBracketLevel--;
                                break;
                        }
                    }

                    if (splitIndex > 0)
                    {
                        //split at defined index
                        var before = textToProcess.Substring(0, splitIndex);
                        var after = textToProcess.Substring(splitIndex + 1);

                        lines[i].Text = before;
                        if (after.Length > 0)
                        {
                            //inset into lines
                            lines.Insert(i + 1, new FexLine()
                            {
                                Level = lines[i].Level + 1,
                                Text = after
                            });
                        }

                        //skip next line, as want only to break once
                        i++;
                    }
                    else
                    {
                        lines[i].Text = textToProcess;
                    }
                }
            }

            //remove all colons at the end of the line
        }

        private static LineNode GetLineNodeSimple(string line, bool isTitle)
        {
            var res = new List<TextNode>();
            line = line.Trim();
            res.Add(new TextNode() { TextType = isTitle ? TextType.Bold : TextType.Normal, Text = ParseText(line) });
            return new LineNode(res);
        }

        private static string ParseText(string str)
        {
            return str.Trim();
        }
    }
}
