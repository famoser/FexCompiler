using FexCompiler.Models;

namespace FexCompiler.Services
{
    public abstract class Parser
    {
        public static List<Line> Parse(string[] lines)
        {
            var normalizedLines = ConvertStartSpacesToTabs(lines);
            var fexLines = ConvertToFexLines(normalizedLines);
            RemoveColonAtEndOfLines(fexLines);
            return fexLines;
        }

        private static List<string> ConvertStartSpacesToTabs(string[] lines)
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
        private static List<Line> ConvertToFexLines(List<string> fileInput)
        {
            var res = new List<Line>();
            for (var index = 0; index < fileInput.Count; index++)
            {
                var currentLine = fileInput[index];

                //ignore emtpy lines
                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    continue;
                }

                //create our line
                var fexLine = new Line();

                //set line level
                while (currentLine.StartsWith("\t"))
                {
                    currentLine = currentLine.Substring(1);
                    fexLine.Level++;
                }

                //set text
                fexLine.Text = currentLine.Trim();

                //verbatim detected
                if (fexLine.Text == "```")
                {
                    //skip header
                    fexLine.Text = "";
                    index++;

                    //mark node as to be verbatim
                    fexLine.IsVerbatim = true;

                    //prefix which can be cleared up
                    var removePrefix = new string('\t', fexLine.Level);

                    //collapse rest of verbatim into this line
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

                    //issue warning because no codeContent end found
                    if (!foundEnd)
                    {
                        Console.WriteLine("no end for ``` found");
                    }

                    //cut off last "\n"
                    if (fexLine.Text.Length > 0)
                    {
                        fexLine.Text = fexLine.Text.Substring(0, fexLine.Text.Length - 1);
                    }
                }
                //if level is 0, this could be a header
                else if (fexLine.Level == 0)
                {
                    //check for next line if header mark is set
                    if (index + 1 < fileInput.Count)
                    {
                        var nextLine = fileInput[index + 1];
                        //if next line is only ===, but at least 3 then set level to -2
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
        /// applies colon rules
        /// double colon escapes colon
        /// split at colon if not inside brackets
        /// </summary>
        /// <param name="lines"></param>
        private static void RemoveColonAtEndOfLines(List<Line> lines)
        {
            //split on colon if there is content afterwards
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsVerbatim)
                {
                    continue;
                }

                if (lines[i].Text.Contains(":"))
                {
                    //only split if its single :
                    var textToProcess = lines[i].Text;

                    //cut off last :
                    if (textToProcess[^1] == ':')
                    {
                        textToProcess = textToProcess.Substring(0, textToProcess.Length - 1);
                    }

                    lines[i].Text = textToProcess;
                }
            }
        }
    }
    
    public class Line
    {
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public bool IsVerbatim { get; set; }
    }
}
