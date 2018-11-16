using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NotificationService.Entities
{
    [DataContract]
    public class SlackMessage
    {
        [DataMember(Name = "channel")]
        public string Channel { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }

        /// <summary>
        /// A plain-text summary of the attachment.
        /// </summary>
        [DataMember(Name = "fallback")]
        public string FallbackText { get; set; }

        /// <summary>
        /// An optional value that can either be one of good, warning, danger, or any hex color code (eg. #439FE0).
        /// </summary>
        [DataMember(Name = "color")]
        public string Color { get; set; }

        /// <summary>
        /// This is optional text that appears above the message attachment block.
        /// </summary>
        [DataMember(Name = "pretext")]
        public string Pretext { get; set; }

        /// <summary>
        /// Small text used to display the author's name.
        /// </summary>
        [DataMember(Name = "author_name")]
        public string AuthorName { get; set; }

        /// <summary>
        /// A valid URL that will hyperlink the author_name text. Will only work if author_name is present.
        /// </summary>
        [DataMember(Name = "author_link")]
        public string AuthorLink { get; set; }

        /// <summary>
        /// A valid URL that displays a small 16x16px image to the left of the author_name text. Will only work if author_name is present.
        /// </summary>
        [DataMember(Name = "author_icon")]
        public string AuthorIcon { get; set; }

        /// <summary>
        /// The title is displayed as larger, bold text near the top of a message attachment.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// By passing a valid URL in the title_link parameter (optional), the title text will be hyperlinked.
        /// </summary>
        [DataMember(Name = "title_link")]
        public string TitleLink { get; set; }

        /// <summary>
        /// This is the main text in a message attachment, and can contain standard message markup.
        /// The content will automatically collapse if it contains 700+ characters or 5+ linebreaks, and will display a "Show more..." link to expand the content.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Fields are defined as an array, and hashes contained within it will be displayed in a table inside the message attachment.
        /// </summary>
        [DataMember(Name = "fields")]
        public IList<SlackMessageField> Fields { get; set; }

        /// <summary>
        /// A valid URL to an image file that will be displayed inside a message attachment. Currently supported formats: GIF, JPEG, PNG, and BMP.
        /// Large images will be resized to a maximum width of 400px or a maximum height of 500px, while still maintaining the original aspect ratio.
        /// </summary>
        [DataMember(Name = "image_url")]
        public string ImageURL { get; set; }

        /// <summary>
        /// A valid URL to an image file that will be displayed as a thumbnail on the right side of a message attachment. Currently supported formats: GIF, JPEG, PNG, and BMP.
        /// The thumbnail's longest dimension will be scaled down to 75px while maintaining the aspect ratio of the image. The filesize of the image must also be less than 500 KB.
        /// </summary>
        [DataMember(Name = "thumb_url")]
        public string ThumbURL { get; set; }

        /// <summary>
        /// To enable formatting on attachment fields, set the mrkdwn_in array on each attachment to the list of fields to process.
        /// Valid values for mrkdwn_in are: ["pretext", "text", "fields"]. Setting "fields" will enable markup formatting for the value of each field.
        /// </summary>
        [DataMember(Name = "mrkdwn_in")]
        public string[] Markdown { get; set; }
    }
}
