using System;
using System.Collections.Generic;

namespace TomskGO.Models.API
{
    public class NewsModel
    {
        public long OriginalId { get; set; }

        public string ShortDescription { get; set; }

        public string FullText { get; set; }
        
        public DateTime Date { get; set; }
        
        public string PreviewSource { get; set; }
        
        public string SourceLabel { get; set; }
        
        public List<NewsTag> Tags { get; set; }

        public List<NewsMember> Members { get; set; }

        public NewsAttachment Attachments { get; set; }
        
        public bool? AttachmentsVisible { get; set; }

        public bool? MembersVisible { get; set; }
    }
}