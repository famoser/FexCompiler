namespace Famoser.FexCompiler.Models.Document
{
    public class Content
    {
        private Content(ContentType contentType, string text)
        {
            ContentType = contentType;
            Text = text;
        }

        public string Text { get; private set; }
        public ContentType ContentType { get; private set; }

        public static Content FromCode(string code)
        {
            return new Content(ContentType.Code, code);
        }

        public static Content FromText(string text)
        {
            return new Content(ContentType.Text, text);
        }
}
}
