﻿using System;

namespace Famoser.FexCompiler.Models.Document
{
    public class MetaDataModel
    {
        public DateTime GeneratedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public string Hash { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
    }
}
