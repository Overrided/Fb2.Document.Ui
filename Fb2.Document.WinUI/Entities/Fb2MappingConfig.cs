using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.Entities
{
    public class PoemConfig : SectionConfig
    {
        public TextAlignment VerseHorizontalAlignment { get; }

        public TextAlignment TextAuthorHorizontalAlignment { get; }

        public TextAlignment DateHorizontalAlignment { get; }

        public PoemConfig(
            TextAlignment titleHorizontalAlignment = TextAlignment.Center,
            TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
            TextAlignment epigraphHorizontalAlignment = TextAlignment.Right,
            TextAlignment verseHorizontalAlignment = TextAlignment.Center,
            TextAlignment textAuthorHorizontalAlignment = TextAlignment.Right,
            TextAlignment dateHorizontalAlignment = TextAlignment.Right)
            : base(titleHorizontalAlignment, subtitleHorizontalAlignment, epigraphHorizontalAlignment)
        {
            VerseHorizontalAlignment = verseHorizontalAlignment;
            TextAuthorHorizontalAlignment = textAuthorHorizontalAlignment;
            DateHorizontalAlignment = dateHorizontalAlignment;
        }
    }

    public class SectionConfig : ConfigBase
    {
        public TextAlignment SubtitleHorizontalAlignment { get; }

        public SectionConfig(
            TextAlignment titleHorizontalAlignment = TextAlignment.Center,
            TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
            TextAlignment epigraphHorizontalAlignment = TextAlignment.Right)
            : base(titleHorizontalAlignment, epigraphHorizontalAlignment)
        {
            SubtitleHorizontalAlignment = subtitleHorizontalAlignment;
        }
    }

    public class ConfigBase
    {
        public TextAlignment TitleHorizontalAlignment { get; }
        public TextAlignment EpigraphHorizontalAlignment { get; }

        public ConfigBase(
            TextAlignment titleHorizontalAlignment = TextAlignment.Center,
            TextAlignment epigraphHorizontalAlignment = TextAlignment.Right)
        {
            TitleHorizontalAlignment = titleHorizontalAlignment;
            EpigraphHorizontalAlignment = epigraphHorizontalAlignment;
        }
    }

    public class QuoteConfig
    {
        public TextAlignment SubtitleHorizontalAlignment { get; }
        public TextAlignment TextAuthorHorizontalAlignment { get; }

        public QuoteConfig(
            TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
            TextAlignment textAuthorHorizontalAlignment = TextAlignment.Right)
        {
            SubtitleHorizontalAlignment = subtitleHorizontalAlignment;
            TextAuthorHorizontalAlignment = textAuthorHorizontalAlignment;
        }
    }

    public class AnnotationConfig
    {
        public TextAlignment SubTitleHorizontalAlignment { get; }

        public AnnotationConfig(TextAlignment subTitleHorizontalAlignment = TextAlignment.Center)
        {
            SubTitleHorizontalAlignment = subTitleHorizontalAlignment;
        }
    }

    // TODO : add bool UseStyles / UseDefaultStyles
    public class Fb2MappingConfig
    {
        public int BaseFontSize { get; set; }

        public double ParagraphIndent { get; set; }

        public bool HighlightUnsafe { get; set; }

        public bool UseStyles { get; set; }

        public PoemConfig Poem { get; set; }

        public ConfigBase Body { get; set; }

        public SectionConfig Section { get; set; }

        public QuoteConfig Quote { get; set; }

        public AnnotationConfig Annotation { get; set; }

        public Fb2MappingConfig(
            int baseFontSize = 18,
            double paragraphIndent = 20,
            bool highlightUnsafe = false,
            bool useStyles = true,
            PoemConfig poemConfig = null,
            ConfigBase bodyConfig = null,
            SectionConfig bodySectionConfig = null,
            QuoteConfig citeConfig = null,
            AnnotationConfig annotation = null)
        {
            BaseFontSize = baseFontSize;
            ParagraphIndent = paragraphIndent;
            HighlightUnsafe = highlightUnsafe;
            UseStyles = useStyles;

            Poem = poemConfig ?? new PoemConfig();
            Body = bodyConfig ?? new ConfigBase();
            Section = bodySectionConfig ?? new SectionConfig();
            Quote = citeConfig ?? new QuoteConfig();
            Annotation = annotation ?? new AnnotationConfig();
        }
    }
}
