using System;
using Famoser.FexCompiler.Services.Latex.Visitor;

namespace Famoser.FexCompiler.Services.Latex.Tree
{
    class EncapsulatedTerm : Term
    {
        public EncapsulatedTerm(char encapsulator, Term term)
        {
            Encapsulator = encapsulator;
            Term = term;
        }

        public char Encapsulator { private set; get; }

        public bool ShowEncapsulatorTerm { set; get; } = true;

        public Term Term { private set; get; }
        
        public static bool IsEncapsulatorStart(char candidate)
        {
            return candidate == '(' || candidate == '{' || candidate == '[' || candidate == '<';
        }

        public static char GetEncapsulatorEnd(char start)
        {
            switch (start)
            {
                case '(':
                    return ')';
                case '{':
                    return '}';
                case '[':
                    return ']';
                case '<':
                    return '>';
            }

            throw new Exception("provided " + start + " is not an encapsulator start");
        }

        public static bool IsEncapsulatorEnd(char candidate, char start)
        {
            return GetEncapsulatorEnd(start) == candidate;
        }

        public override void Accept(TermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
