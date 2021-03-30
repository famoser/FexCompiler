using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Famoser.FexCompiler.Models;
using Famoser.FexCompiler.Services.Interface;

namespace Famoser.FexCompiler.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class FexService : IProcessService<List<FexLine>>
    {
        private readonly string[] _lines;
        public FexService(string[] lines)
        {
            _lines = lines;
        }

        public List<FexLine> Process()
        {
            var normalizedLines = ConvertStartSpacesToTabs(_lines);
            var fexLines = ConvertToFexLines(normalizedLines);
            NormalizeFexLineLevels(fexLines);
            TightenFexLineLevels(fexLines);
            FixFexLineLevels(fexLines);
            RemoveColonAtEndOfLines(fexLines);
            return fexLines;
        }

        private List<string> ConvertStartSpacesToTabs(string[] lines)
        {
            var result = new List<string>();

            foreach (var line in lines)
            {
                var currentLine = line;
                var tabCount = 0;
                while (currentLine.StartsWith("    "))
                {
                    currentLine = currentLine.Substring(4);
                    tabCount++;
                }

                var tabs = new string('\t', tabCount);
                result.Add(tabs + currentLine);
            }


            return result;
        }

        /// <summary>
        /// convert the array to fexLine objects
        /// cleans up empty lines, takes care of headers and codeContent sections
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns></returns>
        private List<FexLine> ConvertToFexLines(List<string> fileInput)
        {
            var res = new List<FexLine>();
            for (var index = 0; index < fileInput.Count; index++)
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

                //codeContent detected
                if (fexLine.Text == "```")
                {
                    //skip codeContent header
                    fexLine.Text = "";
                    index++;

                    //mark node as to be codeContent
                    fexLine.IsCode = true;

                    //prefix which can be cleared up
                    var removePrefix = new string('\t', fexLine.Level);

                    //collapse rest of codeContent into this line
                    var foundEnd = false;
                    for (; index < fileInput.Count; index++)
                    {
                        var test = fileInput[index].Trim();
                        if (test == "```")
                        {
                            foundEnd = true;
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

                        //append newline
                        fexLine.Text += "\n";
                    }

                    //cut off last "\n"
                    if (fexLine.Text.Length > 0)
                        fexLine.Text = fexLine.Text.Substring(0, fexLine.Text.Length - 1);

                    //issue warning because no codeContent end found
                    if (!foundEnd)
                    {
                        Console.WriteLine("no end for ``` found");
                    }
                }
                //if level is 0, this could be a header
                else if (fexLine.Level == 0)
                {
                    //check for next line if header mark is set
                    if (index + 1 < fileInput.Count)
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

            bool strangeLinesFound;
            do
            {
                strangeLinesFound = false;

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
                        for (; i < lines.Count; i++)
                        {
                            if (lines[i].Level >= faultyLevel)
                            {
                                lines[i].Level -= correction;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (i == lines.Count)
                        {
                            break;
                        }
                    }

                    lastLevel = lines[i].Level;
                }
            } while (strangeLinesFound);
        }

        /// <summary>
        /// tries to find any mistakes in levels
        /// </summary>
        /// <param name="lines"></param>
        private void FixFexLineLevels(List<FexLine> lines)
        {
            if (lines.Count == 0)
            {
                return;
            }

            //1: ensure levels start at 0, else add dummy lines till it does
            while (lines[0].Level > 0)
            {
                lines.Insert(0, new FexLine()
                {
                    Level = lines[0].Level - 1,
                    Text = "unknown title"
                });
            }
        }

        /// <summary>
        /// applies colon rules
        /// double colon escapes colon
        /// split at colon if not inside brackets
        /// </summary>
        /// <param name="lines"></param>
        private void RemoveColonAtEndOfLines(List<FexLine> lines)
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

                    //cut off last :
                    if (textToProcess[textToProcess.Length - 1] == ':')
                    {
                        textToProcess = textToProcess.Substring(0, textToProcess.Length - 1);
                    }

                    lines[i].Text = textToProcess;
                }
            }
        }
    }
}
