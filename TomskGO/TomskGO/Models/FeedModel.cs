using System.Collections.Generic;

namespace TomskGO.Models
{
    class FeedModel
    {
        public string ShortDescription { get; set; }
        public string FullText { get; set; }
        public string PreviewSource { get; set; }
        public string SourceLabel { get; set; }
        public List<string> Tags { get; set; }

        public Attachment Attachments { get; set; }

        public bool AttachmentsVisible { get; set; }

        public class Attachment
        {
            public List<Link> Links { get; set; }
            public List<Photo> Photos { get; set; }
            public List<Audio> Audios { get; set; }

            public bool LinksVisible { get; set; }
            public bool PhotosVisible { get; set; }
            public bool AudiosVisible { get; set; }

            public class Link
            {
                public string Title { get; set; }
                public string Url { get; set; }
            }

            public class Photo
            {
                public string ImageSource { get; set; }
            }

            public class Audio
            {
                public List<string> Artists { get; set; }
                public string SongName { get; set; }
            }
        }
    }
}