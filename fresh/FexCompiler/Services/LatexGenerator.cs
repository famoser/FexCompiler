using FexCompiler.Models;

namespace FexCompiler.Services;

public static class LatexGenerator
{
    // ReSharper disable once StringLiteralTypo
    private const string TemplateHeader = """
                                          \documentclass[8pt, twocolumn]{article}

                                          % to be able to use UTF-8
                                          \usepackage[utf8]{inputenc}
                                          % for alpha, beta and other mathematical symbols
                                          \usepackage{textgreek}
                                          % to be able to change header sizes & margins
                                          \usepackage[small,compact]{titlesec}
                                          % provide font size 8pt
                                          \usepackage{extsizes}
                                          % use strange symbols like \degree
                                          \usepackage{gensymb}

                                          % add subsubparagraph
                                          \makeatletter
                                          \newcounter{subsubparagraph}[subparagraph]
                                          \renewcommand\thesubsubparagraph{%
                                            \thesubparagraph.\@arabic\c@subsubparagraph}
                                          \newcommand\subsubparagraph{%
                                            \@startsection{subsubparagraph}    % counter
                                              {6}                              % level
                                              {0cm}                     % indent
                                              {0cm} % beforeskip
                                              {0ex}                           % afterskip
                                              {\normalfont\normalsize\bfseries}}
                                          \newcommand\l@subsubparagraph{\@dottedtocline{6}{10em}{5em}}
                                          \newcommand{\subsubparagraphmark}[1]{}
                                          \makeatother

                                          % consistent naming for easier handling in code
                                          \newcommand{\subsubsubsection}[1]{\paragraph{#1}\mbox{}\vspace{2pt}\\}
                                          \newcommand{\subsubsubsubsection}[1]{\subparagraph{#1}\mbox{}\vspace{2pt}\\}
                                          \newcommand{\subsubsubsubsubsection}[1]{\subsubparagraph{#1}\mbox{}\vspace{2pt}\\}

                                          % number 6 levels deep
                                          \setcounter{secnumdepth}{6}

                                          % make content bigger
                                          \setlength{\hoffset}{-50pt}
                                          \setlength{\textwidth}{560pt}
                                          \setlength{\voffset}{-100pt}
                                          \setlength{\textheight}{770pt}

                                          % remove paragraph spacing
                                          \setlength{\parindent}{0pt}
                                          \setlength{\parskip}{0pt}

                                          % align paragraphs left
                                          \raggedright

                                          % set spacing of headers (thanks fibonacci)
                                          \titlespacing{\section}{0pt}{13pt}{8pt}
                                          \titlespacing{\subsection}{0pt}{8pt}{5pt}
                                          \titlespacing{\subsubsection}{0pt}{5pt}{3pt}
                                          \titlespacing{\paragraph}{0pt}{3pt}{2pt}
                                          \titlespacing{\subparagraph}{0pt}{2pt}{1pt}
                                          \titlespacing{\subsubparagraph}{0pt}{1pt}{1pt}
                                          """;

    public static string Generate(Document document)
    {
        var latex = TemplateHeader +
                    "\n\\title{" + document.Title + "\\\\ \\vspace{6pt} \\small{" + document.Statistics.CharacterCount +
                    " characters in " + document.Statistics.WordCount + " words on " + document.Statistics.LineCount +
                    " lines}}" +
                    "\n\\author{" + document.Author + "}";

        var latexContent = GenerateLatexContent(document.Content, document.Sections, 0);
        return latex +
               "\n\n\\begin{document}\n\\maketitle\n" + latexContent + "\n\\end{document}";
    }

    private static string GenerateLatexContent(List<Content> content, List<Section> sections, int level)
    {
        var result = ToLatex(content);

        foreach (var section in sections)
        {
            // print title
            var sectionName = "section";
            for (var i = 0; i < level && i < 4; i++) sectionName = "sub" + sectionName;
            var title = "\\" + sectionName + "{" + EscapeTextToLatex(section.Header) + "}\n";
            result += title;


            // if children have no children, output last level: paragraphs
            if (section.Children.All(c => !c.Children.Any()))
            {
                var paragraphSpacer = "\\vspace{5pt}\n";

                // if text before, add spacer now
                if (result != "")
                {
                    result += paragraphSpacer;
                }

                foreach (var sectionChild in section.Children)
                {
                    result += "\\textbf{" + EscapeTextToLatex(sectionChild.Header) + "}\\\\\n";
                    result += ToLatex(sectionChild.Content);
                    result += paragraphSpacer;
                }
            }
            else
            {
                result += GenerateLatexContent(section.Content, section.Children, level + 1);
            }
        }


        return result;
    }

    private static string ToLatex(List<Content> content)
    {
        var result = "";
        foreach (var line in content)
        {
            if (line.IsVerbatim)
            {
                result += "\\begin{verbatim}\n" +
                          line.Text +
                          "\n\\end{verbatim}";
            }
            else
            {
                //write text
                var textContent = EscapeTextToLatex(line.Text);
                if (textContent != "")
                    //latex newline + OS newline
                    result += textContent + "\\\\ " + Environment.NewLine;
            }
        }

        return result;
    }


    private static string EscapeTextToLatex(string text)
    {
        var result = new System.Text.StringBuilder(text.Length);

        foreach (var character in text)
        {
            result.Append(character switch
            {
                '\\' => "\\textbackslash{}",
                '&' => "\\&",
                '%' => "\\%",
                '$' => "\\$",
                '#' => "\\#",
                '_' => "\\_",
                '{' => "\\{",
                '}' => "\\}",
                'α' => "\\alpha ",
                'β' => "\\beta ",
                'σ' => "\\sigma ",
                '~' => "\\textasciitilde{}",
                '^' => "\\textasciicircum{}",
                _ => character.ToString()
            });
        }

        return result.ToString();
    }
}